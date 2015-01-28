using System;
using System.IO;
using System.Linq;
using System.Threading;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.Infrastructure.TransmitProtocols;
using Ista.FileServices.MarketFiles.Contexts;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.Models;
using Ista.Miramar.Interfaces;

namespace Ista.FileServices.MarketFiles
{
    public class TransmitFileTask : IMiramarTask
    {
        private readonly int clientId;
        private readonly ILogger logger;
        private readonly IAdminDataAccess adminDataAccess;
        private readonly IClientDataAccess clientDataAccess;
        private readonly IMarketFile marketDataAccess;
        
        public TransmitFileTask(IAdminDataAccess adminDataAccess, IClientDataAccess clientDataAccess, IMarketFile marketDataAccess, ILogger logger, int clientId)
        {
            this.adminDataAccess = adminDataAccess;
            this.clientDataAccess = clientDataAccess;
            this.marketDataAccess = marketDataAccess;
            this.logger = logger;
            this.clientId = clientId;
        }

        public void Execute(CancellationToken token)
        {
            var configuration = adminDataAccess.LoadExportConfiguration(clientId);
            var context = new TransmitFileContext
            {
                DirectoryArchive = Path.Combine(configuration.DirectoryArchive, DateTime.Now.ToString("yyyyMM")),
                DirectoryDecrypted = configuration.DirectoryDecrypted,
                DirectoryEncrypted = configuration.DirectoryEncrypted,
                DirectoryException = configuration.DirectoryException,
            };

            logger.TraceFormat(
                "Executing Transmit Task with configuration: Client Id: {0} \nDecrypted: \"{1}\" \nEncrypted: \"{2}\" \nArchive: \"{3}\" \nException: \"{4}\" \n",
                clientId, context.DirectoryDecrypted, context.DirectoryEncrypted, context.DirectoryArchive,
                context.DirectoryException);

            using (logger.NestLog("Transmit"))
            {
                Execute(context, token);
            }
        }

        public void Execute(TransmitFileContext context, CancellationToken token)
        {
            var marketFiles = marketDataAccess.ListEncryptedOutboundMarketFiles()
                .Where(x => !x.FileType.Equals("CBF", StringComparison.Ordinal))
                .ToArray();

            if (!marketFiles.Any())
                return;

            logger.DebugFormat("Transmitting {0} encrypted outbound market file(s).", marketFiles.Length);

            var ports = clientDataAccess.ListCspDunsPort();
            foreach (var marketFile in marketFiles)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                var port = IdentifyCspDunsPort(marketFile, ports);
                if (port == null)
                {
                    var message = string.Format("No CspDunsPort found for LDCID: {0} and CSPDUNSID: {1}.",
                        marketFile.LdcId, marketFile.CspDunsId);

                    logger.Error(message);

                    marketFile.Status = MarketFileStatusOptions.Error;
                    marketFile.ProcessError = message;
                    marketFile.ProcessDate = DateTime.Now;
                    marketDataAccess.UpdateMarketFile(marketFile);
                    continue;
                }

                if (!port.TransportEnabledFlag)
                {
                    logger.InfoFormat("CspDunsPort {0} is not enabled for transport.", marketFile.CspDunsId);

                    marketFile.Status = MarketFileStatusOptions.Error;
                    marketFile.ProcessError = "Transport not enabled.";
                    marketFile.ProcessDate = DateTime.Now;
                    marketDataAccess.UpdateMarketFile(marketFile);
                    continue;
                }

                logger.TraceFormat("Transmitting file \"{0}\" for CSPDUNS {1}.", marketFile.FileName, port.CspDunsId);
                TransmitMarketFile(context, marketFile, port);
            }
        }

        public void TransmitMarketFile(TransmitFileContext context, MarketFileModel marketFile, CspDunsPortModel port)
        {
            var fileName = string.Concat(marketFile.FileName, ".pgp");
            var encryptedFilePath = Path.Combine(context.DirectoryArchive, "Encrypted", fileName);
            var sourceFilePath = Path.Combine(port.DirectoryOut, fileName);
            var targetFilePath = Path.Combine(port.DirectoryOut, "Complete", fileName);

            var encryptedFile = new FileInfo(encryptedFilePath);
            if (encryptedFile.Exists)
                encryptedFile.CopyTo(sourceFilePath, true);

            var sourceFile = new FileInfo(sourceFilePath);

            try
            {
                var result = TransmitFile(sourceFile, port);

                marketFile.Status = (result.Transmitted)
                    ? MarketFileStatusOptions.Transmitted
                    : MarketFileStatusOptions.Error;
                marketFile.ProcessError = result.Message;
                marketFile.ProcessDate = DateTime.Now;
                marketDataAccess.UpdateMarketFile(marketFile);

                if (result.Transmitted)
                    MoveFile(sourceFile, targetFilePath);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(ex, "Unknown error occurred while transmitting file \"{0}\".",
                    marketFile.FileName);

                marketFile.Status = MarketFileStatusOptions.Error;
                marketFile.ProcessError = ex.Message;
                marketFile.ProcessDate = DateTime.Now;
                marketDataAccess.UpdateMarketFile(marketFile);
            }
        }

        public TransmitResult TransmitFile(FileInfo targetFile, CspDunsPortModel port)
        {
            switch (port.Protocol)
            {
                case "FTP":
                    return PostFileFtp(targetFile.FullName, port);
                case "GISB1.4":
                case "NAESB1.6":
                    return PostFileHttp(targetFile.FullName, port);
                case "SOAP":
                    return PostFileSoap(targetFile.FullName, port);
            }

            var message = string.Format("Unsupported protocol \"{0}\" found for Port \"{1}\".",
                port.Protocol, port.PortId);

            logger.Error(message);
            return new TransmitResult
            {
                Message = message,
                Transmitted = false,
            };
        }

        public CspDunsPortModel IdentifyCspDunsPort(MarketFileModel marketFile, CspDunsPortModel[] ports)
        {
            var ldcId = marketFile.LdcId;
            var fileType = marketFile.FileType;
            
            var port = ports.FirstOrDefault(x =>
                !string.IsNullOrWhiteSpace(x.FileType) && x.FileType.Equals(fileType) &&
                (!x.LdcId.HasValue || x.LdcId.Equals(ldcId)));

            if (port != null)
                return port;

            return ports.FirstOrDefault(x =>
                string.IsNullOrWhiteSpace(x.FileType) && (!x.LdcId.HasValue || x.LdcId.Equals(ldcId)));
        }

        public TransmitResult PostFileFtp(string fileName, CspDunsPortModel port)
        {
            var ftpRemoteDirectory = fileName.Contains("_MTCR")
                ? "custbill"
                : string.Empty;

            var ftpProtocol = new ProtocolFtp();
            return ftpProtocol.TransmitFile(fileName, port.FtpRemoteServer, ftpRemoteDirectory,
                port.FtpUserId, port.FtpPassword);
        }

        public TransmitResult PostFileHttp(string fileName, CspDunsPortModel port)
        {
            var cspDuns = clientDataAccess.LoadDunsByCspDunsId(port.CspDunsId);
            if (string.IsNullOrWhiteSpace(cspDuns))
            {
                var message = string.Format("CspDuns not found for CSPDUNSID \"{0}\".", port.CspDunsId);

                logger.Error(message);
                return new TransmitResult
                {
                    Message = message,
                    Transmitted = false,
                };
            }

            switch (port.Protocol.ToUpper())
            {
                case "GISB1.4":
                    var gisbProtocol = new ProtocolGisbHttp();
                    return gisbProtocol.TransmitFile(fileName, port.FtpRemoteServer, port.FtpUserId,
                        port.FtpPassword, cspDuns, port.GisbCommonCode);
                case "NAESB1.6":
                    var naesbProtocol = new ProtocolNaesbHttp();
                    return naesbProtocol.TransmitFile(fileName, port.FtpRemoteServer, port.FtpUserId,
                        port.FtpPassword, cspDuns, port.GisbCommonCode);
            }

            return new TransmitResult
            {
                Message = string.Empty,
                Transmitted = false,
            };
        }

        public TransmitResult PostFileSoap(string fileName, CspDunsPortModel port)
        {
            var soapProtocol = new ProtocolSoap();
            return soapProtocol.TransmitFile(fileName, port.FtpRemoteServer, port.FtpUserId, port.FtpPassword);
        }

        public void MoveFile(FileInfo sourceFile, string targetFile)
        {
            try
            {
                if (File.Exists(targetFile))
                    File.Delete(targetFile);

                sourceFile.MoveTo(targetFile);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(ex,
                    "Error occurred while moving file from source directory \"{0}\" to target directory \"{1}\".",
                    sourceFile.FullName, targetFile);
            }
        }
    }
}
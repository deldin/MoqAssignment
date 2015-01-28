using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Transactions;
using Ista.FileServices.Infrastructure.FtpHandlers;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Contexts;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.Models;
using Ista.Miramar.Interfaces;

namespace Ista.FileServices.MarketFiles
{
    public class ErcotTransmitFileTask : AbstractErcotFileTask, IMiramarTask
    {
        private readonly int clientId;
        private readonly ILogger logger;
        private readonly IAdminDataAccess adminDataAccess;
        private readonly IClientDataAccess clientDataAccess;
        private readonly IMarketFile marketDataAccess;
        
        public ErcotTransmitFileTask(IAdminDataAccess adminDataAccess, IClientDataAccess clientDataAccess, IMarketFile marketDataAccess, ILogger logger, int clientId)
            : base(clientDataAccess, marketDataAccess)
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
            var context = new ErcotFileContext
            {
                DirectoryArchive = configuration.DirectoryArchive,
                DirectoryDecrypted = configuration.DirectoryDecrypted,
                DirectoryEncrypted = configuration.DirectoryEncrypted,
                DirectoryException = configuration.DirectoryException,
            };

            logger.TraceFormat(
                "Executing Ercot Transmit Task with configuration: Client Id: {0} \nDecrypted: \"{1}\" \nEncrypted: \"{2}\" \nArchive: \"{3}\" \nException: \"{4}\" \n",
                clientId, context.DirectoryDecrypted, context.DirectoryEncrypted, context.DirectoryArchive,
                context.DirectoryException);

            using (logger.NestLog("Transmit"))
            {
                Execute(context, token);
            }
        }

        public void Execute(ErcotFileContext context, CancellationToken token)
        {
            var marketFiles = marketDataAccess.ListEncryptedOutboundMarketFiles()
                .Where(x => x.FileType.Equals("CBF", StringComparison.Ordinal))
                .ToArray();

            if (!marketFiles.Any())
                return;

            logger.DebugFormat("Transmitting {0} Customer Billing File(s).", marketFiles.Length);

            var cspDunsPorts = clientDataAccess.ListCspDunsPort();
            var grouping = marketFiles
                .GroupBy(x => x.CspDunsId ?? 0)
                .Select(x => new
                {
                    CspDunsId = x.Key,
                    MarketFiles = x.ToArray(),
                });

            foreach (var item in grouping)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                var port = IdentifyCspDunsPort(cspDunsPorts, item.CspDunsId);
                if (port == null)
                {
                    var marketFileIds = string.Join(", ", item.MarketFiles.Select(x => x.MarketFileId));

                    logger.InfoFormat(
                        "Unable to find Texas Port for Csp Duns Id {0}. Market Files {1} have been updated to Error status.",
                        item.CspDunsId, marketFileIds);

                    UpdateMarketFilesToError(item.MarketFiles, MissingCspDunsTexasPort);
                    continue;
                }

                if (!port.TransportEnabledFlag)
                {
                    logger.WarnFormat("Csp Duns Port {0} is not enabled for transport.", item.CspDunsId);
                    continue;
                }

                TransportFiles(context, port, item.MarketFiles, token);
            }
        }

        public void TransportFiles(ErcotFileContext context, CspDunsPortModel port, MarketFileModel[] models, CancellationToken token)
        {
            var configuration = new SecureBlackboxFtpConfiguration
            {
                FtpRemoteServer = port.FtpRemoteServer,
                FtpRemoteDirectory = string.Empty,
                FtpUsername = port.FtpUserId,
                FtpPassword = port.FtpPassword,
                FtpPort = 21,
                FtpSsl = false,
            };

            var ftpHandler = new SecureBlackboxFtpHandler(configuration);
            var options = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };

            foreach (var marketFile in models)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                var fileName = string.Concat(marketFile.FileName, ".pgp");

                var targetPath = Path.Combine(context.DirectoryEncrypted, fileName);
                var targetInfo = new FileInfo(targetPath);
                if (!targetInfo.Exists)
                {
                    logger.WarnFormat("Unable to FTP file \"{0}\". File does not exist or has been deleted.", fileName);
                    continue;
                }

                try
                {
                    var ftpOutPath = Path.Combine(port.DirectoryOut, fileName);
                    targetInfo.CopyTo(ftpOutPath, true);

                    var ftpOutInfo = new FileInfo(ftpOutPath);
                    if (!ftpOutInfo.Exists)
                    {
                        logger.WarnFormat("Unable to FTP file \"{0}\". File could not be copied to output directory.", fileName);
                        continue;
                    }

                    logger.InfoFormat("Trasmitting file \"{0}\" to FTP Server \"{1}\".",
                        ftpOutPath, port.FtpRemoteServer);

                    using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
                    {
                        ftpHandler.SendFiles("/custbill", port.DirectoryOut,
                            name => name.Equals(fileName, StringComparison.OrdinalIgnoreCase));

                        var completedPath = Path.Combine(port.DirectoryOut, @"\Complete\", ftpOutInfo.Name);
                        var completedInfo = new FileInfo(completedPath);
                        if (completedInfo.Exists)
                            completedInfo.Delete();

                        ftpOutInfo.MoveTo(completedPath);

                        var encryptedArchivePath = Path.Combine(context.DirectoryArchive, @"\Encrypted\",
                            targetInfo.Name);
                        targetInfo.MoveTo(encryptedArchivePath);

                        marketFile.Status = MarketFileStatusOptions.Transmitted;
                        marketFile.ProcessDate = DateTime.Now;
                        marketDataAccess.UpdateMarketFile(marketFile);

                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat(ex, "Unknown error occurred while transmitting file \"{0}\".", fileName);
                }
            }
        }
    }
}

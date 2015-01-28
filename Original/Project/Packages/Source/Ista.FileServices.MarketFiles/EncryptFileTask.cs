using System;
using System.IO;
using System.Threading;
using System.Transactions;
using Ista.FileServices.Infrastructure;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Contexts;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.Miramar.Interfaces;

namespace Ista.FileServices.MarketFiles
{
    public class EncryptFileTask : IMiramarTask
    {
        private readonly int clientId;
        private readonly ILogger logger;
        private readonly IAdminDataAccess adminDataAccess;
        private readonly IClientDataAccess clientDataAccess;
        private readonly IMarketFile marketDataAccess;

        public EncryptFileTask(IAdminDataAccess adminDataAccess, IClientDataAccess clientDataAccess, IMarketFile marketDataAccess, ILogger logger, int clientId)
        {
            this.logger = logger;
            this.adminDataAccess = adminDataAccess;
            this.clientDataAccess = clientDataAccess;
            this.marketDataAccess = marketDataAccess;
            this.clientId = clientId;
        }

        public void Execute(CancellationToken token)
        {
            var configuration = adminDataAccess.LoadExportConfiguration(clientId);
            var context = new EncryptFileContext
            {
                DirectoryArchive = Path.Combine(configuration.DirectoryArchive, DateTime.Now.ToString("yyyyMM")),
                DirectoryDecrypted = configuration.DirectoryDecrypted,
                DirectoryEncrypted = configuration.DirectoryEncrypted,
                DirectoryException = configuration.DirectoryException,
            };

            logger.TraceFormat(
                "Executing Encrypt Task with configuration: Client Id: {0} \nEncrypted: \"{1}\" \nDecrypted: \"{2}\" \nArchive: \"{3}\" \nException: \"{4}\" \n",
                clientId, context.DirectoryEncrypted, context.DirectoryDecrypted, context.DirectoryArchive,
                context.DirectoryException);

            using (logger.NestLog("Encrypt"))
            {
                Execute(context, token);
            }
        }

        public void Execute(EncryptFileContext context, CancellationToken token)
        {
            var cspDunsPorts = clientDataAccess.ListCspDunsPort();
            foreach (var port in cspDunsPorts)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                if (!port.EncryptionEnabledFlag)
                    continue;

                logger.TraceFormat(
                    "Identified CSP DUNS Port {0} for LDC \"{1}\" \nDirectory In: \"{2}\" \nDirectory Out: \"{3}\" \n",
                    port.CspDunsPortId, port.LdcShortName, port.DirectoryIn, port.DirectoryOut);

                IPgpEncryption pgpEncryption;

                try
                {
                    pgpEncryption = InfrastructureFactory
                        .CreatePgpEncryptor(port.PgpEncryptionKey, port.PgpSignatureKey, port.PgpPassphrase);
                }
                catch (IOException ex)
                {
                    logger.ErrorFormat(ex,
                        "Unable to create PGP Encryption class for Csp Duns Port \"{0}\".", port.CspDunsPortId);
                    continue;
                }

                var identifier = port.TradingPartnerId.Substring(0, 3);

                var ldcId = port.LdcId ?? 0;
                if (ldcId == 0)
                {
                    var clientSearchPattern = string.Format("*{0}*", identifier);
                    EncryptFiles(pgpEncryption, context, clientSearchPattern, token);
                }
                else
                {
                    var fileExtension = port.ProviderId == 2 ? ".x12" : ".txt";
                    var extendedSearchPattern = string.Format("*{0}*{1}*{2}", identifier, port.LdcShortName, fileExtension);
                    EncryptFiles(pgpEncryption, context, extendedSearchPattern, token);
                }
            }

            MoveFiles(context, "*", token);
        }

        public void EncryptFiles(IPgpEncryption pgpEncryption, EncryptFileContext context, string searchPattern, CancellationToken token)
        {
            var sourceDirectory = new DirectoryInfo(context.DirectoryDecrypted);
            if (!sourceDirectory.Exists)
            {
                logger.ErrorFormat("Decryption Directory \"{0}\" was not found or has been deleted.",
                    context.DirectoryDecrypted);
                return;
            }

            var destinationDirectory = new DirectoryInfo(context.DirectoryEncrypted);
            if (!destinationDirectory.Exists)
            {
                logger.ErrorFormat("Encryption Directory \"{0}\" was not found or has been deleted.",
                    context.DirectoryEncrypted);
                return;
            }

            var archiveDirectory = new DirectoryInfo(context.DirectoryArchive);
            if (!archiveDirectory.Exists)
            {
                archiveDirectory.Create();
                logger.TraceFormat("Archive Directory \"{0}\" was created.", archiveDirectory.FullName);
            }

            FileInfo[] sourceFiles;

            try
            {
                sourceFiles = sourceDirectory.GetFiles(searchPattern);
                logger.TraceFormat("Identified {0} file(s) in directory \"{1}\" with search pattern \"{2}\".",
                    sourceFiles.Length, sourceDirectory.FullName, searchPattern);

                if (sourceFiles.Length == 0)
                    return;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(ex,
                                   "Error occurred while retrieving files from directory \"{1}\" with search pattern \"{2}\".",
                                   sourceDirectory, searchPattern);

                return;
            }

            EncryptFiles(pgpEncryption, context, sourceFiles, token);
        }

        public void EncryptFiles(IPgpEncryption pgpEncryption, EncryptFileContext context, FileInfo[] sourceFiles, CancellationToken token)
        {
            var archiveDirectory = context.DirectoryArchive;
            var archiveInfo = new DirectoryInfo(archiveDirectory);
            if (!archiveInfo.Exists)
                archiveInfo.Create();

            foreach (var sourceFile in sourceFiles)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                var targetFileName = string.Concat(sourceFile.Name, ".pgp");
                var targetPath = Path.Combine(context.DirectoryEncrypted, targetFileName);

                try
                {
                    var options = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
                    {
                        logger.DebugFormat("Encrypting \"{0}\" to \"{1}\".", sourceFile.FullName, targetPath);
                        pgpEncryption.EncryptFile(sourceFile.FullName, targetPath);

                        var marketFile = marketDataAccess.LoadOutboundMarketFileByName(sourceFile.Name);
                        marketFile.Status = MarketFileStatusOptions.Encrypted;
                        marketFile.ProcessDate = DateTime.Now;
                        marketDataAccess.UpdateMarketFile(marketFile);

                        logger.InfoFormat("Encrypted file \"{0}\" to \"{1}\".", sourceFile.FullName, targetPath);

                        var archivePath = Path.Combine(archiveInfo.FullName, sourceFile.Name);
                        MoveFile(sourceFile.FullName, archivePath);
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat(ex, "Unknown error occurred while encrypting file \"{0}\".", sourceFile.FullName);

                    var options = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
                    {
                        var marketFile = marketDataAccess.LoadOutboundMarketFileByName(sourceFile.Name);
                        marketFile.Status = MarketFileStatusOptions.Error;
                        marketFile.ProcessDate = DateTime.Now;
                        marketFile.ProcessError = "Failed To Encrypt File.";
                        marketDataAccess.UpdateMarketFile(marketFile);

                        var exceptionPath = Path.Combine(context.DirectoryException, sourceFile.Name);
                        MoveFile(sourceFile.FullName, exceptionPath);
                        scope.Complete();
                    }
                }
            }
        }

        public void MoveFiles(EncryptFileContext context, string searchPattern, CancellationToken token)
        {
            var sourceDirectory = new DirectoryInfo(context.DirectoryEncrypted);
            if (!sourceDirectory.Exists)
            {
                logger.ErrorFormat("Encryption Directory \"{0}\" was not found or has been deleted.",
                    context.DirectoryEncrypted);
                return;
            }

            var archiveDirectory = new DirectoryInfo(context.DirectoryArchive);
            if (!archiveDirectory.Exists)
            {
                archiveDirectory.Create();
                logger.TraceFormat("Archive Directory \"{0}\" was created.", archiveDirectory.FullName);
            }

            var destinationPath = Path.Combine(context.DirectoryArchive, "Encrypted");
            var destinationDirectory = new DirectoryInfo(destinationPath);
            if (!destinationDirectory.Exists)
            {
                destinationDirectory.Create();
                logger.TraceFormat("Archive Directory \"{0}\" was created.", destinationDirectory.FullName);
            }

            FileInfo[] sourceFiles;

            try
            {
                sourceFiles = sourceDirectory.GetFiles(searchPattern);
                logger.TraceFormat("Identified {0} file(s) in directory \"{1}\" with search pattern \"{2}\".",
                    sourceFiles.Length, sourceDirectory.FullName, searchPattern);

                if (sourceFiles.Length == 0)
                    return;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(ex,
                                   "Error occurred while retrieving files from directory \"{1}\" with search pattern \"{2}\".",
                                   sourceDirectory, searchPattern);

                return;
            }

            MoveFiles(destinationPath, sourceFiles, token);
        }

        public void MoveFiles(string destinationPath, FileInfo[] sourceFiles, CancellationToken token)
        {
            foreach (var sourceFile in sourceFiles)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                MoveFile(sourceFile.FullName, Path.Combine(destinationPath, sourceFile.Name));
            }
        }

        public void MoveFile(string sourceFile, string destinationFile)
        {
            try
            {
                if (File.Exists(destinationFile))
                    File.Delete(destinationFile);
                File.Move(sourceFile, destinationFile);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(ex,
                                   "Error occurred while moving file from source directory \"{1}\" to desination directory \"{2}\".",
                                   sourceFile, destinationFile);
            }
        }
    }
}
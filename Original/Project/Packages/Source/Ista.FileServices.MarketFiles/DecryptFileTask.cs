using System;
using System.IO;
using System.Threading;
using Ista.FileServices.Infrastructure;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Contexts;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.Miramar.Interfaces;

namespace Ista.FileServices.MarketFiles
{
    public class DecryptFileTask : IMiramarTask
    {
        private readonly int clientId;
        private readonly ILogger logger;
        private readonly IAdminDataAccess adminDataAccess;
        private readonly IClientDataAccess clientDataAccess;
        
        public DecryptFileTask(IAdminDataAccess adminDataAccess, IClientDataAccess clientDataAccess, ILogger logger, int clientId)
        {
            this.logger = logger;
            this.adminDataAccess = adminDataAccess;
            this.clientDataAccess = clientDataAccess;
            this.clientId = clientId;
        }

        public void Execute(CancellationToken token)
        {
            var configuration = adminDataAccess.LoadImportConfiguration(clientId);
            var context = new DecryptFileContext
            {
                DirectoryArchive = configuration.DirectoryArchive,
                DirectoryDecrypted = configuration.DirectoryDecrypted,
                DirectoryEncrypted = configuration.DirectoryEncrypted,
                DirectoryException = configuration.DirectoryException,
            };

            logger.TraceFormat(
                "Executing Decrypt Task with configuration: Client Id: {4} \nEncrypted: \"{0}\" \nDecrypted: \"{1}\" \nArchive: \"{2}\" \nException: \"{3}\" \n",
                context.DirectoryEncrypted, context.DirectoryDecrypted, context.DirectoryArchive,
                context.DirectoryException, clientId);

            using (logger.NestLog("Decrypt"))
            {
                Execute(context, token);
            }
        }

        public void Execute(DecryptFileContext context, CancellationToken token)
        {
            var cspDunsPorts = clientDataAccess.ListCspDunsPort();
            foreach (var port in cspDunsPorts)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

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

                var ldcId = port.LdcId ?? 0;
                if (ldcId == 0)
                {
                    var identifier = port.TradingPartnerId.Substring(0, 3);
                    var clientSearchPattern = string.Format("*{0}*", identifier);
                    var dunsSearchPattern = string.Format("*{0}*", port.Duns);

                    DecryptFiles(pgpEncryption, context, clientSearchPattern, token);
                    DecryptFiles(pgpEncryption, context, dunsSearchPattern, token);
                    continue;
                }

                var sourceDirectory = new DirectoryInfo(port.DirectoryIn);
                if (!sourceDirectory.Exists)
                {
                    logger.ErrorFormat("Encryption Directory \"{0}\" was not found or has been deleted.",
                        sourceDirectory.FullName);
                    continue;
                }

                var sourceFiles = sourceDirectory.GetFiles();
                logger.TraceFormat("Identified {0} file(s) in directory \"{1}\" (all files).",
                    sourceFiles.Length, sourceDirectory.FullName);

                if (sourceFiles.Length == 0)
                    continue;

                DecryptFiles(pgpEncryption, context, sourceFiles, token);
            }
        }

        public void DecryptFiles(IPgpEncryption pgpEncryption, DecryptFileContext context, string searchPattern, CancellationToken token)
        {
            var sourceDirectory = new DirectoryInfo(context.DirectoryEncrypted);
            if (!sourceDirectory.Exists)
            {
                logger.ErrorFormat("Encryption Directory \"{0}\" was not found or has been deleted.",
                    context.DirectoryEncrypted);
                return;
            }

            var sourceFiles = sourceDirectory.GetFiles(searchPattern);
            logger.TraceFormat("Identified {0} file(s) in directory \"{1}\" with search pattern \"{2}\".",
                sourceFiles.Length, sourceDirectory.FullName, searchPattern);

            if (sourceFiles.Length == 0)
                return;

            DecryptFiles(pgpEncryption, context, sourceFiles, token);
        }

        public void DecryptFiles(IPgpEncryption pgpEncryption, DecryptFileContext context, FileInfo[] sourceFiles, CancellationToken token)
        {
            foreach (var sourceFile in sourceFiles)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                var targetFileName = string.Concat(sourceFile.Name, ".txt");
                var targetPath = Path.Combine(context.DirectoryDecrypted, targetFileName);

                try
                {
                    logger.DebugFormat("Decrypting \"{0}\" to \"{1}\".", sourceFile.FullName, targetPath);
                    pgpEncryption.DecryptFile(sourceFile.FullName, targetPath);

                    logger.InfoFormat("Decrypted file \"{0}\" to \"{1}\".", sourceFile.FullName, targetPath);

                    var archivePath = Path.Combine(context.DirectoryArchive, "Encrypted", sourceFile.Name);
                    sourceFile.MoveTo(archivePath);
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat(ex, "Unknown error occurred while decrypting file \"{0}\".", sourceFile.FullName);

                    var exceptionPath = Path.Combine(context.DirectoryException, sourceFile.Name);
                    sourceFile.MoveTo(exceptionPath);
                }
            }
        }
    }
}

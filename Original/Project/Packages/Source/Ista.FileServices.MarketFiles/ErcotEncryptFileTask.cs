using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Transactions;
using Ista.FileServices.Infrastructure;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Contexts;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.Models;
using Ista.Miramar.Interfaces;

namespace Ista.FileServices.MarketFiles
{
    public class ErcotEncryptFileTask : AbstractErcotFileTask, IMiramarTask
    {
        private readonly int clientId;
        private readonly ILogger logger;
        private readonly IAdminDataAccess adminDataAccess;
        private readonly IClientDataAccess clientDataAccess;
        private readonly IMarketFile marketDataAccess;

        public ErcotEncryptFileTask(IAdminDataAccess adminDataAccess, IClientDataAccess clientDataAccess, IMarketFile marketDataAccess, ILogger logger, int clientId)
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
                "Executing Ercot Encrypt Task with configuration: Client Id: {0} \nDecrypted: \"{1}\" \nEncrypted: \"{2}\" \nArchive: \"{3}\" \nException: \"{4}\" \n",
                clientId, context.DirectoryDecrypted, context.DirectoryEncrypted, context.DirectoryArchive,
                context.DirectoryException);

            using (logger.NestLog("Encrypt"))
            {
                Execute(context, token);
            }
        }

        public void Execute(ErcotFileContext context, CancellationToken token)
        {
            var marketFiles = marketDataAccess.ListInsertedOutboundMarketFiles()
                .Where(x => x.FileType.Equals("CBF", StringComparison.Ordinal))
                .ToArray();

            if (!marketFiles.Any())
                return;

            logger.DebugFormat("Encrypting {0} Customer Billing File(s).", marketFiles.Length);

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
                        "Unable to find Texas Port for Csp Duns Id {0}. Market File(s) ({1}) have been updated to Error status.",
                        item.CspDunsId, marketFileIds);

                    UpdateMarketFilesToError(item.MarketFiles, MissingCspDunsTexasPort);
                    continue;
                }

                if (!port.TransportEnabledFlag)
                {
                    logger.WarnFormat("Csp Duns Port {0} is not enabled for transport.", item.CspDunsId);
                    continue;
                }

                EncryptFiles(context, port, item.MarketFiles, token);
            }
        }

        public void EncryptFiles(ErcotFileContext context, CspDunsPortModel port, MarketFileModel[] models, CancellationToken token)
        {
            var pgpEncryption = InfrastructureFactory.CreatePgpEncryptor(port.PgpEncryptionKey, port.PgpSignatureKey, port.PgpPassphrase);
            var options = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };

            foreach (var marketFile in models)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                var sourcePath = Path.Combine(context.DirectoryDecrypted, marketFile.FileName);
                var sourceInfo = new FileInfo(sourcePath);
                if (!sourceInfo.Exists)
                {
                    logger.WarnFormat("Unable to encrypt file \"{0}\". File does not exist or has been deleted.",
                        marketFile.FileName);

                    continue;
                }

                try
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
                    {
                        var archivePath = Path.Combine(context.DirectoryArchive, DateTime.Now.ToString("yyyyMM"), marketFile.FileName);
                        sourceInfo.CopyTo(archivePath, true);

                        var targetPath = Path.Combine(context.DirectoryEncrypted, marketFile.FileName);
                        var targetName = string.Concat(targetPath, ".pgp");
                        pgpEncryption.EncryptFile(sourceInfo.FullName, targetName);

                        marketFile.Status = MarketFileStatusOptions.Encrypted;
                        marketFile.ProcessDate = DateTime.Now;
                        marketDataAccess.UpdateMarketFile(marketFile);

                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat(ex, "Unknown error occurred while encrypting file \"{0}\".",
                        marketFile.FileName);
                }
            }
        }
    }
}

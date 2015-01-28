using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Transactions;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Contexts;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Factories;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.Models;
using Ista.Miramar.Interfaces;

namespace Ista.FileServices.MarketFiles
{
    public class ExportMarketFileTask : IMiramarTask
    {
        private readonly int clientId;
        private readonly ILogger logger;
        private readonly IAdminDataAccess adminDataAccess;
        private readonly IMarketFile marketFileDataAccess;

        public ExportMarketFileTask(IAdminDataAccess adminDataAccess, IMarketFile marketFileDataAccess, ILogger logger, int clientId)
        {
            this.adminDataAccess = adminDataAccess;
            this.marketFileDataAccess = marketFileDataAccess;
            this.logger = logger;
            this.clientId = clientId;
        }

        public void Execute(CancellationToken token)
        {
            var providers = adminDataAccess.ListProviders(clientId);
            var configuration = adminDataAccess.LoadExportConfiguration(clientId);

            var contexts = ExportContextFactory.CreateContextList(providers, configuration);
            foreach (var context in contexts)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                logger.TraceFormat(
                    "Executing Export Task with configuration: Client Id: {0}; Provider: {1}; File Type: \"{2}\"; Extension: \"{3}\" \nDirectory Out: \"{4}\" \n",
                    clientId, context.ProviderId, context.FileType, context.Extension, context.DirectoryOut);

                using (logger.NestLog("Export"))
                {
                    Execute(context, token);
                }
            }
        }

        public void Execute(ExportFileContext context, CancellationToken token)
        {
            var directoryInfo = new DirectoryInfo(context.DirectoryOut);
            if (!directoryInfo.Exists)
                throw new DirectoryNotFoundException();

            var exporter = ExportParserFactory.GetParser(context, logger);
            using (logger.NestLog(context.FileType))
            {
                var results = exporter.Export(token);

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                if (results == null || !results.Any())
                    return;

                logger.DebugFormat("Identified {0} \"{1}\" files(s) to export.", results.Length, context.FileType);
                var handler = ExportHandlerFactory.GetHandler(context, logger);

                HandleDuplicateLdcTradingPartners(results);
                foreach (var result in results)
                {
                    if (result.HeaderCount == 0)
                        continue;

                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();

                    ExportResult(context, result, handler);
                }
            }
        }

        public void ExportResult(ExportFileContext context, IMarketFileExportResult result, IExportTransactionHandler handler)
        {
            var fileName = result.GenerateFileName(context.FileType, context.Extension);
            var marketFile = new MarketFileModel
            {
                DirectionFlag = false,
                FileName = fileName,
                FileType = context.FileType,
                ProcessError = string.Empty,
                ProcessStatus = "N",
                LdcId = result.LdcId,
                CspDunsId = result.CspDunsId,
                CspDunsTradingPartnerId = result.CspDunsTradingPartnerId,
            };

            try
            {
                var filePath = Path.Combine(context.DirectoryOut, fileName);
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                    throw new InvalidOperationException();

                var options = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
                using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
                {
                    marketFile.ProcessDate = DateTime.Now;
                    marketFile.Status = MarketFileStatusOptions.Inserted;
                    var marketFileId = marketFileDataAccess
                        .InsertMarketFile(marketFile);

                    using (var stream = fileInfo.Open(FileMode.Create, FileAccess.Write, FileShare.None))
                    using (var writer = new StreamWriter(stream))
                    {
                        result.FinalizeDocument(marketFileId);
                        writer.Write(result.Content);
                        logger.DebugFormat("Wrote {0} \"{1}\" transaction(s) to file \"{2}\".",
                            result.HeaderCount, context.FileType, fileName);
                    }

                    foreach (var headerKey in result.HeaderKeys)
                        handler.UpdateHeader(headerKey, marketFileId, fileName);

                    logger.InfoFormat("Exported {0} \"{1}\" transaction(s). File Name \"{2}\".",
                        result.HeaderCount, context.FileType, fileName);

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(ex, "Unknown error occurred while exporting file \"{0}\".", context.FileType);
            }
        }

        public void HandleDuplicateLdcTradingPartners(IMarketFileExportResult[] results)
        {
            if (!results.Any())
                return;

            var groupedDuplicates = results
                .GroupBy(x => new
                {
                    x.LdcShortName, 
                    TradingPartnerId = (string.IsNullOrEmpty(x.TradingPartnerId) 
                        ? string.Empty
                        : x.TradingPartnerId.Substring(0, 3))
                })
                .Select(x => new
                {
                    x.Key.LdcShortName,
                    x.Key.TradingPartnerId,
                    Duplicates = x.ToArray(),
                }).ToArray();

            if (!groupedDuplicates.Any())
                return;

            foreach (var duplicates in groupedDuplicates.Select(item => item.Duplicates))
            {
                if (duplicates.Length <= 1)
                    continue;

                for (int index = 0, count = 1; index < duplicates.Length; index++, count++)
                    duplicates[index].DuplicateFileIdentifier = count;
            }
        }
    }
}

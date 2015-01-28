using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
    public class ImportMarketFileTask : IMiramarTask
    {
        private readonly int clientId;
        private readonly ILogger logger;
        private readonly IAdminDataAccess adminDataAccess;
        private readonly IMarketFile marketFileDataAccess;

        public ImportMarketFileTask(IAdminDataAccess adminDataAccess, IMarketFile marketFileDataAccess, ILogger logger, int clientId)
        {
            this.adminDataAccess = adminDataAccess;
            this.marketFileDataAccess = marketFileDataAccess;
            this.logger = logger;
            this.clientId = clientId;
        }

        public void Execute(CancellationToken token)
        {
            var providers = adminDataAccess.ListProviders(clientId);
            var configuration = adminDataAccess.LoadImportConfiguration(clientId);

            var contexts = ImportContextFactory.CreateContextList(providers, configuration);
            foreach (var context in contexts)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                logger.TraceFormat(
                    "Executing Import Task with configuration: Client Id: {0}; Provider: {1}; File Type: \"{2}\"; File Pattern: \"{3}\" \nDirectory In: \"{4}\" \nDirectory Archive: \"{5}\" \nDirectory Exception: \"{6}\" \n",
                    clientId, context.ProviderId, context.FileType, context.FilePattern, context.DirectoryIn,
                    context.DirectoryInArchive, context.DirectoryInException);

                using (logger.NestLog("Import"))
                {
                    Execute(context, token);
                }
            }
        }

        public void Execute(ImportFileContext context, CancellationToken token)
        {
            var directoryInfo = new DirectoryInfo(context.DirectoryIn);
            if (!directoryInfo.Exists)
                throw new DirectoryNotFoundException();

            var fileList = directoryInfo.GetFiles(context.FilePattern, SearchOption.TopDirectoryOnly);
            logger.TraceFormat("Identified {0} file(s) in directory \"{1}\" with search pattern \"{2}\".",
                fileList.Length, directoryInfo.FullName, context.FilePattern);

            if (fileList.Length == 0)
                return;

            var parser = ImportParserFactory.GetParser(context, logger);
            var handler = ImportHandlerFactory.GetHandler(context, logger);

            logger.DebugFormat("Iterating over {0} \"{1}\" file(s).", fileList.Length, context.FileType);
            using (logger.NestLog(context.FileType))
            {
                foreach (var file in fileList)
                {
                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();

                    if (context.ProviderId == 1)
                    {
                        var fileExists = marketFileDataAccess.MarketFileExists(file.Name);
                        if (fileExists)
                        {
                            logger.ErrorFormat("{0} Transaction file \"{1}\" already exists.", context.FileType,
                                file.Name);
                            logger.DebugFormat("File \"{0}\" moved to Exception folder.", file.Name);

                            MoveFile(file, context.DirectoryInException);
                            continue;
                        }
                    }

                    var result = ParseFile(context, file, parser);
                    if (result == null)
                    {
                        logger.ErrorFormat("Unable to parse file \"{0}\".", file.Name);
                        continue;
                    }

                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();

                    logger.DebugFormat("Parsed {0} \"{1}\" transaction(s) from file \"{2}\".",
                        result.Headers.Length, context.FileType, file.Name);

                    var headers = result.Headers;
                    if (!headers.Any())
                    {
                        var archivePath = Path.Combine(context.DirectoryInArchive, DateTime.Now.ToString("yyyyMM"));
                        MoveFile(file, archivePath);

                        continue;
                    }

                    ImportResult(context, file, result, handler);
                }
            }
        }

        public IMarketFileParseResult ParseFile(ImportFileContext context, FileInfo file, IMarketFileParser parser)
        {
            try
            {
                using (var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return parser.Parse(stream);
                }
            }
            catch (Exception ex)
            {
                if (ex is IOException)
                {
                    var code = Marshal.GetHRForException(ex) & ((1 << 16) - 1);
                    if (code == 32 || code == 33)
                    {
                        logger.ErrorFormat(ex,
                            "Unable to access file \"{0}\". Attempt to read file will be made in the next iteration.",
                            file.FullName);

                        return null;
                    }
                }

                logger.ErrorFormat(ex, "Unknown error occurred while parsing file \"{0}\".", file);

                MarkFileAsErrored(context, file, ex);
                MoveFile(file, context.DirectoryInException);
            }

            return null;
        }

        public void ImportResult(ImportFileContext context, FileInfo file, IMarketFileParseResult result, IImportTransactionHandler handler)
        {
            var imported = false;
            var marketFile = new MarketFileModel
            {
                DirectionFlag = true,
                FileName = file.Name,
                FileType = context.FileType,
                ProcessError = string.Empty,
                ProcessStatus = "N",
                LdcId = 0,
                CspDunsId = 0,
            };
            
            try
            {
                var options = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
                using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
                {
                    if (context.ProviderId == 2)
                    {
                        marketFile.ProcessStatus = "Y";
                        marketFile.SenderTranNum = result.InterchangeControlNbr;
                        marketFile.TransactionCount = result.TransactionAuditCount;
                    }

                    marketFile.Status = MarketFileStatusOptions.Imported;
                    marketFile.ProcessDate = DateTime.Now;
                    var marketFileId = marketFileDataAccess.InsertMarketFile(marketFile);

                    foreach (var header in result.Headers)
                        handler.ProcessHeader(header, marketFileId);

                    marketFileDataAccess.InsertAuditRecord(marketFileId,
                        result.TransactionAuditCount, result.TransactionActualCount);

                    if (result.TransactionAuditCount != result.TransactionActualCount)
                        logger.ErrorFormat(
                            "Transaction Count does not match Transaction Audit Count. File Name \"{0}\".", file.Name);

                    logger.InfoFormat("Imported {0} \"{1}\" transaction(s). File Name \"{2}\".",
                        result.Headers.Length, context.FileType, file.Name);

                    imported = true;
                    scope.Complete();
                }

                var archivePath = Path.Combine(context.DirectoryInArchive, DateTime.Now.ToString("yyyyMM"));
                MoveFile(file, archivePath);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(ex, "Unknown error occurred while import file \"{0}\".", file);

                if (!imported)
                    MarkFileAsErrored(context, file, ex);
                MoveFile(file, context.DirectoryInException);
            }
        }

        public void MarkFileAsErrored(ImportFileContext context, FileInfo file, Exception ex)
        {
            marketFileDataAccess.InsertMarketFile(new MarketFileModel
            {
                CspDunsId = 0,
                LdcId = 0,
                DirectionFlag = true,
                FileName = file.Name,
                FileType = context.FileType,
                ProcessError = ex.Message,
                ProcessStatus = "N",
                Status = MarketFileStatusOptions.Error,
            });
        }

        public void MoveFile(FileInfo file, string targetDirectory)
        {
            var targetName = Path.Combine(targetDirectory, file.Name);
            var targetInfo = new FileInfo(targetName);
            if (targetInfo.Exists)
                targetInfo.Delete();

            file.MoveTo(targetName);
        }
    }
}

using System;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Handlers
{
    public class ImportClientInfoHandler : IImportTransactionHandler
    {
        private readonly ILogger logger;
        private readonly IClientCustomerInfoImport clientDataAccess;

        public ImportClientInfoHandler(IClientCustomerInfoImport clientDataAccess, ILogger logger)
        {
            this.clientDataAccess = clientDataAccess;
            this.logger = logger;
        }

        public void SaveHeader(TypeCustomerInfoFile file)
        {
            logger.Trace("Start inserting customer info file.");

            var fileKey = clientDataAccess.InsertFile(file);
            logger.DebugFormat("Inserted Customer Billing File \"{0}\".", fileKey);

            foreach (var errorRecord in file.ErrorRecords)
            {
                errorRecord.FileId = fileKey;
                var errorKey = clientDataAccess.InsertErrorRecord(errorRecord);
                logger.DebugFormat("Inserted Error Record \"{0}\" for Customer Billing File \"{1}\".", errorKey, fileKey);
            }

            logger.Trace("Completed inserting customer info file.");
        }

        public void ProcessHeader(IMarketHeaderModel model, int marketFileId)
        {
            var file = model as TypeCustomerInfoFile;
            if (file == null)
                throw new InvalidOperationException();

            SaveHeader(file);
        }
    }
}

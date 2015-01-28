using System;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;

namespace Ista.FileServices.MarketFiles.Handlers
{
    public class ExportCustomerInfoHandler : IExportTransactionHandler
    {
        private readonly ILogger logger;
        private readonly IClientCustomerInfoExport clientDataAccess;

        public ExportCustomerInfoHandler(IClientCustomerInfoExport clientDataAccess, ILogger logger)
        {
            this.clientDataAccess = clientDataAccess;
            this.logger = logger;
        }

        public void UpdateHeader(int fileId, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException("fileName");

            logger.Trace("Start update customer billing file.");

            clientDataAccess.UpdateCustomerInfoFile(fileId, CustomerInfoFileStatusOptions.Sent);

            logger.Trace("Completed update customer billing file.");
        }

        void IExportTransactionHandler.UpdateHeader(int headerKey, int marketFileId, string fileName)
        {
            UpdateHeader(headerKey, fileName);
        }
    }
}

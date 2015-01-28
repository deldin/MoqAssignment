using System;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;

namespace Ista.FileServices.MarketFiles.Handlers
{
    public class Export810Handler : IExportTransactionHandler
    {
        private readonly ILogger logger;
        private readonly IMarket810Export marketDataAccess;

        public Export810Handler(IMarket810Export marketDataAccess, ILogger logger)
        {
            this.marketDataAccess = marketDataAccess;
            this.logger = logger;
        }

        public void UpdateHeader(int headerKey, int marketFileId, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException("fileName");

            marketDataAccess.UpdateHeader(headerKey, marketFileId, fileName);
            logger.DebugFormat("Updated 810 Header {0} with Market File Id {1}", headerKey, marketFileId);
        }

        void IExportTransactionHandler.UpdateHeader(int headerKey, int marketFileId, string fileName)
        {
            UpdateHeader(headerKey, marketFileId, fileName);
        }
    }
}

using System;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Handlers
{
    public class Import248Handler : IImportTransactionHandler
    {
        private readonly ILogger logger;
        private readonly IMarket248Import marketDataAccess;

        public Import248Handler(IMarket248Import marketDataAccess, ILogger logger)
        {
            this.marketDataAccess = marketDataAccess;
            this.logger = logger;
        }

        public void SaveHeader(Type248Header header, int marketFileId)
        {
            header.MarketFileId = marketFileId;
            SaveHeader(header);
        }

        public void SaveHeader(Type248Header header)
        {
            logger.Trace("Start inserting header.");

            var headerKey = marketDataAccess.InsertHeader(header);
            logger.InfoFormat("Inserted Header \"{0}\".", headerKey);

            foreach (var detail in header.Details)
            {
                detail.HeaderKey = headerKey;
                var detailKey = marketDataAccess.InsertDetail(detail);
                logger.DebugFormat("Inserted Detail \"{0}\" for Header \"{1}\".", detailKey, headerKey);
            }

            logger.Trace("Completed inserted header.");
        }

        void IImportTransactionHandler.ProcessHeader(IMarketHeaderModel model, int marketFileId)
        {
            var header = model as Type248Header;
            if (header == null)
                throw new InvalidOperationException();

            SaveHeader(header, marketFileId);
        }
    }
}

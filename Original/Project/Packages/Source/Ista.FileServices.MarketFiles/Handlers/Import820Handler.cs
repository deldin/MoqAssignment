using System;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Handlers
{
    public class Import820Handler : IImportTransactionHandler
    {
        private readonly ILogger logger;
        private readonly IMarket820Import marketDataAccess;

        public Import820Handler(IMarket820Import marketDataAccess, ILogger logger)
        {
            this.marketDataAccess = marketDataAccess;
            this.logger = logger;
        }

        public void SaveHeader(Type820Header header, int marketFileId)
        {
            header.MarketFileId = marketFileId;
            SaveHeader(header);
        }

        public void SaveHeader(Type820Header header)
        {
            logger.Trace("Start inserting header.");

            var headerKey = marketDataAccess.InsertHeader(header);
            logger.DebugFormat("Inserted Header \"{0}\".", headerKey);

            if (header.HeaderKey > 0)
            { 
                foreach (var detail in header.Details)
                {
                    detail.HeaderKey = headerKey;
                    var detailKey = marketDataAccess.InsertDetail(detail);
                    logger.DebugFormat("Inserted Detail \"{0}\" for Header \"{1}\".", detailKey, headerKey);
                }
            }
            else
            {
                logger.WarnFormat("Inserted Header returned Header Key: {0}. Failed duplicate check in insert stored procedure.");
            }

            logger.Trace("Completed inserting header.");
        }

        void IImportTransactionHandler.ProcessHeader(IMarketHeaderModel model, int marketFileId)
        {
            var header = model as Type820Header;
            if (header == null)
                throw new InvalidOperationException();

            SaveHeader(header, marketFileId);
        }
    }
}

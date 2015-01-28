using System;
using System.Linq;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Handlers
{
    public class Import824Handler : IImportTransactionHandler
    {
        private readonly ILogger logger;
        private readonly IMarket824Import marketDataAccess;

        public Import824Handler(IMarket824Import marketDataAccess, ILogger logger)
        {
            this.marketDataAccess = marketDataAccess;
            this.logger = logger;
        }

        public void SaveHeader(Type824Header header, int marketFileId)
        {
            header.MarketFileId = marketFileId;
            SaveHeader(header);
        }

        public void SaveHeader(Type824Header header)
        {
            logger.Trace("Start inserting header.");

            var headerKey = marketDataAccess.InsertHeader(header);
            logger.DebugFormat("Inserted Header \"{0}\".", headerKey);

            foreach (var reason in header.Reasons)
            {
                reason.HeaderKey = headerKey;
                var reasonKey = marketDataAccess.InsertReason(reason);
                logger.DebugFormat("Inserted Reason \"{0}\" for Header \"{1}\".", reasonKey, headerKey);
            }

            foreach (var reference in header.References)
            {
                reference.HeaderKey = headerKey;
                var referenceKey = marketDataAccess.InsertReference(reference);
                logger.DebugFormat("Inserted Reference \"{0}\" for Header \"{1}\".", referenceKey, headerKey);

                if (!reference.TechErrors.Any())
                    continue;

                foreach (var techError in reference.TechErrors)
                {
                    techError.ReferenceKey = referenceKey;
                    var techKey = marketDataAccess.InsertTechError(techError);
                    logger.DebugFormat("Inserted Tech Error \"{0}\" for Reference \"{1}", techKey, referenceKey);
                }
            }

            logger.Trace("Completed inserting header.");
        }

        void IImportTransactionHandler.ProcessHeader(IMarketHeaderModel model, int marketFileId)
        {
            var header = model as Type824Header;
            if (header == null)
                throw new InvalidOperationException();

            SaveHeader(header, marketFileId);
        }
    }
}

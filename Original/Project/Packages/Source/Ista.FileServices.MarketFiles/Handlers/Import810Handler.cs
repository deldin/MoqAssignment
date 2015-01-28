using System;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Handlers
{
    public class Import810Handler : IImportTransactionHandler
    {
        private readonly ILogger logger;
        private readonly IMarket810Import marketDataAccess;

        public Import810Handler(IMarket810Import marketDataAccess, ILogger logger)
        {
            this.marketDataAccess = marketDataAccess;
            this.logger = logger;
        }

        public void SaveHeader(Type810Header header, int marketFileId)
        {
            header.MarketFileId = marketFileId;
            SaveHeader(header);
        }

        public void SaveHeader(Type810Header header)
        {
            logger.Trace("Start inserting header.");

            var headerKey = marketDataAccess.InsertHeader(header);
            logger.DebugFormat("Inserted Header \"{0}\".", headerKey);

            foreach(var balance in header.Balances)
            {
                balance.HeaderKey = headerKey;
                var balanceKey = marketDataAccess.InsertBalance(balance);
                logger.DebugFormat("Inserted Balance \"{0}\" for Header \"{1}\".", balanceKey, headerKey);
            }

            foreach(var payment in header.Payments)
            {
                payment.HeaderKey = headerKey;
                var paymentKey = marketDataAccess.InsertPayment(payment);
                logger.DebugFormat("Inserted Payment \"{0}\" for Header \"{1}\".", paymentKey, headerKey);
            }

            foreach (var detail in header.Details)
            {
                detail.HeaderKey = headerKey;
                var detailKey = marketDataAccess.InsertDetail(detail);
                logger.DebugFormat("Inserted Detail \"{0}\" for Header \"{1}\".", detailKey, headerKey);

                foreach (var detailItem in detail.Items)
                {
                    detailItem.DetailKey = detailKey;
                    var detailItemKey = marketDataAccess.InsertDetailItem(detailItem);
                    logger.DebugFormat("Inserted DetailItem \"{0}\" for Detail \"{1}\".", detailItemKey, detailKey);

                    foreach (var detailItemCharge in detailItem.Charges)
                    {
                        detailItemCharge.ItemKey = detailItemKey;
                        var detailItemChargeKey = marketDataAccess.InsertDetailItemCharge(detailItemCharge);
                        logger.DebugFormat("Inserted DetailItemCharge \"{0}\" for DetailItem \"{1}\".", detailItemChargeKey, detailItemKey);
                    }

                    foreach (var detailItemTax in detailItem.Taxes)
                    {
                        detailItemTax.ItemKey = detailItemKey;
                        var detailItemTaxKey = marketDataAccess.InsertDetailItemTax(detailItemTax);
                        logger.DebugFormat("Inserted DetailItemTax \"{0}\" for DetailItem \"{1}\".", detailItemTaxKey, detailItemKey);
                    }
                }

                foreach (var detailTax in detail.Taxes)
                {
                    detailTax.DetailKey = detailKey;
                    var detailTaxKey = marketDataAccess.InsertDetailTax(detailTax);
                    logger.DebugFormat("Inserted DetailTax \"{0}\" for Detail \"{1}\".", detailTaxKey, detailKey);
                }
            }

            foreach (var name in header.Names)
            {
                name.HeaderKey = headerKey;
                var nameKey = marketDataAccess.InsertName(name);
                logger.DebugFormat("Inserted Name \"{0}\" for Header \"{1}\".", nameKey, headerKey);
            }

            foreach (var summary in header.Summaries)
            {
                summary.HeaderKey = headerKey;
                var summaryKey = marketDataAccess.InsertSummary(summary);
                logger.DebugFormat("Inserted Summary \"{0}\" for Header \"{1}\".", summaryKey, headerKey);
            }

            foreach (var message in header.Messages)
            {
                message.HeaderKey = headerKey;
                var messageKey = marketDataAccess.InsertMessage(message);
                logger.DebugFormat("Inserted Message \"{0}\" for Header \"{1}\".", messageKey, headerKey);
            }

            logger.Trace("Completed inserting header.");
        }

        void IImportTransactionHandler.ProcessHeader(IMarketHeaderModel model, int marketFileId)
        {
            var header = model as Type810Header;
            if (header == null)
                throw new InvalidOperationException();

            SaveHeader(header, marketFileId);
        }
    }
}
using System;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Handlers
{
    public class Import997Handler : IImportTransactionHandler
    {
        private readonly ILogger logger;
        private readonly IMarket997Import marketDataAccess;

        public Import997Handler(IMarket997Import marketDataAccess, ILogger logger)
        {
            this.marketDataAccess = marketDataAccess;
            this.logger = logger;
        }

        public void SaveHeader(Type997Header header, int marketFileId)
        {
            header.MarketFileId = marketFileId;
            SaveHeader(header);
        }

        public void SaveHeader(Type997Header header)
        {
            logger.Trace("Start inserting header.");

            var headerKey = marketDataAccess.InsertHeader(header);
            logger.DebugFormat("Inserted Header \"{0}\".", headerKey);

            foreach (var response in header.Responses)
            {
                response.HeaderKey = headerKey;
                var responseKey = marketDataAccess.InsertResponse(response);
                logger.DebugFormat("Inserted Response \"{0}\" for Header \"{1}\".", responseKey, headerKey);

                foreach (var note in response.Notes)
                {
                    note.ResponseKey = responseKey;
                    var noteKey = marketDataAccess.InsertResponseNote(note);
                    logger.DebugFormat("Inserted Response Note \"{0}\" for Response \"{1}\".", noteKey, responseKey);
                }
            }

            logger.Trace("Completed inserting header.");
        }

        void IImportTransactionHandler.ProcessHeader(IMarketHeaderModel model, int marketFileId)
        {
            var header = model as Type997Header;
            if (header == null)
                throw new InvalidOperationException();

            SaveHeader(header, marketFileId);
        }
    }
}
using System;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Handlers
{
    public class Import650Handler : IImportTransactionHandler
    {
        private readonly ILogger logger;
        private readonly IMarket650Import marketDataAccess;

        public Import650Handler(IMarket650Import marketDataAccess, ILogger logger)
        {
            this.marketDataAccess = marketDataAccess;
            this.logger = logger;
        }

        public void SaveHeader(Type650Header header, int marketFileId)
        {
            header.MarketFileId = marketFileId;
            SaveHeader(header);
        }

        public void SaveHeader(Type650Header header)
        {
            logger.Trace("Start inserting header.");

            var headerKey = marketDataAccess.InsertHeader(header);
            logger.DebugFormat("Inserted Header \"{0}\".", headerKey);

            foreach (var name in header.Names)
            {
                name.HeaderKey = headerKey;
                var nameKey = marketDataAccess.InsertName(name);
                logger.DebugFormat("Inserted Name \"{0}\" for Header \"{1}\".", nameKey, headerKey);
            }

            foreach (var service in header.Services)
            {
                service.HeaderKey = headerKey;
                var serviceKey = marketDataAccess.InsertService(service);
                logger.DebugFormat("Inserted Service \"{0}\" for Header \"{1}\".", serviceKey, headerKey);

                foreach (var change in service.Changes)
                {
                    change.ServiceKey = serviceKey;
                    var changeKey = marketDataAccess.InsertServiceChange(change);
                    logger.DebugFormat("Inserted Service Change \"{0}\" for Service \"{1}\".", changeKey, serviceKey);
                }

                foreach (var meter in service.Meters)
                {
                    meter.ServiceKey = serviceKey;
                    var meterKey = marketDataAccess.InsertServiceMeter(meter);
                    logger.DebugFormat("Inserted Service Meter \"{0}\" for Service \"{1}\".", meterKey, serviceKey);
                }

                foreach (var pole in service.Poles)
                {
                    pole.ServiceKey = serviceKey;
                    var poleKey = marketDataAccess.InsertServicePole(pole);
                    logger.DebugFormat("Inserted Service Pole \"{0}\" for Service \"{1}\".", poleKey, serviceKey);
                }

                foreach (var reject in service.Rejects)
                {
                    reject.ServiceKey = serviceKey;
                    var rejectKey = marketDataAccess.InsertServiceReject(reject);
                    logger.DebugFormat("Inserted Service Reject \"{0}\" for Service \"{1}\".", rejectKey, serviceKey);
                }
            }

            logger.Trace("Completed inserting header.");
        }

        void IImportTransactionHandler.ProcessHeader(IMarketHeaderModel model, int marketFileId)
        {
            var header = model as Type650Header;
            if (header == null)
                throw new InvalidOperationException();

            SaveHeader(header, marketFileId);
        }
    }
}

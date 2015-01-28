using System;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Handlers
{
    public class Import814Handler : IImportTransactionHandler
    {
        private readonly ILogger logger;
        private readonly IMarket814Import marketDataAccess;

        public Import814Handler(IMarket814Import marketDataAccess, ILogger logger)
        {
            this.marketDataAccess = marketDataAccess;
            this.logger = logger;
        }

        public void SaveHeader(Type814Header header, int marketFileId)
        {
            header.MarketFileId = marketFileId;
            SaveHeader(header);
        }

        public void SaveHeader(Type814Header header)
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

                foreach (var serviceAccountChange in service.Changes)
                {
                    serviceAccountChange.ServiceKey = serviceKey;
                    var serviceAccountChangeKey = marketDataAccess.InsertServiceAccountChange(serviceAccountChange);
                    logger.DebugFormat("Inserted Service Account Change \"{0}\" for Service \"{1}\".",
                        serviceAccountChangeKey, serviceKey);
                }

                foreach (var serviceDate in service.Dates)
                {
                    serviceDate.ServiceKey = serviceKey;
                    var serviceDateKey = marketDataAccess.InsertServiceDate(serviceDate);
                    logger.DebugFormat("Inserted Service Date \"{0}\" for Service \"{1}\".", serviceDateKey, serviceKey);
                }

                foreach (var serviceMeter in service.Meters)
                {
                    serviceMeter.ServiceKey = serviceKey;
                    var serviceMeterKey = marketDataAccess.InsertServiceMeter(serviceMeter);
                    logger.DebugFormat("Inserted Service Meter \"{0}\" for Service \"{1}\".", serviceMeterKey, serviceKey);

                    foreach(var serviceMeterChange in serviceMeter.Changes)
                    {
                        serviceMeterChange.MeterKey = serviceMeterKey;
                        var meterChangeKey = marketDataAccess.InsertServiceMeterChange(serviceMeterChange);
                        logger.DebugFormat("Inserted Service Meter Change \"{0}\" for Service Meter \"{1}\".",
                            meterChangeKey, serviceMeterKey);
                    }

                    foreach (var serviceMeterTou in serviceMeter.Tous)
                    {
                        serviceMeterTou.MeterKey = serviceMeterKey;
                        var meterTouKey = marketDataAccess.InsertServiceMeterTou(serviceMeterTou);
                        logger.DebugFormat("Inserted Service Meter TOU \"{0}\" for Service Meter \"{1}\".",
                            meterTouKey, serviceMeterKey);
                    }

                    foreach (var serviceMeterType in serviceMeter.Types)
                    {
                        serviceMeterType.MeterKey = serviceMeterKey;
                        var meterTypeKey = marketDataAccess.InsertServiceMeterType(serviceMeterType);
                        logger.DebugFormat("Inserted Service Meter Type \"{0}\" for Service Meter \"{1}\".",
                            meterTypeKey, serviceMeterKey);
                    }
                }

                foreach (var serviceReject in service.Rejects)
                {
                    serviceReject.ServiceKey = serviceKey;
                    var serviceRejectKey = marketDataAccess.InsertServiceReject(serviceReject);
                    logger.DebugFormat("Inserted Service Reject \"{0}\" for Service \"{1}\".", serviceRejectKey,
                        serviceKey);
                }

                foreach (var serviceStatus in service.Statuses)
                {
                    serviceStatus.ServiceKey = serviceKey;
                    var serviceStatusKey = marketDataAccess.InsertServiceStatus(serviceStatus);
                    logger.DebugFormat("Inserted Service Reject \"{0}\" for Service \"{1}\".", serviceStatusKey,
                        serviceKey);
                }
            }

            logger.Trace("Completed inserting header.");
        }

        void IImportTransactionHandler.ProcessHeader(IMarketHeaderModel model, int marketFileId)
        {
            var header = model as Type814Header;
            if (header == null)
                throw new InvalidOperationException();

            SaveHeader(header, marketFileId);
        }
    }
}
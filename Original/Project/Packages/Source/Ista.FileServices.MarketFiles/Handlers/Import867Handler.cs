using System;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Handlers
{
    public class Import867Handler : IImportTransactionHandler
    {
        private readonly ILogger _logger;
        private readonly IMarket867Import _marketDataAccess;

        public Import867Handler(IMarket867Import marketDataAccess, ILogger logger)
        {
            _marketDataAccess = marketDataAccess;
           _logger = logger;
        }

        public void SaveHeader(Type867Header header, int marketFileId)
        {
            header.MarketFileId = marketFileId;
            SaveHeader(header);
        }

        public void SaveHeader(Type867Header header)
        {
            _logger.Trace("Start inserting header.");

            var headerKey = _marketDataAccess.InsertHeader(header);
            _logger.DebugFormat("Inserted Header \"{0}\".", headerKey);
            
            foreach (var name in header.Names)
            {
                name.HeaderKey = headerKey;
                _marketDataAccess.InsertName(name);
                _logger.DebugFormat("Inserted Name \"{0}\" for Header \"{1}\".", name.EntityName, headerKey);
            }

            foreach(var intervalDetail in header.IntervalDetails)
            {
                intervalDetail.HeaderKey = headerKey;
                var intervalDetailKey = _marketDataAccess.InsertIntervalDetail(intervalDetail);
                _logger.DebugFormat("Inserted Interval Detail \"{0}\" for Header \"{1}\".", intervalDetailKey, headerKey);

                foreach(var qty in intervalDetail.IntervalDetailQtys)
                {
                    qty.IntervalDetailKey = intervalDetailKey;
                    _marketDataAccess.InsertIntervalDetailQty(qty);
                    _logger.DebugFormat("Inserted Interval Detail Qty \"{0}\" for Interval Detail  \"{1}\" for Header\"{2}\".", qty.Qualifier, intervalDetailKey, headerKey);
                }
            }

            foreach(var intervalSummary in header.IntervalSummaries)
            {
                intervalSummary.HeaderKey = headerKey;
                var intervalSummaryKey = _marketDataAccess.InsertIntervalSummary(intervalSummary);
                _logger.DebugFormat("Inserted Interval Summary \"{0}\" for Header \"{1}\".", intervalSummaryKey, headerKey);

                foreach(var qty in intervalSummary.IntervalSummaryQtys)
                {
                    qty.IntervalSummaryKey = intervalSummaryKey;
                    _marketDataAccess.InsertIntervalSummaryQty(qty);
                    _logger.DebugFormat("Inserted Interval Summary Qty \"{0}\" for Interval Summary  \"{1}\" for Header\"{2}\".", qty.Qualifier, intervalSummaryKey, headerKey);
                }
            }
            
            foreach(var acctBillQty in header.AccountBillQtys)
            {
                acctBillQty.HeaderKey = headerKey;
                var acctBillQtyKey = _marketDataAccess.InsertAccountBillQuanity(acctBillQty);
                _logger.DebugFormat("Inserted Account Bill Quantity \"{0}\" for Header \"{1}\".", acctBillQtyKey, headerKey);
            }
            
            foreach(var gasProfileFactorEval in header.GasProfileFactorEvaluations)
            {
                gasProfileFactorEval.HeaderKey = headerKey;
                _marketDataAccess.InsertGasProfileFactorEvaluation(gasProfileFactorEval);
                _logger.DebugFormat("Inserted Gas Profile Factor Evaluation with [CustomerServiceInitDate] \"{0}\" for Header \"{1}\".", gasProfileFactorEval.CustomerServiceInitDate, headerKey);
            }
            
            foreach(var gasProfileFactorSample in header.GasProfileFactorSamples)
            {
                gasProfileFactorSample.HeaderKey = headerKey;
                _marketDataAccess.InsertGasProfileFactorSample(gasProfileFactorSample);
                _logger.DebugFormat("Inserted Gas Profile Factor Sample with [AnnualPeriod] \"{0}\" for Header \"{1}\".", gasProfileFactorSample.AnnualPeriod, headerKey);
            }
            
            foreach(var intervalSummaryAcrossMeter in header.IntervalSummaryAcrossMeters)
            {
                intervalSummaryAcrossMeter.HeaderKey = headerKey;
                var intervalSummaryAcrossMeterKey = _marketDataAccess.InsertIntervalSummaryAcrossMeters(intervalSummaryAcrossMeter);
                _logger.DebugFormat("Inserted Interval Summary Across Meter \"{0}\" for Header \"{1}\".", intervalSummaryAcrossMeterKey, headerKey);

                foreach(var qty in intervalSummaryAcrossMeter.IntervalSummaryAcrossMetersQtys)
                {
                    qty.IntervalSummaryAcrossMetersKey = intervalSummaryAcrossMeterKey;
                    _marketDataAccess.InsertIntervalSummaryAcrossMetersQty(qty);
                    _logger.DebugFormat("Inserted Interval Summary Across Meter Qty  with [Qualifier] \"{0}\" for Interval Summary Across Meter  \"{1}\" for Header\"{2}\".", qty.Qualifier, intervalSummaryAcrossMeterKey, headerKey);
                }
            }

            foreach(var netIntervalSummary in header.NetIntervalSummaries)
            {
                netIntervalSummary.HeaderKey = headerKey;
                var netIntervalSummaryKey = _marketDataAccess.InsertNetIntervalSummary(netIntervalSummary);
                _logger.DebugFormat("Inserted Net Interval Summary \"{0}\" for Header \"{1}\".", netIntervalSummaryKey, headerKey);

                foreach(var qty in netIntervalSummary.NetIntervalSummaryQtys)
                {
                    qty.NetIntervalSummaryKey = netIntervalSummaryKey;
                    _marketDataAccess.InsertNetIntervalSummaryQty(qty);
                    _logger.DebugFormat("Inserted Net Interval Summary Qty  with [Qualifier] \"{0}\" for Net Interval Summary   \"{1}\" for Header\"{2}\".", qty.Qualifier, netIntervalSummaryKey, headerKey);
                }
            }

            foreach(var nonIntervalSummary in header.NonIntervalSummaries)
            {
                nonIntervalSummary.HeaderKey = headerKey;
                var nonIntervalSummaryKey = _marketDataAccess.InsertNonIntervalSummary(nonIntervalSummary);
                _logger.DebugFormat("Inserted Non Interval Summary \"{0}\" for Header \"{1}\".", nonIntervalSummaryKey, headerKey);

                foreach(var qty in nonIntervalSummary.NonIntervalSummaryQtys)
                {
                    qty.NonIntervalSummaryKey = nonIntervalSummaryKey;
                    _marketDataAccess.InsertNonIntervalSummaryQty(qty);
                    _logger.DebugFormat("Inserted Non Interval Summary Qty  with [Qualifier] \"{0}\" for Non Interval Summary  \"{1}\" for Header\"{2}\".", qty.Qualifier, nonIntervalSummaryKey, headerKey);
                }
            }

            foreach(var nonIntervalDetail in header.NonIntervalDetails)
            {
                nonIntervalDetail.HeaderKey = headerKey;
                var nonIntervalDetailKey = _marketDataAccess.InsertNonIntervalDetail(nonIntervalDetail);
                _logger.DebugFormat("Inserted Non Interval Detail \"{0}\" for Header \"{1}\".", nonIntervalDetailKey, headerKey);

                foreach(var qty in nonIntervalDetail.NonIntervalDetailQtys)
                {
                    qty.NonIntervalDetailKey = nonIntervalDetailKey;
                    _marketDataAccess.InsertNonIntervalDetailQty(qty);
                    _logger.DebugFormat("Inserted Non Interval Detail Qty  with [Qualifier] \"{0}\" for Non Interval Detail  \"{1}\" for Header\"{2}\".", qty.Qualifier, nonIntervalDetailKey, headerKey);
                }
            }
            
            foreach(var scheduledDeterminant in header.ScheduleDeterminants)
            {
                scheduledDeterminant.HeaderKey = headerKey;
                var scheduledDeterminantKey = _marketDataAccess.InsertScheduleDeterminants(scheduledDeterminant);
                _logger.DebugFormat("Inserted Scheduled Determinant \"{0}\" for Header \"{1}\".", scheduledDeterminantKey, headerKey);
            }
            
            foreach(var headerSwitch in header.Switches)
            {
                headerSwitch.HeaderKey = headerKey;
                var headerSwitchKey = _marketDataAccess.InsertSwitch(headerSwitch);
                _logger.DebugFormat("Inserted Switch \"{0}\" for Header \"{1}\".", headerSwitchKey, headerKey);

                foreach(var qty in headerSwitch.SwitchQtys)
                {
                    qty.SwitchKey = headerSwitchKey;
                    _marketDataAccess.InsertSwitchQty(qty);
                    _logger.DebugFormat("Inserted Switch Qty  with [Qualifier] \"{0}\" for Switch  \"{1}\" for Header\"{2}\".", qty.Qualifier, headerSwitchKey, headerKey);
                }
            }
            
            foreach(var unMeterDetail in header.UnMeterDetails)
            {
                unMeterDetail.HeaderKey = headerKey;
                var unMeterDetailKey = _marketDataAccess.InsertUnMeterDetail(unMeterDetail);
                _logger.DebugFormat("Inserted UnMeter Detail \"{0}\" for Header \"{1}\".", unMeterDetailKey, headerKey);

                foreach(var qty in unMeterDetail.UnMeterDetailQtys)
                {
                    qty.UnMeterDetailKey = unMeterDetailKey;
                    _marketDataAccess.InsertUnMeterDetailQty( qty);
                    _logger.DebugFormat("Inserted UnMeter Detail Qty  with [Qualifier] \"{0}\" for UnMeter Detail  \"{1}\" for Header\"{2}\".", qty.Qualifier, unMeterDetailKey, headerKey);
                }
            }
            
            foreach(var unMeterSummary in header.UnMeterSummaries)
            {
                unMeterSummary.HeaderKey = headerKey;
                var unMeterSummaryKey = _marketDataAccess.InsertUnMeterSummary(unMeterSummary);
                _logger.DebugFormat("Inserted UnMeter Summary \"{0}\" for Header \"{1}\".", unMeterSummaryKey, headerKey);

                foreach(var qty in unMeterSummary.UnMeterSummaryQtys)
                {
                    qty.UnMeterSummaryKey = unMeterSummaryKey;
                    _marketDataAccess.InsertUnMeterSummaryQty(qty);
                     _logger.DebugFormat("Inserted UnMeter Summary Qty  with [Qualifier] \"{0}\" for UnMeter Summary  \"{1}\" for Header\"{2}\".", qty.Qualifier,unMeterSummaryKey, headerKey);
                }
            }
            
            _logger.Trace("Completed inserting header.");
        }

        public void ProcessHeader(IMarketHeaderModel model, int marketFileId)
        {
            var header = model as Type867Header;
            if (header == null)
                throw new InvalidOperationException();

            SaveHeader(header, marketFileId);
        }
    }
}

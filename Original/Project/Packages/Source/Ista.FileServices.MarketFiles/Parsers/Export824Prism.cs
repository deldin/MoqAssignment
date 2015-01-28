using System.Linq;
using System.Threading;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Parsers.ExportContexts;

namespace Ista.FileServices.MarketFiles.Parsers
{
    public class Export824Prism : IMarketFileExporter
    {
        private readonly IClientDataAccess clientDataAccess;
        private readonly IMarket824Export marketDataAccess;
        private readonly ILogger logger;

        public Export824Prism(IClientDataAccess clientDataAccess, IMarket824Export marketDataAccess, ILogger logger)
        {
            this.clientDataAccess = clientDataAccess;
            this.marketDataAccess = marketDataAccess;
            this.logger = logger;
        }

        public IMarketFileExportResult[] Export(CancellationToken token)
        {
            var cspDunsPorts = clientDataAccess.ListCspDunsPort();
            var prismPorts = cspDunsPorts
                .Where(x => x.ProviderId == 1)
                .ToArray();

            var context = new Prism824Context();
            foreach (var prismPort in prismPorts)
            {
                if (token.IsCancellationRequested)
                    break;

                var headers = marketDataAccess.ListUnprocessed(prismPort.LdcDuns, prismPort.Duns, 1);
                if (headers.Length == 0)
                {
                    logger.TraceFormat("Zero 824 Prism records found to export for TDSP Duns \"{0}\" and CR Duns \"{1}\".",
                        prismPort.LdcDuns, prismPort.Duns);
                    continue;
                }

                logger.DebugFormat("Exporting {0} unprocessed 824 record(s) for TDSP Duns \"{1}\" and CR Duns \"{2}\".",
                    headers.Length, prismPort.LdcDuns, prismPort.Duns);

                foreach (var header in headers)
                {
                    if (!header.HeaderKey.HasValue)
                        continue;

                    if (token.IsCancellationRequested)
                        break;

                    var headerKey = header.HeaderKey.Value;

                    context.Initialize();

                    var identifiedMarket = clientDataAccess.IdentifyMarket(header.TdspDuns);
                    if (identifiedMarket.HasValue)
                        context.SetMarket(identifiedMarket.Value);
                    
                    context.SetFileProperties(prismPort, header.TdspDuns, "ADV");
                    context.SetHeaderId(headerKey);

                    WriteHeader(context, header);
                    WriteHeaderData(context, header);
                    WriteTransactionInfo(context, header);
                    WriteReason(context, header);
                }
            }

            return context.Models;
        }

        public void WriteHeader(Prism824Context context, Type824Header header)
        {
            var line = string.Format("SH|{0}|{1}|O|", context.TradingPartnerId, header.TransactionNbr);
            context.AppendLine(line);
            logger.TraceFormat("Wrote 824 \"SH\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteHeaderData(Prism824Context context, Type824Header header)
        {
            var clearingHouseName = string.Empty;
            var clearingHouseDuns = string.Empty;

            if (context.Market == MarketOptions.Texas)
            {
                clearingHouseName = "ERCOT";
                clearingHouseDuns = "183529049";
            }

            var partnerId = context.TradingPartnerId;
            var stateId = partnerId.Substring(3, 2);

            var line = string.Format("01|{0}|{1}|{2}|{3}|{4}|{5}|{6}|||||{7}|{8}|||||||||||{9}|{10}|", partnerId,
                stateId, header.TransactionDate, header.TransactionNbr, header.ActionCode, header.TdspName,
                header.TdspDuns, header.CrName, header.CrDuns, clearingHouseName, clearingHouseDuns);

            context.AppendLine(line);
            logger.TraceFormat("Wrote 824 \"01\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteTransactionInfo(Prism824Context context, Type824Header header)
        {
            var esiId = IdentifyEsiId(context, header);

            var line = string.Format("10|{0}|{1}|{2}|{3}|||||{4}||", context.TradingPartnerId, header.AppAckCode,
                header.ReferenceNbr, header.TransactionSetNbr, esiId);

            context.AppendLine(line);
            logger.TraceFormat("Wrote 824 \"10\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteReason(Prism824Context context, Type824Header header)
        {
            if (!header.HeaderKey.HasValue)
                return;

            var headerKey = header.HeaderKey.Value;
            var reasons = marketDataAccess.ListReasons(headerKey);
            foreach (var reason in reasons)
            {
                var line = string.Format("20|{0}|{1}|{2}|", context.TradingPartnerId, reason.ReasonCode,
                    reason.ReasonText);

                context.AppendLine(line);
                logger.TraceFormat("Wrote 824 \"20\" PRISM line for Header {0}", header.HeaderKey);
            }
        }

        public string IdentifyEsiId(Prism824Context context, Type824Header header)
        {
            if (context.Market == MarketOptions.Maryland)
                return string.Empty;

            return header.EsiId;
        }
    }
}

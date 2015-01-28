using System.Linq;
using System.Threading;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Parsers.ExportContexts;

namespace Ista.FileServices.MarketFiles.Parsers
{
    public class Export820Prism : IMarketFileExporter
    {
        private readonly IClientDataAccess clientDataAccess;
        private readonly IMarket820Export marketDataAccess;
        private readonly ILogger logger;

        public Export820Prism(IClientDataAccess clientDataAccess, IMarket820Export marketDataAccess, ILogger logger)
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

            var context = new Prism820Context();
            foreach (var prismPort in prismPorts)
            {
                if (token.IsCancellationRequested)
                    break;

                var headers = marketDataAccess.ListUnprocessed(prismPort.LdcDuns, prismPort.Duns, 1);
                if (headers.Length == 0)
                {
                    logger.TraceFormat("Zero 820 Prism records found to export for TDSP Duns \"{0}\" and CR Duns \"{1}\".",
                        prismPort.LdcDuns, prismPort.Duns);
                    continue;
                }

                logger.DebugFormat("Exporting {0} unprocessed 820 record(s) for TDSP Duns \"{1}\" and CR Duns \"{2}\".",
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

                    context.SetFileProperties(prismPort, header.TdspDuns, "PYM");
                    context.SetHeaderId(headerKey);

                    WriteHeader(context, header);
                    WritePayment(context, header);
                    WriteDetail(context, header);
                }
            }

            return context.Models;
        }

        public void WriteHeader(Prism820Context context, Type820Header header)
        {
            var line = string.Format("SH|{0}|{1}|O|", context.TradingPartnerId, header.TraceReferenceNbr);
            context.AppendLine(line);
            logger.TraceFormat("Wrote 820 \"SH\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WritePayment(Prism820Context context, Type820Header header)
        {
            var partnerId = context.TradingPartnerId;
            var stateId = partnerId.Substring(3, 2);

            var line = string.Format("01|{0}|{1}|{2}|{3}|{4}||||||||{5}|{6}|{7}|{8}|{9}|{10}|{11}||||", partnerId,
                stateId, header.TransactionTypeCode, header.TotalAmount, header.PaymentMethodCode,
                header.TransactionDate, header.TraceReferenceNbr, header.CrName, header.CrDuns, header.TdspName,
                header.TdspDuns, header.CreditDebitFlag);

            context.AppendLine(line);
            logger.TraceFormat("Wrote 820 \"01\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteDetail(Prism820Context context, Type820Header header)
        {
            if (!header.HeaderKey.HasValue)
                return;

            var headerKey = header.HeaderKey.Value;
            var details = marketDataAccess.ListDetails(headerKey);
            if (details == null || details.Length == 0)
                return;

            foreach (var detail in details)
            {
                if (!detail.DetailKey.HasValue)
                    continue;

                var premiseNumber = IdentifyEsiId(context, detail);
                var membershipId = IdentifyLdcAccountNumber(context, detail);

                var line = string.Format("20|{0}|{5}||{1}|||||{2}||||{3}|||{4}|", context.TradingPartnerId,
                    detail.PaymentAmount, detail.CrossReferenceNbr, detail.ReferenceNbr, premiseNumber, membershipId);

                context.AppendLine(line);
                logger.TraceFormat("Wrote 820 \"20\" PRISM line for Header {0}", header.HeaderKey);
            }
        }

        public string IdentifyEsiId(Prism820Context context, Type820Detail detail)
        {
            if (context.Market == MarketOptions.Maryland)
                return string.Empty;

            return detail.EsiId;
        }

        public string IdentifyLdcAccountNumber(Prism820Context context, Type820Detail detail)
        {
            if (context.Market == MarketOptions.Maryland)
                return detail.EsiId;

            // original code has Util.GetString(dr, "MembershipID")
            // which is a column that was not found in the 820 Detail table
            // the "Util" call would result in an empty string being
            // returned

            return string.Empty;
        }
    }
}

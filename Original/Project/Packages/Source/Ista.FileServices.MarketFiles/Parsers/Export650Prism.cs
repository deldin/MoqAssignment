using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Parsers.ExportContexts;

namespace Ista.FileServices.MarketFiles.Parsers
{
    public class Export650Prism : IMarketFileExporter
    {
        private static readonly Regex numericExp = new Regex("[^0-9]");

        private readonly IClientDataAccess clientDataAccess;
        private readonly IMarket650Export marketDataAccess;
        private readonly ILogger logger;

        public Export650Prism(IClientDataAccess clientDataAccess, IMarket650Export marketDataAccess, ILogger logger)
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

            var context = new Prism650Context();
            foreach (var prismPort in prismPorts)
            {
                if (token.IsCancellationRequested)
                    break;

                var headers = marketDataAccess.ListUnprocessed(prismPort.LdcDuns, prismPort.Duns, 1);
                if (headers.Length == 0)
                {
                    logger.TraceFormat("Zero 650 Prism records found to export for TDSP Duns \"{0}\" and CR Duns \"{1}\".",
                        prismPort.LdcDuns, prismPort.Duns);
                    continue;
                }

                logger.DebugFormat("Exporting {0} unprocessed 650 record(s) for TDSP Duns \"{1}\" and CR Duns \"{2}\".",
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
                    
                    context.SetFileProperties(prismPort, header.TdspDuns, "MTR");
                    context.SetHeaderId(headerKey);
                    context.TransactionSetPurposeCode = header.TransactionSetPurposeCode;

                    WriteHeader(context, header);
                    WriteAccount(context, header);
                    WriteService(context, header);
                }
            }

            return context.Models;
        }

        public void WriteHeader(Prism650Context context, Type650Header header)
        {
            var line = string.Format("SH|{0}|{1}|O|", context.TradingPartnerId, header.TransactionNbr);
            context.AppendLine(line);
            logger.TraceFormat("Wrote 650 \"SH\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteAccount(Prism650Context context, Type650Header header)
        {
            if (!header.HeaderKey.HasValue)
                return;

            var headerKey = header.HeaderKey.Value;
            var names = marketDataAccess.ListNames(headerKey);
            var name = names.FirstOrDefault(x => x.EntityIdType.Equals("8R", StringComparison.Ordinal));

            if (name == null)
            {
                name = new Type650Name
                {
                    EntityName = string.Empty,
                    EntityName2 = string.Empty,
                    EntityName3 = string.Empty,
                    Address1 = string.Empty,
                    Address2 = string.Empty,
                    City = string.Empty,
                    State = string.Empty,
                    PostalCode = string.Empty,
                    ContactName = string.Empty,
                    ContactPhoneNbr1 = string.Empty,
                    ContactPhoneNbr2 = string.Empty,
                };
            }

            var phone1 = numericExp.Replace(name.ContactPhoneNbr1, string.Empty);
            var phone2 = numericExp.Replace(name.ContactPhoneNbr2, string.Empty);
            var postalCode = numericExp.Replace(name.PostalCode, string.Empty);

            if (phone1.Length > 16)
                phone1 = phone1.Substring(0, 16);

            if (phone2.Length > 16)
                phone2 = phone2.Substring(0, 16);

            var partnerId = context.TradingPartnerId;
            var stateId = partnerId.Substring(3, 2);

            var line =
                string.Format(
                    "01|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}|{20}|{21}|{22}|{23}|"
                    , partnerId
                    , stateId
                    , header.TransactionSetPurposeCode.ToUpper()
                    , header.TransactionDate.ToUpper()
                    , header.TransactionNbr.ToUpper()
                    , header.ReferenceNbr.ToUpper()
                    , header.TransactionType.ToUpper()
                    , header.ActionCode.ToUpper()
                    , name.EntityName.ToUpper().ToAscii()
                    , name.EntityName2.ToUpper().ToAscii()
                    , name.EntityName3.ToUpper().ToAscii()
                    , name.Address1.ToUpper()
                    , name.Address2.ToUpper()
                    , name.City.ToUpper()
                    , name.State.ToUpper()
                    , postalCode
                    , name.ContactName.ToUpper().ToAscii()
                    , phone1
                    , phone2
                    , header.TdspName.ToUpper()
                    , header.TdspDuns.ToUpper()
                    , header.CrName.ToUpper()
                    , header.CrDuns.ToUpper()
                    , header.ProcessedReceivedDateTime);

            context.AppendLine(line);
            logger.TraceFormat("Wrote 650 \"01\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteService(Prism650Context context, Type650Header header)
        {
            if (!header.HeaderKey.HasValue)
                return;

            var headerKey = header.HeaderKey.Value;
            var services = marketDataAccess.ListServices(headerKey);
            if (services == null || services.Length == 0)
            {
                logger.ErrorFormat("No service record for 650 Key {0}.", headerKey);
                return;
            }

            var service = services.First();
            var line =
                string.Format(
                    "10|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|||||||{9}|{10}|{11}|||||||||||||||||{12}|{13}|{14}|"
                    , context.TradingPartnerId
                    , service.PurposeCode.ToUpper()
                    , service.PriorityCode.ToUpper()
                    , service.EsiId.ToUpper()
                    , service.SpecialProcessCode.ToUpper()
                    , service.ServiceReqDate.ToUpper()
                    , service.NotBeforeDate.ToUpper()
                    , service.CallAhead.ToUpper()
                    , service.PremLocation.ToUpper()
                    , service.ReportRemarks.ToUpper()
                    , service.Directions.ToUpper().ToAscii()
                    , service.MeterNbr.ToUpper()
                    , service.Membership.ToUpper()
                    , service.RemarksPermanentSuspend.ToUpper()
                    , service.DisconnectAuthorization.ToUpper());

            context.AppendLine(line);
            logger.TraceFormat("Wrote 650 \"10\" PRISM line for Header {0}", header.HeaderKey);

            if (context.TransactionSetPurposeCode.Equals("13"))
            {
                WriteServicePole(context, service);
                WriteServiceChangeReason(context, service);
                return;
            }

            WriteServiceReject(context, service);
        }

        public void WriteServicePole(Prism650Context context, Type650Service service)
        {
            if (service == null)
                return;

            var serviceKey = service.ServiceKey ?? 0;
            if (serviceKey == 0)
                return;

            var poles = marketDataAccess.ListServicePoles(serviceKey);
            if (poles == null || poles.Length == 0)
                return;

            foreach (var pole in poles)
            {
                if (!pole.ServicePoleKey.HasValue)
                    continue;

                var line = string.Format("11|{0}|{1}|", context.TradingPartnerId, pole.PoleNbr);
                context.AppendLine(line);
                logger.TraceFormat("Wrote 650 \"11\" PRISM line for Header {0}", service.HeaderKey);
            }
        }

        public void WriteServiceChangeReason(Prism650Context context, Type650Service service)
        {
            if (service == null)
                return;

            var serviceKey = service.ServiceKey ?? 0;
            if (serviceKey == 0)
                return;

            var changes = marketDataAccess.ListServiceChanges(serviceKey);
            if (changes == null || changes.Length == 0)
                return;

            foreach (var change in changes)
            {
                if (!change.ServiceChangeKey.HasValue)
                    continue;

                var line = string.Format("12|{0}|{1}|", context.TradingPartnerId, change.ChangeReason);
                context.AppendLine(line);
                logger.TraceFormat("Wrote 650 \"12\" PRISM line for Header {0}", service.HeaderKey);
            }
        }

        public void WriteServiceReject(Prism650Context context, Type650Service service)
        {
            if (service == null)
                return;

            var serviceKey = service.ServiceKey ?? 0;
            if (serviceKey == 0)
                return;

            var rejects = marketDataAccess.ListServiceRejects(serviceKey);
            if (rejects == null || rejects.Length == 0)
                return;

            foreach(var reject in rejects)
            {
                if (!reject.ServiceRejectKey.HasValue)
                    continue;

                var line = string.Format("13|{0}|{1}|{2}|||", context.TradingPartnerId, reject.RejectCode,
                    reject.RejectReason);

                context.AppendLine(line);
                logger.TraceFormat("Wrote 650 \"13\" PRISM line for Header {0}", service.HeaderKey);
            }
        }
    }
}

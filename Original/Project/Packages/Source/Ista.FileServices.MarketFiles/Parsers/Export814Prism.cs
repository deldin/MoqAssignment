using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Parsers.ExportContexts;

namespace Ista.FileServices.MarketFiles.Parsers
{
    public class Export814Prism : IMarketFileExporter
    {
        private static readonly Regex numericExp = new Regex("[^0-9]");

        private readonly IClientDataAccess clientDataAccess;
        private readonly IMarket814Export exportDataAccess;
        private readonly ILogger logger;

        public Export814Prism(IClientDataAccess clientDataAccess, IMarket814Export exportDataAccess, ILogger logger)
        {
            this.clientDataAccess = clientDataAccess;
            this.exportDataAccess = exportDataAccess;
            this.logger = logger;
        }

        public IMarketFileExportResult[] Export(CancellationToken token)
        {
            var cspDunsPorts = clientDataAccess.ListCspDunsPort();
            var prismPorts = cspDunsPorts
                .Where(x => x.ProviderId == 1)
                .ToArray();

            var context = new Prism814Context();
            foreach (var prismPort in prismPorts)
            {
                if (token.IsCancellationRequested)
                    break;

                var headers = exportDataAccess.ListUnprocessed(prismPort.LdcDuns, prismPort.Duns, 1);
                if (headers.Length == 0)
                {
                    logger.TraceFormat("Zero 814 Prism records found to export for TDSP Duns \"{0}\" and CR Duns \"{1}\".",
                        prismPort.LdcDuns, prismPort.Duns);
                    continue;
                }

                logger.DebugFormat("Exporting {0} unprocessed 814 record(s) for TDSP Duns \"{1}\" and CR Duns \"{2}\".",
                    headers.Length, prismPort.LdcDuns, prismPort.Duns);
                
                foreach (var header in headers)
                {
                    if (!header.HeaderKey.HasValue)
                        continue;

                    if (token.IsCancellationRequested)
                        break;

                    var headerKey = header.HeaderKey.Value;

                    context.Initialize();
                    context.ActionCode = header.ActionCode;

                    var identifiedMarket = clientDataAccess.IdentifyMarket(header.TdspDuns);
                    if (identifiedMarket.HasValue)
                        context.SetMarket(identifiedMarket.Value);

                    if (context.ActionCode.Equals("PC", StringComparison.Ordinal) ||
                        context.ActionCode.Equals("PD", StringComparison.Ordinal) ||
                        context.Market != MarketOptions.Texas)
                        context.SetFileProperties(prismPort, header.TdspDuns, "ENR");
                    else
                        context.SetFileProperties(prismPort, "183529049", "ENR");

                    context.SetHeaderId(headerKey);
                    var services = exportDataAccess.ListServices(headerKey);
                    WriteHeader(context, header, services);
                    WriteAccount(context, header);
                    WriteName(context, header);
                    WriteService(context, header, services);
                }
            }

            return context.Models;
        }

        public void WriteHeader(Prism814Context context, Type814Header header, Type814Service[] services)
        {
            string type;

            var actionCode = context.ActionCode.TrimStart('0');
            switch(actionCode)
            {
                case "7":
                case "10":
                case "24":
                    type = "D";
                    break;
                case "8":
                case "9":
                    type = "R";
                    break;
                case "12":
                case "13":
                case "21":
                    type = "C";
                    break;
                case "PC":
                case "PD":
                case "C":
                case "D":
                    type = actionCode;
                    break;
                case "18":
                    type = "D";
                    if (services.Any())
                    {
                        var service = services[0];
                        if (service.MaintenanceTypeCode.Equals("021"))
                            type = "E";
                    }
                    break;
                case "":
                    type = "E";
                    if (context.Market == MarketOptions.Maryland)
                    {
                        if (!services.Any())
                            type = "E";
                        else
                        {
                            var service = services[0];
                            type = service.MaintenanceTypeCode;
                            if (service.MaintenanceTypeCode.Equals("HU") ||
                                service.MaintenanceTypeCode.Equals("HI"))
                                type = "E";
                        }
                    }
                    break;
                default:
                    type = "E";
                    break;

            }

            var line = string.Format("SH|{0}|{1}|{2}|O|",
                context.TradingPartnerId, header.TransactionNbr, type);

            context.AppendLine(line);
            logger.TraceFormat("Wrote 814 \"SH\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteAccount(Prism814Context context, Type814Header header)
        {
            var ercotName = string.Empty;
            var ercotDuns = string.Empty;
            var transactionSetId = string.Empty;
            var stateId = context.TradingPartnerId.Substring(3, 2);
            
            if (context.Market == MarketOptions.Texas)
            {
                ercotName = "ERCOT";
                ercotDuns = "183529049";
            }

            var actionCode = context.ActionCode;
            if (context.Market == MarketOptions.Texas)
                transactionSetId = (actionCode.StartsWith("0"))
                                       ? actionCode.Substring(1)
                                       : actionCode;

            var line = string.Format("01|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}||{9}|||||{10}|{11}|{12}|",
                context.TradingPartnerId,
                stateId,
                header.TransactionSetPurposeCode,
                header.TransactionNbr,
                header.ReferenceNbr,
                header.TdspDuns,
                header.TdspName.ToUpper(),
                header.CrDuns,
                header.CrName.ToUpper(),
                header.TransactionDate,
                transactionSetId,
                ercotName,
                ercotDuns);

            context.AppendLine(line);
            logger.TraceFormat("Wrote 814 \"01\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteName(Prism814Context context, Type814Header header)
        {
            var ignoredActionCodes = new[] {"7", "9", "19", "21", "23", "29"};
            var actionCode = context.ActionCode;
            if (ignoredActionCodes.Contains(actionCode))
                return;

            var headerKey = header.HeaderKey ?? 0;
            Type814Name[] names = exportDataAccess.ListNames(headerKey);
            foreach (var name in names)
            {
                if (context.Market == MarketOptions.Maryland && name.EntityIdType.Equals("N1", StringComparison.Ordinal))
                    continue;

                var entityName = name.EntityName.ToUpper().ToAscii();
                var contactPhone1 = numericExp.Replace(name.ContactPhoneNbr1, string.Empty);
                var contactPhone2 = numericExp.Replace(name.ContactPhoneNbr2, string.Empty);
                var postalCode = numericExp.Replace(name.PostalCode, string.Empty);

                if (entityName.Length > 60)
                    entityName = entityName.Substring(0, 60);

                if (contactPhone1.Length > 16)
                    contactPhone1 = contactPhone1.Substring(0, 16);

                if (contactPhone2.Length > 16)
                    contactPhone2 = contactPhone2.Substring(0, 16);

                var entityIdCode = name.EntityIdCode;
                var entityName2 = name.EntityName2.ToUpper().ToAscii();
                var entityName3 = name.EntityName3.ToUpper().ToAscii();
                var contactName = name.ContactName.ToUpper().ToAscii();
                var address1 = name.Address1.ToUpper().ToAscii();
                var address2 = name.Address2.ToUpper().ToAscii();
                var city = name.City.ToUpper().ToAscii();
                var state = name.State.ToUpper();
                var email = name.EntityEmail.ToUpper();

                if (context.Market == MarketOptions.Maryland)
                {
                    entityIdCode = string.Empty;
                    entityName2 = string.Empty;
                    entityName3 = string.Empty;
                    contactPhone1 = string.Empty;
                    contactName = string.Empty;
                    address1 = string.Empty;
                    address2 = string.Empty;
                    city = string.Empty;
                    state = string.Empty;
                    postalCode = string.Empty;
                }

                var line =
                    string.Format("05|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}||{12}|{13}|{14}|{15}|{16}||||",
                        context.TradingPartnerId, name.EntityIdType, entityName, entityIdCode, entityName2, entityName3,
                        address1, address2, city, state, postalCode, name.CountryCode, contactPhone1, contactPhone1,
                        contactName, contactPhone2, email);

                context.AppendLine(line);
                logger.TraceFormat("Wrote 814 \"05\" PRISM line for Header {0}", header.HeaderKey);
            }
        }

        public void WriteService(Prism814Context context, Type814Header header, Type814Service[] services)
        {
            if (context.Market == MarketOptions.Maryland)
            {
                WriteServiceByMaintenanceTypeCode(context, header, services);
                return;
            }

            var actionCode = context.ActionCode.TrimStart('0');
            switch (actionCode)
            {
                case "7":
                case "10":
                case "19":
                case "24":
                    WriteServiceDrop(context, header, services);
                    break;
                case "8":
                case "9":
                    WriteServiceReinstatement(context, header, services);
                    break;
                case "12":
                case "13":
                case "21":
                case "PC":
                    WriteServiceChange(context, header, services);
                    break;
                case "PD":
                    WriteServiceChangeResponse(context, header, services);
                    break;
                case "18":
                    WriteServiceEnrollment(context, header, services);
                    WriteServiceDrop(context, header, services);
                    break;
                case "":
                    return;
                default:
                    WriteServiceEnrollment(context, header, services);
                    break;
            }
        }

        public void WriteServiceByMaintenanceTypeCode(Prism814Context context, Type814Header header, Type814Service[] services)
        {
            if (string.IsNullOrWhiteSpace(context.ActionCode))
                return;

            var actionCode = context.ActionCode.TrimStart('0');
            switch(actionCode)
            {
                case "E":
                    WriteServiceEnrollment(context, header, services);
                    break;
                case "C":
                    WriteServiceChange(context, header, services);
                    break;
                case "7":
                case "D":
                    WriteServiceDrop(context, header, services);
                    break;
                case "HU":
                    WriteServiceHistoricalUsageRequest(context, header, services);
                    break;
            }
        }

        public void WriteServiceChange(Prism814Context context, Type814Header header, Type814Service[] services)
        {
            var actionCode = context.ActionCode.TrimStart('0');
            foreach (var service in services)
            {
                if (!service.ServiceKey.HasValue)
                    continue;

                var serviceKey = service.ServiceKey.Value;
                var lineTrackingNumber = "1";
                var crDuns = string.Empty;
                var specialNeedsFlag = string.Empty;

                if (context.Market != MarketOptions.Texas)
                    lineTrackingNumber = serviceKey.ToString();
                if (actionCode.Equals("PC", StringComparison.Ordinal))
                    specialNeedsFlag = service.SpecialNeedsIndicator;

                var membershipId = IdentifyLdcAccountNumber(context, service);
                var premiseNumber = IdentifyEsiId(context, service);
                var serviceActionCode = string.Empty;

                if (actionCode.Equals("C", StringComparison.Ordinal))
                    serviceActionCode = "A";
                if (actionCode.Equals("13") || actionCode.Equals("21"))
                {
                    if (service.ActionCode.Equals("WQ", StringComparison.Ordinal))
                        serviceActionCode = "A";
                    if (service.ActionCode.Equals("U", StringComparison.Ordinal))
                        serviceActionCode = "R";
                }

                var changes = exportDataAccess.ListServiceAccountChanges(serviceKey);
                if (context.Market == MarketOptions.Maryland)
                {
                    crDuns = header.CrDuns;
                    if (changes.Any(x => x.ChangeReason.Equals("REFPC", StringComparison.Ordinal)))
                        continue;
                }

                var line =
                    string.Format(
                        "10|{0}|{1}|{2}||{3}|C|{4}||{5}|||{9}||||||||||||||||||{6}|{10}||||||{7}||||||{8}|||||||||||{11}|",
                        context.TradingPartnerId, service.ServiceType1, service.ServiceType2, lineTrackingNumber,
                        serviceActionCode, membershipId, service.BillType, service.CsaFlag, premiseNumber, crDuns,
                        service.BillCalculator, specialNeedsFlag);

                context.AppendLine(line);
                logger.TraceFormat("Wrote 814 \"10\" PRISM line for Header {0}", header.HeaderKey);

                if (context.Market == MarketOptions.Maryland)
                {
                    WriteServiceMeter(context, service);
                    WriteServiceMeterChange(context, service);
                    WriteServiceAccountChange(context, service, changes);
                    continue;
                }

                if (actionCode.Equals("12"))
                    WriteServiceDate(context, service);

                if (actionCode.Equals("PC", StringComparison.Ordinal))
                    WriteServiceMeter(context, service);
            }
        }

        public void WriteServiceChangeResponse(Prism814Context context, Type814Header header, Type814Service[] services)
        {
            var actionCode = context.ActionCode.TrimStart('0');
            foreach (var service in services)
            {
                if (!service.ServiceKey.HasValue)
                    continue;

                var membershipId = IdentifyLdcAccountNumber(context, service);
                var premiseNumber = IdentifyEsiId(context, service);
                var serviceActionCode = string.Empty;

                if (actionCode.Equals("PD", StringComparison.Ordinal))
                    serviceActionCode = "A";

                var line =
                    string.Format("10|{0}|{1}|{2}||1|C|{3}||{4}||||||||||||||||||||||||||||||||||{5}|||||",
                        context.TradingPartnerId, service.ServiceType1, service.ServiceType2, serviceActionCode,
                        membershipId, premiseNumber);
                
                context.AppendLine(line);
                logger.TraceFormat("Wrote 814 \"10\" PRISM line for Header {0}", header.HeaderKey);
            }
        }

        public void WriteServiceDrop(Prism814Context context, Type814Header header, Type814Service[] services)
        {
            var actionCode = context.ActionCode.TrimStart('0');
            foreach (var service in services)
            {
                if (!service.ServiceKey.HasValue)
                    continue;

                var serviceKey = service.ServiceKey.Value;
                var lineTrackingNumber = "1";
                var crDuns = string.Empty;
                var dropCode = string.Empty;
                var specialNeedsFlag = string.Empty;

                if (context.Market != MarketOptions.Texas)
                    lineTrackingNumber = serviceKey.ToString();
                if (actionCode.Equals("29"))
                    dropCode = "09";
                if (actionCode.Equals("10"))
                    specialNeedsFlag = service.SpecialNeedsIndicator;

                var membershipId = IdentifyLdcAccountNumber(context, service);
                var premiseNumber = IdentifyEsiId(context, service);
                var serviceActionCode = service.ActionCode;
                
                switch (actionCode)
                {
                    case "18":
                        if (!service.MaintenanceTypeCode.Equals("002"))
                            return;
                        break;
                    case "D":
                        dropCode = "P";
                        break;
                }

                if (context.Market == MarketOptions.Maryland)
                {
                    crDuns = header.CrDuns;
                    if (serviceActionCode.Equals("Q", StringComparison.Ordinal))
                        serviceActionCode = string.Empty;
                }

                var line =
                    string.Format(
                        "10|{0}|{1}|{2}|{3}|{4}|D|{5}|{6}|{7}|||{12}||||||||||||||||||{8}|||||||{9}||||||{10}|||{11}||||||||{14}||||{13}|",
                        context.TradingPartnerId, service.ServiceType1, service.ServiceType2, service.ServiceType3,
                        lineTrackingNumber, serviceActionCode, dropCode, membershipId, service.BillType, service.CsaFlag,
                        premiseNumber, service.EnergizedFlag, crDuns, service.MeterAccessNote, specialNeedsFlag);

                context.AppendLine(line);
                logger.TraceFormat("Wrote 814 \"10\" PRISM line for Header {0}", header.HeaderKey);

                if (actionCode.Equals("10") || actionCode.Equals("24"))
                    WriteServiceDate(context, service);

                WriteServiceReject(context, service);
                WriteServiceStatus(context, service);

                if (actionCode.Equals("7") || actionCode.Equals("19"))
                    continue;

                WriteServiceMeter(context, service);
            }
        }

        public void WriteServiceEnrollment(Prism814Context context, Type814Header header, Type814Service[] services)
        {
            var actionCode = context.ActionCode.TrimStart('0');
            foreach (var service in services)
            {
                if (!service.ServiceKey.HasValue)
                    continue;

                var serviceKey = service.ServiceKey.Value;
                var lineTrackingNumber = "1";
                var specialNeedsFlag = string.Empty;

                if (context.Market != MarketOptions.Texas)
                    lineTrackingNumber = serviceKey.ToString();
                if (actionCode.Equals("1") || actionCode.Equals("16"))
                    specialNeedsFlag = service.SpecialNeedsIndicator;

                var membershipId = IdentifyLdcAccountNumber(context, service);
                var premiseNumber = IdentifyEsiId(context, service);
                var serviceActionCode = string.Empty;
                var serviceTransactionTypeCode = string.Empty;

                switch (actionCode)
                {
                    case "15":
                        serviceActionCode = service.ActionCode;
                        serviceTransactionTypeCode = service.MaintenanceTypeCode;
                        break;
                    case "18":
                        if (!service.MaintenanceTypeCode.Equals("021"))
                            return;
                        serviceActionCode = service.ActionCode;
                        break;
                    case "29":
                        serviceActionCode = service.ActionCode;
                        serviceTransactionTypeCode = service.MaintenanceTypeCode;
                        break;
                    case "26":
                        if (context.Market == MarketOptions.Texas)
                            membershipId = string.Empty;
                        break;
                    default:
                        serviceActionCode = service.ActionCode;
                        break;
                }

                var priorityCode = service.PriorityCode;
                if (priorityCode.Length >= 2)
                    priorityCode = priorityCode.Substring(0, 2);

                if (context.Market != MarketOptions.Texas)
                    serviceActionCode = string.Empty;

                var line =
                    string.Format(
                        "10|{0}|{1}|{2}|{3}|{4}|E|{5}|{6}|{7}|||||||||||||{8}||||||||{9}|{10}|||||||||||{11}|{12}||{13}|||||||||{15}||||{14}|||||||||||",
                        context.TradingPartnerId, service.ServiceType1, service.ServiceType2, service.ServiceType3,
                        lineTrackingNumber, serviceActionCode, serviceTransactionTypeCode, membershipId,
                        service.NotificationWaiver, service.BillType, service.BillCalculator, service.ServiceType4,
                        premiseNumber, priorityCode, service.MeterAccessNote, specialNeedsFlag);

                context.AppendLine(line);
                logger.TraceFormat("Wrote 814 \"10\" PRISM line for Header {0}", header.HeaderKey);

                if (!actionCode.Equals("23") && !actionCode.Equals("29"))
                {
                    if (service.ServiceType3.Equals("SW", StringComparison.Ordinal) ||
                        service.ServiceType3.Equals("MVI", StringComparison.Ordinal))
                        WriteServiceDate(context, service);
                }

                if (context.Market == MarketOptions.Maryland && actionCode.Equals("E"))
                {
                    WriteServiceDate(context, service);
                    WriteServiceRateData(context, service);
                }

                if (actionCode.Equals("29") || actionCode.Equals("15"))
                {
                    if (serviceActionCode.Equals("R", StringComparison.Ordinal))
                        WriteServiceReject(context, service);
                }

                if (!actionCode.Equals("18") && !actionCode.Equals("21") && !actionCode.Equals("23") && !actionCode.Equals("26") && !actionCode.Equals("29"))
                    WriteServiceMeter(context, service);
            }
        }

        public void WriteServiceHistoricalUsageRequest(Prism814Context context, Type814Header header, Type814Service[] services)
        {
            foreach (var service in services)
            {
                if (!service.ServiceKey.HasValue)
                    continue;

                var crDuns = string.Empty;
                if (context.Market != MarketOptions.Texas)
                    crDuns = header.CrDuns;
                
                var membershipId = IdentifyLdcAccountNumber(context, service);
                
                var line =
                    string.Format(
                        "10|{0}|{1}|{2}|{3}|{4}|E|||{5}|||{11}||||||||||{6}||||||||{7}|{8}|||||||||||{9}|||{10}|||{12}|",
                        context.TradingPartnerId, service.ServiceType1, service.ServiceType2, service.ServiceType3,
                        header.TransactionNbr, membershipId, service.NotificationWaiver, service.BillType,
                        service.BillCalculator, service.ServiceType4, service.PriorityCode, crDuns,
                        service.FundsAuthorization);

                context.AppendLine(line);
                logger.TraceFormat("Wrote 814 \"10\" PRISM line for Header {0}", header.HeaderKey);
            }
        }

        public void WriteServiceReinstatement(Prism814Context context, Type814Header header, Type814Service[] services)
        {
            var actionCode = context.ActionCode.TrimStart('0');
            foreach (var service in services)
            {
                if (!service.ServiceKey.HasValue)
                    continue;

                var premiseNumber = IdentifyEsiId(context, service);
                var line = string.Format("10|{0}|{1}|{2}||1|R|{3}||||||||||||||||||||||||||||||||||||{4}|||||",
                    context.TradingPartnerId, service.ServiceType1, service.ServiceType2, service.ActionCode,
                    premiseNumber);

                context.AppendLine(line);
                logger.TraceFormat("Wrote 814 \"10\" PRISM line for Header {0}", header.HeaderKey);
                
                if (actionCode.Equals("9"))
                    WriteServiceReject(context, service);

                WriteServiceStatus(context, service);
            }
        }

        public void WriteServiceDate(Prism814Context context, Type814Service service)
        {
            if (service == null)
                return;

            var serviceKey = service.ServiceKey ?? 0;
            if (serviceKey == 0)
                return;

            var nextDate = string.Empty;
            var moveInDate = string.Empty;
            var moveOutDate = string.Empty;
            var switchDate = string.Empty;

            var statuses = exportDataAccess.ListServiceStatuses(serviceKey);
            var statusCode = string.Empty;
            if (statuses.Any())
                statusCode = statuses[0].StatusCode;

            var actionCode = context.ActionCode.TrimStart('0');
            switch (actionCode)
            {
                case "7":
                case "10":
                    switchDate = service.SpecialReadSwitchDate;
                    break;
                case "24":
                    moveOutDate = service.SpecialReadSwitchDate;
                    break;
                case "8":
                case "9":
                    nextDate = service.SpecialReadSwitchDate;
                    break;
                case "12":
                case "13":
                    if (statusCode.Equals("DTM375"))
                        moveInDate = service.SpecialReadSwitchDate;
                    if (statusCode.Equals("DTM376"))
                        moveOutDate = service.SpecialReadSwitchDate;
                    break;
                case "":
                    if (service.MaintenanceTypeCode.Equals("E"))
                        moveInDate = service.SpecialReadSwitchDate;
                    if (service.MaintenanceTypeCode.Equals("D"))
                        moveOutDate = service.SpecialReadSwitchDate;
                    break;
                default:
                    if (service.ServiceType3.Equals("SW", StringComparison.Ordinal))
                        switchDate = service.SpecialReadSwitchDate;
                    else
                        moveInDate = service.SpecialReadSwitchDate;
                    break;
            }

            var agreementDate = DateTime.Now.ToString("yyyyMMdd");
            var agreementTime = DateTime.Now.ToString("hhmmss");
            if (context.Market == MarketOptions.Texas)
            {
                agreementDate = service.RtoDate;
                agreementTime = service.RtoTime;
            }

            var line = string.Format("11|{0}|{1}|{2}||{3}||||||{4}||||{5}|{6}||||", context.TradingPartnerId,
                agreementDate, agreementTime, nextDate, switchDate, moveInDate, moveOutDate);

            context.AppendLine(line);
            logger.TraceFormat("Wrote 814 \"11\" PRISM line for Header {0}", service.HeaderKey);
        }

        public void WriteServiceMeter(Prism814Context context, Type814Service service)
        {
            if (service == null)
                return;

            var serviceKey = service.ServiceKey ?? 0;
            if (serviceKey == 0)
                return;

            var meters = exportDataAccess.ListServiceMeters(serviceKey);
            for (int index = 0, count = meters.Length; index < count; index++)
            {
                var line = string.Format("30|{0}|||||||||||||||||||||||||||||||||||||", context.TradingPartnerId);
                context.AppendLine(line);
                logger.TraceFormat("Wrote 814 \"30\" PRISM line for Header {0}", service.HeaderKey);
            }
        }

        public void WriteServiceMeterChange(Prism814Context context, Type814Service service)
        {
            if (service == null)
                return;

            var serviceKey = service.ServiceKey ?? 0;
            if (serviceKey == 0)
                return;

            var changes = exportDataAccess.ListServiceMeterChangesByService(serviceKey);
            foreach (var change in changes)
            {
                var line = string.Format("38|{0}|{1}|{2}|", context.TradingPartnerId, change.ChangeReason,
                    change.ChangeDescription);

                context.AppendLine(line);
                logger.TraceFormat("Wrote 814 \"38\" PRISM line for Header {0}", service.HeaderKey);
            }
        }

        public void WriteServiceRateData(Prism814Context context, Type814Service service)
        {
            var line = string.Format("12|{0}|1||||||||||||||", context.TradingPartnerId);
            context.AppendLine(line);
            logger.TraceFormat("Wrote 814 \"12\" PRISM line for Header {0}", service.HeaderKey);
        }

        public void WriteServiceReject(Prism814Context context, Type814Service service)
        {
            if (service == null)
                return;

            var serviceKey = service.ServiceKey ?? 0;
            if (serviceKey == 0)
                return;

            var rejects = exportDataAccess.ListServiceRejects(serviceKey);
            foreach (var reject in rejects)
            {
                var line = string.Format("15|{0}|{1}|{2}|", context.TradingPartnerId, reject.RejectCode,
                    reject.RejectReason);

                context.AppendLine(line);
                logger.TraceFormat("Wrote 814 \"15\" PRISM line for Header {0}", service.HeaderKey);
            }
        }

        public void WriteServiceStatus(Prism814Context context, Type814Service service)
        {
            if (service == null)
                return;

            var serviceKey = service.ServiceKey ?? 0;
            if (serviceKey == 0)
                return;

            var statuses = exportDataAccess.ListServiceStatuses(serviceKey);
            foreach (var status in statuses)
            {
                var line = string.Format("16|{0}|{1}|{2}||", context.TradingPartnerId, status.StatusCode,
                    status.StatusReason);

                context.AppendLine(line);
                logger.TraceFormat("Wrote 814 \"16\" PRISM line for Header {0}", service.HeaderKey);
            }
        }

        public void WriteServiceAccountChange(Prism814Context context, Type814Service service, Type814ServiceAccountChange[] changes)
        {
            foreach (var change in changes)
            {
                var line = string.Format("40|{0}|{1}|{2}|", context.TradingPartnerId, change.ChangeReason,
                    change.ChangeDescription);

                context.AppendLine(line);
                logger.TraceFormat("Wrote 814 \"40\" PRISM line for Header {0}", service.HeaderKey);
            }
        }

        public string IdentifyEsiId(Prism814Context context, Type814Service service)
        {
            if (context.Market == MarketOptions.Maryland)
                return string.Empty;

            return service.EsiId;
        }

        public string IdentifyLdcAccountNumber(Prism814Context context, Type814Service service)
        {
            if (context.Market == MarketOptions.Maryland)
                return service.EsiId;

            return service.MembershipId;
        }
    }
}

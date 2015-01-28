using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Parsers
{
    public class Export814Xml : IMarketFileExporter
    {
        private readonly IClientDataAccess clientDataAccess;
        private readonly IMarketDataAccess marketDataAccess;
        private readonly IMarket814Export exportDataAccess;
        private readonly ILogger logger;

        public Export814Xml(IClientDataAccess clientDataAccess, IMarketDataAccess marketDataAccess, IMarket814Export exportDataAccess, ILogger logger)
        {
            this.clientDataAccess = clientDataAccess;
            this.marketDataAccess = marketDataAccess;
            this.exportDataAccess = exportDataAccess;
            this.logger = logger;
        }

        public IMarketFileExportResult[] Export(CancellationToken token)
        {
            var cspDunsPorts = clientDataAccess.ListCspDunsPort();
            var xmlPorts = cspDunsPorts
                .Where(x => x.ProviderId == 2)
                .ToArray();

            var collection = new List<IMarketFileExportResult>();
            foreach (var xmlPort in xmlPorts)
            {
                if (token.IsCancellationRequested)
                    break;

                var model = new Export814Model
                {
                    CspDuns = xmlPort.Duns,
                    LdcDuns = xmlPort.LdcDuns,
                    LdcShortName = xmlPort.LdcShortName,
                    TradingPartnerId = xmlPort.TradingPartnerId,
                };
                
                var partner = marketDataAccess.LoadCspDunsTradingPartner(xmlPort.Duns, xmlPort.LdcDuns);
                if (partner == null)
                {
                    logger.ErrorFormat(
                        "No CSP DUNS Trading Partner record exists between CR DUNS \"{0}\" and LDC DUNS \"{1}\". 814 Transactions will not be exported.",
                        xmlPort.Duns, xmlPort.LdcDuns);

                    continue;
                }

                marketDataAccess.LoadCspDunsTradingPartnerConfig(partner);
                model.CspDunsTradingPartnerId = partner.CspDunsTradingPartnerId;

                var headers = exportDataAccess.ListUnprocessed(xmlPort.LdcDuns, xmlPort.Duns, 2);
                if (headers.Length == 0)
                {
                    logger.TraceFormat("Zero 814 Xml records found to export for TDSP Duns \"{0}\" and CR Duns \"{1}\".",
                        xmlPort.LdcDuns, xmlPort.Duns);
                    continue;
                }

                logger.DebugFormat("Exporting {0} unprocessed 814 record(s) for TDSP Duns \"{1}\" and CR Duns \"{2}\".",
                    headers.Length, xmlPort.LdcDuns, xmlPort.Duns);

                XNamespace marketNamespace = "http://CIS.Integration.Schema.Market.Common.Market814";
                var document = new XDocument(
                    new XElement(marketNamespace + "Market814",
                        new XAttribute(XNamespace.Xmlns + "ns0", marketNamespace),
                        new XElement("TransactionCount", model.HeaderCount),
                        new XElement("LdcIsaQualifier", partner.GetConfig("TradingPartnerISAQualifier")),
                        new XElement("LdcIsaIdentifier", partner.GetConfig("TradingPartnerISAIdentifier")),
                        new XElement("LdcDuns", partner.TradingPartnerDuns),
                        new XElement("CspIsaQualifier", partner.GetConfig("CspISAQualifier")),
                        new XElement("CspDuns", partner.CspDuns),
                        new XElement("MarketFileId", string.Empty),
                        new XElement("LdcGSIdentifier", partner.GetConfig("TradingPartnerGSIdentifier")),
                        new XElement("LdcN1Qualifier", partner.GetConfig("TradingPartnerN1Qualifier")),
                        new XElement("CspN1Qualifier", partner.GetConfig("CspN1Qualifier")),
                        new XElement("AuthInfoQualifier", string.Empty),
                        new XElement("AuthInfo", string.Empty),
                        new XElement("SecurityInfoQualifier", string.Empty),
                        new XElement("SecurityInfo", string.Empty),
                        new XElement("InterchangeControlNbr", string.Empty),
                        new XElement("AckRequested", string.Empty),
                        new XElement("TestIndicator", string.Empty),
                        new XElement("SubelementSeparator", partner.GetConfig("TradingPartnerSubElementDelimiter")),
                        new XElement("FunctionalGroup", "GE"),
                        new XElement("FunctionalGroupControlNbr", string.Empty),
                        new XElement("CspIsaIdentifier", partner.GetConfig("CspIsaIdentifier")),
                        new XElement("CspGSIdentifier", partner.GetConfig("CspGSIdentifier"))));

                if (document.Root == null)
                {
                    logger.Error("Unable to create 814 XML document. Root node element is null.");
                    continue;
                }

                foreach (var header in headers)
                {
                    if (!header.HeaderKey.HasValue)
                        continue;

                    if (token.IsCancellationRequested)
                        break;

                    var element = new XElement("Transaction",
                        new XElement("SegmentCount", "0"));

                    WriteHeader(element, header);
                    model.AddHeaderKey(header.HeaderKey.Value);

                    document.Root.Add(element);
                }
                
                model.Document = document;
                collection.Add(model);
            }

            return collection.ToArray();
        }

        public void WriteHeader(XContainer container, Type814Header header)
        {
            if (header == null)
                return;

            if (!header.HeaderKey.HasValue)
                return;

            var headerKey = header.HeaderKey.Value;
            var element = new XElement("Header",
                new XElement("HeaderKey", headerKey));

            element.TryAddElement("TransactionSetId", header.TransactionSetId);
            element.TryAddElement("TransactionSetControlNbr", header.TransactionSetControlNbr);
            element.TryAddElement("TransactionSetPurposeCode", header.TransactionSetPurposeCode);
            element.TryAddElement("TransactionNbr", header.TransactionNbr);
            element.TryAddElement("TransactionDate", header.TransactionDate);
            element.TryAddElement("TransactionTime", header.TransactionTime);
            element.TryAddElement("TransactionTimeCode", header.TransactionTimeCode);
            element.TryAddElement("ReferenceNbr", header.ReferenceNbr);
            element.TryAddElement("ActionCode", header.ActionCode);
            element.TryAddElement("TdspDuns", header.TdspDuns);
            element.TryAddElement("TdspName", header.TdspName);
            element.TryAddElement("CrDuns", header.CrDuns);
            element.TryAddElement("CrName", header.CrName);
            element.TryAddElement("SegmentCount", "0");
            element.TryAddElement("TransactionQualifier", header.TransactionQualifier);
            container.Add(element);
            logger.TraceFormat("Added 814 \"Header\" XML element for Header {0}", headerKey);

            var names = exportDataAccess.ListNames(headerKey);
            WriteName(element, names);

            var services = exportDataAccess.ListServices(headerKey);
            WriteService(element, services);
        }

        public void WriteName(XContainer container, Type814Name[] names)
        {
            var loopElement = new XElement("NameLoop");
            container.Add(loopElement);

            if (names == null || names.Length == 0)
                return;

            foreach (var name in names)
            {
                if (!name.NameKey.HasValue)
                    continue;

                var element = new XElement("Name",
                    new XElement("NameKey", name.NameKey),
                    new XElement("HeaderKey", name.HeaderKey),
                    new XElement("EntityIdType", name.EntityIdType),
                    new XElement("EntityName", name.EntityName));

                element.TryAddElement("EntityName2", name.EntityName2);
                element.TryAddElement("EntityName3", name.EntityName3);
                element.TryAddElement("EntityDuns", name.EntityDuns);
                element.TryAddElement("EntityIdCode", name.EntityIdCode);
                element.TryAddElement("Address1", name.Address1);
                element.TryAddElement("Address2", name.Address2);
                element.TryAddElement("City", name.City);
                element.TryAddElement("State", name.State);
                element.TryAddElement("PostalCode", name.PostalCode);
                element.TryAddElement("CountryCode", name.CountryCode);
                element.TryAddElement("County", name.County);
                element.TryAddElement("ContactCode", name.ContactCode);
                element.TryAddElement("ContactName", name.ContactName);
                element.TryAddElement("ContactPhoneNbr1", name.ContactPhoneNbr1);
                element.TryAddElement("ContactPhoneNbr2", name.ContactPhoneNbr2);
                element.TryAddElement("ContactPhoneNbr3", name.ContactPhoneNbr3);
                element.TryAddElement("EntityFirstName", name.EntityFirstName);
                element.TryAddElement("EntityLastName", name.EntityLastName);
                element.TryAddElement("CustType", name.CustType);
                element.TryAddElement("TaxingDistrict", name.TaxingDistrict);
                element.TryAddElement("EntityMiddleName", name.EntityMiddleName);
                loopElement.Add(element);
                logger.TraceFormat("Added 814 \"Name\" XML element for Header {0}", name.HeaderKey);
            }
        }

        public void WriteService(XContainer container, Type814Service[] services)
        {
            var loopElement = new XElement("ServiceLoop");
            container.Add(loopElement);

            if (services == null || services.Length == 0)
                return;

            foreach (var service in services)
            {
                if (!service.ServiceKey.HasValue)
                    continue;

                var serviceKey = service.ServiceKey.Value;
                var element = new XElement("Service",
                    new XElement("ServiceKey", service.ServiceKey),
                    new XElement("HeaderKey", service.HeaderKey));

                element.TryAddElement("AssignId", service.AssignId);
                element.Add(
                    new XElement("ServiceTypeCode1", service.ServiceTypeCode1),
                    new XElement("ServiceType1", service.ServiceType1),
                    new XElement("ServiceTypeCode2", service.ServiceTypeCode2),
                    new XElement("ServiceType2", service.ServiceType2));
                element.TryAddElement("ServiceTypeCode3", service.ServiceTypeCode3);
                element.TryAddElement("ServiceType3", service.ServiceType3);
                element.TryAddElement("ServiceTypeCode4", service.ServiceTypeCode4);
                element.TryAddElement("ServiceType4", service.ServiceType4);
                element.Add(
                    new XElement("ActionCode", service.ActionCode),
                    new XElement("MaintenanceTypeCode", service.MaintenanceTypeCode));
                element.TryAddElement("DistributionLossFactorCode", service.DistributionLossFactorCode);
                element.TryAddElement("PremiseType", service.PremiseType);
                element.TryAddElement("BillType", service.BillType);
                element.TryAddElement("BillCalculator", service.BillCalculator);
                element.Add(new XElement("EsiId", service.EsiId));
                element.TryAddElement("SpecialNeedsIndicator", service.SpecialNeedsIndicator);
                element.TryAddElement("StationId", service.StationId);
                element.TryAddElement("PowerRegion", service.PowerRegion);
                element.TryAddElement("EnergizedFlag", service.EnergizedFlag);
                element.TryAddElement("NotificationWaiver", service.NotificationWaiver);
                element.TryAddElement("EsiIdStartDate", service.EsiIdStartDate);
                element.TryAddElement("EsiIdEndDate", service.EsiIdEndDate);
                element.TryAddElement("EsiIdEligibilityDate", service.EsiIdEligbilityDate);
                element.TryAddElement("SpecialReadSwitchDate", service.SpecialReadSwitchDate);
                element.TryAddElement("SpecialReadSwitchTime", service.SpecialReadSwitchTime);
                element.TryAddElement("PriorityCode", service.PriorityCode);
                element.TryAddElement("RTODate", service.RtoDate);
                element.TryAddElement("RTOTime", service.RtoTime);
                element.TryAddElement("PermitIndicator", service.PermitIndicator);
                element.TryAddElement("CSAFlag", service.CsaFlag);
                element.TryAddElement("MembershipID", service.MembershipId);
                element.TryAddElement("ESPAccountNumber", service.EspAccountNumber);
                element.TryAddElement("LDCBillingCycle", service.LdcBillingCycle);
                element.TryAddElement("LDCBudgetBillingCycle", service.LdcBudgetBillingCycle);
                element.TryAddElement("WaterHeaters", service.WaterHeaters);
                element.TryAddElement("LDCBudgetBillingStatus", service.LdcBudgetBillingStatus);
                element.TryAddElement("PaymentArrangement", service.PaymentArrangement);
                element.TryAddElement("NextMeterReadDate", service.NextMeterReadDate);
                element.TryAddElement("ParticipatingInterest", service.ParticipatingInterest);
                element.TryAddElement("EligibleLoadPercentage", service.EligibleLoadPercentage);
                element.TryAddElement("TaxExemptionPercent", service.TaxExceptionPercent);
                element.TryAddElement("CapacityObligation", service.CapacityObligation);
                element.TryAddElement("TransmissionObligation", service.TransmissionObligation);
                element.TryAddElement("TotalKWHHistory", service.TotalKwhHistory);
                element.TryAddElement("NumberOfMonthsHistory", service.NumberOfMonthsHistory);
                element.TryAddElement("PeakDemandHistory", service.PeakDemandHistory);
                element.TryAddElement("AirConditioners", service.AirConditioners);
                element.TryAddElement("PreviousEsiId", service.PreviousEsiId);
                element.TryAddElement("GasPoolId", service.GasPoolId);
                element.TryAddElement("LBMPZone", service.LbmpZone);
                element.TryAddElement("ResidentialTaxPortion", service.ResidentialTaxPortion);

                // trim leading zeros
                element.TryAddElement("ESPCommodityPrice", service.EspCommodityPrice,
                    x => (x.Substring(0, 1) == "0" ? x.Substring(1) : x));
                element.TryAddElement("ESPFixedCharge", service.EspFixedCharge,
                    x => (x.Substring(0, 1) == "0" ? x.Substring(1) : x));
                element.TryAddElement("ESPChargesCommTaxRate", service.EspChargesCommTaxRate,
                    x => (x.Substring(0, 1) == "0" ? x.Substring(1) : x));
                element.TryAddElement("ESPChargesResTaxRate", service.EspChargesResTaxRate,
                    x => (x.Substring(0, 1) == "0" ? x.Substring(1) : x));

                element.TryAddElement("GasSupplyServiceOption", service.GasSupplyServiceOption);
                element.TryAddElement("GasSupplyServiceOptionCode", service.GasSupplyServiceOptionCode);
                element.TryAddElement("BudgetBillingStatus", service.BudgetBillingStatus);
                element.TryAddElement("FixedMonthlyCharge", service.FixedMonthlyCharge);
                element.TryAddElement("TaxRate", service.TaxRate);
                element.TryAddElement("MeterCycleCodeDesc", service.MeterCycleCodeDesc);
                element.TryAddElement("MeterCycleCode", service.MeterCycleCode);
                element.TryAddElement("BillCycleCodeDesc", service.BillCycleCodeDesc);
                element.TryAddElement("FeeApprovedApplied", service.FeeApprovedApplied);
                element.TryAddElement("MarketerCustomerAccountNumber", service.MarketerCustomerAccountNumber);
                element.TryAddElement("HumanNeeds", service.HumanNeeds);
                element.TryAddElement("NewCustomerIndicator", service.NewCustomerIndicator);
                element.TryAddElement("NewPremiseIndicator", service.NewPremiseIndicator);
                element.TryAddElement("CustomerAuthorization", service.CustomerAuthorization);
                element.TryAddElement("LDCAccountBalance", service.LdcAccountBalance);
                element.TryAddElement("DisputedAmount", service.DisputedAmount);
                element.TryAddElement("CurrentBalance", service.CurrentBalance);
                element.TryAddElement("ArrearsBalance", service.ArrearsBalance);
                element.TryAddElement("LDCSupplierBalance", service.LdcSupplierBalance);
                element.TryAddElement("BudgetPlan", service.BudgetPlan);
                element.TryAddElement("BudgetInstallment", service.BudgetInstallment);
                element.TryAddElement("Deposit", service.Deposit);
                element.TryAddElement("RemainingUtilBalanceBucket1", service.RemainingUtilBalanceBucket1);
                element.TryAddElement("RemainingUtilBalanceBucket2", service.RemainingUtilBalanceBucket2);
                element.TryAddElement("RemainingUtilBalanceBucket3", service.RemainingUtilBalanceBucket3);
                element.TryAddElement("RemainingUtilBalanceBucket4", service.RemainingUtilBalanceBucket4);
                element.TryAddElement("RemainingUtilBalanceBucket5", service.RemainingUtilBalanceBucket5);
                element.TryAddElement("RemainingUtilBalanceBucket6", service.RemainingUtilBalanceBucket6);
                element.TryAddElement("IntervalStatusType", service.IntervalStatusType);
                element.TryAddElement("PaymentOption", service.PaymentOption);
                element.TryAddElement("SystemNumber", service.SystemNumber);
                element.TryAddElement("SpecialMeterConfig", service.SpecialMeterConfig);
                element.TryAddElement("MaximumGeneration", service.MaximumGeneration);
                element.TryAddElement("DaysInArrears", service.DaysInArrears);
                element.TryAddElement("ServiceDeliveryPoint", service.ServiceDeliveryPoint);
                element.TryAddElement("GasCapacityAssignment", service.GasCapacityAssignment);
                loopElement.Add(element);
                logger.TraceFormat("Added 814 \"Service\" XML element for Header {0}", service.HeaderKey);

                var meters = exportDataAccess.ListServiceMeters(serviceKey);
                WriteServiceMeter(element, meters);

                var rejects = exportDataAccess.ListServiceRejects(serviceKey);
                WriteServiceReject(element, rejects);

                var statuses = exportDataAccess.ListServiceStatuses(serviceKey);
                WriteServiceStatus(element, statuses);

                var changes = exportDataAccess.ListServiceAccountChanges(serviceKey);
                WriteServiceAccountChange(element, changes);

                var dates = exportDataAccess.ListServiceDates(serviceKey);
                WriteServiceDate(element, dates);
            }
        }

        public void WriteServiceMeter(XContainer container, Type814ServiceMeter[] meters)
        {
            if (meters == null || meters.Length == 0)
                return;

            var loopElement = new XElement("ServiceMeterLoop");
            container.Add(loopElement);

            foreach (var meter in meters)
            {
                if (!meter.MeterKey.HasValue)
                    continue;

                var meterKey = meter.MeterKey.Value;
                var element = new XElement("ServiceMeter",
                    new XElement("MeterKey", meter.MeterKey),
                    new XElement("ServiceKey", meter.ServiceKey),
                    new XElement("EntityIdCode", meter.EntityIdCode),
                    new XElement("MeterNumber", meter.MeterNumber));

                element.TryAddElement("MeterCode", meter.MeterCode);
                element.TryAddElement("LoadProfile", meter.LoadProfile);
                element.TryAddElement("RateClass", meter.RateClass);
                element.TryAddElement("RateSubClass", meter.RateSubClass);
                element.TryAddElement("MeterCycle", meter.MeterCycle);
                element.TryAddElement("MeterCycleDayOfMonth", meter.MeterCycleDayOfMonth);
                element.TryAddElement("SpecialNeedsIndicator", meter.SpecialNeedsIndicator);
                element.TryAddElement("OldMeterNumber", meter.OldMeterNumber);
                element.TryAddElement("MeterOwnerIndicator", meter.MeterOwnerIndicator);
                element.TryAddElement("EntityType", meter.EntityType);
                element.TryAddElement("TimeOfUse", meter.TimeOfUse);
                element.TryAddElement("ESPRateCode", meter.EspRateCode);
                element.TryAddElement("PricingStructureCode", meter.PricingStructureCode);

                element.Add(
                    new XElement("MeterOwner", meter.MeterOwner),
                    new XElement("MeterOwnerDUNS", meter.MeterOwnerDuns),
                    new XElement("MeterInstaller", meter.MeterInstaller),
                    new XElement("MeterInstallerDUNS", meter.MeterInstallerDuns),
                    new XElement("MeterReader", meter.MeterReader),
                    new XElement("MeterReaderDUNS", meter.MeterReaderDuns),
                    new XElement("MeterMaintenanceProvider", meter.MeterMaintenanceProvider),
                    new XElement("MeterMaintenanceProviderDUNS", meter.MeterMaintenanceProviderDuns),
                    new XElement("MeterDataManagementAgent", meter.MeterDataManagementAgent),
                    new XElement("MeterDataManagementAgentDUNS", meter.MeterDataManagementAgentDuns),
                    new XElement("SchedulingCoordinator", meter.SchedulingCoordinator),
                    new XElement("SchedulingCoordinatorDUNS", meter.SchedulingCoordinatorDuns),
                    new XElement("InstallPending", meter.MeterInstallPending),
                    new XElement("PackageOption", meter.PackageOption),
                    new XElement("UsageCode", meter.UsageCode),
                    new XElement("MeterServiceVoltage", meter.MeterServiceVoltage),
                    new XElement("SummaryInterval", meter.SummaryInterval));
                
                loopElement.Add(element);
                logger.TraceFormat("Added 814 \"ServiceMeter\" XML element for Service {0}", meter.ServiceKey);

                var changes = exportDataAccess.ListServiceMeterChanges(meterKey);
                WriteServiceMeterChange(element, changes);

                var tous = exportDataAccess.ListServiceMeterTous(meterKey);
                WriteServiceMeterTou(element, tous);

                var types = exportDataAccess.ListServiceMeterTypes(meterKey);
                WriteServiceMeterType(element, types);
            }
        }

        public void WriteServiceMeterChange(XContainer container, Type814ServiceMeterChange[] changes)
        {
            if (changes == null || changes.Length == 0)
                return;

            var loopElement = new XElement("ServiceMeterChangeLoop");
            container.Add(loopElement);

            foreach (var change in changes)
            {
                if (!change.ChangeKey.HasValue)
                    continue;
                
                loopElement.Add(new XElement("ServiceMeterChange",
                    new XElement("MeterChangeKey", change.ChangeKey),
                    new XElement("MeterKey", change.MeterKey),
                    new XElement("ChangeReason", change.ChangeReason),
                    new XElement("ChangeDescription", change.ChangeDescription)));
                logger.TraceFormat("Added 814 \"ServiceMeterChange\" XML element for Meter {0}", change.MeterKey);
            }
        }

        public void WriteServiceMeterTou(XContainer container, Type814ServiceMeterTou[] tous)
        {
            if (tous == null || tous.Length == 0)
                return;

            var loopElement = new XElement("ServiceMeterTOULoop");
            container.Add(loopElement);

            foreach (var tou in tous)
            {
                if (!tou.TouKey.HasValue)
                    continue;

                loopElement.Add(new XElement("ServiceMeterTOU",
                    new XElement("MeterTOUKey", tou.TouKey),
                    new XElement("MeterKey", tou.MeterKey),
                    new XElement("TOUCode", tou.TouCode),
                    new XElement("MeasurementType", tou.MeasurementType)));
                logger.TraceFormat("Added 814 \"ServiceMeterTOU\" XML element for Meter {0}", tou.MeterKey);
            }
        }

        public void WriteServiceMeterType(XContainer container, Type814ServiceMeterType[] types)
        {
            if (types == null || types.Length == 0)
                return;

            var loopElement = new XElement("ServiceMeterTypeLoop");
            container.Add(loopElement);

            foreach (var type in types)
            {
                if (!type.TypeKey.HasValue)
                    continue;

                var element = new XElement("ServiceMeterType",
                    new XElement("MeterTypeKey", type.TypeKey),
                    new XElement("MeterKey", type.MeterKey));

                element.TryAddElement("MeterMultiplier", type.MeterMultiplier);
                element.TryAddElement("MeterType", type.MeterType);
                element.TryAddElement("ProductType", type.ProductType);
                element.TryAddElement("TimeOfUse", type.TimeOfUse);
                element.TryAddElement("NumberOfDials", type.NumberOfDials);
                element.TryAddElement("UnmeteredNumberOfDevices", type.UnmeteredNumberOfDevices);
                element.TryAddElement("UnmeteredDescription", type.UnmeteredDescription);
                element.TryAddElement("StartMeterRead", type.StartMeterRead);
                element.TryAddElement("EndMeterRead", type.EndMeterRead);
                element.TryAddElement("ChangeReason", type.ChangeReason);
                element.TryAddElement("TimeOfUse2", type.TimeOfUse2);
                loopElement.Add(element);
                logger.TraceFormat("Added 814 \"ServiceMeterType\" XML element for Meter {0}", type.MeterKey);
            }
        }

        public void WriteServiceReject(XContainer container, Type814ServiceReject[] rejects)
        {
            if (rejects == null || rejects.Length == 0)
                return;

            var loopElement = new XElement("ServiceRejectLoop");
            container.Add(loopElement);

            foreach (var reject in rejects)
            {
                if (!reject.RejectKey.HasValue)
                    continue;
                
                loopElement.Add(new XElement("ServiceReject",
                    new XElement("RejectKey", reject.RejectKey),
                    new XElement("ServiceKey", reject.ServiceKey),
                    new XElement("RejectCode", reject.RejectCode),
                    new XElement("RejectReason", reject.RejectReason)));
                logger.TraceFormat("Added 814 \"ServiceReject\" XML element for Service {0}", reject.ServiceKey);
            }
        }

        public void WriteServiceStatus(XContainer container, Type814ServiceStatus[] statuses)
        {
            if (statuses == null || statuses.Length == 0)
                return;

            var loopElement = new XElement("ServiceStatusLoop");
            container.Add(loopElement);

            foreach (var status in statuses)
            {
                if (!status.StatusKey.HasValue)
                    continue;

                var element = new XElement("ServiceReject",
                    new XElement("StatusKey", status.StatusKey),
                    new XElement("ServiceKey", status.ServiceKey));

                element.TryAddElement("StatusReason", status.StatusReason);
                element.TryAddElement("StatusType", status.StatusType);

                loopElement.Add(element);
                logger.TraceFormat("Added 814 \"ServiceReject\" XML element for Service {0}", status.ServiceKey);
            }
        }

        public void WriteServiceAccountChange(XContainer container, Type814ServiceAccountChange[] changes)
        {
            if (changes == null || changes.Length == 0)
                return;

            var loopElement = new XElement("ServiceAccountChangeLoop");
            container.Add(loopElement);

            foreach (var change in changes)
            {
                if (!change.ChangeKey.HasValue)
                    continue;

                loopElement.Add(new XElement("ServiceAccountChange",
                    new XElement("ChangeKey", change.ChangeKey),
                    new XElement("ServiceKey", change.ServiceKey),
                    new XElement("ChangeReason", change.ChangeReason),
                    new XElement("ChangeDescription", change.ChangeDescription)));
                logger.TraceFormat("Added 814 \"ServiceAccountChange\" XML element for Service {0}", change.ServiceKey);
            }
        }

        public void WriteServiceDate(XContainer container, Type814ServiceDate[] dates)
        {
            if (dates == null || dates.Length == 0)
                return;

            var loopElement = new XElement("ServiceDateLoop");
            container.Add(loopElement);

            foreach (var date in dates)
            {
                if (!date.DateKey.HasValue)
                    continue;

                loopElement.Add(new XElement("ServiceDate",
                    new XElement("DateKey", date.DateKey),
                    new XElement("ServiceKey", date.ServiceKey),
                    new XElement("Qualifier", date.Qualifier),
                    new XElement("Date", date.Date),
                    new XElement("Time", date.Time),
                    new XElement("TimeCode", date.TimeCode),
                    new XElement("PeriodFormat", date.PeriodFormat),
                    new XElement("Period", date.Period),
                    new XElement("NotesDate", date.NotesDate)));
                logger.TraceFormat("Added 814 \"ServiceDate\" XML element for Service {0}", date.ServiceKey);
            }
        }
    }
}

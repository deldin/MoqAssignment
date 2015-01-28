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
    public class Export650Xml : IMarketFileExporter
    {
        private readonly IClientDataAccess clientDataAccess;
        private readonly IMarketDataAccess marketDataAccess;
        private readonly IMarket650Export exportDataAccess;
        private readonly ILogger logger;

        public Export650Xml(IClientDataAccess clientDataAccess, IMarketDataAccess marketDataAccess, IMarket650Export exportDataAccess, ILogger logger)
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

                var model = new Export650Model
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
                        "No CSP DUNS Trading Partner record exists between CR DUNS \"{0}\" and LDC DUNS \"{1}\". 650 Transactions will not be exported.",
                        xmlPort.Duns, xmlPort.LdcDuns);

                    continue;
                }

                marketDataAccess.LoadCspDunsTradingPartnerConfig(partner);
                model.CspDunsTradingPartnerId = partner.CspDunsTradingPartnerId;

                var headers = exportDataAccess.ListUnprocessed(xmlPort.LdcDuns, xmlPort.Duns, 2);
                if (headers.Length == 0)
                {
                    logger.TraceFormat("Zero 650 Xml records found to export for TDSP Duns \"{0}\" and CR Duns \"{1}\".",
                        xmlPort.LdcDuns, xmlPort.Duns);
                    continue;
                }

                logger.DebugFormat("Exporting {0} unprocessed 650 record(s) for TDSP Duns \"{1}\" and CR Duns \"{2}\".", 
                    headers.Length, xmlPort.LdcDuns, xmlPort.Duns);

                XNamespace marketNamespace = "http://CIS.Integration.Schema.Market.Common.Market650";
                var document = new XDocument(
                    new XElement(marketNamespace + "Market650",
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
                    logger.Error("Unable to create 650 XML document. Root node element is null.");
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

        public void WriteHeader(XContainer container, Type650Header header)
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
            element.TryAddElement("TransactionDate", header.TransactionDate);
            element.TryAddElement("TransactionNbr", header.TransactionNbr);
            element.TryAddElement("ReferenceNbr", header.ReferenceNbr);
            element.TryAddElement("TransactionType", header.TransactionType);
            element.TryAddElement("ActionCode", header.ActionCode);
            element.TryAddElement("TdspName", header.TdspName);
            element.TryAddElement("TdspDuns", header.TdspDuns);
            element.TryAddElement("CrName", header.CrName);
            element.TryAddElement("CrDuns", header.CrDuns);
            element.TryAddElement("ProcessedReceivedDateTime", header.ProcessedReceivedDateTime);
            container.Add(element);
            logger.TraceFormat("Added 650 \"Header\" XML element for Header {0}", headerKey);

            var names = exportDataAccess.ListNames(headerKey);
            WriteName(element, names);

            var services = exportDataAccess.ListServices(headerKey);
            WriteService(element, services);
        }

        public void WriteName(XContainer container, Type650Name[] names)
        {
            if (names == null || names.Length == 0)
                return;

            var loopElement = new XElement("NameLoop");
            container.Add(loopElement);

            foreach (var name in names)
            {
                if (!name.NameKey.HasValue)
                    continue;

                var element = new XElement("Name",
                    new XElement("NameKey", name.NameKey),
                    new XElement("HeaderKey", name.HeaderKey));

                element.TryAddElement("EntityIdType", name.EntityIdType);
                element.TryAddElement("EntityName", name.EntityName);
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
                element.TryAddElement("ContactCode", name.ContactCode);
                element.TryAddElement("ContactPhoneNbr1", name.ContactPhoneNbr1);
                element.TryAddElement("ContactPhoneNbr2", name.ContactPhoneNbr2);
                loopElement.Add(element);
                logger.TraceFormat("Added 650 \"Name\" XML element for Header {0}", name.HeaderKey);
            }
        }

        public void WriteService(XContainer container, Type650Service[] services)
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
                    new XElement("ServiceKey", serviceKey),
                    new XElement("HeaderKey", service.HeaderKey));

                element.TryAddElement("PurposeCode", service.PurposeCode);
                element.TryAddElement("PriorityCode", service.PriorityCode);
                element.TryAddElement("ESIID", service.EsiId);
                element.TryAddElement("SpecialProcessCode", service.SpecialProcessCode);
                element.TryAddElement("ServiceReqDate", service.ServiceReqDate);
                element.TryAddElement("NotBeforeDate", service.NotBeforeDate);
                element.TryAddElement("CallAhead", service.CallAhead);
                element.TryAddElement("PremLocation", service.PremLocation);
                element.TryAddElement("AccStatusCode", service.AccStatusCode);
                element.TryAddElement("AccStatusDesc", service.AccStatusDesc);
                element.TryAddElement("EquipLocation", service.EquipLocation);
                element.TryAddElement("ServiceOrderNbr", service.ServiceOrderNbr);
                element.TryAddElement("CompletionDate", service.CompletionDate);
                element.TryAddElement("CompletionTime", service.CompletionTime);
                element.TryAddElement("ReportRemarks", service.ReportRemarks);
                element.TryAddElement("Directions", service.Directions);
                element.TryAddElement("MeterNbr", service.MeterNbr);
                element.TryAddElement("MeterReadDate", service.MeterReadDate);
                element.TryAddElement("MeterTestDate", service.MeterTestDate);
                element.TryAddElement("MeterTestResults", service.MeterTestResults);
                element.TryAddElement("IncidentCode", service.IncidentCode);
                element.TryAddElement("EstRestoreDate", service.EstRestoreDate);
                element.TryAddElement("EstRestoreTime", service.EstRestoreTime);
                element.TryAddElement("IntStartDate", service.IntStartDate);
                element.TryAddElement("IntStartTime", service.IntStartTime);
                element.TryAddElement("RepairRecommended", service.RepairRecommended);
                element.TryAddElement("Rescheduled", service.Rescheduled);
                element.TryAddElement("InterDuractionPeriod", service.InterDurationPeriod);
                element.TryAddElement("AreaOutage", service.AreaOutage);
                element.TryAddElement("CustRepairRemarks", service.CustRepairRemarks);
                element.TryAddElement("MeterReadUOM", service.MeterReadUom);
                element.TryAddElement("MeterRead", service.MeterRead);
                element.TryAddElement("Membership", service.Membership);
                element.TryAddElement("RemarksPermanentSuspend", service.RemarksPermanentSuspend);
                element.TryAddElement("DisconnectAuthorization", service.DisconnectAuthorization);
                loopElement.Add(element);
                logger.TraceFormat("Added 650 \"Service\" XML element for Header {0}", service.HeaderKey);

                var changes = exportDataAccess.ListServiceChanges(serviceKey);
                WriteServiceChange(element, changes);

                var meters = exportDataAccess.ListServiceMeters(serviceKey);
                WriteServiceMeter(element, meters);

                var poles = exportDataAccess.ListServicePoles(serviceKey);
                WriteServicePole(element, poles);

                var rejects = exportDataAccess.ListServiceRejects(serviceKey);
                WriteServiceReject(element, rejects);
            }
        }

        public void WriteServiceChange(XContainer container, Type650ServiceChange[] changes)
        {
            if (changes == null || changes.Length == 0)
                return;

            var loopElement = new XElement("ServiceChangeLoop");
            container.Add(loopElement);

            foreach (var change in changes)
            {
                if (!change.ServiceChangeKey.HasValue)
                    continue;

                var element = new XElement("ServiceChange",
                    new XElement("ServiceKey", change.ServiceKey),
                    new XElement("ServiceChangeKey", change.ServiceChangeKey));

                element.TryAddElement("ChangeReason", change.ChangeReason);
                loopElement.Add(element);
                logger.TraceFormat("Added 650 \"ServiceChange\" XML element for Service {0}", change.ServiceKey);
            }
        }

        public void WriteServiceMeter(XContainer container, Type650ServiceMeter[] meters)
        {
            if (meters == null || meters.Length == 0)
                return;

            var loopElement = new XElement("ServiceMeterLoop");
            container.Add(loopElement);

            foreach (var meter in meters)
            {
                if (!meter.ServiceMeterKey.HasValue)
                    continue;

                var element = new XElement("ServiceMeter",
                    new XElement("ServiceKey", meter.ServiceKey),
                    new XElement("ServiceMeterKey", meter.ServiceMeterKey));

                element.TryAddElement("MeterNumber", meter.MeterNumber);
                loopElement.Add(element);
                logger.TraceFormat("Added 650 \"ServiceMeter\" XML element for Header {0}", meter.ServiceKey);
            }
        }

        public void WriteServicePole(XContainer container, Type650ServicePole[] poles)
        {
            if (poles == null || poles.Length == 0)
                return;

            var loopElement = new XElement("ServicePoleLoop");
            container.Add(loopElement);

            foreach (var pole in poles)
            {
                if (!pole.ServicePoleKey.HasValue)
                    continue;

                var element = new XElement("ServicePole",
                    new XElement("ServiceKey", pole.ServiceKey),
                    new XElement("ServicePoleKey", pole.ServicePoleKey));

                element.TryAddElement("PoleNbr", pole.PoleNbr);
                loopElement.Add(element);
                logger.TraceFormat("Added 650 \"ServicePole\" XML element for Header {0}", pole.ServiceKey);
            }
        }

        public void WriteServiceReject(XContainer container, Type650ServiceReject[] rejects)
        {
            if (rejects == null || rejects.Length == 0)
                return;

            var loopElement = new XElement("ServiceRejectLoop");
            container.Add(loopElement);

            foreach (var reject in rejects)
            {
                if (!reject.ServiceRejectKey.HasValue)
                    continue;

                var element = new XElement("ServiceReject",
                    new XElement("ServiceKey", reject.ServiceKey),
                    new XElement("ServiceRejectKey", reject.ServiceRejectKey));

                element.TryAddElement("RejectCode", reject.RejectCode);
                element.TryAddElement("RejectReason", reject.RejectReason);
                element.TryAddElement("UnexCode", reject.UnexCode);
                element.TryAddElement("UnexReason", reject.UnexReason);
                loopElement.Add(element);
                logger.TraceFormat("Added 650 \"ServiceReject\" XML element for Header {0}", reject.ServiceKey);
            }
        }
    }
}

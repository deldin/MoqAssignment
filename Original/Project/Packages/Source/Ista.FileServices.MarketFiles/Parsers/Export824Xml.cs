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
    public class Export824Xml : IMarketFileExporter
    {
        private readonly IClientDataAccess clientDataAccess;
        private readonly IMarketDataAccess marketDataAccess;
        private readonly IMarket824Export exportDataAccess;
        private readonly ILogger logger;

        public Export824Xml(IClientDataAccess clientDataAccess, IMarketDataAccess marketDataAccess, IMarket824Export exportDataAccess, ILogger logger)
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

                var model = new Export824Model
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
                        "No CSP DUNS Trading Partner record exists between CR DUNS \"{0}\" and LDC DUNS \"{1}\". 824 Transactions will not be exported.",
                        xmlPort.Duns, xmlPort.LdcDuns);

                    continue;
                }

                marketDataAccess.LoadCspDunsTradingPartnerConfig(partner);
                model.CspDunsTradingPartnerId = partner.CspDunsTradingPartnerId;

                var headers = exportDataAccess.ListUnprocessed(xmlPort.LdcDuns, xmlPort.Duns, 2);
                if (headers.Length == 0)
                {
                    logger.TraceFormat("Zero 824 Xml records found to export for TDSP Duns \"{0}\" and CR Duns \"{1}\".",
                        xmlPort.LdcDuns, xmlPort.Duns);
                    continue;
                }

                logger.DebugFormat("Exporting {0} unprocessed 824 record(s) for TDSP Duns \"{1}\" and CR Duns \"{2}\".",
                    headers.Length, xmlPort.LdcDuns, xmlPort.Duns);

                XNamespace marketNamespace = "http://CIS.Integration.Schema.Market.Common.Market824";
                var document = new XDocument(
                    new XElement(marketNamespace + "Market824",
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
                    logger.Error("Unable to create 824 XML document. Root node element is null.");
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

        public void WriteHeader(XContainer container, Type824Header header)
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
            element.TryAddElement("ReportTypeCode", header.ReportTypeCode);
            element.TryAddElement("ActionCode", header.ActionCode);
            element.TryAddElement("TdspName", header.TdspName);
            element.TryAddElement("TdspDuns", header.TdspDuns);
            element.TryAddElement("CrName", header.CrName);
            element.TryAddElement("CrDuns", header.CrDuns);
            element.TryAddElement("AppAckCode", header.AppAckCode);
            element.TryAddElement("ReferenceNbr", header.ReferenceNbr);
            element.TryAddElement("TransactionSetNbr", header.TransactionSetNbr);
            element.TryAddElement("EsiId", header.EsiId);
            element.TryAddElement("CrQualifier", header.CrQualifier);
            element.TryAddElement("TdspQualifier", header.TdspQualifier);
            element.TryAddElement("ESPUtilityAccountNumber", header.EspUtilityAccountNumber);
            element.TryAddElement("CustomerName", header.CustomerName);
            element.TryAddElement("ESPCustomerAccountNumber", header.EspCustomerAccountNumber);
            element.TryAddElement("PreviousUtilityAccountNumber", header.PreviousUtilityAccountNumber);
            container.Add(element);
            logger.TraceFormat("Added 824 \"Header\" XML element for Header {0}", headerKey);

            var reasons = exportDataAccess.ListReasons(headerKey);
            WriteReason(element, reasons);

            var references = exportDataAccess.ListReferences(headerKey);
            WriteReference(element, references);
        }

        public void WriteReason(XContainer container, Type824Reason[] reasons)
        {
            if (reasons == null || reasons.Length == 0)
                return;

            var loopElement = new XElement("ReasonLoop");
            container.Add(loopElement);

            foreach (var reason in reasons)
            {
                if (!reason.ReasonKey.HasValue)
                    continue;

                var reasonKey = reason.ReasonKey.Value;
                var element = new XElement("Reason",
                    new XElement("ReasonKey", reasonKey),
                    new XElement("HeaderKey", reason.HeaderKey));

                element.TryAddElement("ReasonCode", reason.ReasonCode);
                element.TryAddElement("ReasonText", reason.ReasonText);
                loopElement.Add(element);
                logger.TraceFormat("Added 824 \"Reason\" XML element for Header {0}", reason.HeaderKey);
            }
        }

        public void WriteReference(XContainer container, Type824Reference[] references)
        {
            if (references == null || references.Length == 0)
                return;

            var loopElement = new XElement("ReferenceLoop");
            container.Add(loopElement);

            foreach (var reference in references)
            {
                if (!reference.ReferenceKey.HasValue)
                    continue;

                var referenceKey = reference.ReferenceKey.Value;
                var element = new XElement("Reference",
                    new XElement("ReferenceKey", referenceKey),
                    new XElement("HeaderKey", reference.HeaderKey));

                element.TryAddElement("AppAckCode", reference.AppAckCode);
                element.TryAddElement("ReferenceQualifier", reference.ReferenceQualifier);
                element.TryAddElement("ReferenceNbr", reference.ReferenceNbr);
                element.TryAddElement("TransactionSetId", reference.TransactionSetId);
                element.TryAddElement("CrossReferenceNbr", reference.CrossReferenceNbr);
                element.TryAddElement("PurchaseOrderNbr", reference.PurchaseOrderNbr);
                element.TryAddElement("PaymentsAppliedThroughDate", reference.PaymentsAppliedThroughDate);
                element.TryAddElement("TotalPaymentsApplied", reference.TotalPaymentsApplied);
                element.TryAddElement("PaymentDueDate", reference.PaymentDueDate);
                element.TryAddElement("TotalAmountDue", reference.TotalAmountDue);
                loopElement.Add(element);
                logger.TraceFormat("Added 824 \"Reference\" XML element for Header {0}", reference.HeaderKey);
            }
        }
    }
}

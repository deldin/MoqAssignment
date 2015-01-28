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
    public class Export820Xml : IMarketFileExporter
    {
        private readonly IClientDataAccess clientDataAccess;
        private readonly IMarketDataAccess marketDataAccess;
        private readonly IMarket820Export exportDataAccess;
        private readonly ILogger logger;

        public Export820Xml(IClientDataAccess clientDataAccess, IMarketDataAccess marketDataAccess, IMarket820Export exportDataAccess, ILogger logger)
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

                var model = new Export820Model
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
                        "No CSP DUNS Trading Partner record exists between CR DUNS \"{0}\" and LDC DUNS \"{1}\". 820 Transactions will not be exported.",
                        xmlPort.Duns, xmlPort.LdcDuns);

                    continue;
                }

                marketDataAccess.LoadCspDunsTradingPartnerConfig(partner);
                model.CspDunsTradingPartnerId = partner.CspDunsTradingPartnerId;

                var headers = exportDataAccess.ListUnprocessed(xmlPort.LdcDuns, xmlPort.Duns, 2);
                if (headers.Length == 0)
                {
                    logger.TraceFormat("Zero 820 Xml records found to export for TDSP Duns \"{0}\" and CR Duns \"{1}\".",
                        xmlPort.LdcDuns, xmlPort.Duns);
                    continue;
                }

                logger.DebugFormat("Exporting {0} unprocessed 820 record(s) for TDSP Duns \"{1}\" and CR Duns \"{2}\".",
                    headers.Length, xmlPort.LdcDuns, xmlPort.Duns);

                XNamespace marketNamespace = "http://CIS.Integration.Schema.Market.Common.Market820";
                var document = new XDocument(
                    new XElement(marketNamespace + "Market820",
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
                    logger.Error("Unable to create 820 XML document. Root node element is null.");
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

        public void WriteHeader(XContainer container, Type820Header header)
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
            element.TryAddElement("TransactionDate", header.TransactionDate);
            element.TryAddElement("TransactionNbr", header.TransactionNbr);
            element.TryAddElement("TotalAmount", header.TotalAmount);
            element.TryAddElement("CreditDebitFlag", header.CreditDebitFlag);
            element.TryAddElement("PaymentMethodCode", header.PaymentMethodCode);
            element.TryAddElement("TraceReferenceNbr", header.TraceReferenceNbr);
            element.TryAddElement("TdspDunsStructureCode", header.TdspDunsStructureCode);
            element.TryAddElement("TdspName", header.TdspName);
            element.TryAddElement("TdspDuns", header.TdspDuns);
            element.TryAddElement("CrName", header.CrName);
            element.TryAddElement("CrDuns", header.CrDuns);
            element.TryAddElement("ESPUtilityAccountNumber", header.ESPUtilityAccountNumber);
            element.TryAddElement("CreateDate", header.CreateDate);
            container.Add(element);
            logger.TraceFormat("Added 820 \"Header\" XML element for Header {0}", headerKey);

            var details = exportDataAccess.ListDetails(headerKey);
            WriteDetail(element, details);
        }

        public void WriteDetail(XContainer container, Type820Detail[] details)
        {
            if (details == null || details.Length == 0)
                return;

            var loopElement = new XElement("DetailLoop");
            container.Add(loopElement);

            foreach (var detail in details)
            {
                if (!detail.DetailKey.HasValue)
                    continue;

                var element = new XElement("Detail",
                    new XElement("DetailKey", detail.DetailKey),
                    new XElement("HeaderKey", detail.HeaderKey));

                element.TryAddElement("AssignedId", detail.AssignedId);
                element.TryAddElement("ReferenceId", detail.ReferenceId);
                element.TryAddElement("ReferenceNbr", detail.ReferenceNbr);
                element.TryAddElement("CrossReferenceNbr", detail.CrossReferenceNbr);
                element.TryAddElement("PaymentActionCode", detail.PaymentActionCode);
                element.TryAddElement("PaymentAmount", detail.PaymentAmount);
                element.TryAddElement("AdjustmentReasonCode", detail.AdjustmentReasonCode);
                element.TryAddElement("AdjustmentAmount", detail.AdjustmentAmount);
                element.TryAddElement("EsiId", detail.EsiId);
                element.TryAddElement("ProcessFlag", detail.ProcessFlag.ToString());
                element.TryAddElement("ProcessDate", detail.ProcessDate.ToString());
                element.TryAddElement("CommodityCode", detail.CommodityCode);
                element.TryAddElement("InvoiceAmount", detail.InvoiceAmount);
                element.TryAddElement("DiscountAmount", detail.DiscountAmount);
                element.TryAddElement("PrevUtilityAccountNumber", detail.PrevUtilityAccountNumber);
                element.TryAddElement("ESPAccountNumber", detail.ESPAccountNumber);
                element.TryAddElement("CustomerName", detail.CustomerName);
                element.TryAddElement("DatePosted", detail.DatePosted);
                element.TryAddElement("UnmeteredServiceDesignator", detail.UnmeteredServiceDesignator);
                loopElement.Add(element);
                logger.TraceFormat("Added 820 \"Detail\" XML element for Header {0}", detail.HeaderKey);
            }
        }
    }
}

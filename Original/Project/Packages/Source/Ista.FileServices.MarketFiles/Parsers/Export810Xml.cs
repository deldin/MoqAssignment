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
    public class Export810Xml : IMarketFileExporter
    {
        private readonly IClientDataAccess clientDataAccess;
        private readonly IMarketDataAccess marketDataAccess;
        private readonly IMarket810Export exportDataAccess;
        private readonly ILogger logger;

        public Export810Xml(IClientDataAccess clientDataAccess, IMarketDataAccess marketDataAccess, IMarket810Export exportDataAccess, ILogger logger)
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

                var model = new Export810Model
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
                        "No CSP DUNS Trading Partner record exists between CR DUNS \"{0}\" and LDC DUNS \"{1}\". 810 Transactions will not be exported.",
                        xmlPort.Duns, xmlPort.LdcDuns);

                    continue;
                }

                marketDataAccess.LoadCspDunsTradingPartnerConfig(partner);
                model.CspDunsTradingPartnerId = partner.CspDunsTradingPartnerId;

                var headers = exportDataAccess.ListUnprocessed(xmlPort.LdcDuns, xmlPort.Duns, 2);
                if (headers.Length == 0)
                {
                    logger.TraceFormat("Zero 810 Xml records found to export for TDSP Duns \"{0}\" and CR Duns \"{1}\".",
                        xmlPort.LdcDuns, xmlPort.Duns);
                    continue;
                }

                logger.DebugFormat("Exporting {0} unprocessed 810 record(s) for TDSP Duns \"{1}\" and CR Duns \"{2}\".",
                    headers.Length, xmlPort.LdcDuns, xmlPort.Duns);

                XNamespace marketNamespace = "http://CIS.Integration.Schema.Market.Common.Market810";
                var document = new XDocument(
                    new XElement(marketNamespace + "Market810",
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
                    logger.Error("Unable to create 810 XML document. Root node element is null.");
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

        public void WriteHeader(XContainer container, Type810Header header)
        {
            if (header == null)
                return;

            if (!header.HeaderKey.HasValue)
                return;

            var headerKey = header.HeaderKey.Value;
            var element = new XElement("Header",
                new XElement("HeaderKey", headerKey),
                new XElement("MarketFileId", header.MarketFileId));

            element.TryAddElement("TransactionSetId", header.TransactionSetId);
            element.TryAddElement("TransactionSetControlNbr", header.TransactionSetControlNbr);
            element.TryAddElement("TransactionDate", header.TransactionDate);
            element.TryAddElement("InvoiceNbr", header.InvoiceNbr);
            element.Add(new XElement("ReleaseNbr", header.ReleaseNbr));
            element.TryAddElement("TransactionTypeCode", header.TransactionTypeCode);
            element.TryAddElement("TransactionSetPurposeCode", header.TransactionSetPurposeCode);
            element.TryAddElement("OriginalInvoiceNbr", header.OriginalInvoiceNbr);
            element.TryAddElement("EsiId", header.EsiId);

            if (string.IsNullOrEmpty(header.AlternateEsiId))
                element.TryAddElement("AlternateEsiId", header.EsiId);
            else
                element.TryAddElement("AlternateEsiId", header.AlternateEsiId);

            element.TryAddElement("CRAccountNumber", header.CrAccountNumber);
            element.TryAddElement("PaymentDueDate", header.PaymentDueDate);
            element.TryAddElement("TdspDuns", header.TdspDuns);
            element.TryAddElement("TdspName", header.TdspName);
            element.TryAddElement("CrDuns", header.CrDuns);
            element.TryAddElement("CrName", header.CrName);

            decimal totalAmount;
            if (decimal.TryParse(header.TotalAmount, out totalAmount))
                totalAmount *= 100;
            else
                totalAmount = 0M;

            element.TryAddElement("TotalAmount", totalAmount.ToString("0"));
            element.TryAddElement("CustNoForESCO", header.CustNoForESCO);
            element.TryAddElement("PreviousUtilityAccountNumber", header.PreviousUtilityAccountNumber);
            element.TryAddElement("BillPresenter", header.BillPresenter);
            element.TryAddElement("BillCalculator", header.BillCalculator);
            element.TryAddElement("GasPoolId", header.GasPoolId);
            element.TryAddElement("CustomerDUNS", header.CustomerDUNS);
            element.TryAddElement("ServiceDeliveryPoint", header.ServiceDeliveryPoint);
            container.Add(element);
            logger.TraceFormat("Added 810 \"Header\" XML element for Header {0}", headerKey);

            var balances = exportDataAccess.ListBalances(headerKey);
            WriteBalance(element, balances);

            var details = exportDataAccess.ListDetails(headerKey);
            WriteDetail(element, details);

            var names = exportDataAccess.ListNames(headerKey);
            WriteName(element, names);

            var summaries = exportDataAccess.ListSummaries(headerKey);
            WriteSummary(element, summaries);

            var messages = exportDataAccess.ListMessages(headerKey);
            WriteMessage(element, messages);

            if (clientDataAccess.ShouldExportMeterData(header.TdspDuns))
                WriteMeter(element, details[0].Items);
        }

        public void WriteBalance(XContainer container, Type810Balance[] balances)
        {
            if (!balances.Any()) 
                return;

            var loopElement = new XElement("BalanceLoop");
            container.Add(loopElement);

            foreach (var balance in balances)
            {
                if (!balance.BalanceKey.HasValue)
                    continue;

                var element = new XElement("Balance",
                    new XElement("HeaderKey", balance.HeaderKey),
                    new XElement("BalanceKey", balance.BalanceKey));

                element.TryAddElement("TotalOutstandingBalance", balance.TotalOutstandingBalance);
                element.TryAddElement("BeginningBalance", balance.BeginningBalance);
                element.TryAddElement("BudgetCumulativeDifference", balance.BudgetCumulativeDifference);
                element.TryAddElement("BudgetMonthDifference", balance.BudgetMonthDifference);
                element.TryAddElement("BudgetBilledToDate", balance.BudgetBilledToDate);
                element.TryAddElement("ActualBilledToDate", balance.ActualBilledToDate);
    
                loopElement.Add(element);
                logger.TraceFormat("Added 810 \"Balance\" XML element for Header {0}", balance.HeaderKey);
            }
        }

        public void WriteDetail(XContainer container, Type810Detail[] details)
        {
            if (!details.Any()) 
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
                element.TryAddElement("ServiceTypeCode", detail.ServiceTypeCode);
                element.TryAddElement("ServiceType", detail.ServiceType);
                element.TryAddElement("ServiceClassCode", detail.ServiceClassCode);
                element.TryAddElement("ServiceClass", detail.ServiceClass);
                element.TryAddElement("RateClass", detail.RateClass);
                element.TryAddElement("RateSubClass", detail.RateSubClass);
                element.TryAddElement("ServicePeriodStartDate", detail.ServicePeriodStartDate);
                element.TryAddElement("ServicePeriodEndDate", detail.ServicePeriodEndDate);
                element.TryAddElement("MeterNumber", detail.MeterNumber);
                element.TryAddElement("BillCycle", detail.BillCycle);
                element.TryAddElement("GasPoolId", detail.GasPoolId);
                element.TryAddElement("ServiceAgreement", detail.ServiceAgreement);
                element.TryAddElement("RateCode", detail.RateCode);
                element.TryAddElement("SupplierContractID", detail.SupplierContractId);
                element.TryAddElement("UtilityContractID", detail.UtilityContractId);

                loopElement.Add(element);
                logger.TraceFormat("Added 810 \"Detail\" XML element for Header {0}", detail.HeaderKey);

                var detailItems = exportDataAccess.ListDetailItems(detail.DetailKey.Value);
                WriteDetailItem(element, detailItems);

                var detailTaxes = exportDataAccess.ListDetailTaxes(detail.DetailKey.Value);
                WriteDetailTax(element, detailTaxes);
            }
        }

        public void WriteDetailItem(XContainer container, Type810DetailItem[] detailItems)
        {
            if (!detailItems.Any()) 
                return;

            var loopElement = new XElement("DetailItemLoop");
            container.Add(loopElement);

            foreach (var detailItem in detailItems)
            {
                if (!detailItem.ItemKey.HasValue)
                    continue;

                var element = new XElement("DetailItem",
                    new XElement("DetailItemKey", detailItem.ItemKey),
                    new XElement("DetailKey", detailItem.DetailKey));

                element.TryAddElement("AssignedId", detailItem.AssignedId);
                element.TryAddElement("RelationshipCode", detailItem.RelationshipCode);
                element.TryAddElement("ServiceOrderCompleteDate", detailItem.ServiceOrderCompleteDate);
                element.TryAddElement("UnmeteredServiceDateRange", detailItem.UnmeteredServiceDateRange);
                element.TryAddElement("InvoiceNbr", detailItem.InvoiceNbr);
                element.TryAddElement("ServiceOrderNbr", detailItem.ServiceOrderNbr);
                element.TryAddElement("Consumption", detailItem.Consumption);
                element.TryAddElement("EffectiveDate", detailItem.EffectiveDate);

                loopElement.Add(element);
                logger.TraceFormat("Added 810 \"DetailItem\" XML element for Detail {0}", detailItem.DetailKey);

                var detailItemCharges = exportDataAccess.ListDetailItemCharges(detailItem.ItemKey.Value);
                WriteDetailItemCharge(element, detailItemCharges);

                var detailItemTaxes = exportDataAccess.ListDetailItemTaxes(detailItem.ItemKey.Value);
                WriteDetailItemTax(element, detailItemTaxes);
            }
        }

        public void WriteDetailItemCharge(XContainer container, Type810DetailItemCharge[] detailItemCharges)
        {
            if (!detailItemCharges.Any()) 
                return;

            var loopElement = new XElement("DetailItemChargeLoop");
            container.Add(loopElement);

            foreach (var detailItemCharge in detailItemCharges)
            {
                if (!detailItemCharge.ChargeKey.HasValue)
                    continue;

                var element = new XElement("DetailItemCharge",
                    new XElement("DetailItemChargeKey", detailItemCharge.ChargeKey),
                    new XElement("DetailItemKey", detailItemCharge.ItemKey));

                element.TryAddElement("ChargeIndicator", detailItemCharge.ChargeIndicator);
                element.TryAddElement("AgencyCode", detailItemCharge.AgencyCode);
                element.TryAddElement("ChargeCode", detailItemCharge.ChargeCode);
                element.TryAddElement("Amount", detailItemCharge.Amount.Replace(".",""));
                element.TryAddElement("Rate", detailItemCharge.Rate);
                element.TryAddElement("UOM", detailItemCharge.UOM);
                element.TryAddElement("Quantity", detailItemCharge.Quantity);
                element.TryAddElement("Description", detailItemCharge.Description);
                element.TryAddElement("PrintSeqId", detailItemCharge.PrintSeqId);
                element.TryAddElement("EnergyCharges", detailItemCharge.EnergyCharges);

                loopElement.Add(element);
                logger.TraceFormat("Added 810 \"DetailItemChange\" XML element for Detail Item {0}", detailItemCharge.ItemKey);
            }
        }

        public void WriteDetailItemTax(XContainer container, Type810DetailItemTax[] detailItemTaxes)
        {
            if (!detailItemTaxes.Any()) 
                return;

            var loopElement = new XElement("DetailItemTaxLoop");
            container.Add(loopElement);

            foreach (var detailItemTax in detailItemTaxes)
            {
                if (!detailItemTax.TaxKey.HasValue)
                    continue;

                var element = new XElement("DetailItemTax",
                    new XElement("DetailItemTaxKey", detailItemTax.TaxKey),
                    new XElement("DetailItemKey", detailItemTax.ItemKey));

                element.TryAddElement("TaxTypeCode", detailItemTax.TaxTypeCode);
                element.TryAddElement("TaxAmount", detailItemTax.TaxAmount);
                element.TryAddElement("RelationshipCode", detailItemTax.RelationshipCode);

                loopElement.Add(element);
                logger.TraceFormat("Added 810 \"DetailItemTax\" XML element for Header {0}", detailItemTax.ItemKey);
            }
        }

        public void WriteDetailTax(XContainer container, Type810DetailTax[] detailTaxes)
        {
            if (!detailTaxes.Any()) 
                return;

            var loopElement = new XElement("DetailTaxLoop");
            container.Add(loopElement);

            foreach (var detailTax in detailTaxes)
            {
                if (!detailTax.TaxKey.HasValue)
                    continue;

                var element = new XElement("DetailTax",
                    new XElement("DetailTaxKey", detailTax.TaxKey),
                    new XElement("DetailKey", detailTax.DetailKey));

                element.TryAddElement("AssignedId", detailTax.AssignedId);
                element.TryAddElement("MonetaryAmount", detailTax.MonetaryAmount);
                element.TryAddElement("Percent", detailTax.Percent);
                element.TryAddElement("RelationshipCode", detailTax.RelationshipCode);
                element.TryAddElement("DollarBasis", detailTax.DollarBasis);
                element.TryAddElement("JurisdictionCodeQualifier", detailTax.JurisdictionCodeQualifier);
                element.TryAddElement("JurisdictionCode", detailTax.JurisdictionCode);
                element.TryAddElement("TaxTypeCode", detailTax.TaxTypeCode);

                loopElement.Add(element);
                logger.TraceFormat("Added 810 \"DetailTax\" XML element for Detail {0}", detailTax.DetailKey);
            }
        }

        public void WriteName(XContainer container, Type810Name[] names)
        {
            if (!names.Any()) 
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
                element.TryAddElement("EntityDuns", name.EntityDuns);
                element.TryAddElement("EntityIdCode", name.EntityIdCode);
                element.TryAddElement("EntityName2", name.EntityName2);
                element.TryAddElement("EntityName3", name.EntityName3);
                element.TryAddElement("Address1", name.Address1);
                element.TryAddElement("Address2", name.Address2);
                element.TryAddElement("City", name.City);
                element.TryAddElement("State", name.State);
                element.TryAddElement("PostalCode", name.PostalCode);
                
                loopElement.Add(element);
                logger.TraceFormat("Added 810 \"Name\" XML element for Header {0}", name.HeaderKey);
            }
        }

        public void WriteSummary(XContainer container, Type810Summary[] summaries)
        {
            if (!summaries.Any()) 
                return;

            var loopElement = new XElement("SummaryLoop");
            container.Add(loopElement);

            foreach (var summary in summaries)
            {
                if (!summary.SummaryKey.HasValue)
                    continue;

                var element = new XElement("Summary",
                    new XElement("SummaryKey", summary.SummaryKey),
                    new XElement("HeaderKey", summary.HeaderKey));

                element.TryAddElement("TotalAmount", summary.TotalAmount.Replace(".", ""));
                element.TryAddElement("TotalLineItems", summary.TotalLineItems);
                element.TryAddElement("TotalSegments", summary.TotalSegments);
                element.TryAddElement("TransactionSetControlNbr", summary.TransactionSetControlNbr);

                loopElement.Add(element);
                logger.TraceFormat("Added 810 \"Summary\" XML element for Header {0}", summary.HeaderKey);
            }
        }

        public void WriteMessage(XContainer container, Type810Message[] messages)
        {
            if (!messages.Any()) 
                return;

            var loopElement = new XElement("MessageLoop");
            container.Add(loopElement);

            foreach (var message in messages)
            {
                if (!message.MessageKey.HasValue)
                    continue;

                var element = new XElement("Message",
                    new XElement("HeaderKey", message.HeaderKey),
                    new XElement("MessageKey", message.MessageKey));

                element.TryAddElement("ItemDescType", message.ItemDescType);
                element.TryAddElement("ProductCode", message.ProductCode);
                element.TryAddElement("Description", message.Description);
                element.TryAddElement("PositionCode", message.PositionCode);

                loopElement.Add(element);
                logger.TraceFormat("Added 810 \"Message\" XML element for Header {0}", message.HeaderKey);
            }
        }

        public void WriteMeter(XContainer container, Type810DetailItem[] items)
        {
            var loopElement = new XElement("MeterLoop");
            container.Add(loopElement);

            foreach (var item in items)
            {
                var meters = clientDataAccess.ListMeterConsumptionByInvoice(item.InvoiceNbr);
                if (meters.Any())
                {
                    var meter = meters[0];
                    var element = new XElement("Meter",
                        new XElement("InvoiceId", meter.InvoiceId),
                        new XElement("MeterType", meter.MeterType),
                        new XElement("RateCodeValue", meter.RateCodeValue),
                        new XElement("MeterNumber", meter.MeterNumber),
                        new XElement("UOM", meter.UOM),
                        new XElement("BeginningRead", meter.BegRead),
                        new XElement("EndingRead", meter.EndRead),
                        new XElement("ReadType", "AA"),
                        new XElement("MeterMeasureType", "NT"),
                        new XElement("Multiplier", meter.MeterFactor),
                        new XElement("SigCode", "51"),
                        new XElement("TotalConsumption", meter.TotalConsumption),
                        new XElement("ServicePeriodStartDate", meter.ServicePeriodStartDate),
                        new XElement("ServicePeriodEndDate", meter.ServicePeriodEndDate));

                    loopElement.Add(element);
                    logger.TraceFormat("Added 810 \"Meter\" XML element for Invoice {0}", item.InvoiceNbr);
                }
            }
        }
    }
}

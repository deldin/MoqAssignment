using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Parsers
{
    public class Import810Xml : IMarketFileParser
    {
        private readonly ILogger logger;

        public Import810Xml(ILogger logger)
        {
            this.logger = logger;
        }

        public IMarketFileParseResult Parse(string fileName)
        {
            logger.DebugFormat("Importing File \"{0}\"", fileName);

            var marketFile = new FileInfo(fileName);
            if (!marketFile.Exists)
            {
                logger.DebugFormat("File \"{0}\" does not exist or has been deleted.", fileName);
                return new Import814Model();
            }

            using (var stream = marketFile.OpenRead())
            {
                return Parse(stream);
            }
        }

        public IMarketFileParseResult Parse(Stream stream)
        {
            var context = new Import810Model();
            var document = XDocument.Load(stream);

            var documentElement = document.Root;
            if (documentElement == null)
                throw new InvalidOperationException();

            var namespaces = documentElement.Attributes()
                .Where(x => x.IsNamespaceDeclaration)
                .GroupBy(x => (x.Name.Namespace == XNamespace.None) ? string.Empty : x.Name.LocalName,
                    x => XNamespace.Get(x.Value))
                .ToDictionary(x => x.Key, x => x.First());

            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var countElement = documentElement.Element(empty + "TransactionCount");
            if (countElement != null)
                context.TransactionAuditCount = (int)countElement;

            context.InterchangeControlNbr = documentElement.GetChildText(empty + "InterchangeControlNbr");

            var headerElements = documentElement.Descendants(empty + "Header");
            foreach (var headerElement in headerElements)
            {
                var header = ParseHeader(headerElement, namespaces);
                context.AddHeader(header);
                context.TransactionActualCount++;
            }

            return context;
        }

        public Type810Header ParseHeader(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type810Header
            {
                Direction = true,
                TransactionSetId = element.GetChildText(empty + "TransactionSetId"),
                TransactionSetControlNbr = element.GetChildText(empty + "TransactionSetControlNbr"),
                TransactionDate = element.GetChildText(empty + "TransactionDate"),
                InvoiceNbr = element.GetChildText(empty + "InvoiceNbr"),
                ReleaseNbr = element.GetChildText(empty + "ReleaseNbr"),
                TransactionTypeCode = element.GetChildText(empty + "TransactionTypeCode"),
                TransactionSetPurposeCode = element.GetChildText(empty + "TransactionSetPurposeCode"),
                OriginalInvoiceNbr = element.GetChildText(empty + "OriginalInvoiceNbr"),
                EsiId = element.GetChildText(empty + "EsiId"),
                CrAccountNumber = element.GetChildText(empty + "CRAccountNumber"),
                PaymentDueDate = element.GetChildText(empty + "PaymentDueDate"),
                TdspDuns = element.GetChildText(empty + "TdspDuns"),
                TdspName = element.GetChildText(empty + "TdspName"),
                CrDuns = element.GetChildText(empty + "CrDuns"),
                CrName = element.GetChildText(empty + "CrName"),
                TotalAmount = element.GetChildText(empty + "TotalAmount"),
                CustNoForESCO = element.GetChildText(empty + "CustNoForESCO"),
                PreviousUtilityAccountNumber = element.GetChildText(empty + "PreviousUtilityAccountNumber"),
                BillPresenter = element.GetChildText(empty + "BillPresenter"),
                BillCalculator = element.GetChildText(empty + "BillCalculator"),
                GasPoolId = element.GetChildText(empty + "GasPoolId"),
                CustomerDUNS = element.GetChildText(empty + "CustomerDUNS"),
                ChangeCode = element.GetChildText(empty + "ChangeCode"),
                ChangeCodeDesc = element.GetChildText(empty + "ChangeCodeDesc"),
                BillingCycleNumber = element.GetChildText(empty + "BillingCycleNumber"),
                InvoicePeriodStart = element.GetChildText(empty + "InvoicePeriodStart"),
                InvoicePeriodEnd = element.GetChildText(empty + "InvoicePeriodEnd"),
                AlternateEsiId = element.GetChildText(empty + "AlternateEsiId"),
                ServiceDeliveryPoint = element.GetChildText(empty + "ServiceDeliveryPoint"),
                TransasctionTypeId = 24,
            };

            var balanceLoopElement = element.Element(empty + "BalanceLoop");
            if(balanceLoopElement != null)
            {
                var balanceElements = balanceLoopElement.Elements(empty + "Balance");
                foreach (var balanceElement in balanceElements)
                {
                    var balanceModel = ParseBalance(balanceElement, namespaces);
                    model.AddBalance(balanceModel);
                }
            }

            var paymentLoopElement = element.Element(empty + "PaymentLoop");
            if (paymentLoopElement != null)
            {
                var paymentElements = paymentLoopElement.Elements(empty + "Payment");
                foreach (var paymentElement in paymentElements)
                {
                    var paymentModel = ParsePayment(paymentElement, namespaces);
                    model.AddPayment(paymentModel);
                }
            }

            var detailLoopElement = element.Element(empty + "DetailLoop");
            if (detailLoopElement != null)
            {
                var detailElements = detailLoopElement.Elements(empty + "Detail");
                foreach (var detailElement in detailElements)
                {
                    var detailModel = ParseDetail(detailElement, namespaces);
                    model.AddDetail(detailModel);
                }
            }

            var nameLoopElement = element.Element(empty + "NameLoop");
            if (nameLoopElement != null)
            {
                var nameElements = nameLoopElement.Elements(empty + "Name");
                foreach (var nameElement in nameElements)
                {
                    var nameModel = ParseName(nameElement, namespaces);
                    model.AddName(nameModel);
                }
            }

            var summaryLoopElement = element.Element(empty + "SummaryLoop");
            if (summaryLoopElement != null)
            {
                var summaryElements = summaryLoopElement.Elements(empty + "Summary");
                foreach (var summaryElement in summaryElements)
                {
                    var summaryModel = ParseSummary(summaryElement, namespaces);
                    model.AddSummary(summaryModel);
                }
            }

            var messageLoopElement = element.Element(empty + "MessageLoop");
            if (messageLoopElement != null)
            {
                var messageElements = messageLoopElement.Elements(empty + "Message");
                foreach (var messageElement in messageElements)
                {
                    var messageModel = ParseMessage(messageElement, namespaces);
                    model.AddMessage(messageModel);
                }
            }
 
            return model;
        }

        public Type810Balance ParseBalance(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type810Balance
            {
                TotalOutstandingBalance = element.GetChildText(empty + "TotalOutstandingBalance"),
                BeginningBalance = element.GetChildText(empty + "BeginningBalance"),
                BudgetCumulativeDifference = element.GetChildText(empty + "BudgetCumulativeDifference"),
                BudgetMonthDifference = element.GetChildText(empty + "BudgetMonthDifference"),
                BudgetBilledToDate = element.GetChildText(empty + "BudgetBilledToDate"),
                ActualBilledToDate = element.GetChildText(empty + "ActualBilledToDate"),             
            };

            return model;
        }

        public Type810Payment ParsePayment(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type810Payment
            {
                AmountQualifierCode = element.GetChildText(empty + "AmountQualifierCode"),
                MonetaryAmount = element.GetChildText(empty + "MonetaryAmount"),
                TimeUnit = element.GetChildText(empty + "TimeUnit"),
                DateTimeQualifier = element.GetChildText(empty + "DateTimeQualifier"),
                Date = element.GetChildText(empty + "Date"),
            };

            return model;
        }

        public Type810Detail ParseDetail(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type810Detail
            {
                AssignedId = element.GetChildText(empty + "AssignedId"),
                ServiceTypeCode = element.GetChildText(empty + "ServiceTypeCode"),
                ServiceType = element.GetChildText(empty + "ServiceType"),
                ServiceClassCode = element.GetChildText(empty + "ServiceClassCode"),
                ServiceClass = element.GetChildText(empty + "ServiceClass"),
                RateClass = element.GetChildText(empty + "RateClass"),
                RateSubClass = element.GetChildText(empty + "RateSubClass"),
                ServicePeriodStartDate = element.GetChildText(empty + "ServicePeriodStartDate"),
                ServicePeriodEndDate = element.GetChildText(empty + "ServicePeriodEndDate"),
                MeterNumber = element.GetChildText(empty + "MeterNumber"),
                BillCycle = element.GetChildText(empty + "BillCycle"),
                GasPoolId = element.GetChildText(empty + "GasPoolId"),
                ServiceAgreement = element.GetChildText(empty + "ServiceAgreement"),
                RateCode = element.GetChildText(empty + "RateCode"),
                SupplierContractId = element.GetChildText(empty + "SupplierContractID"),
                UtilityContractId = element.GetChildText(empty + "UtilityContractID"),
                ServiceDeliveryPoint = element.GetChildText(empty + "ServiceDeliveryPoint"),
            };

            var itemLoopElement = element.Element(empty + "DetailItemLoop");
            if (itemLoopElement != null)
            {
                var itemElements = itemLoopElement.Elements(empty + "DetailItem");
                foreach (var itemElement in itemElements)
                {
                    var itemModel = ParseDetailItem(itemElement, namespaces);
                    model.AddItem(itemModel);
                }
            }

            var taxLoopElement = element.Element(empty + "DetailTaxLoop");
            if (taxLoopElement != null)
            {
                var taxElements = taxLoopElement.Elements(empty + "DetailTax");
                foreach (var taxElement in taxElements)
                {
                    var taxModel = ParseDetailTax(taxElement, namespaces);
                    model.AddTax(taxModel);
                }
            }

            return model;
        }

        public Type810DetailTax ParseDetailTax(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type810DetailTax
            {
                AssignedId = element.GetChildText(empty + "AssignedId"),
                MonetaryAmount = element.GetChildText(empty + "MonetaryAmount"),
                Percent = element.GetChildText(empty + "Percent"),
                RelationshipCode = element.GetChildText(empty + "RelationshipCode"),
                DollarBasis = element.GetChildText(empty + "DollarBasis"),
                TaxTypeCode = element.GetChildText(empty + "TaxTypeCode"),
                JurisdictionCode = element.GetChildText(empty + "JurisdictionCode"),
                JurisdictionCodeQualifier = element.GetChildText(empty + "JurisdictionCodeQualifier"),
                ExemptCode = element.GetChildText(empty + "ExemptCode"),
                Description = element.GetChildText(empty + "Description"),
            };

            return model;
        }

        public Type810DetailItem ParseDetailItem(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type810DetailItem
            {
                AssignedId = element.GetChildText(empty + "AssignedId"),
                RelationshipCode = element.GetChildText(empty + "RelationshipCode"),
                ServiceOrderCompleteDate = element.GetChildText(empty + "ServiceOrderCompleteDate"),
                UnmeteredServiceDateRange = element.GetChildText(empty + "UnmeteredServiceDateRange"),
                InvoiceNbr = element.GetChildText(empty + "InvoiceNbr"),
                ServiceOrderNbr = element.GetChildText(empty + "ServiceOrderNbr"),
                Consumption = element.GetChildText(empty + "Consumption"),
                EffectiveDate = element.GetChildText(empty + "EffectiveDate"),
                SequenceId = element.GetChildText(empty + "SequenceId"),
            };

            var itemChargeLoopElement = element.Element(empty + "DetailItemChargeLoop");
            if (itemChargeLoopElement != null)
            {
                var itemChargeElements = itemChargeLoopElement.Elements(empty + "DetailItemCharge");
                foreach (var itemChargeElement in itemChargeElements)
                {
                    var itemChargeModel = ParseDetailItemCharge(itemChargeElement, namespaces);
                    model.AddCharge(itemChargeModel);
                }
            }

            var detailItemTaxLoopElement = element.Element(empty + "DetailItemTaxLoop");
            if (detailItemTaxLoopElement != null)
            {
                var detailItemTaxElements = detailItemTaxLoopElement.Elements(empty + "DetailItemTax");
                foreach (var detailItemTaxElement in detailItemTaxElements)
                {
                    var detailItemTaxModel = ParseDetailItemTax(detailItemTaxElement, namespaces);
                    model.AddTax(detailItemTaxModel);
                }
            }

            return model;
        }

        public Type810DetailItemCharge ParseDetailItemCharge(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type810DetailItemCharge
            {
                ChargeIndicator = element.GetChildText(empty + "ChargeIndicator"),
                AgencyCode = element.GetChildText(empty + "AgencyCode"),
                ChargeCode = element.GetChildText(empty + "ChargeCode"),
                Amount = element.GetChildText(empty + "Amount"),
                Rate = element.GetChildText(empty + "Rate"),
                UOM = element.GetChildText(empty + "UOM"),
                Quantity = element.GetChildText(empty + "Quantity"),
                Description = element.GetChildText(empty + "Description"),
                PrintSeqId = element.GetChildText(empty + "PrintSeqId"),
                EnergyCharges = element.GetChildText(empty + "EnergyCharges"),
            };

            return model;
        }

        public Type810DetailItemTax ParseDetailItemTax(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type810DetailItemTax
            {
                TaxTypeCode = element.GetChildText(empty + "TaxTypeCode"),
                TaxAmount = element.GetChildText(empty + "TaxAmount"),
                RelationshipCode = element.GetChildText(empty + "RelationshipCode"),
            };

            return model;
        }

        public Type810Name ParseName(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type810Name
            {
                EntityIdType = element.GetChildText(empty + "EntityIdType"),
                EntityName = element.GetChildText(empty + "EntityName"),
                EntityDuns = element.GetChildText(empty + "EntityDuns"),
                EntityIdCode = element.GetChildText(empty + "EntityIdCode"),
                Address1 = element.GetChildText(empty + "Address1"),
                Address2 = element.GetChildText(empty + "Address2"),
                City = element.GetChildText(empty + "City"),
                State = element.GetChildText(empty + "State"),
                PostalCode = element.GetChildText(empty + "PostalCode"),
                EntityName2 = element.GetChildText(empty + "EntityName2"),
                EntityName3 = element.GetChildText(empty + "EntityName3"),
            };

            return model;
        }

        public Type810Summary ParseSummary(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type810Summary
            {
                TotalAmount = element.GetChildText(empty + "TotalAmount"),
                TotalLineItems = element.GetChildText(empty + "TotalLineItems"),
                TotalSegments = element.GetChildText(empty + "TotalSegments"),
                TransactionSetControlNbr = element.GetChildText(empty + "EntityIdType"),
            };

            return model;
        }

        public Type810Message ParseMessage(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type810Message
            {
                ItemDescType = element.GetChildText(empty + "ItemDescType"),
                ProductCode = element.GetChildText(empty + "ProductCode"),
                Description = element.GetChildText(empty + "Description"),
                PositionCode = element.GetChildText(empty + "PositionCode"),
            };

            return model;
        }
    }
}

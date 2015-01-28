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
    public class Import824Xml : IMarketFileParser
    {
        private readonly ILogger logger;

        public Import824Xml(ILogger logger)
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
                return Import824Model.Empty;
            }

            using (var stream = marketFile.OpenRead())
            {
                return Parse(stream);
            }
        }

        public IMarketFileParseResult Parse(Stream stream)
        {
            var context = new Import824Model();
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

        public Type824Header ParseHeader(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type824Header
            {
                TransactionSetId = element.GetChildText(empty + "TransactionSetId"),
                TransactionSetControlNbr = element.GetChildText(empty + "TransactionSetControlNbr"),
                TransactionSetPurposeCode = element.GetChildText(empty + "TransactionSetPurposeCode"),
                TransactionDate = element.GetChildText(empty + "TransactionDate"),
                TransactionNbr = element.GetChildText(empty + "TransactionNbr"),
                ReportTypeCode = element.GetChildText(empty + "ReportTypeCode"),
                ActionCode = element.GetChildText(empty + "ActionCode"),
                TdspDuns = element.GetChildText(empty + "TdspDuns"),
                TdspName = element.GetChildText(empty + "TdspName"),
                CrDuns = element.GetChildText(empty + "CrDuns"),
                CrName = element.GetChildText(empty + "CrName"),
                AppAckCode = element.GetChildText(empty + "AppAckCode"),
                ReferenceNbr = element.GetChildText(empty + "ReferenceNbr"),
                TransactionSetNbr = element.GetChildText(empty + "TransactionSetNbr"),
                EsiId = element.GetChildText(empty + "EsiId"),
                CrQualifier = element.GetChildText(empty + "CrQualifier"),
                TdspQualifier = element.GetChildText(empty + "TdspQualifier"),
                EspUtilityAccountNumber = element.GetChildText(empty + "ESPUtilityAccountNumber"),
                EspCustomerAccountNumber = element.GetChildText(empty + "ESPCustomerAccountNumber"),
                PreviousUtilityAccountNumber = element.GetChildText(empty + "PreviousUtilityAccountNumber"),
                CustomerName = element.GetChildText(empty + "CustomerName"),
            };

            var reasonLoopElement = element.Element(empty + "ReasonLoop");
            if (reasonLoopElement != null)
            {
                var reasonElements = reasonLoopElement.Elements(empty + "Reason");
                foreach (var reasonElement in reasonElements)
                {
                    var reasonModel = ParseReason(reasonElement, namespaces);
                    model.AddReason(reasonModel);
                }
            }

            var referenceLoopElement = element.Element(empty + "ReferenceLoop");
            if (referenceLoopElement != null)
            {
                var referenceElements = referenceLoopElement.Elements(empty + "Reference");
                foreach (var referenceElement in referenceElements)
                {
                    var referenceModel = ParseReference(referenceElement, namespaces);
                    model.AddReference(referenceModel);
                }
            }

            return model;
        }

        public Type824Reason ParseReason(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type824Reason
            {
                ReasonCode = element.GetChildText(empty + "ReasonCode"),
                ReasonText = element.GetChildText(empty + "ReasonText"),
            };

            return model;
        }

        public Type824Reference ParseReference(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type824Reference
            {
                AppAckCode = element.GetChildText(empty + "AppAckCode"),
                ReferenceQualifier = element.GetChildText(empty + "ReferenceQualifier"),
                ReferenceNbr = element.GetChildText(empty + "ReferenceNbr"),
                TransactionSetId = element.GetChildText(empty + "TransactionSetId"),
                CrossReferenceNbr = element.GetChildText(empty + "CrossReferenceNbr"),
                PurchaseOrderNbr = element.GetChildText(empty + "PurchaseOrderNbr"),
                PaymentsAppliedThroughDate = element.GetChildText(empty + "PaymentsAppliedThroughDate"),
                TotalPaymentsApplied = element.GetChildText(empty + "TotalPaymentsApplied"),
                PaymentDueDate = element.GetChildText(empty + "PaymentDueDate"),
                TotalAmountDue = element.GetChildText(empty + "TotalAmountDue"),
            };

            var errorLoopElement = element.Element(empty + "TechErrorLoop");
            if (errorLoopElement != null)
            {
                var errorElements = errorLoopElement.Elements(empty + "TechError");
                foreach (var errorElement in errorElements)
                {
                    var errorModel = ParseTechError(errorElement, namespaces);
                    model.AddTechError(errorModel);
                }
            }

            return model;
        }

        public Type824TechError ParseTechError(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type824TechError
            {
                TechErrorCode = element.GetChildText(empty + "TechErrorCode"),
                BadElementCopy = element.GetChildText(empty + "BadElementCopy"),
                TechErrorNote = element.GetChildText(empty + "TechErrorNote"),
            };

            return model;
        }
    }
}

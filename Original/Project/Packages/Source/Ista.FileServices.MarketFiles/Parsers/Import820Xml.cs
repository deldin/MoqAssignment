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
    public class Import820Xml : IMarketFileParser
    {
        private readonly ILogger logger;

        public Import820Xml(ILogger logger)
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
                return new Import820Model();
            }
            
            using (var stream = marketFile.OpenRead())
            {
                return Parse(stream);
            }
        }

        public IMarketFileParseResult Parse(Stream stream)
        {
            var context = new Import820Model();
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

        public Type820Header ParseHeader(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type820Header
            {
                CrDuns = element.GetChildText(empty + "CrDuns"),
                CreateDate = element.GetChildText(empty + "CreateDate"),
                CreditDebitFlag = element.GetChildText(empty + "CreditDebitFlag"),
                CrName = element.GetChildText(empty + "CrName"),
                ESPUtilityAccountNumber = element.GetChildText(empty + "ESPUtilityAccountNumber"),
                PaymentMethodCode = element.GetChildText(empty + "PaymentMethodCode"),
                TdspDuns = element.GetChildText(empty + "TdspDuns"),
                TdspDunsStructureCode = element.GetChildText(empty + "TdspDunsStructureCode"),
                TdspName = element.GetChildText(empty + "TdspName"),
                TotalAmount = element.GetChildText(empty + "TotalAmount"),
                TraceReferenceNbr = element.GetChildText(empty + "TraceReferenceNbr"),
                TransactionDate = element.GetChildText(empty + "TransactionDate"),
                TransactionNbr = element.GetChildText(empty + "TransactionNbr"),
                TransactionSetControlNbr = element.GetChildText(empty + "TransactionSetControlNbr"),
                TransactionSetId = element.GetChildText(empty + "TransactionSetId"),
                TransactionTypeCode = element.GetChildText(empty + "TransactionTypeCode"),
            };

            var detailLoopElement = element.Element(empty + "DetailLoop");
            if (detailLoopElement != null)
            {
                var nameElements = detailLoopElement.Elements(empty + "Detail");
                foreach(var nameElement in nameElements)
                {
                    var detailModel = ParseDetail(nameElement, namespaces);
                    model.AddDetail(detailModel);
                }
            }

            return model;
        }

        public Type820Detail ParseDetail(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type820Detail
            {
                AdjustmentAmount = element.GetChildText(empty + "AdjustmentAmount"),
                AdjustmentReasonCode = element.GetChildText(empty + "AdjustmentReasonCode"),
                AssignedId = element.GetChildText(empty + "AssignedId"),
                CommodityCode = element.GetChildText(empty + "CommodityCode"),
                CrossReferenceNbr = element.GetChildText(empty + "CrossReferenceNbr"),
                CustomerName = element.GetChildText(empty + "CustomerName"),
                DatePosted = element.GetChildText(empty + "DatePosted"),
                DiscountAmount = element.GetChildText(empty + "DiscountAmount"),
                EsiId = element.GetChildText(empty + "EsiId"),
                ESPAccountNumber = element.GetChildText(empty + "ESPAccountNumber"),
                InvoiceAmount = element.GetChildText(empty + "InvoiceAmount"),
                PaymentActionCode = element.GetChildText(empty + "PaymentActionCode"),
                PaymentAmount = element.GetChildText(empty + "PaymentAmount"),
                PrevUtilityAccountNumber = element.GetChildText(empty + "PrevUtilityAccountNumber"),
                ReferenceId = element.GetChildText(empty + "ReferenceId"),
                ReferenceNbr = element.GetChildText(empty + "ReferenceNbr"),
                UnmeteredServiceDesignator = element.GetChildText(empty + "UnmeteredServiceDesignator"),
            };
            
            return model;
        }
    }
}

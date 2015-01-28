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
    public class Import248Xml : IMarketFileParser
    {
        private readonly ILogger logger;

        public Import248Xml(ILogger logger)
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
                return Import248Model.Empty;
            }

            using (var stream = marketFile.OpenRead())
            {
                return Parse(stream);
            }
        }

        public IMarketFileParseResult Parse(Stream stream)
        {
            var context = new Import248Model();
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

        public Type248Header ParseHeader(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type248Header
            {
                ControlNbr = element.GetChildText(empty + "ControlNbr"),
                CrDuns = element.GetChildText(empty + "CrDuns"),
                CrName = element.GetChildText(empty + "CrName"),
                LDCDuns = element.GetChildText(empty + "LDCDuns"),
                LDCName = element.GetChildText(empty + "LDCName"),
                SegmentCount = element.GetChildText(empty + "SegmentCount"),
                StructureCode = element.GetChildText(empty + "StructureCode"),
                TransactionDate = element.GetChildText(empty + "TransactionDate"),
                TransactionReferenceNbr = element.GetChildText(empty + "TransactionReferenceNbr"),
                TransactionSetPurposeCode = element.GetChildText(empty + "TransactionSetPurposeCode"),
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

        public Type248Detail ParseDetail(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type248Detail
            {
                BalanceAmount = element.GetChildText(empty + "BalanceAmount"),
                CustomerName = element.GetChildText(empty + "CustomerName"),
                CustomerTelephone1 = element.GetChildText(empty + "CustomerTelephone1"),
                CustomerTelephone2 = element.GetChildText(empty + "CustomerTelephone2"),
                ESIID = element.GetChildText(empty + "ESIID"),
                ESPAccountNbr = element.GetChildText(empty + "ESPAccountNbr"),
                HierarchicalID = element.GetChildText(empty + "HierarchicalID"),
                HierarchicalLevelCode = element.GetChildText(empty + "HierarchicalLevelCode"),
                InvoiceAmount = element.GetChildText(empty + "InvoiceAmount"),
                InvoiceDate = element.GetChildText(empty + "InvoiceDate"),
                InvoiceNbr = element.GetChildText(empty + "InvoiceNbr"),
                MarketerCustomerAccountNumber = element.GetChildText(empty + "MarketerCustomerAccountNumber"),
                OldLdcAccountNbr = element.GetChildText(empty + "OldLdcAccountNbr"),
                ReinstatementDate = element.GetChildText(empty + "ReinstatementDate"),
                ServiceTypeCode = element.GetChildText(empty + "ServiceTypeCode"),
                WriteOffAccountNbr = element.GetChildText(empty + "WriteOffAccountNbr"),
                WriteOffDate = element.GetChildText(empty + "WriteOffDate"),
            };
            
            return model;
        }
    }
}

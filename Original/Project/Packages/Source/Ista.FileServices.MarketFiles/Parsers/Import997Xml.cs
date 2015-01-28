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
    public class Import997Xml : IMarketFileParser
    {
        private readonly ILogger logger;

        public Import997Xml(ILogger logger)
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
                return Import997Model.Empty;
            }

            using (var stream = marketFile.OpenRead())
            {
                return Parse(stream);
            }
        }

        public IMarketFileParseResult Parse(Stream stream)
        {
            var context = new Import997Model();
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

        public Type997Header ParseHeader(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type997Header
            {
                AcknowledgeCode = element.GetChildText(empty + "AcknowledgeCode"),
                FunctionalGroup = element.GetChildText(empty + "FunctionalGroup"),
                SegmentCount = element.GetChildText(empty + "SegmentCount"),
                SyntaxErrorCode1 = element.GetChildText(empty + "SyntaxErrorCode1"),
                SyntaxErrorCode2 = element.GetChildText(empty + "SyntaxErrorCode2"),
                SyntaxErrorCode3 = element.GetChildText(empty + "SyntaxErrorCode3"),
                SyntaxErrorCode4 = element.GetChildText(empty + "SyntaxErrorCode4"),
                SyntaxErrorCode5 = element.GetChildText(empty + "SyntaxErrorCode5"),
                TransactionNbr = element.GetChildText(empty + "TransactionNbr"),
                TransactionSetsAccepted = element.GetChildText(empty + "TransactionSetsAccepted"),
                TransactionSetsIncluded = element.GetChildText(empty + "TransactionSetsIncluded"),
                TransactionSetsReceived = element.GetChildText(empty + "TransactionSetsReceived"),
            };
            
            var responseLoopElement = element.Element(empty + "ResponseLoop");
            if (responseLoopElement != null)
            {
                var nameElements = responseLoopElement.Elements(empty + "Response");
                foreach(var nameElement in nameElements)
                {
                    var responseModel = ParseResponse(nameElement, namespaces);
                    model.AddResponse(responseModel);
                }
            }

            return model;
        }

        public Type997Response ParseResponse(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type997Response
            {
                AcknowledgementCode = element.GetChildText(empty + "AcknowledgementCode"),
                ControlNbr = element.GetChildText(empty + "ControlNbr"),
                IdentifierCode = element.GetChildText(empty + "IdentifierCode"),
                SyntaxErrorCode1 = element.GetChildText(empty + "SyntaxErrorCode1"),
                SyntaxErrorCode2 = element.GetChildText(empty + "SyntaxErrorCode2"),
                SyntaxErrorCode3 = element.GetChildText(empty + "SyntaxErrorCode3"),
                SyntaxErrorCode4 = element.GetChildText(empty + "SyntaxErrorCode4"),
                SyntaxErrorCode5 = element.GetChildText(empty + "SyntaxErrorCode5"),
            };

            var responseNoteLoopElement = element.Element(empty + "ResponseNoteLoop");
            if (responseNoteLoopElement != null)
            {
                var nameElements = responseNoteLoopElement.Elements(empty + "ResponseNote");
                foreach (var nameElement in nameElements)
                {
                    var responseNoteModel = ParseResponseNote(nameElement, namespaces);
                    model.AddResponseNote(responseNoteModel);
                }
            }
            
            return model;
        }

        public Type997ResponseNote ParseResponseNote(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type997ResponseNote
            {
                ElementCopy = element.GetChildText(empty + "ElementCopy"),
                ElementPosition = element.GetChildText(empty + "ElementPosition"),
                ElementReferenceNbr = element.GetChildText(empty + "ElementReferenceNbr"),
                ElementSyntaxErrorCode = element.GetChildText(empty + "ElementSyntaxErrorCode"),
                LoopIdentifierCode = element.GetChildText(empty + "LoopIdentifierCode"),
                SegmentIdCode = element.GetChildText(empty + "SegmentIdCode"),
                SegmentPosition = element.GetChildText(empty + "SegmentPosition"),
                SegmentSyntaxErrorCode = element.GetChildText(empty + "SegmentSyntaxErrorCode"),
            };

            return model;
        }
    }
}

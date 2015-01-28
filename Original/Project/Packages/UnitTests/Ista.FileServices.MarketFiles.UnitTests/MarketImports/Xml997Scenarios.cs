using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Should;

namespace Ista.FileServices.MarketFiles.UnitTests.MarketImports
{
    [TestClass]
    public class Xml997Scenarios
    {
        private Dictionary<string, XNamespace> namespaces;
        private ILogger logger;
        
        private Import997Xml concern;
        
        [TestInitialize]
        public void SetUp()
        {
            namespaces = new Dictionary<string, XNamespace>();
            logger = MockRepository.GenerateStub<ILogger>();

            concern = new Import997Xml(logger);
        }

        [TestMethod]
        public void ParseHeader_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <AcknowledgeCode>AcknowledgeCode</AcknowledgeCode>
                    <FunctionalGroup>FunctionalGroup</FunctionalGroup>
                    <SegmentCount>SegmentCount</SegmentCount>
                    <SyntaxErrorCode1>SyntaxErrorCode1</SyntaxErrorCode1>
                    <SyntaxErrorCode2>SyntaxErrorCode2</SyntaxErrorCode2>
                    <SyntaxErrorCode3>SyntaxErrorCode3</SyntaxErrorCode3>
                    <SyntaxErrorCode4>SyntaxErrorCode4</SyntaxErrorCode4>
                    <SyntaxErrorCode5>SyntaxErrorCode5</SyntaxErrorCode5>
                    <TransactionNbr>TransactionNbr</TransactionNbr>
                    <TransactionSetsAccepted>TransactionSetsAccepted</TransactionSetsAccepted>
                    <TransactionSetsIncluded>TransactionSetsIncluded</TransactionSetsIncluded>
                    <TransactionSetsReceived>TransactionSetsReceived</TransactionSetsReceived>
                </root>";

            // act
            var element = XElement.Parse(xml);
            var actual = concern.ParseHeader(element, namespaces);

            // assert
            actual.AcknowledgeCode.ShouldEqual("AcknowledgeCode");
            actual.FunctionalGroup.ShouldEqual("FunctionalGroup");
            actual.SegmentCount.ShouldEqual("SegmentCount");
            actual.SyntaxErrorCode1.ShouldEqual("SyntaxErrorCode1");
            actual.SyntaxErrorCode2.ShouldEqual("SyntaxErrorCode2");
            actual.SyntaxErrorCode3.ShouldEqual("SyntaxErrorCode3");
            actual.SyntaxErrorCode4.ShouldEqual("SyntaxErrorCode4");
            actual.SyntaxErrorCode5.ShouldEqual("SyntaxErrorCode5");
            actual.TransactionNbr.ShouldEqual("TransactionNbr");
            actual.TransactionSetsAccepted.ShouldEqual("TransactionSetsAccepted");
            actual.TransactionSetsIncluded.ShouldEqual("TransactionSetsIncluded");
            actual.TransactionSetsReceived.ShouldEqual("TransactionSetsReceived");
        }

        [TestMethod]
        public void ParseHeader_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            const string xml = @"<root></root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseHeader(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        public void ParseHeader_ShouldReturnChildren_WhenElementsSupplied()
        {
            // arrange
            const string xml = @"
                <root>
                    <ResponseLoop><Response><something /></Response></ResponseLoop>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseHeader(element, namespaces);

            // assert
            actual.ShouldNotBeNull();

            actual.Responses.Count().ShouldEqual(1);
        }

        [TestMethod]
        public void ParseResponse_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <AcknowledgementCode>AcknowledgementCode</AcknowledgementCode>
                    <ControlNbr>ControlNbr</ControlNbr>
                    <IdentifierCode>IdentifierCode</IdentifierCode>
                    <SyntaxErrorCode1>SyntaxErrorCode1</SyntaxErrorCode1>
                    <SyntaxErrorCode2>SyntaxErrorCode2</SyntaxErrorCode2>
                    <SyntaxErrorCode3>SyntaxErrorCode3</SyntaxErrorCode3>
                    <SyntaxErrorCode4>SyntaxErrorCode4</SyntaxErrorCode4>
                    <SyntaxErrorCode5>SyntaxErrorCode5</SyntaxErrorCode5>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseResponse(element, namespaces);

            // assert
            actual.AcknowledgementCode.ShouldEqual("AcknowledgementCode");
            actual.ControlNbr.ShouldEqual("ControlNbr");
            actual.IdentifierCode.ShouldEqual("IdentifierCode");
            actual.SyntaxErrorCode1.ShouldEqual("SyntaxErrorCode1");
            actual.SyntaxErrorCode2.ShouldEqual("SyntaxErrorCode2");
            actual.SyntaxErrorCode3.ShouldEqual("SyntaxErrorCode3");
            actual.SyntaxErrorCode4.ShouldEqual("SyntaxErrorCode4");
            actual.SyntaxErrorCode5.ShouldEqual("SyntaxErrorCode5");
        }

        [TestMethod]
        public void ParseResponse_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            const string xml = @"<root></root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseResponse(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        public void ParseResponse_ShouldReturnChildren_WhenElementsSupplied()
        {
            // arrange
            const string xml = @"
                <root>
                    <ResponseNoteLoop><ResponseNote><something /></ResponseNote></ResponseNoteLoop>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseResponse(element, namespaces);

            // assert
            actual.ShouldNotBeNull();

            actual.Notes.Count().ShouldEqual(1);
        }

        [TestMethod]
        public void ParseResponseNote_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <ElementCopy>ElementCopy</ElementCopy>
                    <ElementPosition>ElementPosition</ElementPosition>
                    <ElementReferenceNbr>ElementReferenceNbr</ElementReferenceNbr>
                    <ElementSyntaxErrorCode>ElementSyntaxErrorCode</ElementSyntaxErrorCode>
                    <LoopIdentifierCode>LoopIdentifierCode</LoopIdentifierCode>
                    <SegmentIdCode>SegmentIdCode</SegmentIdCode>
                    <SegmentPosition>SegmentPosition</SegmentPosition>
                    <SegmentSyntaxErrorCode>SegmentSyntaxErrorCode</SegmentSyntaxErrorCode>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseResponseNote(element, namespaces);

            // assert
            actual.ElementCopy.ShouldEqual("ElementCopy");
            actual.ElementPosition.ShouldEqual("ElementPosition");
            actual.ElementReferenceNbr.ShouldEqual("ElementReferenceNbr");
            actual.ElementSyntaxErrorCode.ShouldEqual("ElementSyntaxErrorCode");
            actual.LoopIdentifierCode.ShouldEqual("LoopIdentifierCode");
            actual.SegmentIdCode.ShouldEqual("SegmentIdCode");
            actual.SegmentPosition.ShouldEqual("SegmentPosition");
            actual.SegmentSyntaxErrorCode.ShouldEqual("SegmentSyntaxErrorCode");
        }

        [TestMethod]
        public void ParseResponseNote_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            const string xml = @"<root></root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseResponseNote(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }
    }
}

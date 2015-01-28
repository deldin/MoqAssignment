using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Should;

namespace Ista.FileServices.MarketFiles.UnitTests.MarketImports
{
    [TestClass]
    public class Xml824Scenarios
    {
        private Dictionary<string, XNamespace> namespaces;
        private ILogger logger;
        
        private Import824Xml concern;
        
        [TestInitialize]
        public void SetUp()
        {
            namespaces = new Dictionary<string, XNamespace>();
            logger = MockRepository.GenerateStub<ILogger>();

            concern = new Import824Xml(logger);
        }

        [TestMethod]
        public void Sample_File()
        {
            // arrange
            const string xml = @"<ns0:Market824 xmlns:ns0=""http://CIS.Integration.Schema.Market.Common.Market824"">
	<TransactionCount>1</TransactionCount>
	<LdcDuns>007914468</LdcDuns>
	<CspDuns>626058655</CspDuns>
	<FunctionalGroup>AG</FunctionalGroup>
	<Transaction>
		<Header>
			<TransactionSetId>824</TransactionSetId>
			<TransactionSetPurposeCode>11</TransactionSetPurposeCode>
			<TransactionNbr>82420130119034642516376</TransactionNbr>
			<TransactionDate>20130119</TransactionDate>
			<ActionCode>EV</ActionCode>
			<TdspDuns>007914468</TdspDuns>
			<TdspName>PECO ENERGY</TdspName>
			<CrDuns>626058655</CrDuns>
			<CrName>IGS ENERGY INC</CrName>
			<AppAckCode>TR</AppAckCode>
			<ReferenceNbr>2013-01-19-02.56.48.533283</ReferenceNbr>
			<TransactionSetNbr>810</TransactionSetNbr>
			<EsiId>4477015075</EsiId>
			<CustomerName>GLORIBEL ORTIZ-GARCIA</CustomerName>
			<ESPCustomerAccountNumber/>
			<TradingPartnerID>IGSPA007914468ADV</TradingPartnerID>
			<ReasonLoop>
				<Reason>
					<ReasonCode>DIS</ReasonCode>
					<ReasonText>820 PENDING UNTIL DISPUTE RESOLUTION</ReasonText>
				</Reason>
			</ReasonLoop>
			<ReferenceLoop>
				<Reference>
					<AppAckCode>TR</AppAckCode>
					<ReferenceQualifier>TN</ReferenceQualifier>
					<ReferenceNbr>2013-01-19-02.56.48.533283</ReferenceNbr>
					<TransactionSetId>810</TransactionSetId>
					<CrossReferenceNbr>2012-12-26-19.40.52.358398</CrossReferenceNbr>
				</Reference>
			</ReferenceLoop>
		</Header>
	</Transaction>
</ns0:Market824>";

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var result = concern.Parse(stream);

            // assert
            result.TransactionActualCount.ShouldEqual(1);
            result.TransactionAuditCount.ShouldEqual(1);

            var header = result.Headers[0] as Type824Header;
            Assert.IsNotNull(header);
            header.TransactionSetId.ShouldEqual("824");
            header.TransactionSetPurposeCode.ShouldEqual("11");
            header.TransactionNbr.ShouldEqual("82420130119034642516376");
            header.TransactionDate.ShouldEqual("20130119");
            header.ActionCode.ShouldEqual("EV");
            header.TdspDuns.ShouldEqual("007914468");
            header.TdspName.ShouldEqual("PECO ENERGY");
            header.CrDuns.ShouldEqual("626058655");
            header.CrName.ShouldEqual("IGS ENERGY INC");
            header.AppAckCode.ShouldEqual("TR");
            header.ReferenceNbr.ShouldEqual("2013-01-19-02.56.48.533283");
            header.TransactionSetNbr.ShouldEqual("810");
            header.EsiId.ShouldEqual("4477015075");
            header.CustomerName.ShouldEqual("GLORIBEL ORTIZ-GARCIA");
            header.EspCustomerAccountNumber.ShouldBeEmpty();

            header.Reasons.Count().ShouldEqual(1);
            var reason = header.Reasons[0];

            reason.ReasonCode.ShouldEqual("DIS");
            reason.ReasonText.ShouldEqual("820 PENDING UNTIL DISPUTE RESOLUTION");

            header.References.Count().ShouldEqual(1);
            var reference = header.References[0];

            reference.AppAckCode.ShouldEqual("TR");
            reference.ReferenceQualifier.ShouldEqual("TN");
            reference.ReferenceNbr.ShouldEqual("2013-01-19-02.56.48.533283");
            reference.TransactionSetId.ShouldEqual("810");
            reference.CrossReferenceNbr.ShouldEqual("2012-12-26-19.40.52.358398");
        }

        [TestMethod]
        public void ParseHeader_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <ActionCode>ActionCode</ActionCode>
                    <AppAckCode>AppAckCode</AppAckCode>
                    <CrDuns>CrDuns</CrDuns>
                    <CrName>CrName</CrName>
                    <CrQualifier>CrQualifier</CrQualifier>
                    <CustomerName>CustomerName</CustomerName>
                    <EsiId>EsiId</EsiId>
                    <ESPCustomerAccountNumber>ESPCustomerAccountNumber</ESPCustomerAccountNumber>
                    <ESPUtilityAccountNumber>ESPUtilityAccountNumber</ESPUtilityAccountNumber>
                    <PreviousUtilityAccountNumber>PreviousUtilityAccountNumber</PreviousUtilityAccountNumber>
                    <ReferenceNbr>ReferenceNbr</ReferenceNbr>
                    <ReportTypeCode>ReportTypeCode</ReportTypeCode>
                    <TdspDuns>TdspDuns</TdspDuns>
                    <TdspName>TdspName</TdspName>
                    <TdspQualifier>TdspQualifier</TdspQualifier>
                    <TransactionDate>TransactionDate</TransactionDate>
                    <TransactionNbr>TransactionNbr</TransactionNbr>
                    <TransactionSetControlNbr>TransactionSetControlNbr</TransactionSetControlNbr>
                    <TransactionSetId>TransactionSetId</TransactionSetId>
                    <TransactionSetNbr>TransactionSetNbr</TransactionSetNbr>
                    <TransactionSetPurposeCode>TransactionSetPurposeCode</TransactionSetPurposeCode>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseHeader(element, namespaces);

            // assert
            actual.ActionCode.ShouldEqual("ActionCode");
            actual.AppAckCode.ShouldEqual("AppAckCode");
            actual.CrDuns.ShouldEqual("CrDuns");
            actual.CrName.ShouldEqual("CrName");
            actual.CrQualifier.ShouldEqual("CrQualifier");
            actual.CustomerName.ShouldEqual("CustomerName");
            actual.EsiId.ShouldEqual("EsiId");
            actual.EspCustomerAccountNumber.ShouldEqual("ESPCustomerAccountNumber");
            actual.EspUtilityAccountNumber.ShouldEqual("ESPUtilityAccountNumber");
            actual.PreviousUtilityAccountNumber.ShouldEqual("PreviousUtilityAccountNumber");
            actual.ReferenceNbr.ShouldEqual("ReferenceNbr");
            actual.ReportTypeCode.ShouldEqual("ReportTypeCode");
            actual.TdspDuns.ShouldEqual("TdspDuns");
            actual.TdspName.ShouldEqual("TdspName");
            actual.TdspQualifier.ShouldEqual("TdspQualifier");
            actual.TransactionDate.ShouldEqual("TransactionDate");
            actual.TransactionNbr.ShouldEqual("TransactionNbr");
            actual.TransactionSetControlNbr.ShouldEqual("TransactionSetControlNbr");
            actual.TransactionSetId.ShouldEqual("TransactionSetId");
            actual.TransactionSetNbr.ShouldEqual("TransactionSetNbr");
            actual.TransactionSetPurposeCode.ShouldEqual("TransactionSetPurposeCode");
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
            var xml = @"
                <root>
                    <ReasonLoop><Reason><something /></Reason></ReasonLoop>
                    <ReferenceLoop><Reference><something /></Reference></ReferenceLoop>
                </root>";

            // act
            var element = XElement.Parse(xml);
            var actual = concern.ParseHeader(element, namespaces);

            // assert
            actual.ShouldNotBeNull();

            actual.Reasons.Count().ShouldEqual(1);
            actual.References.Count().ShouldEqual(1);
        }

        [TestMethod]
        public void ParseReason_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            var xml = @"
                <root>
                    <ReasonCode>ReasonCode</ReasonCode>
                    <ReasonText>ReasonText</ReasonText>
                </root>";

            // act
            var element = XElement.Parse(xml);
            var actual = concern.ParseReason(element, namespaces);

            // assert
            actual.ReasonCode.ShouldEqual("ReasonCode");
            actual.ReasonText.ShouldEqual("ReasonText");
        }

        [TestMethod]
        public void ParseReason_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            var xml = @"<root></root>";

            // act
            var element = XElement.Parse(xml);
            var actual = concern.ParseReason(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        public void ParseReference_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            var xml = @"
                <root>
                    <AppAckCode>AppAckCode</AppAckCode>
                    <CrossReferenceNbr>CrossReferenceNbr</CrossReferenceNbr>
                    <PaymentDueDate>PaymentDueDate</PaymentDueDate>
                    <PaymentsAppliedThroughDate>PaymentsAppliedThroughDate</PaymentsAppliedThroughDate>
                    <PurchaseOrderNbr>PurchaseOrderNbr</PurchaseOrderNbr>
                    <ReferenceNbr>ReferenceNbr</ReferenceNbr>
                    <ReferenceQualifier>ReferenceQualifier</ReferenceQualifier>
                    <TotalAmountDue>TotalAmountDue</TotalAmountDue>
                    <TotalPaymentsApplied>TotalPaymentsApplied</TotalPaymentsApplied>
                    <TransactionSetId>TransactionSetId</TransactionSetId>
                </root>";

            // act
            var element = XElement.Parse(xml);
            var actual = concern.ParseReference(element, namespaces);

            // assert
            actual.AppAckCode.ShouldEqual("AppAckCode");
            actual.CrossReferenceNbr.ShouldEqual("CrossReferenceNbr");
            actual.PaymentDueDate.ShouldEqual("PaymentDueDate");
            actual.PaymentsAppliedThroughDate.ShouldEqual("PaymentsAppliedThroughDate");
            actual.PurchaseOrderNbr.ShouldEqual("PurchaseOrderNbr");
            actual.ReferenceNbr.ShouldEqual("ReferenceNbr");
            actual.ReferenceQualifier.ShouldEqual("ReferenceQualifier");
            actual.TotalAmountDue.ShouldEqual("TotalAmountDue");
            actual.TotalPaymentsApplied.ShouldEqual("TotalPaymentsApplied");
            actual.TransactionSetId.ShouldEqual("TransactionSetId");
        }

        [TestMethod]
        public void ParseReference_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            var xml = @"<root></root>";

            // act
            var element = XElement.Parse(xml);
            var actual = concern.ParseReference(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        public void ParseReference_ShouldReturnChildren_WhenElementsSupplied()
        {
            // arrange
            var xml = @"
                <root>
                    <TechErrorLoop><TechError><something /></TechError></TechErrorLoop>
                </root>";

            // act
            var element = XElement.Parse(xml);
            var actual = concern.ParseReference(element, namespaces);

            // assert
            actual.ShouldNotBeNull();

            actual.TechErrors.Count().ShouldEqual(1);
        }

        [TestMethod]
        public void ParseTechError_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            var xml = @"
                <root>
                    <BadElementCopy>BadElementCopy</BadElementCopy>
                    <TechErrorCode>TechErrorCode</TechErrorCode>
                    <TechErrorNote>TechErrorNote</TechErrorNote>
                </root>";

            // act
            var element = XElement.Parse(xml);
            var actual = concern.ParseTechError(element, namespaces);

            // assert
            actual.BadElementCopy.ShouldEqual("BadElementCopy");
            actual.TechErrorCode.ShouldEqual("TechErrorCode");
            actual.TechErrorNote.ShouldEqual("TechErrorNote");
        }

        [TestMethod]
        public void ParseTechError_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            var xml = @"<root></root>";

            // act
            var element = XElement.Parse(xml);
            var actual = concern.ParseTechError(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }
    }
}

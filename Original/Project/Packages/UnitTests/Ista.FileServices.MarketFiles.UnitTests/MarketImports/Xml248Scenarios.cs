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
    public class Xml248Scenarios
    {
        private Dictionary<string, XNamespace> namespaces;
        private ILogger logger;
        
        private Import248Xml concern;
        
        [TestInitialize]
        public void SetUp()
        {
            namespaces = new Dictionary<string, XNamespace>();
            logger = MockRepository.GenerateStub<ILogger>();

            concern = new Import248Xml(logger);
        }

        [TestMethod]
        public void Sample_File()
        {
            // arrange
            const string xml = @"<ns0:Market248 xmlns:ns0=""http://CIS.Integration.Schema.Market.Common.Market248"">
	<TransactionCount>1</TransactionCount>
	<LdcDuns>002899953</LdcDuns>
	<CspDuns>626058655</CspDuns>
	<FunctionalGroupControlNbr>SU</FunctionalGroupControlNbr>
	<Transaction>
		<Header>
			<TransactionSetPurposeCode>22</TransactionSetPurposeCode>
			<TransactionReferenceNbr>201212312204290701500705592187</TransactionReferenceNbr>
			<TransactionDate>20130101</TransactionDate>
			<CrDuns>626058655</CrDuns>
			<CrName>IGS ENERGY</CrName>
			<LDCDuns>002899953</LDCDuns>
			<LDCName>OHIO POWER COMPANY</LDCName>
			<TradingPartnerID>IGSOH002899953WRT</TradingPartnerID>
			<DetailLoop>
				<Detail>
					<CustomerName>CARISSA D VANSCODER</CustomerName>
					<ESPAccountNbr>10368543</ESPAccountNbr>
					<ESIID>00140060763459521</ESIID>
					<OldLdcAccountNbr/>
					<WriteOffAccountNbr/>
					<MarketerCustomerAccountNbr/>
					<ServiceTypeCode/>
					<CustomerTelephone1>4196178104</CustomerTelephone1>
					<CustomerTelephone2>4192942614</CustomerTelephone2>
					<BalanceAmount>114.59</BalanceAmount>
					<WriteOffDate>20121231</WriteOffDate>
					<ReinstatementDate/>
				</Detail>
			</DetailLoop>
		</Header>
	</Transaction>
</ns0:Market248>";

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var result = concern.Parse(stream);

            // assert
            result.TransactionActualCount.ShouldEqual(1);
            result.TransactionAuditCount.ShouldEqual(1);

            var header = result.Headers[0] as Type248Header;
            Assert.IsNotNull(header);
            header.TransactionSetPurposeCode.ShouldEqual("22");
            header.TransactionReferenceNbr.ShouldEqual("201212312204290701500705592187");
            header.TransactionDate.ShouldEqual("20130101");
            header.CrDuns.ShouldEqual("626058655");
            header.CrName.ShouldEqual("IGS ENERGY");
            header.LDCDuns.ShouldEqual("002899953");
            header.LDCName.ShouldEqual("OHIO POWER COMPANY");

            header.Details.Count().ShouldEqual(1);
            var detail = header.Details[0];

            detail.CustomerName.ShouldEqual("CARISSA D VANSCODER");
            detail.ESPAccountNbr.ShouldEqual("10368543");
            detail.ESIID.ShouldEqual("00140060763459521");
            detail.OldLdcAccountNbr.ShouldBeEmpty();
            detail.WriteOffAccountNbr.ShouldBeEmpty();
            detail.ServiceTypeCode.ShouldBeEmpty();
            detail.CustomerTelephone1.ShouldEqual("4196178104");
            detail.CustomerTelephone2.ShouldEqual("4192942614");
            detail.BalanceAmount.ShouldEqual("114.59");
            detail.WriteOffDate.ShouldEqual("20121231");
            detail.ReinstatementDate.ShouldBeEmpty();
        }

        [TestMethod]
        public void ParseDetail_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <BalanceAmount>BalanceAmount</BalanceAmount>
                    <CustomerName>CustomerName</CustomerName>
                    <CustomerTelephone1>CustomerTelephone1</CustomerTelephone1>
                    <CustomerTelephone2>CustomerTelephone2</CustomerTelephone2>
                    <ESIID>ESIID</ESIID>
                    <ESPAccountNbr>ESPAccountNbr</ESPAccountNbr>
                    <HierarchicalID>HierarchicalID</HierarchicalID>
                    <HierarchicalLevelCode>HierarchicalLevelCode</HierarchicalLevelCode>
                    <InvoiceAmount>InvoiceAmount</InvoiceAmount>
                    <InvoiceDate>InvoiceDate</InvoiceDate>
                    <InvoiceNbr>InvoiceNbr</InvoiceNbr>
                    <MarketerCustomerAccountNumber>MarketerCustomerAccountNumber</MarketerCustomerAccountNumber>
                    <OldLdcAccountNbr>OldLdcAccountNbr</OldLdcAccountNbr>
                    <ReinstatementDate>ReinstatementDate</ReinstatementDate>
                    <ServiceTypeCode>ServiceTypeCode</ServiceTypeCode>
                    <WriteOffAccountNbr>WriteOffAccountNbr</WriteOffAccountNbr>
                    <WriteOffDate>WriteOffDate</WriteOffDate>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseDetail(element, namespaces);

            // assert
            actual.BalanceAmount.ShouldEqual("BalanceAmount");
            actual.CustomerName.ShouldEqual("CustomerName");
            actual.CustomerTelephone1.ShouldEqual("CustomerTelephone1");
            actual.CustomerTelephone2.ShouldEqual("CustomerTelephone2");
            actual.ESIID.ShouldEqual("ESIID");
            actual.ESPAccountNbr.ShouldEqual("ESPAccountNbr");
            actual.HierarchicalID.ShouldEqual("HierarchicalID");
            actual.HierarchicalLevelCode.ShouldEqual("HierarchicalLevelCode");
            actual.InvoiceAmount.ShouldEqual("InvoiceAmount");
            actual.InvoiceDate.ShouldEqual("InvoiceDate");
            actual.InvoiceNbr.ShouldEqual("InvoiceNbr");
            actual.MarketerCustomerAccountNumber.ShouldEqual("MarketerCustomerAccountNumber");
            actual.OldLdcAccountNbr.ShouldEqual("OldLdcAccountNbr");
            actual.ReinstatementDate.ShouldEqual("ReinstatementDate");
            actual.ServiceTypeCode.ShouldEqual("ServiceTypeCode");
            actual.WriteOffAccountNbr.ShouldEqual("WriteOffAccountNbr");
            actual.WriteOffDate.ShouldEqual("WriteOffDate");
        }

        [TestMethod]
        public void ParseDetail_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            const string xml = @"<root></root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseDetail(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        public void ParseHeader_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <ControlNbr>ControlNbr</ControlNbr>
                    <CrDuns>CrDuns</CrDuns>
                    <CrName>CrName</CrName>
                    <LDCDuns>LDCDuns</LDCDuns>
                    <LDCName>LDCName</LDCName>
                    <SegmentCount>SegmentCount</SegmentCount>
                    <StructureCode>StructureCode</StructureCode>
                    <TransactionDate>TransactionDate</TransactionDate>
                    <TransactionReferenceNbr>TransactionReferenceNbr</TransactionReferenceNbr>
                    <TransactionSetPurposeCode>TransactionSetPurposeCode</TransactionSetPurposeCode>
                    <TransactionTypeCode>TransactionTypeCode</TransactionTypeCode>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseHeader(element, namespaces);

            // assert
            actual.ControlNbr.ShouldEqual("ControlNbr");
            actual.CrDuns.ShouldEqual("CrDuns");
            actual.CrName.ShouldEqual("CrName");
            actual.LDCDuns.ShouldEqual("LDCDuns");
            actual.LDCName.ShouldEqual("LDCName");
            actual.SegmentCount.ShouldEqual("SegmentCount");
            actual.StructureCode.ShouldEqual("StructureCode");
            actual.TransactionDate.ShouldEqual("TransactionDate");
            actual.TransactionReferenceNbr.ShouldEqual("TransactionReferenceNbr");
            actual.TransactionSetPurposeCode.ShouldEqual("TransactionSetPurposeCode");
            actual.TransactionTypeCode.ShouldEqual("TransactionTypeCode");
        }

        [TestMethod]
        public void ParseHeader_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            const string xml = @"<root></root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseDetail(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        public void ParseHeader_ShouldReturnChildren_WhenElementsSupplied()
        {
            // arrange
            const string xml = @"
                <root>
                    <DetailLoop><Detail><something /></Detail></DetailLoop>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseHeader(element, namespaces);

            // assert
            actual.ShouldNotBeNull();

            actual.Details.Count().ShouldEqual(1);
        }
    }
}

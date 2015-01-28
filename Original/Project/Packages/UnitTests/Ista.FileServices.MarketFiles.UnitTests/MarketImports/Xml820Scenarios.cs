using System.Collections.Generic;
using System.IO;
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
    public class Xml820Scenarios
    {
        private Dictionary<string, XNamespace> namespaces;
        private ILogger logger;
        
        private Import820Xml concern;
        
        [TestInitialize]
        public void SetUp()
        {
            namespaces = new Dictionary<string, XNamespace>();
            logger = MockRepository.GenerateStub<ILogger>();

            concern = new Import820Xml(logger);
        }

        [TestMethod]
        public void Sample_File()
        {
            // arrange
            const string xml = @"<ns0:Market820 xmlns:ns0=""http://CIS.Integration.Schema.Market.Common.Market820"">
	<TransactionCount>1</TransactionCount>
	<LdcDuns>002899953</LdcDuns>
	<CspDuns>626058655</CspDuns>
	<Transaction>
		<Header>
			<TransactionSetId>820</TransactionSetId>
			<TransactionTypeCode>I</TransactionTypeCode>
			<TransactionDate>20130122</TransactionDate>
			<TotalAmount>8045.19</TotalAmount>
			<CreditDebitFlag>C</CreditDebitFlag>
			<PaymentMethodCode>ACH</PaymentMethodCode>
			<TraceReferenceNbr>AP001302564</TraceReferenceNbr>
			<TdspDuns>002899953</TdspDuns>
			<TdspName>OHIO POWER COMPANY</TdspName>
			<CrDuns>626058655</CrDuns>
			<CrName>IGS ENERGY</CrName>
			<PaymentFormatCode>CTX</PaymentFormatCode>
			<PayerIdentificationNbr/>
			<PayerAccountNbrQualifier/>
			<PayerAccountNbr/>
			<PayeeIdentificationNbr/>
			<PayeeAccountNbrQualifier/>
			<PayeeAccountNbr/>
			<TradingPartnerID>IGSOH002899953PYM</TradingPartnerID>
			<DetailLoop>
				<Detail>
					<PaymentActionCode>PO</PaymentActionCode>
					<PaymentAmount>53.34</PaymentAmount>
					<AdjustmentReasonCode/>
					<AdjustmentAmount/>
					<EsiId>00140060700732645</EsiId>
					<ESPAccountNumber>10349043</ESPAccountNumber>
					<DatePosted>20130122</DatePosted>
				</Detail>
				<Detail>
					<PaymentActionCode>PO</PaymentActionCode>
					<PaymentAmount>41.24</PaymentAmount>
					<AdjustmentReasonCode/>
					<AdjustmentAmount/>
					<EsiId>00140060701021272</EsiId>
					<ESPAccountNumber>10312314</ESPAccountNumber>
					<DatePosted>20130122</DatePosted>
				</Detail>
			</DetailLoop>
		</Header>
	</Transaction>
</ns0:Market820>";

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var result = concern.Parse(stream);

            // assert
            result.TransactionActualCount.ShouldEqual(1);
            result.TransactionAuditCount.ShouldEqual(1);

            var header = result.Headers[0] as Type820Header;
            Assert.IsNotNull(header);
            header.TransactionSetId.ShouldEqual("820");
            header.TransactionTypeCode.ShouldEqual("I");
            header.TransactionDate.ShouldEqual("20130122");
            header.TotalAmount.ShouldEqual("8045.19");
            header.CreditDebitFlag.ShouldEqual("C");
            header.PaymentMethodCode.ShouldEqual("ACH");
            header.TraceReferenceNbr.ShouldEqual("AP001302564");
            header.TdspDuns.ShouldEqual("002899953");
            header.TdspName.ShouldEqual("OHIO POWER COMPANY");
            header.CrDuns.ShouldEqual("626058655");
            header.CrName.ShouldEqual("IGS ENERGY");

            header.Details.Length.ShouldEqual(2);

            var detail = header.Details[0];
            detail.PaymentActionCode.ShouldEqual("PO");
            detail.PaymentAmount.ShouldEqual("53.34");
            detail.AdjustmentReasonCode.ShouldBeEmpty();
            detail.AdjustmentAmount.ShouldBeEmpty();
            detail.EsiId.ShouldEqual("00140060700732645");
            detail.ESPAccountNumber.ShouldEqual("10349043");
            detail.DatePosted.ShouldEqual("20130122");
        }

        [TestMethod]
        public void Parse_Detail_Should_Return_Object_With_Valid_Values_When_Xml_Is_Valid()
        {
            // arrange
            const string xml = @"<root>
    <AdjustmentAmount>AdjustmentAmount</AdjustmentAmount>
    <AdjustmentReasonCode>AdjustmentReasonCode</AdjustmentReasonCode>
    <AssignedId>AssignedId</AssignedId>
    <CommodityCode>CommodityCode</CommodityCode>
    <CrossReferenceNbr>CrossReferenceNbr</CrossReferenceNbr>
    <CustomerName>CustomerName</CustomerName>
    <DatePosted>DatePosted</DatePosted>
    <DiscountAmount>DiscountAmount</DiscountAmount>
    <EsiId>EsiId</EsiId>
    <ESPAccountNumber>ESPAccountNumber</ESPAccountNumber>
    <InvoiceAmount>InvoiceAmount</InvoiceAmount>
    <PaymentActionCode>PaymentActionCode</PaymentActionCode>
    <PaymentAmount>PaymentAmount</PaymentAmount>
    <PrevUtilityAccountNumber>PrevUtilityAccountNumber</PrevUtilityAccountNumber>
    <ReferenceId>ReferenceId</ReferenceId>
    <ReferenceNbr>ReferenceNbr</ReferenceNbr>
    <UnmeteredServiceDesignator>UnmeteredServiceDesignator</UnmeteredServiceDesignator>
</root>";

            // act
            var detailElement = XElement.Parse(xml);
            var actual = concern.ParseDetail(detailElement, namespaces);
            
            // assert
            actual.AdjustmentAmount.ShouldEqual("AdjustmentAmount");
            actual.AdjustmentReasonCode.ShouldEqual("AdjustmentReasonCode");
            actual.AssignedId.ShouldEqual("AssignedId");
            actual.CommodityCode.ShouldEqual("CommodityCode");
            actual.CrossReferenceNbr.ShouldEqual("CrossReferenceNbr");
            actual.CustomerName.ShouldEqual("CustomerName");
            actual.DatePosted.ShouldEqual("DatePosted");
            actual.DiscountAmount.ShouldEqual("DiscountAmount");
            actual.EsiId.ShouldEqual("EsiId");
            actual.ESPAccountNumber.ShouldEqual("ESPAccountNumber");
            actual.InvoiceAmount.ShouldEqual("InvoiceAmount");
            actual.PaymentActionCode.ShouldEqual("PaymentActionCode");
            actual.PaymentAmount.ShouldEqual("PaymentAmount");
            actual.PrevUtilityAccountNumber.ShouldEqual("PrevUtilityAccountNumber");
            actual.ReferenceId.ShouldEqual("ReferenceId");
            actual.ReferenceNbr.ShouldEqual("ReferenceNbr");
            actual.UnmeteredServiceDesignator.ShouldEqual("UnmeteredServiceDesignator");
        }

        [TestMethod]
        public void Parse_Header_Should_Return_Object_With_Valid_Values_When_Xml_Is_Valid()
        {
            // arrange
            const string xml = @"
                <root>
                    <CrDuns>CrDuns</CrDuns>
                    <CreateDate>CreateDate</CreateDate>
                    <CreditDebitFlag>CreditDebitFlag</CreditDebitFlag>
                    <CrName>CrName</CrName>
                    <ESPUtilityAccountNumber>ESPUtilityAccountNumber</ESPUtilityAccountNumber>
                    <PaymentMethodCode>PaymentMethodCode</PaymentMethodCode>
                    <TdspDuns>TdspDuns</TdspDuns>
                    <TdspDunsStructureCode>TdspDunsStructureCode</TdspDunsStructureCode>
                    <TdspName>TdspName</TdspName>
                    <TotalAmount>TotalAmount</TotalAmount>
                    <TraceReferenceNbr>TraceReferenceNbr</TraceReferenceNbr>
                    <TransactionDate>TransactionDate</TransactionDate>
                    <TransactionNbr>TransactionNbr</TransactionNbr>
                    <TransactionSetControlNbr>TransactionSetControlNbr</TransactionSetControlNbr>
                    <TransactionSetId>TransactionSetId</TransactionSetId>
                    <TransactionTypeCode>TransactionTypeCode</TransactionTypeCode>
                </root>";

            // act
            var headerElement = XElement.Parse(xml);
            var actual = concern.ParseHeader(headerElement, namespaces);

            // assert
            actual.CrDuns.ShouldEqual("CrDuns");
            actual.CreateDate.ShouldEqual("CreateDate");
            actual.CreditDebitFlag.ShouldEqual("CreditDebitFlag");
            actual.CrName.ShouldEqual("CrName");
            actual.ESPUtilityAccountNumber.ShouldEqual("ESPUtilityAccountNumber");
            actual.PaymentMethodCode.ShouldEqual("PaymentMethodCode");
            actual.TdspDuns.ShouldEqual("TdspDuns");
            actual.TdspDunsStructureCode.ShouldEqual("TdspDunsStructureCode");
            actual.TdspName.ShouldEqual("TdspName");
            actual.TotalAmount.ShouldEqual("TotalAmount");
            actual.TraceReferenceNbr.ShouldEqual("TraceReferenceNbr");
            actual.TransactionDate.ShouldEqual("TransactionDate");
            actual.TransactionNbr.ShouldEqual("TransactionNbr");
            actual.TransactionSetControlNbr.ShouldEqual("TransactionSetControlNbr");
            actual.TransactionSetId.ShouldEqual("TransactionSetId");
            actual.TransactionTypeCode.ShouldEqual("TransactionTypeCode");
        }
    }
}

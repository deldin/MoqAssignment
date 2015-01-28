using System;
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
    public class Xml867Scenarios
    {
        private Dictionary<string, XNamespace> _namespaces;
        private ILogger _logger;

        private Import867Xml _concern;

        [TestInitialize]
        public void SetUp()
        {
            _namespaces = new Dictionary<string, XNamespace>();
            _logger = MockRepository.GenerateStub<ILogger>();

            _concern = new Import867Xml(_logger);
        }

        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingHeader_ShouldReturnObjectWithValidValues_WhenXmlIsValid()// ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<ns0:Market867 xmlns:ns0=""http://CIS.Integration.Schema.Market.Common.Market820"">
<TransactionCount>1</TransactionCount>  
<LdcDuns>007909427AC</LdcDuns>
<LdcName>PPL</LdcName>
<CspDuns>962954579</CspDuns>
<CspName>STREAM ENERGY</CspName>
<FunctionalGroup>PT</FunctionalGroup>
<Transaction>
<Header>
<TransactionSetId>867</TransactionSetId>
<TransactionSetPurposeCode>00</TransactionSetPurposeCode>
<TransactionNbr>86720130313095125308527</TransactionNbr>
<TransactionDate>20130313</TransactionDate>
<ReportTypeCode>DD</ReportTypeCode>
<ActionCode>ActionCodeValue</ActionCode>
<DocumentDueDate>20130313095125</DocumentDueDate>
<EsiId>7777777778EsiId</EsiId>
<MembershipId>7777777778MemId</MembershipId>
<OriginalTransactionNbr>OrigTransNum</OriginalTransactionNbr>
<TdspDuns>007909427AC</TdspDuns>
<TdspName>PPL</TdspName>
<CrDuns>962954579</CrDuns>
<CrName>STREAM ENERGY</CrName>
<CrIdentifier>CrIdent</CrIdentifier>
<UtilityAccountNumber>7777777778UtiliAcctNum</UtilityAccountNumber>
<PreviousUtilityAccountNumber>PrevUtilAcctNum</PreviousUtilityAccountNumber>
<BillCalculator>LDC</BillCalculator>
<ESPAccountNumber>3001246090</ESPAccountNumber>
<TradingPartnerID>STEPA007909427USE</TradingPartnerID>
<DoorHangerFlag>DoorHangerFlagValue</DoorHangerFlag>
<ESNCount>99</ESNCount>
<EstimationDescription>EstDesc</EstimationDescription>
<EstimationReason>EstReason</EstimationReason>
<InvoiceNbr>InvoiceNum</InvoiceNbr>
<NextMeterReadDate>20130130</NextMeterReadDate>
<PowerRegion>PowerReg</PowerRegion>
<ReferenceNbr>RefNum</ReferenceNbr>
<TransactionSetControlNbr>TransControlNbr</TransactionSetControlNbr>
<UtilityContractID>UtilContract</UtilityContractID>
</Header></Transaction></ns0:Market867>";

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var result = _concern.Parse(stream);

            // assert
            result.TransactionActualCount.ShouldEqual(1);
            result.TransactionAuditCount.ShouldEqual(1);

            var header = result.Headers[0] as Type867Header;
            Assert.IsNotNull(header);
            header.ActionCode.ShouldEqual("ActionCodeValue");
            header.CrDuns.ShouldEqual("962954579");
            header.CrName.ShouldEqual("STREAM ENERGY");
            header.DirectionFlag = true;
            header.DocumentDueDate.ShouldEqual("20130313095125");
            header.DoorHangerFlag.ShouldEqual("DoorHangerFlagValue");
            header.EsiId.ShouldEqual("7777777778EsiId");
            header.EsnCount.ShouldEqual("99");
            header.EspAccountNumber.ShouldBeNull(); //Not set by XML Parser
            header.EstimationDescription.ShouldEqual("EstDesc");
            header.EstimationReason.ShouldEqual("EstReason");
            header.InvoiceNbr.ShouldEqual("InvoiceNum");
            header.MarketFileId.ShouldEqual(0);// MarketFileId is never being set in XML Parser
            header.MarketID.ShouldEqual(0);//MarketId us never being set in XML Parser
            header.NextMeterReadDate.ShouldEqual("20130130");
            header.OriginalTransactionNbr.ShouldEqual("OrigTransNum");
            header.PowerRegion.ShouldEqual("PowerReg");
            header.PreviousUtilityAccountNumber.ShouldEqual("PrevUtilAcctNum");
            header.ProcessDate.ShouldEqual(DateTime.MinValue); //ProcessDate is not set in XML parser and rightly so, it should be default for DateTime
            header.ProcessFlag.ShouldEqual(false);//Is not set so it should be false 
            header.ProviderID.ShouldEqual(0);//Is not set in XML Parser so it should still be 0
            header.ReferenceNbr.ShouldEqual("RefNum");
            header.ReportTypeCode.ShouldEqual("DD");
            header.TdspDuns.ShouldEqual("007909427AC");
            header.TdspName.ShouldEqual("PPL");
            header.TransactionDate.ShouldEqual("20130313");
            header.TransactionNbr.ShouldEqual("86720130313095125308527");
            header.TransactionSetControlNbr.ShouldEqual("TransControlNbr");
            header.TransactionSetId.ShouldEqual("867");
            header.TransactionSetPurposeCode.ShouldEqual("00");
            header.TransactionTypeID.ShouldEqual(0); // TransactionTypeID is not being set by the xml parser
            header.UtilityAccountNumber.ShouldEqual("7777777778UtiliAcctNum");
            header.UtilityContractID.ShouldEqual("UtilContract"); 
           


        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingHeader_ShouldParse_EvenIfNodeIsEmpty() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<root></root>";

            // act

            var element = XElement.Parse(xml);
            var actual = _concern.ParseHeader(element, _namespaces);

            // assert
            actual.ShouldNotBeNull();
        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingName_ShouldReturnObjectWithValidValues_WhenXmlIsValid()// ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<ns0:Market867 xmlns:ns0=""http://CIS.Integration.Schema.Market.Common.Market820"">
<TransactionCount>1</TransactionCount>  
<LdcDuns>007909427AC</LdcDuns>
<LdcName>PPL</LdcName>
<CspDuns>962954579</CspDuns>
<CspName>STREAM ENERGY</CspName>
<FunctionalGroup>PT</FunctionalGroup>
<Transaction>
<Header>
<TransactionSetId>867</TransactionSetId>
<TransactionSetPurposeCode>00</TransactionSetPurposeCode>
<TransactionNbr>86720130313095125308527</TransactionNbr>
<TransactionDate>20130313</TransactionDate>
<ReportTypeCode>DD</ReportTypeCode>
<ActionCode>ActionCodeValue</ActionCode>
<DocumentDueDate>20130313095125</DocumentDueDate>
<EsiId>7777777778EsiId</EsiId>
<MembershipId>7777777778MemId</MembershipId>
<OriginalTransactionNbr>OrigTransNum</OriginalTransactionNbr>
<TdspDuns>007909427AC</TdspDuns>
<TdspName>PPL</TdspName>
<CrDuns>962954579</CrDuns>
<CrName>STREAM ENERGY</CrName>
<CrIdentifier>CrIdent</CrIdentifier>
<UtilityAccountNumber>7777777778UtiliAcctNum</UtilityAccountNumber>
<PreviousUtilityAccountNumber>PrevUtilAcctNum</PreviousUtilityAccountNumber>
<BillCalculator>LDC</BillCalculator>
<ESPAccountNumber>3001246090</ESPAccountNumber>
<TradingPartnerID>STEPA007909427USE</TradingPartnerID>
<DoorHangerFlag>DoorHangerFlagValue</DoorHangerFlag>
<ESNCount>99</ESNCount>
<EstimationDescription>EstDesc</EstimationDescription>
<EstimationReason>EstReason</EstimationReason>
<InvoiceNbr>InvoiceNum</InvoiceNbr>
<NextMeterReadDate>20130130</NextMeterReadDate>
<PowerRegion>PowerReg</PowerRegion>
<ReferenceNbr>RefNum</ReferenceNbr>
<TransactionSetControlNbr>TransControlNbr</TransactionSetControlNbr>
<UtilityContractID>UtilContract</UtilityContractID>
<NameLoop>
    <Name>
        <EntityIdType>8R</EntityIdType> 
        <EntityName>JR EDWARD A WARNER</EntityName> 
        <CustomerIdentificationNumber /> 
        <EntityDuns>EntityDunsVal</EntityDuns>
        <EntityIdCode>EntityIdCodeVal</EntityIdCode>
        <ServiceAddress1>ServAdd1</ServiceAddress1>
        <ServiceAddress2>ServAdd2</ServiceAddress2>
        <ServiceCity>ServiceCityVal</ServiceCity>
        <ServiceState>ServiceStateVal</ServiceState>
        <ServiceZipCode>ServiceZipCodeVal</ServiceZipCode>
    </Name>
    <Name>
        <EntityIdType>8RXX</EntityIdType> 
        <EntityName>JR EDWARD A WARNERXX</EntityName> 
        <CustomerIdentificationNumber /> 
        <EntityDuns>EntityDunsValXX</EntityDuns>
        <EntityIdCode>EntityIdCodeValXX</EntityIdCode>
        <ServiceAddress1>ServAdd1XX</ServiceAddress1>
        <ServiceAddress2>ServAdd2XX</ServiceAddress2>
        <ServiceCity>ServiceCityValXX</ServiceCity>
        <ServiceState>ServiceStateValXX</ServiceState>
        <ServiceZipCode>ServiceZipCodeValXX</ServiceZipCode>
    </Name>
</NameLoop>
</Header></Transaction></ns0:Market867>";

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var result = _concern.Parse(stream);

            // assert
            var header = result.Headers[0] as Type867Header;
            Assert.IsNotNull(header);

            //Check if count of Names is correct
            Assert.AreEqual(header.Names.Length, 2);


            var name = header.Names[0];
            Assert.IsNotNull(name);

            name.EntityDuns.ShouldEqual("EntityDunsVal");
            name.EntityIdCode.ShouldEqual("EntityIdCodeVal");
            name.EntityIdType.ShouldEqual("8R");
            name.EntityName.ShouldEqual("JR EDWARD A WARNER");
            name.ServiceAddress1.ShouldEqual("ServAdd1");
            name.ServiceAddress2.ShouldEqual("ServAdd2");
            name.ServiceCity.ShouldEqual("ServiceCityVal");
            name.ServiceState.ShouldEqual("ServiceStateVal");
            name.ServiceZipCode.ShouldEqual("ServiceZipCodeVal");

            //Fields not set by parser
            name.HeaderKey.ShouldEqual(0);
            name.NameKey.ShouldEqual(0); 
            

            var nameSecond = header.Names[1];
            Assert.IsNotNull(nameSecond);

            nameSecond.EntityDuns.ShouldEqual("EntityDunsValXX");
            nameSecond.EntityIdCode.ShouldEqual("EntityIdCodeValXX");
            nameSecond.EntityIdType.ShouldEqual("8RXX");
            nameSecond.EntityName.ShouldEqual("JR EDWARD A WARNERXX");
            nameSecond.ServiceAddress1.ShouldEqual("ServAdd1XX");
            nameSecond.ServiceAddress2.ShouldEqual("ServAdd2XX");
            nameSecond.ServiceCity.ShouldEqual("ServiceCityValXX");
            nameSecond.ServiceState.ShouldEqual("ServiceStateValXX");
            nameSecond.ServiceZipCode.ShouldEqual("ServiceZipCodeValXX");


            //Fields not set by parser
            nameSecond.HeaderKey.ShouldEqual(0);
            nameSecond.NameKey.ShouldEqual(0);
        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingName_ShouldParse_EvenIfNodeIsEmpty() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<root></root>";

            // act

            var element = XElement.Parse(xml);
            var actual = _concern.ParseName(element, _namespaces);

            // assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingNonIntervalDetail_ShouldReturnObjectWithValidValues_WhenXmlIsValid()// ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<ns0:Market867 xmlns:ns0=""http://CIS.Integration.Schema.Market.Common.Market820"">
<TransactionCount>1</TransactionCount>  
<LdcDuns>007909427AC</LdcDuns>
<LdcName>PPL</LdcName>
<CspDuns>962954579</CspDuns>
<CspName>STREAM ENERGY</CspName>
<FunctionalGroup>PT</FunctionalGroup>
<Transaction>
<Header>
<TransactionSetId>867</TransactionSetId>
<TransactionSetPurposeCode>00</TransactionSetPurposeCode>
<TransactionNbr>86720130313095125308527</TransactionNbr>
<TransactionDate>20130313</TransactionDate>
<ReportTypeCode>DD</ReportTypeCode>
<ActionCode>ActionCodeValue</ActionCode>
<DocumentDueDate>20130313095125</DocumentDueDate>
<EsiId>7777777778EsiId</EsiId>
<MembershipId>7777777778MemId</MembershipId>
<OriginalTransactionNbr>OrigTransNum</OriginalTransactionNbr>
<TdspDuns>007909427AC</TdspDuns>
<TdspName>PPL</TdspName>
<CrDuns>962954579</CrDuns>
<CrName>STREAM ENERGY</CrName>
<CrIdentifier>CrIdent</CrIdentifier>
<UtilityAccountNumber>7777777778UtiliAcctNum</UtilityAccountNumber>
<PreviousUtilityAccountNumber>PrevUtilAcctNum</PreviousUtilityAccountNumber>
<BillCalculator>LDC</BillCalculator>
<ESPAccountNumber>3001246090</ESPAccountNumber>
<TradingPartnerID>STEPA007909427USE</TradingPartnerID>
<DoorHangerFlag>DoorHangerFlagValue</DoorHangerFlag>
<ESNCount>99</ESNCount>
<EstimationDescription>EstDesc</EstimationDescription>
<EstimationReason>EstReason</EstimationReason>
<InvoiceNbr>InvoiceNum</InvoiceNbr>
<NextMeterReadDate>20130130</NextMeterReadDate>
<PowerRegion>PowerReg</PowerRegion>
<ReferenceNbr>RefNum</ReferenceNbr>
<TransactionSetControlNbr>TransControlNbr</TransactionSetControlNbr>
<UtilityContractID>UtilContract</UtilityContractID>
<NonIntervalDetailLoop>
    <NonIntervalDetail>
        <TypeCode>PL</TypeCode> 
        <MeterNumber>791105657</MeterNumber>
        <MovementTypeCode>MovTypeCode</MovementTypeCode> 
        <ServicePeriodStart>20130214</ServicePeriodStart> 
        <ServicePeriodEnd>20130315</ServicePeriodEnd> 
        <ExchangeDate>20140125</ExchangeDate> 
        <MeterRole>A</MeterRole> 
        <MeterUOM>KH</MeterUOM> 
        <MeterInterval>MON</MeterInterval> 
        <CommodityCode>ComCode</CommodityCode>
        <NumberOfDials>5.0</NumberOfDials> 
        <ServicePointId>ServPointId</ServicePointId>
        <UtilityRateServiceClass>OE-RSD</UtilityRateServiceClass> 
        <RateSubClass>CH</RateSubClass> 
        <Ratchet_DateTime>20150215</Ratchet_DateTime>
        <ServiceIndicator /> 
        <ReadingNote /> 
        <NonIntervalDetailQtyLoop>
            <NonIntervalDetailQty>
                <Qualifier>KA</Qualifier> 
                <Quantity>666.0</Quantity> 
                <MeasurementCode>AE</MeasurementCode> 
                <UOM>KH</UOM> 
                <BeginRead>41244</BeginRead> 
                <EndRead>41910</EndRead> 
                <MeasurementSignificanceCode>51</MeasurementSignificanceCode> 
                <MeterMultiplier>1</MeterMultiplier> 
				<TransformerLossFactor>TranLossFact</TransformerLossFactor>
				<PowerFactor>PowerFactorVal</PowerFactor>
				<RangeMin>RangeMinVal</RangeMin>
				<RangeMax>RangeMaxVal</RangeMax>
				<ThermFactor>ThermFactorVal</ThermFactor>
				<DegreeDayFactor>DegreeDayFactorVal</DegreeDayFactor>
				<BackoutCredit>BackoutCreditVal</BackoutCredit> 
                <CompositeUOM>CompositeUOMVal</CompositeUOM>
            </NonIntervalDetailQty>
            <NonIntervalDetailQty>
            </NonIntervalDetailQty>
        </NonIntervalDetailQtyLoop>
    </NonIntervalDetail>
    <NonIntervalDetail>
    </NonIntervalDetail>
</NonIntervalDetailLoop>
</Header></Transaction></ns0:Market867>";

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var result = _concern.Parse(stream);

            // assert
            var header = result.Headers[0] as Type867Header;
            Assert.IsNotNull(header);

            //Check if count of Names is correct
            Assert.AreEqual(header.NonIntervalDetails.Length, 2);


            var nonIntdetail = header.NonIntervalDetails[0];
            Assert.IsNotNull(nonIntdetail);

            nonIntdetail.CommodityCode.ShouldEqual("ComCode");
            nonIntdetail.ExchangeDate.ShouldEqual("20140125");
            nonIntdetail.HeaderKey.ShouldEqual(0); // Is not set in XML Parser
            nonIntdetail.LoadProfile.ShouldBeNull(); // Not set in XML Parser
            nonIntdetail.MeterInterval.ShouldEqual("MON");
            nonIntdetail.MeterNumber.ShouldEqual("791105657");
            nonIntdetail.MeterRole.ShouldEqual("A");
            nonIntdetail.MeterUom.ShouldEqual("KH");
            nonIntdetail.MovementTypeCode.ShouldEqual("MovTypeCode");
            nonIntdetail.NonIntervalSummaryKey.ShouldEqual(0); //This is not set in XML Parser
            nonIntdetail.NonIntervalDetailKey.ShouldEqual(0); //This is not set in XML Parser
            nonIntdetail.NumberOfDials.ShouldEqual("5.0");
            nonIntdetail.RatchetDateTime.ShouldEqual("20150215");
            nonIntdetail.RateSubClass.ShouldEqual("CH");
            nonIntdetail.SequenceNumber.ShouldBeNull(); //This is not set in XML Parser
            nonIntdetail.ServiceIndicator.ShouldBeNull(); //This is not set in XML Parser
            nonIntdetail.ServicePeriodEnd.ShouldEqual("20130315");
            nonIntdetail.ServicePeriodStart.ShouldEqual("20130214");
            nonIntdetail.ServicePointId.ShouldEqual("ServPointId");
            nonIntdetail.TypeCode.ShouldEqual("PL");
            nonIntdetail.UtilityRateServiceClass.ShouldEqual("OE-RSD");

            Assert.AreEqual(nonIntdetail.NonIntervalDetailQtys.Length, 2);

            var qty = nonIntdetail.NonIntervalDetailQtys[0];
            Assert.IsNotNull(qty);

            //Fields set in XML Parser
            qty.Qualifier.ShouldEqual("KA");
            qty.Quantity.ShouldEqual("666.0");
            qty.MeasurementCode.ShouldEqual("AE");
            qty.Uom.ShouldEqual("KH");
            qty.BeginRead.ShouldEqual("41244");
            qty.EndRead.ShouldEqual("41910");
            qty.MeasurementSignificanceCode.ShouldEqual("51");
            qty.MeterMultiplier.ShouldEqual("1");
            qty.TransformerLossFactor.ShouldEqual("TranLossFact");
            qty.PowerFactor.ShouldEqual("PowerFactorVal");
            qty.RangeMin.ShouldEqual("RangeMinVal");
            qty.RangeMax.ShouldEqual("RangeMaxVal");
            qty.ThermFactor.ShouldEqual("ThermFactorVal");
            qty.DegreeDayFactor.ShouldEqual("DegreeDayFactorVal");
            qty.BackoutCredit.ShouldEqual("BackoutCreditVal");
            qty.CompositeUom.ShouldEqual("CompositeUOMVal"); 
            

            //Fields not set in XML Parser
            qty.NonIntervalDetailKey.ShouldEqual(0);
            qty.NonIntervalDetailQtyKey.ShouldEqual(0);
            qty.ProcessDate.ShouldEqual(DateTime.MinValue);
            qty.ProcessFlag.ShouldEqual((short)0);
            qty.Status.ShouldBeNull();
            


            var qtySecond = nonIntdetail.NonIntervalDetailQtys[1];
            Assert.IsNotNull(qtySecond);


            var nonIntdetailSecond = header.NonIntervalDetails[1];
            Assert.IsNotNull(nonIntdetailSecond);



        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingNonIntervalDetail_ShouldParse_EvenIfNodeIsEmpty() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<root></root>";

            // act

            var element = XElement.Parse(xml);
            var actual = _concern.ParseNonIntervalDetail(element, _namespaces);

            // assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingNonIntervalSummary_ShouldReturnObjectWithValidValues_WhenXmlIsValid()// ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<ns0:Market867 xmlns:ns0=""http://CIS.Integration.Schema.Market.Common.Market820"">
<TransactionCount>1</TransactionCount>  
<LdcDuns>007909427AC</LdcDuns>
<LdcName>PPL</LdcName>
<CspDuns>962954579</CspDuns>
<CspName>STREAM ENERGY</CspName>
<FunctionalGroup>PT</FunctionalGroup>
<Transaction>
<Header>
<TransactionSetId>867</TransactionSetId>
<TransactionSetPurposeCode>00</TransactionSetPurposeCode>
<TransactionNbr>86720130313095125308527</TransactionNbr>
<TransactionDate>20130313</TransactionDate>
<ReportTypeCode>DD</ReportTypeCode>
<ActionCode>ActionCodeValue</ActionCode>
<DocumentDueDate>20130313095125</DocumentDueDate>
<EsiId>7777777778EsiId</EsiId>
<MembershipId>7777777778MemId</MembershipId>
<OriginalTransactionNbr>OrigTransNum</OriginalTransactionNbr>
<TdspDuns>007909427AC</TdspDuns>
<TdspName>PPL</TdspName>
<CrDuns>962954579</CrDuns>
<CrName>STREAM ENERGY</CrName>
<CrIdentifier>CrIdent</CrIdentifier>
<UtilityAccountNumber>7777777778UtiliAcctNum</UtilityAccountNumber>
<PreviousUtilityAccountNumber>PrevUtilAcctNum</PreviousUtilityAccountNumber>
<BillCalculator>LDC</BillCalculator>
<ESPAccountNumber>3001246090</ESPAccountNumber>
<TradingPartnerID>STEPA007909427USE</TradingPartnerID>
<DoorHangerFlag>DoorHangerFlagValue</DoorHangerFlag>
<ESNCount>99</ESNCount>
<EstimationDescription>EstDesc</EstimationDescription>
<EstimationReason>EstReason</EstimationReason>
<InvoiceNbr>InvoiceNum</InvoiceNbr>
<NextMeterReadDate>20130130</NextMeterReadDate>
<PowerRegion>PowerReg</PowerRegion>
<ReferenceNbr>RefNum</ReferenceNbr>
<TransactionSetControlNbr>TransControlNbr</TransactionSetControlNbr>
<UtilityContractID>UtilContract</UtilityContractID>
<NonIntervalSummaryLoop>
    <NonIntervalSummary>
        <TypeCode>SU</TypeCode> 
        <MeterUOM>MKH</MeterUOM>
        <MeterInterval>MON</MeterInterval>
        <CommodityCode>CommCodeVal</CommodityCode>
        <NumberOfDials>4.5</NumberOfDials>
        <ServicePointId>ServPntID</ServicePointId>
        <UtilityRateServiceClass>UtilRateClass</UtilityRateServiceClass>
        <RateSubClass>RatSubClass</RateSubClass>
        <ServicePeriodStart>20121227</ServicePeriodStart> 
        <ServicePeriodEnd>20130123</ServicePeriodEnd> 
        <NonIntervalSummaryQtyLoop>
			<NonIntervalSummaryQty>
				<Qualifier>QD</Qualifier> 
				<Quantity>2829.0</Quantity> 
				<MeasurementSignificanceCode>51</MeasurementSignificanceCode> 
				<ServicePeriodStart>20121227</ServicePeriodStart> 
				<ServicePeriodEnd>20130123</ServicePeriodEnd> 
				<RangeMin>RangeMinVal</RangeMin>
				<RangeMax>RangeMaxVal</RangeMax>
				<ThermFactor>ThermFactorVal</ThermFactor>
				<DegreeDayFactor>DegreeDayFactorVal</DegreeDayFactor>
				<BackoutCredit>BackoutCreditVal</BackoutCredit> 
				<CompositeUOM>CompositeUOMVal</CompositeUOM>
			</NonIntervalSummaryQty>
            <NonIntervalSummaryQty>
            </NonIntervalSummaryQty>
        </NonIntervalSummaryQtyLoop>
    </NonIntervalSummary>
    <NonIntervalSummary>
    </NonIntervalSummary>
</NonIntervalSummaryLoop>
</Header></Transaction></ns0:Market867>";

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var result = _concern.Parse(stream);

            // assert
            var header = result.Headers[0] as Type867Header;
            Assert.IsNotNull(header);

            //Check if count of Names is correct
            Assert.AreEqual(header.NonIntervalSummaries.Length, 2);


            var nonIntSummary = header.NonIntervalSummaries[0];
            Assert.IsNotNull(nonIntSummary);

            //Fiels set in XML Parser
            nonIntSummary.TypeCode.ShouldEqual("SU");
            nonIntSummary.MeterUOM.ShouldEqual("MKH");
            nonIntSummary.MeterInterval.ShouldEqual("MON");
            nonIntSummary.CommodityCode.ShouldEqual("CommCodeVal");
            nonIntSummary.NumberOfDials.ShouldEqual("4.5");
            nonIntSummary.ServicePointId.ShouldEqual("ServPntID");
            nonIntSummary.UtilityRateServiceClass.ShouldEqual("UtilRateClass");
            nonIntSummary.RateSubClass.ShouldEqual("RatSubClass");

            //Fiels not set in XML Parser
            nonIntSummary.ActualSwitchDate.ShouldBeNull();
            nonIntSummary.HeaderKey.ShouldEqual(0);
            nonIntSummary.NonIntervalSummaryKey.ShouldEqual(0);
            nonIntSummary.SequenceNumber.ShouldBeNull();
            nonIntSummary.ServiceIndicator.ShouldBeNull();
            


            Assert.AreEqual(nonIntSummary.NonIntervalSummaryQtys.Length, 2);

            var qty = nonIntSummary.NonIntervalSummaryQtys[0];
            Assert.IsNotNull(qty);

            //Fields set in XML Parser
            qty.Qualifier.ShouldEqual("QD");
            qty.Quantity.ShouldEqual("2829.0");
            qty.MeasurementSignificanceCode.ShouldEqual("51");
            qty.ServicePeriodStart.ShouldEqual("20121227");
            qty.ServicePeriodEnd.ShouldEqual("20130123");
            qty.RangeMin.ShouldEqual("RangeMinVal");
            qty.RangeMax.ShouldEqual("RangeMaxVal");
            qty.ThermFactor.ShouldEqual("ThermFactorVal");
            qty.DegreeDayFactor.ShouldEqual("DegreeDayFactorVal");
            qty.CompositeUom.ShouldEqual("CompositeUOMVal");


            //Fields not set in XML Parser
            qty.NonIntervalSummaryKey.ShouldEqual(0);
            qty.NonIntervalSummaryQtyKey.ShouldEqual(0);
            qty.MeterMultiplier.ShouldBeNull();


            var qtySecond = nonIntSummary.NonIntervalSummaryQtys[1];
            Assert.IsNotNull(qtySecond);


            var nonIntSummarySecond = header.NonIntervalSummaries[1];
            Assert.IsNotNull(nonIntSummarySecond);



        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingNonIntervalSummary_ShouldParse_EvenIfNodeIsEmpty() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<root></root>";

            // act

            var element = XElement.Parse(xml);
            var actual = _concern.ParseNonIntervalSummary(element, _namespaces);

            // assert
            actual.ShouldNotBeNull();
        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingIntervalDetail_ShouldReturnObjectWithValidValues_WhenXmlIsValid()// ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<ns0:Market867 xmlns:ns0=""http://CIS.Integration.Schema.Market.Common.Market820"">
<TransactionCount>1</TransactionCount>  
<LdcDuns>007909427AC</LdcDuns>
<LdcName>PPL</LdcName>
<CspDuns>962954579</CspDuns>
<CspName>STREAM ENERGY</CspName>
<FunctionalGroup>PT</FunctionalGroup>
<Transaction>
<Header>
<TransactionSetId>867</TransactionSetId>
<TransactionSetPurposeCode>00</TransactionSetPurposeCode>
<TransactionNbr>86720130313095125308527</TransactionNbr>
<TransactionDate>20130313</TransactionDate>
<ReportTypeCode>DD</ReportTypeCode>
<ActionCode>ActionCodeValue</ActionCode>
<DocumentDueDate>20130313095125</DocumentDueDate>
<EsiId>7777777778EsiId</EsiId>
<MembershipId>7777777778MemId</MembershipId>
<OriginalTransactionNbr>OrigTransNum</OriginalTransactionNbr>
<TdspDuns>007909427AC</TdspDuns>
<TdspName>PPL</TdspName>
<CrDuns>962954579</CrDuns>
<CrName>STREAM ENERGY</CrName>
<CrIdentifier>CrIdent</CrIdentifier>
<UtilityAccountNumber>7777777778UtiliAcctNum</UtilityAccountNumber>
<PreviousUtilityAccountNumber>PrevUtilAcctNum</PreviousUtilityAccountNumber>
<BillCalculator>LDC</BillCalculator>
<ESPAccountNumber>3001246090</ESPAccountNumber>
<TradingPartnerID>STEPA007909427USE</TradingPartnerID>
<DoorHangerFlag>DoorHangerFlagValue</DoorHangerFlag>
<ESNCount>99</ESNCount>
<EstimationDescription>EstDesc</EstimationDescription>
<EstimationReason>EstReason</EstimationReason>
<InvoiceNbr>InvoiceNum</InvoiceNbr>
<NextMeterReadDate>20130130</NextMeterReadDate>
<PowerRegion>PowerReg</PowerRegion>
<ReferenceNbr>RefNum</ReferenceNbr>
<TransactionSetControlNbr>TransControlNbr</TransactionSetControlNbr>
<UtilityContractID>UtilContract</UtilityContractID>
<IntervalDetailLoop>
    <IntervalDetail>
        <IntervalDetail_Key>1</IntervalDetail_Key>
        <IntervalSummary_Key>2</IntervalSummary_Key>
        <Header_Key>3</Header_Key>
        <TypeCode>PM</TypeCode>
        <MeterNumber>428728887</MeterNumber>
        <ServicePeriodStart>20121108</ServicePeriodStart>
        <ServicePeriodEnd>20121210</ServicePeriodEnd>
        <ExchangeDate>20140125</ExchangeDate> 
        <ChannelNumber>ChanNum</ChannelNumber>
        <MeterRole>A</MeterRole> 
        <MeterUOM>KH</MeterUOM> 
        <MeterInterval>MON</MeterInterval> 
        <CommodityCode>ComCode</CommodityCode>
        <NumberOfDials>5.0</NumberOfDials> 
        <ServicePointId>ServPointId</ServicePointId>
        <UtilityRateServiceClass>OE-RSD</UtilityRateServiceClass> 
        <RateSubClass>CH</RateSubClass> 
        <IntervalDetailQtyLoop>
            <IntervalDetailQty>
                <IntervalDetail_Key>1</IntervalDetail_Key>
                <IntervalDetailQty_Key>2</IntervalDetailQty_Key>
                <Qualifier>QD</Qualifier>
                <Quantity>49.08</Quantity>
                <IntervalEndDate>20121210</IntervalEndDate>
                <IntervalEndTime>2200</IntervalEndTime>
                <RangeMin>RangeMinVal</RangeMin>
				<RangeMax>RangeMaxVal</RangeMax>
				<ThermFactor>ThermFactorVal</ThermFactor>
                <DegreeDayFactor>DegreeDayFactorVal</DegreeDayFactor>
            </IntervalDetailQty>
            <IntervalDetailQty>
                <Qualifier>QD</Qualifier>
                <Quantity>49.2</Quantity>
                <IntervalEndDate>20121210</IntervalEndDate>
                <IntervalEndTime>2215</IntervalEndTime>
            </IntervalDetailQty>
            <IntervalDetailQty>
            </IntervalDetailQty>
        </IntervalDetailQtyLoop>
    </IntervalDetail>
    <IntervalDetail>
    </IntervalDetail>
</IntervalDetailLoop>
</Header></Transaction></ns0:Market867>";

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var result = _concern.Parse(stream);

            // assert
            var header = result.Headers[0] as Type867Header;
            Assert.IsNotNull(header);

            //Check if count of Names is correct
            Assert.AreEqual(header.IntervalDetails.Length, 2);


            var intDetail = header.IntervalDetails[0];
            Assert.IsNotNull(intDetail);

            intDetail.IntervalDetailKey.ShouldEqual(1);
            intDetail.IntervalSummaryKey.ShouldEqual(2);
            intDetail.HeaderKey.ShouldEqual(3);
            intDetail.TypeCode.ShouldEqual("PM");
            intDetail.MeterNumber.ShouldEqual("428728887");
            intDetail.ServicePeriodStart.ShouldEqual("20121108");
            intDetail.ServicePeriodEnd.ShouldEqual("20121210");
            intDetail.ExchangeDate.ShouldEqual("20140125");
            intDetail.ChannelNumber.ShouldEqual("ChanNum");
            intDetail.MeterUOM.ShouldEqual("KH");
            intDetail.MeterInterval.ShouldEqual("MON");
            intDetail.MeterRole.ShouldEqual("A");
            intDetail.CommodityCode.ShouldEqual("ComCode");
            intDetail.NumberOfDials.ShouldEqual("5.0");
            intDetail.ServicePointId.ShouldEqual("ServPointId");
            intDetail.UtilityRateServiceClass.ShouldEqual("OE-RSD");
            intDetail.RateSubClass.ShouldEqual("CH");

            Assert.AreEqual(intDetail.IntervalDetailQtys.Length, 3);

            var qty = intDetail.IntervalDetailQtys[0];
            Assert.IsNotNull(qty);

            //Fields set in XML Parser
            qty.IntervalDetailKey.ShouldEqual(1);
            qty.IntervalDetailQtyKey.ShouldEqual(2);
            qty.DegreeDayFactor.ShouldEqual("DegreeDayFactorVal");
            qty.IntervalEndDate.ShouldEqual("20121210");
            qty.IntervalEndTime.ShouldEqual("2200");
            qty.ProcessFlag.ShouldEqual((short)0);
            qty.ProcessDate.ToShortDateString().ShouldEqual(DateTime.Now.ToShortDateString());
            qty.Qualifier.ShouldEqual("QD");
            qty.Quantity.ShouldEqual("49.08");
            qty.RangeMin.ShouldEqual("RangeMinVal");
            qty.RangeMax.ShouldEqual("RangeMaxVal");
            qty.ThermFactor.ShouldEqual("ThermFactorVal");
            
            

            var qtySecond = intDetail.IntervalDetailQtys[1];
            Assert.IsNotNull(qtySecond);

            qtySecond.IntervalDetailKey.ShouldEqual(0);
            qtySecond.IntervalDetailQtyKey.ShouldEqual(0);

            var intDetailSecond = header.IntervalDetails[1];
            Assert.IsNotNull(intDetailSecond);

            intDetailSecond.IntervalDetailKey.ShouldEqual(0);
            intDetailSecond.IntervalSummaryKey.ShouldEqual(0);
            intDetailSecond.HeaderKey.ShouldEqual(0);

        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingIntervalDetail_ShouldParse_EvenIfNodeIsEmpty() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<root></root>";

            // act

            var element = XElement.Parse(xml);
            var actual = _concern.ParseIntervalDetail(element, _namespaces);

            // assert
            actual.ShouldNotBeNull();
        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingIntervalSummary_ShouldReturnObjectWithValidValues_WhenXmlIsValid()// ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<ns0:Market867 xmlns:ns0=""http://CIS.Integration.Schema.Market.Common.Market820"">
<TransactionCount>1</TransactionCount>  
<LdcDuns>007909427AC</LdcDuns>
<LdcName>PPL</LdcName>
<CspDuns>962954579</CspDuns>
<CspName>STREAM ENERGY</CspName>
<FunctionalGroup>PT</FunctionalGroup>
<Transaction>
<Header>
<TransactionSetId>867</TransactionSetId>
<TransactionSetPurposeCode>00</TransactionSetPurposeCode>
<TransactionNbr>86720130313095125308527</TransactionNbr>
<TransactionDate>20130313</TransactionDate>
<ReportTypeCode>DD</ReportTypeCode>
<ActionCode>ActionCodeValue</ActionCode>
<DocumentDueDate>20130313095125</DocumentDueDate>
<EsiId>7777777778EsiId</EsiId>
<MembershipId>7777777778MemId</MembershipId>
<OriginalTransactionNbr>OrigTransNum</OriginalTransactionNbr>
<TdspDuns>007909427AC</TdspDuns>
<TdspName>PPL</TdspName>
<CrDuns>962954579</CrDuns>
<CrName>STREAM ENERGY</CrName>
<CrIdentifier>CrIdent</CrIdentifier>
<UtilityAccountNumber>7777777778UtiliAcctNum</UtilityAccountNumber>
<PreviousUtilityAccountNumber>PrevUtilAcctNum</PreviousUtilityAccountNumber>
<BillCalculator>LDC</BillCalculator>
<ESPAccountNumber>3001246090</ESPAccountNumber>
<TradingPartnerID>STEPA007909427USE</TradingPartnerID>
<DoorHangerFlag>DoorHangerFlagValue</DoorHangerFlag>
<ESNCount>99</ESNCount>
<EstimationDescription>EstDesc</EstimationDescription>
<EstimationReason>EstReason</EstimationReason>
<InvoiceNbr>InvoiceNum</InvoiceNbr>
<NextMeterReadDate>20130130</NextMeterReadDate>
<PowerRegion>PowerReg</PowerRegion>
<ReferenceNbr>RefNum</ReferenceNbr>
<TransactionSetControlNbr>TransControlNbr</TransactionSetControlNbr>
<UtilityContractID>UtilContract</UtilityContractID>
<IntervalSummaryLoop>
    <IntervalSummary>
        <TypeCode>BO</TypeCode>
        <MeterNumber>428728887</MeterNumber>
        <ServicePeriodStart>20121108</ServicePeriodStart>
		<ServicePeriodEnd>20121210</ServicePeriodEnd>
        <MovementTypeCode>MovTypeCode</MovementTypeCode>
        <MeterUOM>KH</MeterUOM>
        <MeterInterval>015</MeterInterval>
        <ExchangeDate>20140101</ExchangeDate>
        <ChannelNumber>ChannNum</ChannelNumber>
        <MeterRole>A</MeterRole>
        <CommodityCode>CommCode</CommodityCode>
        <NumberOfDials>6.00</NumberOfDials>
        <ServicePointId>ServPointId</ServicePointId>
        <UtilityRateServiceClass>OE-RSD</UtilityRateServiceClass> 
        <RateSubClass>CH</RateSubClass> 
        <IntervalSummaryQtyLoop>
            <IntervalSummaryQty>
				<Qualifier>QD</Qualifier>
				<Quantity>145200.0</Quantity>
				<MeasurementCode>AA</MeasurementCode>
				<CompositeUOM>KHM</CompositeUOM>
				<UOM>KH</UOM>
				<BeginRead>13398</BeginRead>
				<EndRead>13882</EndRead>
				<MeasurementSignificanceCode>51</MeasurementSignificanceCode>
				<MeterMultiplier>300</MeterMultiplier>
				<TransformerLossFactor>TranLossFac</TransformerLossFactor>
				<PowerFactor>2200</PowerFactor>
				<RangeMin>RangeMinVal</RangeMin>
				<RangeMax>RangeMaxVal</RangeMax>
				<ThermFactor>ThermFactorVal</ThermFactor>
				<DegreeDayFactor>45.6</DegreeDayFactor>
            </IntervalSummaryQty>
			<IntervalSummaryQty />
        </IntervalSummaryQtyLoop>
    </IntervalSummary>
    <IntervalSummary>
    </IntervalSummary>
</IntervalSummaryLoop>
</Header></Transaction></ns0:Market867>";

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var result = _concern.Parse(stream);

            // assert
            var header = result.Headers[0] as Type867Header;
            Assert.IsNotNull(header);

            //Check if count of Names is correct
            Assert.AreEqual(header.IntervalSummaries.Length, 2);


            var intSummary = header.IntervalSummaries[0];
            Assert.IsNotNull(intSummary);

            intSummary.IntervalSummaryKey.ShouldEqual(0);
            intSummary.HeaderKey.ShouldEqual(0);
            intSummary.TypeCode.ShouldEqual("BO");
            intSummary.MeterNumber.ShouldEqual("428728887");
            intSummary.MovementTypeCode.ShouldEqual("MovTypeCode");
            intSummary.ServicePeriodStart.ShouldEqual("20121108");
            intSummary.ServicePeriodEnd.ShouldEqual("20121210");
            intSummary.ExchangeDate.ShouldEqual("20140101");
            intSummary.ChannelNumber.ShouldEqual("ChannNum");
            intSummary.MeterRole.ShouldEqual("A");
            intSummary.MeterUOM.ShouldEqual("KH");
            intSummary.MeterInterval.ShouldEqual("015");
            intSummary.CommodityCode.ShouldEqual("CommCode");
            intSummary.NumberOfDials.ShouldEqual("6.00");
            intSummary.ServicePointId.ShouldEqual("ServPointId");
            intSummary.UtilityRateServiceClass.ShouldEqual("OE-RSD");
            intSummary.RateSubClass.ShouldEqual("CH");

            Assert.AreEqual(intSummary.IntervalSummaryQtys.Length, 2);

            var qty = intSummary.IntervalSummaryQtys[0];
            Assert.IsNotNull(qty);

            //Fields set in XML Parser
            qty.Qualifier.ShouldEqual("QD");
            qty.Quantity.ShouldEqual("145200.0");
            qty.MeasurementCode.ShouldEqual("AA");
            qty.CompositeUom.ShouldEqual("KHM");
            qty.Uom.ShouldEqual("KH");
            qty.BeginRead.ShouldEqual("13398");
            qty.EndRead.ShouldEqual("13882");
            qty.MeasurementSignificanceCode.ShouldEqual("51");
            qty.MeterMultiplier.ShouldEqual("300");
            qty.TransformerLossFactor.ShouldEqual("TranLossFac");
            qty.PowerFactor.ShouldEqual("2200");
            qty.RangeMin.ShouldEqual("RangeMinVal");
            qty.RangeMax.ShouldEqual("RangeMaxVal");
            qty.ThermFactor.ShouldEqual("ThermFactorVal");
            qty.DegreeDayFactor.ShouldEqual("45.6");

            //Fields not set in parse
            qty.IntervalSummaryKey.ShouldEqual(0);
            qty.IntervalSummaryQtyKey.ShouldEqual(0);
            qty.ProcessDate.ShouldEqual(DateTime.MinValue);
            qty.ProcessFlag.ShouldEqual((short)0);



            var qtySecond = intSummary.IntervalSummaryQtys[1];
            Assert.IsNotNull(qtySecond);

            var intSummarySecond = header.IntervalSummaries[1];
            Assert.IsNotNull(intSummarySecond);

            //intDetailSecond.IntervalDetailKey.ShouldEqual(0);
            //intDetailSecond.IntervalSummaryKey.ShouldEqual(0);
            //intDetailSecond.HeaderKey.ShouldEqual(0);

        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingIntervalSummary_ShouldParse_EvenIfNodeIsEmpty() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<root></root>";

            // act

            var element = XElement.Parse(xml);
            var actual = _concern.ParseIntervalSummary(element, _namespaces);

            // assert
            actual.ShouldNotBeNull();
        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingUnMeterDetail_ShouldReturnObjectWithValidValues_WhenXmlIsValid()// ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<ns0:Market867 xmlns:ns0=""http://CIS.Integration.Schema.Market.Common.Market820"">
<TransactionCount>1</TransactionCount>  
<LdcDuns>007909427AC</LdcDuns>
<LdcName>PPL</LdcName>
<CspDuns>962954579</CspDuns>
<CspName>STREAM ENERGY</CspName>
<FunctionalGroup>PT</FunctionalGroup>
<Transaction>
<Header>
<TransactionSetId>867</TransactionSetId>
<TransactionSetPurposeCode>00</TransactionSetPurposeCode>
<TransactionNbr>86720130313095125308527</TransactionNbr>
<TransactionDate>20130313</TransactionDate>
<ReportTypeCode>DD</ReportTypeCode>
<ActionCode>ActionCodeValue</ActionCode>
<DocumentDueDate>20130313095125</DocumentDueDate>
<EsiId>7777777778EsiId</EsiId>
<MembershipId>7777777778MemId</MembershipId>
<OriginalTransactionNbr>OrigTransNum</OriginalTransactionNbr>
<TdspDuns>007909427AC</TdspDuns>
<TdspName>PPL</TdspName>
<CrDuns>962954579</CrDuns>
<CrName>STREAM ENERGY</CrName>
<CrIdentifier>CrIdent</CrIdentifier>
<UtilityAccountNumber>7777777778UtiliAcctNum</UtilityAccountNumber>
<PreviousUtilityAccountNumber>PrevUtilAcctNum</PreviousUtilityAccountNumber>
<BillCalculator>LDC</BillCalculator>
<ESPAccountNumber>3001246090</ESPAccountNumber>
<TradingPartnerID>STEPA007909427USE</TradingPartnerID>
<DoorHangerFlag>DoorHangerFlagValue</DoorHangerFlag>
<ESNCount>99</ESNCount>
<EstimationDescription>EstDesc</EstimationDescription>
<EstimationReason>EstReason</EstimationReason>
<InvoiceNbr>InvoiceNum</InvoiceNbr>
<NextMeterReadDate>20130130</NextMeterReadDate>
<PowerRegion>PowerReg</PowerRegion>
<ReferenceNbr>RefNum</ReferenceNbr>
<TransactionSetControlNbr>TransControlNbr</TransactionSetControlNbr>
<UtilityContractID>UtilContract</UtilityContractID>
<UnmeterDetailLoop>
    <UnmeterDetail>
	    <TypeCode>XX</TypeCode>
	    <CommodityCode>CommCode</CommodityCode>
	    <ServicePeriodStart>20130130</ServicePeriodStart>
	    <ServicePeriodEnd>20130227</ServicePeriodEnd>
	    <ServiceType>ServType</ServiceType>
	    <Description>711Desc</Description>
	    <UtilityRateServiceClass>711</UtilityRateServiceClass>
	    <RateSubClass>RatSubClass</RateSubClass>
	    <LoadProfile>RS02</LoadProfile>
	    <UnmeterDetailQtyLoop>
		    <UnmeterDetailQty>
			    <Qualifier>QD</Qualifier>
			    <Quantity>729.0</Quantity>
			    <CompositeUOM>KHM</CompositeUOM>
			    <UOM>KH</UOM>
			    <NumberOfDevices>10</NumberOfDevices>
			    <ConsumptionPerDevice>12</ConsumptionPerDevice>
			    <BackoutCredit>34.56</BackoutCredit>
		    </UnmeterDetailQty>
		    <UnmeterDetailQty />
	    </UnmeterDetailQtyLoop>
    </UnmeterDetail>
    <UnmeterDetail />
</UnmeterDetailLoop>
</Header></Transaction></ns0:Market867>";

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var result = _concern.Parse(stream);

            // assert
            var header = result.Headers[0] as Type867Header;
            Assert.IsNotNull(header);

            //Check if count of Names is correct
            Assert.AreEqual(header.UnMeterDetails.Length, 2);


            var unMeterSummary = header.UnMeterDetails[0];
            Assert.IsNotNull(unMeterSummary);

            unMeterSummary.TypeCode.ShouldEqual("XX");
            unMeterSummary.CommodityCode.ShouldEqual("CommCode");
            unMeterSummary.ServicePeriodStart.ShouldEqual("20130130");
            unMeterSummary.ServicePeriodEnd.ShouldEqual("20130227");
            unMeterSummary.ServiceType.ShouldEqual("ServType");
            unMeterSummary.Description.ShouldEqual("711Desc");
            unMeterSummary.UtilityRateServiceClass.ShouldEqual("711");
            unMeterSummary.RateSubClass.ShouldEqual("RatSubClass");

            //Fields not set in parse
            unMeterSummary.UnMeterDetailKey.ShouldEqual(0);
            unMeterSummary.HeaderKey.ShouldEqual(0);
            unMeterSummary.LoadProfile.ShouldBeNull();
            

            Assert.AreEqual(unMeterSummary.UnMeterDetailQtys.Length, 2);

            var qty = unMeterSummary.UnMeterDetailQtys[0];
            Assert.IsNotNull(qty);

            //Fields set in XML Parser
            qty.Qualifier.ShouldEqual("QD");
            qty.Quantity.ShouldEqual("729.0");
            qty.CompositeUom.ShouldEqual("KHM");
            qty.Uom.ShouldEqual("KH");
            qty.NumberOfDevices.ShouldEqual("10");
            qty.ConsumptionPerDevice.ShouldEqual("12");
            qty.BackoutCredit.ShouldEqual("34.56");
            

            //Fields not set in parse
            qty.UnMeterDetailKey .ShouldEqual(0);
            qty.UnMeterDetailQtyKey.ShouldEqual(0);
            qty.ProcessDate.ShouldEqual(DateTime.MinValue);
            qty.ProcessFlag.ShouldEqual((short)0);



            var qtySecond = unMeterSummary.UnMeterDetailQtys[1];
            Assert.IsNotNull(qtySecond);

            var unMeterDetailSecond = header.UnMeterDetails[1];
            Assert.IsNotNull(unMeterDetailSecond);

            //intDetailSecond.IntervalDetailKey.ShouldEqual(0);
            //intDetailSecond.IntervalSummaryKey.ShouldEqual(0);
            //intDetailSecond.HeaderKey.ShouldEqual(0);

        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingUnMeterDetail_ShouldParse_EvenIfNodeIsEmpty() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<root></root>";

            // act

            var element = XElement.Parse(xml);
            var actual = _concern.ParseUnMeterDetail(element, _namespaces);

            // assert
            actual.ShouldNotBeNull();
        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingUnMeterSummary_ShouldReturnObjectWithValidValues_WhenXmlIsValid()// ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<ns0:Market867 xmlns:ns0=""http://CIS.Integration.Schema.Market.Common.Market820"">
<TransactionCount>1</TransactionCount>  
<LdcDuns>007909427AC</LdcDuns>
<LdcName>PPL</LdcName>
<CspDuns>962954579</CspDuns>
<CspName>STREAM ENERGY</CspName>
<FunctionalGroup>PT</FunctionalGroup>
<Transaction>
<Header>
<TransactionSetId>867</TransactionSetId>
<TransactionSetPurposeCode>00</TransactionSetPurposeCode>
<TransactionNbr>86720130313095125308527</TransactionNbr>
<TransactionDate>20130313</TransactionDate>
<ReportTypeCode>DD</ReportTypeCode>
<ActionCode>ActionCodeValue</ActionCode>
<DocumentDueDate>20130313095125</DocumentDueDate>
<EsiId>7777777778EsiId</EsiId>
<MembershipId>7777777778MemId</MembershipId>
<OriginalTransactionNbr>OrigTransNum</OriginalTransactionNbr>
<TdspDuns>007909427AC</TdspDuns>
<TdspName>PPL</TdspName>
<CrDuns>962954579</CrDuns>
<CrName>STREAM ENERGY</CrName>
<CrIdentifier>CrIdent</CrIdentifier>
<UtilityAccountNumber>7777777778UtiliAcctNum</UtilityAccountNumber>
<PreviousUtilityAccountNumber>PrevUtilAcctNum</PreviousUtilityAccountNumber>
<BillCalculator>LDC</BillCalculator>
<ESPAccountNumber>3001246090</ESPAccountNumber>
<TradingPartnerID>STEPA007909427USE</TradingPartnerID>
<DoorHangerFlag>DoorHangerFlagValue</DoorHangerFlag>
<ESNCount>99</ESNCount>
<EstimationDescription>EstDesc</EstimationDescription>
<EstimationReason>EstReason</EstimationReason>
<InvoiceNbr>InvoiceNum</InvoiceNbr>
<NextMeterReadDate>20130130</NextMeterReadDate>
<PowerRegion>PowerReg</PowerRegion>
<ReferenceNbr>RefNum</ReferenceNbr>
<TransactionSetControlNbr>TransControlNbr</TransactionSetControlNbr>
<UtilityContractID>UtilContract</UtilityContractID>
<UnmeterSummaryLoop>
<UnmeterSummary>
	<TypeCode>XX</TypeCode>
	<CommodityCode>CommCode</CommodityCode>
	<MeterUOM>KH</MeterUOM>
	<UtilityRateServiceClass>711</UtilityRateServiceClass>
	<RateSubClass>RatSubClass</RateSubClass>
	<UnmeterSummaryQtyLoop>
		<UnmeterSummaryQty>
			<Qualifier>QD</Qualifier>
			<Quantity>729.0</Quantity>
			<MeasurementSignificanceCode>GHY</MeasurementSignificanceCode>
			<ServicePeriodStart>20130210</ServicePeriodStart>
			<ServicePeriodEnd>20130910</ServicePeriodEnd>
		</UnmeterSummaryQty>
		<UnmeterSummaryQty />
	</UnmeterSummaryQtyLoop>
</UnmeterSummary>
<UnmeterSummary />
</UnmeterSummaryLoop>
</Header></Transaction></ns0:Market867>";

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var result = _concern.Parse(stream);

            // assert
            var header = result.Headers[0] as Type867Header;
            Assert.IsNotNull(header);

            //Check if count of Names is correct
            Assert.AreEqual(header.UnMeterSummaries.Length, 2);


            var unMeterSummary = header.UnMeterSummaries[0];
            Assert.IsNotNull(unMeterSummary);

            unMeterSummary.TypeCode.ShouldEqual("XX");
            unMeterSummary.CommodityCode.ShouldEqual("CommCode");
            unMeterSummary.MeterUom.ShouldEqual("KH");
            unMeterSummary.UtilityRateServiceClass.ShouldEqual("711");
            unMeterSummary.RateSubClass.ShouldEqual("RatSubClass");

            //Fields not set in parse
            unMeterSummary.UnMeterSummaryKey.ShouldEqual(0);
            unMeterSummary.HeaderKey.ShouldEqual(0);
            

            Assert.AreEqual(unMeterSummary.UnMeterSummaryQtys.Length, 2);

            var qty = unMeterSummary.UnMeterSummaryQtys[0];
            Assert.IsNotNull(qty);

            //Fields set in XML Parser
            qty.Qualifier.ShouldEqual("QD");
            qty.Quantity.ShouldEqual("729.0");
            qty.MeasurementSignificanceCode.ShouldEqual("GHY");
            qty.ServicePeriodStart.ShouldEqual("20130210");
            qty.ServicePeriodEnd.ShouldEqual("20130910");
            


            //Fields not set in parse
            qty.UnMeterSummaryKey.ShouldEqual(0);
            qty.UnMeterSummaryQtyKey.ShouldEqual(0);




            var qtySecond = unMeterSummary.UnMeterSummaryQtys[1];
            Assert.IsNotNull(qtySecond);

            var unMeterSummarySecond = header.UnMeterSummaries[1];
            Assert.IsNotNull(unMeterSummarySecond);

            unMeterSummarySecond.UnMeterSummaryQtys.Length.ShouldEqual(0);

        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingUnMeterSummary_ShouldParse_EvenIfNodeIsEmpty() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<root></root>";

            // act

            var element = XElement.Parse(xml);
            var actual = _concern.ParseUnMeterSummary(element, _namespaces);

            // assert
            actual.ShouldNotBeNull();
        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingScheduledDeterminants_ShouldReturnObjectWithValidValues_WhenXmlIsValid()// ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<ns0:Market867 xmlns:ns0=""http://CIS.Integration.Schema.Market.Common.Market820"">
<TransactionCount>1</TransactionCount>  
<LdcDuns>007909427AC</LdcDuns>
<LdcName>PPL</LdcName>
<CspDuns>962954579</CspDuns>
<CspName>STREAM ENERGY</CspName>
<FunctionalGroup>PT</FunctionalGroup>
<Transaction>
<Header>
<TransactionSetId>867</TransactionSetId>
<TransactionSetPurposeCode>00</TransactionSetPurposeCode>
<TransactionNbr>86720130313095125308527</TransactionNbr>
<TransactionDate>20130313</TransactionDate>
<ReportTypeCode>DD</ReportTypeCode>
<ActionCode>ActionCodeValue</ActionCode>
<DocumentDueDate>20130313095125</DocumentDueDate>
<EsiId>7777777778EsiId</EsiId>
<MembershipId>7777777778MemId</MembershipId>
<OriginalTransactionNbr>OrigTransNum</OriginalTransactionNbr>
<TdspDuns>007909427AC</TdspDuns>
<TdspName>PPL</TdspName>
<CrDuns>962954579</CrDuns>
<CrName>STREAM ENERGY</CrName>
<CrIdentifier>CrIdent</CrIdentifier>
<UtilityAccountNumber>7777777778UtiliAcctNum</UtilityAccountNumber>
<PreviousUtilityAccountNumber>PrevUtilAcctNum</PreviousUtilityAccountNumber>
<BillCalculator>LDC</BillCalculator>
<ESPAccountNumber>3001246090</ESPAccountNumber>
<TradingPartnerID>STEPA007909427USE</TradingPartnerID>
<DoorHangerFlag>DoorHangerFlagValue</DoorHangerFlag>
<ESNCount>99</ESNCount>
<EstimationDescription>EstDesc</EstimationDescription>
<EstimationReason>EstReason</EstimationReason>
<InvoiceNbr>InvoiceNum</InvoiceNbr>
<NextMeterReadDate>20130130</NextMeterReadDate>
<PowerRegion>PowerReg</PowerRegion>
<ReferenceNbr>RefNum</ReferenceNbr>
<TransactionSetControlNbr>TransControlNbr</TransactionSetControlNbr>
<UtilityContractID>UtilContract</UtilityContractID>
<ScheduleDeterminantsLoop>
<ScheduleDeterminants>
	<CapacityObligation>2.434</CapacityObligation>
	<TransmissionObligation>3.4</TransmissionObligation>
	<LoadProfile>LoadProfVal</LoadProfile>
	<LDCRateClass>111</LDCRateClass>
	<Zone>ZoneVal</Zone>
	<BillCycle>01</BillCycle>
	<MeterNumber>MeterNumberVal</MeterNumber>
	<EffectiveDate>EffectiveDateVal</EffectiveDate>
	<LossFactor>LossFactorVal</LossFactor>
	<ServiceVoltage>ServiceVoltageVal</ServiceVoltage>
	<SpecialMeterConfig>SpecialMeterConfigVal</SpecialMeterConfig>
	<MaximumGeneration>MxGenVal</MaximumGeneration>
    <LDCRateSubClass>UR0</LDCRateSubClass>
</ScheduleDeterminants>
<ScheduleDeterminants />
</ScheduleDeterminantsLoop>
</Header></Transaction></ns0:Market867>";

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var result = _concern.Parse(stream);

            // assert
            var header = result.Headers[0] as Type867Header;
            Assert.IsNotNull(header);

            //Check if count of Names is correct
            Assert.AreEqual(header.ScheduleDeterminants.Length, 2);


            var schDeterminant = header.ScheduleDeterminants[0];
            Assert.IsNotNull(schDeterminant);

            schDeterminant.CapacityObligation.ShouldEqual("2.434");
            schDeterminant.TransmissionObligation.ShouldEqual("3.4");
            schDeterminant.LoadProfile.ShouldEqual("LoadProfVal");
            schDeterminant.LDCRateClass.ShouldEqual("111");
            schDeterminant.Zone.ShouldEqual("ZoneVal");
            schDeterminant.BillCycle.ShouldEqual("01");
            schDeterminant.MeterNumber.ShouldEqual("MeterNumberVal");
            schDeterminant.EffectiveDate.ShouldEqual("EffectiveDateVal");
            schDeterminant.LossFactor.ShouldEqual("LossFactorVal");
            schDeterminant.ServiceVoltage.ShouldEqual("ServiceVoltageVal");
            schDeterminant.SpecialMeterConfig.ShouldEqual("SpecialMeterConfigVal");
            schDeterminant.MaximumGeneration.ShouldEqual("MxGenVal");
            schDeterminant.LDCRateSubClass.ShouldEqual("UR0");

            //Fields not set in parse
            schDeterminant.ScheduleDeterminantsKey.ShouldEqual(0);
            schDeterminant.HeaderKey.ShouldEqual(0);
            
           

            var schDeterminantSecond = header.ScheduleDeterminants[1];
            Assert.IsNotNull(schDeterminantSecond);

            

        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingScheduledDeterminants_ShouldParse_EvenIfNodeIsEmpty() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<root></root>";

            // act

            var element = XElement.Parse(xml);
            var actual = _concern.ParseScheduledDeterminant(element, _namespaces);

            // assert
            actual.ShouldNotBeNull();
        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingIntervalSummaryAcrossMeters_ShouldReturnObjectWithValidValues_WhenXmlIsValid()// ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<ns0:Market867 xmlns:ns0=""http://CIS.Integration.Schema.Market.Common.Market820"">
<TransactionCount>1</TransactionCount>  
<LdcDuns>007909427AC</LdcDuns>
<LdcName>PPL</LdcName>
<CspDuns>962954579</CspDuns>
<CspName>STREAM ENERGY</CspName>
<FunctionalGroup>PT</FunctionalGroup>
<Transaction>
<Header>
<TransactionSetId>867</TransactionSetId>
<TransactionSetPurposeCode>00</TransactionSetPurposeCode>
<TransactionNbr>86720130313095125308527</TransactionNbr>
<TransactionDate>20130313</TransactionDate>
<ReportTypeCode>DD</ReportTypeCode>
<ActionCode>ActionCodeValue</ActionCode>
<DocumentDueDate>20130313095125</DocumentDueDate>
<EsiId>7777777778EsiId</EsiId>
<MembershipId>7777777778MemId</MembershipId>
<OriginalTransactionNbr>OrigTransNum</OriginalTransactionNbr>
<TdspDuns>007909427AC</TdspDuns>
<TdspName>PPL</TdspName>
<CrDuns>962954579</CrDuns>
<CrName>STREAM ENERGY</CrName>
<CrIdentifier>CrIdent</CrIdentifier>
<UtilityAccountNumber>7777777778UtiliAcctNum</UtilityAccountNumber>
<PreviousUtilityAccountNumber>PrevUtilAcctNum</PreviousUtilityAccountNumber>
<BillCalculator>LDC</BillCalculator>
<ESPAccountNumber>3001246090</ESPAccountNumber>
<TradingPartnerID>STEPA007909427USE</TradingPartnerID>
<DoorHangerFlag>DoorHangerFlagValue</DoorHangerFlag>
<ESNCount>99</ESNCount>
<EstimationDescription>EstDesc</EstimationDescription>
<EstimationReason>EstReason</EstimationReason>
<InvoiceNbr>InvoiceNum</InvoiceNbr>
<NextMeterReadDate>20130130</NextMeterReadDate>
<PowerRegion>PowerReg</PowerRegion>
<ReferenceNbr>RefNum</ReferenceNbr>
<TransactionSetControlNbr>TransControlNbr</TransactionSetControlNbr>
<UtilityContractID>UtilContract</UtilityContractID>
<IntervalSummaryAcrossMetersLoop>
<IntervalSummaryAcrossMeters>
	<TypeCode>XD</TypeCode>
	<ServicePeriodStart>20130224</ServicePeriodStart>
	<ServicePeriodStartTime>1237</ServicePeriodStartTime>
	<ServicePeriodEnd>20130324</ServicePeriodEnd>
	<ServicePeriodEndTime>1238</ServicePeriodEndTime>
	<MeterRole>A</MeterRole>
	<MeterUOM>KH</MeterUOM>
	<MeterInterval>MON</MeterInterval>
	<IntervalSummaryAcrossMetersQtyLoop>
		<IntervalSummaryAcrossMetersQty>
			<Qualifier>QD</Qualifier>
			<Quantity>12</Quantity>
			<IntervalEndDate>20130223</IntervalEndDate>
			<IntervalEndTime>1236</IntervalEndTime>
		</IntervalSummaryAcrossMetersQty>
		<IntervalSummaryAcrossMetersQty />
	</IntervalSummaryAcrossMetersQtyLoop>
</IntervalSummaryAcrossMeters>
<IntervalSummaryAcrossMeters />
</IntervalSummaryAcrossMetersLoop>
</Header></Transaction></ns0:Market867>";

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var result = _concern.Parse(stream);

            // assert
            var header = result.Headers[0] as Type867Header;
            Assert.IsNotNull(header);

            //Check if count of Names is correct
            Assert.AreEqual(header.IntervalSummaryAcrossMeters.Length, 2);


            var intSumAcrossMeters = header.IntervalSummaryAcrossMeters[0];
            Assert.IsNotNull(intSumAcrossMeters);

            intSumAcrossMeters.TypeCode.ShouldEqual("XD");
            intSumAcrossMeters.ServicePeriodStart.ShouldEqual("20130224");
            intSumAcrossMeters.ServicePeriodStartTime.ShouldEqual("1237");
            intSumAcrossMeters.ServicePeriodEnd.ShouldEqual("20130324");
            intSumAcrossMeters.ServicePeriodEndTime.ShouldEqual("1238");
            intSumAcrossMeters.MeterRole.ShouldEqual("A");
            intSumAcrossMeters.MeterUOM.ShouldEqual("KH");
            intSumAcrossMeters.MeterInterval.ShouldEqual("MON");


            //Fields not set in parse
            intSumAcrossMeters.IntervalSummaryAcrossMetersKey.ShouldEqual(0);
            intSumAcrossMeters.HeaderKey.ShouldEqual(0);


            Assert.AreEqual(intSumAcrossMeters.IntervalSummaryAcrossMetersQtys.Length, 2);

            var qty = intSumAcrossMeters.IntervalSummaryAcrossMetersQtys[0];
            Assert.IsNotNull(qty);

            //Fields set in XML Parser
            qty.Qualifier.ShouldEqual("QD");
            qty.Quantity.ShouldEqual("12");
            qty.IntervalEndDate.ShouldEqual("20130223");
            qty.IntervalEndTime.ShouldEqual("1236");



            //Fields not set in parse
            qty.IntervalSummaryAcrossMetersKey.ShouldEqual(0);
            qty.IntervalSummaryAcrossMetersQtyKey.ShouldEqual(0);




            var qtySecond = intSumAcrossMeters.IntervalSummaryAcrossMetersQtys[1];
            Assert.IsNotNull(qtySecond);

            var intSumAcrossMetersSecond = header.IntervalSummaryAcrossMeters[1];
            Assert.IsNotNull(intSumAcrossMetersSecond);

            intSumAcrossMetersSecond.IntervalSummaryAcrossMetersQtys.Length.ShouldEqual(0);

        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingIntervalSummaryAcrossMeters_ShouldParse_EvenIfNodeIsEmpty() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<root></root>";

            // act

            var element = XElement.Parse(xml);
            var actual = _concern.ParseIntervalSummaryAcrossMeters(element, _namespaces);

            // assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingGasProfileFactorEvaluations_ShouldReturnObjectWithValidValues_WhenXmlIsValid()// ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<ns0:Market867 xmlns:ns0=""http://CIS.Integration.Schema.Market.Common.Market820"">
<TransactionCount>1</TransactionCount>  
<LdcDuns>007909427AC</LdcDuns>
<LdcName>PPL</LdcName>
<CspDuns>962954579</CspDuns>
<CspName>STREAM ENERGY</CspName>
<FunctionalGroup>PT</FunctionalGroup>
<Transaction>
<Header>
<TransactionSetId>867</TransactionSetId>
<TransactionSetPurposeCode>00</TransactionSetPurposeCode>
<TransactionNbr>86720130313095125308527</TransactionNbr>
<TransactionDate>20130313</TransactionDate>
<ReportTypeCode>DD</ReportTypeCode>
<ActionCode>ActionCodeValue</ActionCode>
<DocumentDueDate>20130313095125</DocumentDueDate>
<EsiId>7777777778EsiId</EsiId>
<MembershipId>7777777778MemId</MembershipId>
<OriginalTransactionNbr>OrigTransNum</OriginalTransactionNbr>
<TdspDuns>007909427AC</TdspDuns>
<TdspName>PPL</TdspName>
<CrDuns>962954579</CrDuns>
<CrName>STREAM ENERGY</CrName>
<CrIdentifier>CrIdent</CrIdentifier>
<UtilityAccountNumber>7777777778UtiliAcctNum</UtilityAccountNumber>
<PreviousUtilityAccountNumber>PrevUtilAcctNum</PreviousUtilityAccountNumber>
<BillCalculator>LDC</BillCalculator>
<ESPAccountNumber>3001246090</ESPAccountNumber>
<TradingPartnerID>STEPA007909427USE</TradingPartnerID>
<DoorHangerFlag>DoorHangerFlagValue</DoorHangerFlag>
<ESNCount>99</ESNCount>
<EstimationDescription>EstDesc</EstimationDescription>
<EstimationReason>EstReason</EstimationReason>
<InvoiceNbr>InvoiceNum</InvoiceNbr>
<NextMeterReadDate>20130130</NextMeterReadDate>
<PowerRegion>PowerReg</PowerRegion>
<ReferenceNbr>RefNum</ReferenceNbr>
<TransactionSetControlNbr>TransControlNbr</TransactionSetControlNbr>
<UtilityContractID>UtilContract</UtilityContractID>
<GasProfileFactorEvaluationLoop>
<GasProfileFactorEvaluation>
	<ProfilePeriodStartDate>20140205</ProfilePeriodStartDate>
	<CustomerServiceInitdate>20130224</CustomerServiceInitdate>
	<UtilityRateServiceClass>1237</UtilityRateServiceClass>
	<RateSubClass>DHNC</RateSubClass>
	<NonHeatLoadFactorQty>1238</NonHeatLoadFactorQty>
	<WeatherNormLoadFactorQty>12</WeatherNormLoadFactorQty>
	<LoadFactorRatio>5.6</LoadFactorRatio>
	<UFGRatePct>45</UFGRatePct>
	<MaximumDeliveryQty>2</MaximumDeliveryQty>
</GasProfileFactorEvaluation>
<GasProfileFactorEvaluation />
</GasProfileFactorEvaluationLoop>
</Header></Transaction></ns0:Market867>";

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var result = _concern.Parse(stream);

            // assert
            var header = result.Headers[0] as Type867Header;
            Assert.IsNotNull(header);

            //Check if count of Names is correct
            Assert.AreEqual(header.GasProfileFactorEvaluations.Length, 2);


            var gasProfEval = header.GasProfileFactorEvaluations[0];
            Assert.IsNotNull(gasProfEval);

            gasProfEval.ProfilePeriodStartDate.ShouldEqual("20140205");
            gasProfEval.CustomerServiceInitDate.ShouldEqual("20130224");
            gasProfEval.UtilityRateServiceClass.ShouldEqual("1237");
            gasProfEval.RateSubClass.ShouldEqual("DHNC");
            gasProfEval.NonHeatLoadFactorQty.ShouldEqual("1238");
            gasProfEval.WeatherNormLoadFactorQty.ShouldEqual("12");
            gasProfEval.LoadFactorRatio.ShouldEqual("5.6");
            gasProfEval.UFGRatePct.ShouldEqual("45");
            gasProfEval.MaximumDeliveryQty.ShouldEqual("2");
           
            //Fields not set in parse
            gasProfEval.GasProfileFactorEvaluationKey.ShouldEqual(0);
            gasProfEval.HeaderKey.ShouldEqual(0);



            var gasProfEvalSecond = header.GasProfileFactorEvaluations[1];
            Assert.IsNotNull(gasProfEvalSecond);



        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingGasProfileFactorEvaluations_ShouldParse_EvenIfNodeIsEmpty() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<root></root>";

            // act

            var element = XElement.Parse(xml);
            var actual = _concern.ParseGasProfileFactorEvaluation(element, _namespaces);

            // assert
            actual.ShouldNotBeNull();
        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingGasProfileFactorSamples_ShouldReturnObjectWithValidValues_WhenXmlIsValid()// ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<ns0:Market867 xmlns:ns0=""http://CIS.Integration.Schema.Market.Common.Market820"">
<TransactionCount>1</TransactionCount>  
<LdcDuns>007909427AC</LdcDuns>
<LdcName>PPL</LdcName>
<CspDuns>962954579</CspDuns>
<CspName>STREAM ENERGY</CspName>
<FunctionalGroup>PT</FunctionalGroup>
<Transaction>
<Header>
<TransactionSetId>867</TransactionSetId>
<TransactionSetPurposeCode>00</TransactionSetPurposeCode>
<TransactionNbr>86720130313095125308527</TransactionNbr>
<TransactionDate>20130313</TransactionDate>
<ReportTypeCode>DD</ReportTypeCode>
<ActionCode>ActionCodSampleue</ActionCode>
<DocumentDueDate>20130313095125</DocumentDueDate>
<EsiId>7777777778EsiId</EsiId>
<MembershipId>7777777778MemId</MembershipId>
<OriginalTransactionNbr>OrigTransNum</OriginalTransactionNbr>
<TdspDuns>007909427AC</TdspDuns>
<TdspName>PPL</TdspName>
<CrDuns>962954579</CrDuns>
<CrName>STREAM ENERGY</CrName>
<CrIdentifier>CrIdent</CrIdentifier>
<UtilityAccountNumber>7777777778UtiliAcctNum</UtilityAccountNumber>
<PreviousUtilityAccountNumber>PrevUtilAcctNum</PreviousUtilityAccountNumber>
<BillCalculator>LDC</BillCalculator>
<ESPAccountNumber>3001246090</ESPAccountNumber>
<TradingPartnerID>STEPA007909427USE</TradingPartnerID>
<DoorHangerFlag>DoorHangerFlagValue</DoorHangerFlag>
<ESNCount>99</ESNCount>
<EstimationDescription>EstDesc</EstimationDescription>
<EstimationReason>EstReason</EstimationReason>
<InvoiceNbr>InvoiceNum</InvoiceNbr>
<NextMeterReadDate>20130130</NextMeterReadDate>
<PowerRegion>PowerReg</PowerRegion>
<ReferenceNbr>RefNum</ReferenceNbr>
<TransactionSetControlNbr>TransControlNbr</TransactionSetControlNbr>
<UtilityContractID>UtilContract</UtilityContractID>
<GasProfileFactorSampleLoop>
<GasProfileFactorSample>
	<ReportMonth>11</ReportMonth>
	<AnnualPeriod>10</AnnualPeriod>
	<NormProjectedUsageQty>1</NormProjectedUsageQty>
	<WeatherNormUsageProjectedQty>2</WeatherNormUsageProjectedQty>
	<NormProjectedDeliveryQty>3</NormProjectedDeliveryQty>
	<WeatherNormProjectedDeliveryQty>4</WeatherNormProjectedDeliveryQty>
	<ProjectedDailyDeliveryQty>5</ProjectedDailyDeliveryQty>
	<DesignProjectedUsageQty>6</DesignProjectedUsageQty>
	<DesignProjectedDeliveryQty>7</DesignProjectedDeliveryQty>
	<ProjectedBalancingUseQty>8</ProjectedBalancingUseQty>
	<ProjectedSwingChargeAmt>9</ProjectedSwingChargeAmt>
</GasProfileFactorSample>
<GasProfileFactorSample />
</GasProfileFactorSampleLoop>
</Header></Transaction></ns0:Market867>";

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var result = _concern.Parse(stream);

            // assert
            var header = result.Headers[0] as Type867Header;
            Assert.IsNotNull(header);

            //Check if count of Names is correct
            Assert.AreEqual(header.GasProfileFactorSamples.Length, 2);


            var gasProfSample = header.GasProfileFactorSamples[0];
            Assert.IsNotNull(gasProfSample);

            gasProfSample.ReportMonth.ShouldEqual("11");
            gasProfSample.AnnualPeriod.ShouldEqual("10");
            gasProfSample.NormProjectedUsageQty.ShouldEqual("1");
            gasProfSample.WeatherNormUsageProjectedQty.ShouldEqual("2");
            gasProfSample.NormProjectedDeliveryQty.ShouldEqual("3");
            gasProfSample.WeatherNormProjectedDeliveryQty.ShouldEqual("4");
            gasProfSample.ProjectedDailyDeliveryQty.ShouldEqual("5");
            gasProfSample.DesignProjectedUsageQty.ShouldEqual("6");
            gasProfSample.DesignProjectedDeliveryQty.ShouldEqual("7");
            gasProfSample.ProjectedBalancingUseQty.ShouldEqual("8");
            gasProfSample.ProjectedSwingChargeAmt.ShouldEqual("9");

            //Fields not set in parse
            gasProfSample.GasProfileFactorSampleKey.ShouldEqual(0);
            gasProfSample.HeaderKey.ShouldEqual(0);



            var gasProfSampleSecond = header.GasProfileFactorSamples[1];
            Assert.IsNotNull(gasProfSampleSecond);



        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void ParsingGasProfileFactorSamples_ShouldParse_EvenIfNodeIsEmpty() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string xml = @"<root></root>";

            // act

            var element = XElement.Parse(xml);
            var actual = _concern.ParseGasProfileFactorSample(element, _namespaces);

            // assert
            actual.ShouldNotBeNull();
        }

    }
}

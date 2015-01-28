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
    public class Xml650Scenarios
    {
        private Dictionary<string, XNamespace> namespaces;
        private ILogger logger;
        
        private Import650Xml concern;
        
        [TestInitialize]
        public void SetUp()
        {
            namespaces = new Dictionary<string, XNamespace>();
            logger = MockRepository.GenerateStub<ILogger>();

            concern = new Import650Xml(logger);
        }

        [TestMethod]
        public void Sample_File()
        {
            // arrange
            const string xml = @"<ns0:Market650 xmlns:ns0=""http://CIS.Integration.Schema.Market.Common.Market650"">
	<TransactionCount>1</TransactionCount>
	<LdcDuns>AGLCPIPES</LdcDuns>
	<LdcName>Atlanta Gas Light</LdcName>
	<CspDuns>176343341GA</CspDuns>
	<CspName>Commerce Energy DBA Just Energy</CspName>
	<Transaction>
		<Header>
			<TransactionSetId>650</TransactionSetId>
			<TransactionSetPurposeCode>11</TransactionSetPurposeCode>
			<TransactionDate>20130118</TransactionDate>
			<TransactionNbr>RMCL456887309819700101</TransactionNbr>
			<ReferenceNbr/>
			<TransactionType>79</TransactionType>
			<ActionCode>51</ActionCode>
			<TdspName>Atlanta Gas Light</TdspName>
			<TdspDuns>AGLCPIPES</TdspDuns>
			<CrName>Commerce Energy DBA Just Energy</CrName>
			<CrDuns>176343341GA</CrDuns>
			<ProcessedReceivedDateTime>201301181039</ProcessedReceivedDateTime>
			<TradingPartnerID>COMGAAGLCPIPESSVC</TradingPartnerID>
			<NameLoop>
				<Name>
					<EntityName>JORGE                         HERNANDEZ</EntityName>
					<EntityName2/>
					<EntityName3/>
					<Address1>5723  EVERGLADES TRL</Address1>
					<Address2>LOT 8A</Address2>
					<City>NORCROSS</City>
					<State>GA</State>
					<PostalCode>30071</PostalCode>
					<ContactName/>
					<ContactPhoneNbr1>770 234-0849</ContactPhoneNbr1>
					<ContactPhoneNbr2/>
				</Name>
			</NameLoop>
			<ServiceLoop>
				<Service>
					<PurposeCode>RC001</PurposeCode>
					<PriorityCode/>
					<ESIID>4568873098</ESIID>
					<SpecialProcessCode/>
					<ServiceReqDate/>
					<NotBeforeDate/>
					<CallAhead/>
					<PremLocation/>
					<AccStatusCode/>
					<AccStatusDesc/>
					<EquipLocation>113278984</EquipLocation>
					<ServiceOrderNbr/>
					<CompletionDate/>
					<CompletionTime/>
					<ReportRemarks/>
					<Directions/>
					<MeterNbr/>
					<MeterReadDate/>
					<MeterTestDate/>
					<MeterTestResults/>
					<IncidentCode/>
					<EstRestoreDate/>
					<EstRestoreTime/>
					<IntStartDate/>
					<IntStartTime/>
					<RepairRecommended/>
					<Rescheduled/>
					<InterDurationPeriod/>
					<AreaOutage/>
					<CustRepairRemarks/>
					<MeterReadUOM/>
					<MeterRead/>
					<MeterReadCode/>
					<Membership>299906457</Membership>
					<ServiceChangeLoop/>
					<ServiceMeterLoop/>
					<ServicePoleLoop/>
					<ServiceRejectLoop/>
				</Service>
			</ServiceLoop>
		</Header>
	</Transaction>
</ns0:Market650>";

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var result = concern.Parse(stream);

            // assert
            result.TransactionActualCount.ShouldEqual(1);
            result.TransactionAuditCount.ShouldEqual(1);

            var header = result.Headers[0] as Type650Header;
            Assert.IsNotNull(header);
            header.TransactionSetId.ShouldEqual("650");
            header.TransactionSetPurposeCode.ShouldEqual("11");
            header.TransactionDate.ShouldEqual("20130118");
            header.TransactionNbr.ShouldEqual("RMCL456887309819700101");
            header.ReferenceNbr.ShouldBeEmpty();
            header.TransactionType.ShouldEqual("79");
            header.ActionCode.ShouldEqual("51");
            header.TdspName.ShouldEqual("Atlanta Gas Light");
            header.TdspDuns.ShouldEqual("AGLCPIPES");
            header.CrName.ShouldEqual("Commerce Energy DBA Just Energy");
            header.CrDuns.ShouldEqual("176343341GA");
            header.ProcessedReceivedDateTime.ShouldEqual("201301181039");

            header.Names.Count().ShouldEqual(1);
            var name = header.Names[0];

            name.EntityName.ShouldEqual("JORGE                         HERNANDEZ");
            name.EntityName2.ShouldBeEmpty();
            name.EntityName3.ShouldBeEmpty();
            name.Address1.ShouldEqual("5723  EVERGLADES TRL");
            name.Address2.ShouldEqual("LOT 8A");
            name.City.ShouldEqual("NORCROSS");
            name.State.ShouldEqual("GA");
            name.PostalCode.ShouldEqual("30071");
            name.ContactName.ShouldBeEmpty();
            name.ContactPhoneNbr1.ShouldEqual("770 234-0849");
            name.ContactPhoneNbr2.ShouldBeEmpty();

            header.Services.Count().ShouldEqual(1);
            var service = header.Services[0];

            service.PurposeCode.ShouldEqual("RC001");
            service.EsiId.ShouldEqual("4568873098");
            service.EquipLocation.ShouldEqual("113278984");
            service.Membership.ShouldEqual("299906457");
        }

        [TestMethod]
        public void ParseHeader_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <ActionCode>ActionCode</ActionCode>
                    <CrDuns>CrDuns</CrDuns>
                    <CrName>CrName</CrName>
                    <ProcessedReceivedDateTime>ProcessedReceivedDateTime</ProcessedReceivedDateTime>
                    <ReferenceNbr>ReferenceNbr</ReferenceNbr>
                    <TdspDuns>TdspDuns</TdspDuns>
                    <TdspName>TdspName</TdspName>
                    <TransactionDate>TransactionDate</TransactionDate>
                    <TransactionNbr>TransactionNbr</TransactionNbr>
                    <TransactionSetControlNbr>TransactionSetControlNbr</TransactionSetControlNbr>
                    <TransactionSetId>TransactionSetId</TransactionSetId>
                    <TransactionSetPurposeCode>TransactionSetPurposeCode</TransactionSetPurposeCode>
                    <TransactionType>TransactionType</TransactionType>
                </root>";

            // act
            var element = XElement.Parse(xml);
            var actual = concern.ParseHeader(element, namespaces);

            // assert
            actual.ActionCode.ShouldEqual("ActionCode");
            actual.CrDuns.ShouldEqual("CrDuns");
            actual.CrName.ShouldEqual("CrName");
            actual.ProcessedReceivedDateTime.ShouldEqual("ProcessedReceivedDateTime");
            actual.ReferenceNbr.ShouldEqual("ReferenceNbr");
            actual.TdspDuns.ShouldEqual("TdspDuns");
            actual.TdspName.ShouldEqual("TdspName");
            actual.TransactionDate.ShouldEqual("TransactionDate");
            actual.TransactionNbr.ShouldEqual("TransactionNbr");
            actual.TransactionSetControlNbr.ShouldEqual("TransactionSetControlNbr");
            actual.TransactionSetId.ShouldEqual("TransactionSetId");
            actual.TransactionSetPurposeCode.ShouldEqual("TransactionSetPurposeCode");
            actual.TransactionType.ShouldEqual("TransactionType");
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
                    <NameLoop><Name><something /></Name></NameLoop>
                    <ServiceLoop><Service><something /></Service></ServiceLoop>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseHeader(element, namespaces);

            // assert
            actual.ShouldNotBeNull();

            actual.Names.Count().ShouldEqual(1);
            actual.Services.Count().ShouldEqual(1);
        }

        [Ignore, TestCategory("LegacyLogicThatShouldFail")]
        public void ParseName_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <Address1>Address1</Address1>
                    <Address2>Address2</Address2>
                    <City>City</City>
                    <ContactCode>ContactCode</ContactCode>
                    <ContactName>ContactName</ContactName>
                    <ContactPhoneNbr1>ContactPhoneNbr1</ContactPhoneNbr1>
                    <ContactPhoneNbr2>ContactPhoneNbr2</ContactPhoneNbr2>
                    <CountryCode>CountryCode</CountryCode>
                    <EntityDuns>EntityDuns</EntityDuns>
                    <EntityIdCode>EntityIdCode</EntityIdCode>
                    <EntityIdType>EntityIdType</EntityIdType>
                    <EntityName>EntityName</EntityName>
                    <EntityName2>EntityName2</EntityName2>
                    <EntityName3>EntityName3</EntityName3>
                    <PostalCode>PostalCode</PostalCode>
                    <State>State</State>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseName(element, namespaces);

            // assert
            actual.Address1.ShouldEqual("Address1");
            actual.Address2.ShouldEqual("Address2");
            actual.City.ShouldEqual("City");
            actual.ContactCode.ShouldBeNull();
            actual.ContactName.ShouldEqual("ContactName");
            actual.ContactPhoneNbr1.ShouldEqual("ContactPhoneNbr1");
            actual.ContactPhoneNbr2.ShouldEqual("ContactPhoneNbr2");
            actual.CountryCode.ShouldEqual("CountryCode");
            actual.EntityDuns.ShouldEqual("EntityDuns");
            actual.EntityIdCode.ShouldEqual("EntityIdCode");
            actual.EntityIdType.ShouldEqual("EntityIdType");
            actual.EntityName.ShouldEqual("EntityName");
            actual.EntityName2.ShouldEqual("EntityName2");
            actual.EntityName3.ShouldEqual("EntityName3");
            actual.PostalCode.ShouldEqual("PostalCode");
            actual.State.ShouldEqual("State");
        }

        [TestMethod]
        public void ParseName_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            const string xml = @"<root></root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseName(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        public void ParseService_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <AccStatusCode>AccStatusCode</AccStatusCode>
                    <AccStatusDesc>AccStatusDesc</AccStatusDesc>
                    <AreaOutage>AreaOutage</AreaOutage>
                    <CallAhead>CallAhead</CallAhead>
                    <CompletionDate>CompletionDate</CompletionDate>
                    <CompletionTime>CompletionTime</CompletionTime>
                    <CustRepairRemarks>CustRepairRemarks</CustRepairRemarks>
                    <Directions>Directions</Directions>
                    <EquipLocation>EquipLocation</EquipLocation>
                    <ESIID>ESIID</ESIID>
                    <EstRestoreDate>EstRestoreDate</EstRestoreDate>
                    <EstRestoreTime>EstRestoreTime</EstRestoreTime>
                    <IncidentCode>IncidentCode</IncidentCode>
                    <InterDurationPeriod>InterDurationPeriod</InterDurationPeriod>
                    <IntStartDate>IntStartDate</IntStartDate>
                    <IntStartTime>IntStartTime</IntStartTime>
                    <Membership>Membership</Membership>
                    <MeterNbr>MeterNbr</MeterNbr>
                    <MeterRead>MeterRead</MeterRead>
                    <MeterReadCode>MeterReadCode</MeterReadCode>
                    <MeterReadDate>MeterReadDate</MeterReadDate>
                    <MeterReadUOM>MeterReadUOM</MeterReadUOM>
                    <MeterTestDate>MeterTestDate</MeterTestDate>
                    <MeterTestResults>MeterTestResults</MeterTestResults>
                    <NotBeforeDate>NotBeforeDate</NotBeforeDate>
                    <PremiseTypeDesc>PremiseTypeDesc</PremiseTypeDesc>
                    <PremiseTypeVerification>PremiseTypeVerification</PremiseTypeVerification>
                    <PremLocation>PremLocation</PremLocation>
                    <PriorityCode>PriorityCode</PriorityCode>
                    <PurposeCode>PurposeCode</PurposeCode>
                    <RemarksPermanentSuspend>RemarksPermanentSuspend</RemarksPermanentSuspend>
                    <RepairRecommended>RepairRecommended</RepairRecommended>
                    <ReportRemarks>ReportRemarks</ReportRemarks>
                    <Rescheduled>Rescheduled</Rescheduled>
                    <ServiceOrderNbr>ServiceOrderNbr</ServiceOrderNbr>
                    <ServiceReqDate>ServiceReqDate</ServiceReqDate>
                    <SpecialProcessCode>SpecialProcessCode</SpecialProcessCode>
                    <SwitchHoldDesc>SwitchHoldDesc</SwitchHoldDesc>
                    <SwitchHoldIndicator>SwitchHoldIndicator</SwitchHoldIndicator>
                    <DisconnectAuthorization>Y</DisconnectAuthorization>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseService(element, namespaces);

            // assert
            actual.AccStatusCode.ShouldEqual("AccStatusCode");
            actual.AccStatusDesc.ShouldEqual("AccStatusDesc");
            actual.AreaOutage.ShouldEqual("AreaOutage");
            actual.CallAhead.ShouldEqual("CallAhead");
            actual.CompletionDate.ShouldEqual("CompletionDate");
            actual.CompletionTime.ShouldEqual("CompletionTime");
            actual.CustRepairRemarks.ShouldEqual("CustRepairRemarks");
            actual.Directions.ShouldEqual("Directions");
            actual.EquipLocation.ShouldEqual("EquipLocation");
            actual.EsiId.ShouldEqual("ESIID");
            actual.EstRestoreDate.ShouldEqual("EstRestoreDate");
            actual.EstRestoreTime.ShouldEqual("EstRestoreTime");
            actual.IncidentCode.ShouldEqual("IncidentCode");
            actual.InterDurationPeriod.ShouldEqual("InterDurationPeriod");
            actual.IntStartDate.ShouldEqual("IntStartDate");
            actual.IntStartTime.ShouldEqual("IntStartTime");
            actual.Membership.ShouldEqual("Membership");
            actual.MeterNbr.ShouldEqual("MeterNbr");
            actual.MeterRead.ShouldEqual("MeterRead");
            actual.MeterReadCode.ShouldEqual("MeterReadCode");
            actual.MeterReadDate.ShouldEqual("MeterReadDate");
            actual.MeterReadUom.ShouldEqual("MeterReadUOM");
            actual.MeterTestDate.ShouldEqual("MeterTestDate");
            actual.MeterTestResults.ShouldEqual("MeterTestResults");
            actual.NotBeforeDate.ShouldEqual("NotBeforeDate");
            actual.PremiseTypeDesc.ShouldBeNull();
            actual.PremiseTypeVerification.ShouldBeNull();
            actual.PremLocation.ShouldEqual("PremLocation");
            actual.PriorityCode.ShouldEqual("PriorityCode");
            actual.PurposeCode.ShouldEqual("PurposeCode");
            actual.RemarksPermanentSuspend.ShouldEqual("RemarksPermanentSuspend");
            actual.RepairRecommended.ShouldEqual("RepairRecommended");
            actual.ReportRemarks.ShouldEqual("ReportRemarks");
            actual.Rescheduled.ShouldEqual("Rescheduled");
            actual.ServiceOrderNbr.ShouldEqual("ServiceOrderNbr");
            actual.ServiceReqDate.ShouldEqual("ServiceReqDate");
            actual.SpecialProcessCode.ShouldEqual("SpecialProcessCode");
            actual.SwitchHoldDesc.ShouldBeNull();
            actual.SwitchHoldIndicator.ShouldBeNull();
            actual.DisconnectAuthorization.ShouldEqual("Y");
        }

        [TestMethod]
        public void ParseService_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            const string xml = @"<root></root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseService(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        public void ParseService_ShouldReturnChildren_WhenElementsSupplied()
        {
            // arrange
            const string xml = @"
                <root>
                    <ServiceChangeLoop><ServiceChange><something /></ServiceChange></ServiceChangeLoop>
                    <ServiceMeterLoop><ServiceMeter><something /></ServiceMeter></ServiceMeterLoop>
                    <ServicePoleLoop><ServicePole><something /></ServicePole></ServicePoleLoop>
                    <ServiceRejectLoop><ServiceReject><something /></ServiceReject></ServiceRejectLoop>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseService(element, namespaces);

            // assert
            actual.ShouldNotBeNull();

            actual.Changes.Count().ShouldEqual(1);
            actual.Meters.Count().ShouldEqual(1);
            actual.Poles.Count().ShouldEqual(1);
            actual.Rejects.Count().ShouldEqual(1);
        }

        [TestMethod]
        public void ParseServiceChange_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <ChangeReason>ChangeReason</ChangeReason>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceChange(element, namespaces);

            // assert
            actual.ChangeReason.ShouldEqual("ChangeReason");
        }

        [TestMethod]
        public void ParseServiceChange_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            const string xml = @"<root></root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceChange(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        public void ParseServiceMeter_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <MeterNumber>MeterNumber</MeterNumber>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceMeter(element, namespaces);

            // assert
            actual.MeterNumber.ShouldEqual("MeterNumber");
        }

        [TestMethod]
        public void ParseServiceMeter_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            const string xml = @"<root></root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceMeter(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        public void ParseServicePole_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <PoleNbr>PoleNbr</PoleNbr>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServicePole(element, namespaces);

            // assert
            actual.PoleNbr.ShouldEqual("PoleNbr");
        }

        [TestMethod]
        public void ParseServicePole_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            const string xml = @"<root></root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServicePole(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        public void ParseServiceReject_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <RejectCode>RejectCode</RejectCode>
                    <RejectReason>RejectReason</RejectReason>
                    <UnexCode>UnexCode</UnexCode>
                    <UnexReason>UnexReason</UnexReason>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceReject(element, namespaces);

            // assert
            actual.RejectCode.ShouldEqual("RejectCode");
            actual.RejectReason.ShouldEqual("RejectReason");
            actual.UnexCode.ShouldEqual("UnexCode");
            actual.UnexReason.ShouldEqual("UnexReason");
        }

        [TestMethod]
        public void ParseServiceReject_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            const string xml = @"<root></root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceReject(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Should;

namespace Ista.FileServices.MarketFiles.UnitTests.MarketImports
{
    [TestClass]
    public class Xml814Scenarios
    {
        private Dictionary<string, XNamespace> namespaces;
        private IMarketDataAccess marketDataAccess;
        private ILogger logger;
        
        private Import814Xml concern;
        
        [TestInitialize]
        public void SetUp()
        {
            namespaces = new Dictionary<string, XNamespace>();
            marketDataAccess = MockRepository.GenerateStub<IMarketDataAccess>();
            logger = MockRepository.GenerateStub<ILogger>();

            concern = new Import814Xml(marketDataAccess, logger);
        }

        [TestMethod]
        public void Sample_File()
        {
            // arrange
            const string xml = @"<ns0:Market814 xmlns:ns0=""http://CIS.Integration.Schema.Market.Common.Market814"">
	<TransactionCount>2</TransactionCount>
	<LdcDuns>006998371</LdcDuns>
	<LdcName>OE-DISTRIBUTION</LdcName>
	<CspDuns>6260586553000</CspDuns>
	<CspName>IGS Energy</CspName>
	<FunctionalGroup>GE</FunctionalGroup>
	<Transaction>
		<Header>
			<TransactionSetId>814</TransactionSetId>
			<TransactionSetPurposeCode>S</TransactionSetPurposeCode>
			<TransactionNbr>041738570-S</TransactionNbr>
			<TransactionDate>20130122</TransactionDate>
			<ReferenceNbr>041738570</ReferenceNbr>
			<ActionCode>HU</ActionCode>
			<TdspDuns>006998371</TdspDuns>
			<TdspName>OE-DISTRIBUTION</TdspName>
			<CrDuns>6260586553000</CrDuns>
			<CrName>IGS Energy</CrName>
			<CommodityCode>EL</CommodityCode>
			<TradingPartnerID>IGSOH799249461ENR</TradingPartnerID>
			<NameLoop>
				<Name>
					<EntityIdType>8R</EntityIdType>
					<EntityName>MINERVA MILLER</EntityName>
				</Name>
			</NameLoop>
			<ServiceLoop>
				<Service>
					<AssignId>20644416</AssignId>
					<ServiceTypeCode1>SH</ServiceTypeCode1>
					<ServiceType1>EL</ServiceType1>
					<ServiceTypeCode2>SH</ServiceTypeCode2>
					<ServiceType2>HU</ServiceType2>
					<ServiceTypeCode3>SH</ServiceTypeCode3>
					<ServiceType3/>
					<ActionCode>A</ActionCode>
					<MaintenanceTypeCode>029</MaintenanceTypeCode>
					<EsiId>08026071840000923169</EsiId>
					<EsiIdStartDate/>
					<EsiIdEndDate/>
					<SpecialReadSwitchDate/>
					<MembershipID>08026071840000923169</MembershipID>
					<ESPAccountNumber>10517878</ESPAccountNumber>
					<PreviousEsiId>277513000530001</PreviousEsiId>
                    <DaysInArrears>79</DaysInArrears>
					<SystemNumber/>
					<ServiceDateLoop>
						<ServiceDate/>
					</ServiceDateLoop>
				</Service>
			</ServiceLoop>
		</Header>
	</Transaction>
	<Transaction>
		<Header>
			<TransactionSetId>814</TransactionSetId>
			<TransactionSetPurposeCode>S</TransactionSetPurposeCode>
			<TransactionNbr>041738625-S</TransactionNbr>
			<TransactionDate>20130122</TransactionDate>
			<ReferenceNbr>041738625</ReferenceNbr>
			<ActionCode>HU</ActionCode>
			<TdspDuns>007904626</TdspDuns>
			<TdspName>TE-DISTRIBUTION</TdspName>
			<CrDuns>6260586553000</CrDuns>
			<CrName>IGS Energy</CrName>
			<CommodityCode>EL</CommodityCode>
			<TradingPartnerID>IGSOH799249461ENR</TradingPartnerID>
			<NameLoop>
				<Name>
					<EntityIdType>8R</EntityIdType>
					<EntityName>THERESA E ROWER</EntityName>
				</Name>
			</NameLoop>
			<ServiceLoop>
				<Service>
					<AssignId>20644226</AssignId>
					<ServiceTypeCode1>SH</ServiceTypeCode1>
					<ServiceType1>EL</ServiceType1>
					<ServiceTypeCode2>SH</ServiceTypeCode2>
					<ServiceType2>HU</ServiceType2>
					<ServiceTypeCode3>SH</ServiceTypeCode3>
					<ServiceType3/>
					<ActionCode>A</ActionCode>
					<MaintenanceTypeCode>029</MaintenanceTypeCode>
					<EsiId>08007588122920030643</EsiId>
					<EsiIdStartDate/>
					<EsiIdEndDate/>
					<SpecialReadSwitchDate/>
					<MembershipID>08007588122920030643</MembershipID>
					<ESPAccountNumber>10517716</ESPAccountNumber>
					<PreviousEsiId>335009003710001</PreviousEsiId>
                    <DaysInArrears>79</DaysInArrears>
					<SystemNumber/>
					<ServiceDateLoop>
						<ServiceDate/>
					</ServiceDateLoop>
				</Service>
			</ServiceLoop>
		</Header>
	</Transaction>
</ns0:Market814>";

            // act
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(xml));
            var result = concern.Parse(stream);

            // assert
            result.TransactionActualCount.ShouldEqual(2);
            result.TransactionAuditCount.ShouldEqual(2);

            var header = result.Headers[0] as Type814Header;
            Assert.IsNotNull(header);
            header.TransactionSetId.ShouldEqual("814");
            header.TransactionSetPurposeCode.ShouldEqual("S");
            header.TransactionNbr.ShouldEqual("041738570-S");
            header.TransactionDate.ShouldEqual("20130122");
            header.ReferenceNbr.ShouldEqual("041738570");
            header.ActionCode.ShouldEqual("HU");
            header.TdspDuns.ShouldEqual("006998371");
            header.TdspName.ShouldEqual("OE-DISTRIBUTION");
            header.CrDuns.ShouldEqual("6260586553000");
            header.CrName.ShouldEqual("IGS Energy");

            header.Names.Count().ShouldEqual(1);
            var name = header.Names[0];

            name.EntityIdType.ShouldEqual("8R");
            name.EntityName.ShouldEqual("MINERVA MILLER");

            header.Services.Count().ShouldEqual(1);
            var service = header.Services[0];

            service.AssignId.ShouldEqual("20644416");
            service.ServiceTypeCode1.ShouldEqual("SH");
            service.ServiceType1.ShouldEqual("EL");
            service.ServiceTypeCode2.ShouldEqual("SH");
            service.ServiceType2.ShouldEqual("HU");
            service.ServiceTypeCode3.ShouldEqual("SH");
            service.ServiceType3.ShouldBeEmpty();
            service.ActionCode.ShouldEqual("A");
            service.MaintenanceTypeCode.ShouldEqual("029");
            service.EsiId.ShouldEqual("08026071840000923169");
            service.EsiIdStartDate.ShouldBeEmpty();
            service.EsiIdEndDate.ShouldBeEmpty();
            service.SpecialReadSwitchDate.ShouldBeEmpty();
            service.MembershipId.ShouldEqual("08026071840000923169");
            service.EspAccountNumber.ShouldEqual("10517878");
            service.PreviousEsiId.ShouldEqual("277513000530001");
            service.DaysInArrears.ShouldEqual("79");
            service.SystemNumber.ShouldBeEmpty();

            service.Dates.Count().ShouldEqual(1);
            var date = service.Dates[0];
            date.ShouldNotBeNull();
            date.Qualifier.ShouldBeEmpty();
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
                    <POLRClass>POLRClass</POLRClass>
                    <ReferenceNbr>ReferenceNbr</ReferenceNbr>
                    <TdspDuns>TdspDuns</TdspDuns>
                    <TdspName>TdspName</TdspName>
                    <TransactionDate>TransactionDate</TransactionDate>
                    <TransactionNbr>TransactionNbr</TransactionNbr>
                    <TransactionQualifier>TransactionQualifier</TransactionQualifier>
                    <TransactionSetControlNbr>TransactionSetControlNbr</TransactionSetControlNbr>
                    <TransactionSetId>TransactionSetId</TransactionSetId>
                    <TransactionSetPurposeCode>TransactionSetPurposeCode</TransactionSetPurposeCode>
                    <TransactionTime>TransactionTime</TransactionTime>
                    <TransactionTimeCode>TransactionTimeCode</TransactionTimeCode>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseHeader(element, namespaces);

            // assert
            actual.ActionCode.ShouldEqual("ActionCode");
            actual.CrDuns.ShouldEqual("CrDuns");
            actual.CrName.ShouldEqual("CrName");
            actual.PolrClass.ShouldBeNull();
            actual.ReferenceNbr.ShouldEqual("ReferenceNbr");
            actual.TdspDuns.ShouldEqual("TdspDuns");
            actual.TdspName.ShouldEqual("TdspName");
            actual.TransactionDate.ShouldEqual("TransactionDate");
            actual.TransactionNbr.ShouldEqual("TransactionNbr");
            actual.TransactionQualifier.ShouldEqual("TransactionQualifier");
            actual.TransactionSetControlNbr.ShouldEqual("TransactionSetControlNbr");
            actual.TransactionSetId.ShouldEqual("TransactionSetId");
            actual.TransactionSetPurposeCode.ShouldEqual("TransactionSetPurposeCode");
            actual.TransactionTime.ShouldEqual("TransactionTime");
            actual.TransactionTimeCode.ShouldEqual("TransactionTimeCode");
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

        [TestMethod]
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
                    <ContactPhoneNbr3>ContactPhoneNbr3</ContactPhoneNbr3>
                    <CountryCode>CountryCode</CountryCode>
                    <County>County</County>
                    <CustType>CustType</CustType>
                    <EntityDuns>EntityDuns</EntityDuns>
                    <EntityEmail>EntityEmail</EntityEmail>
                    <EntityFirstName>EntityFirstName</EntityFirstName>
                    <EntityIdCode>EntityIdCode</EntityIdCode>
                    <EntityIdType>EntityIdType</EntityIdType>
                    <EntityLastName>EntityLastName</EntityLastName>
                    <EntityMiddleName>EntityMiddleName</EntityMiddleName>
                    <EntityName>EntityName</EntityName>
                    <EntityName2>EntityName2</EntityName2>
                    <EntityName3>EntityName3</EntityName3>
                    <PostalCode>PostalCode</PostalCode>
                    <State>State</State>
                    <TaxingDistrict>TaxingDistrict</TaxingDistrict>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseName(element, namespaces);

            // assert
            actual.Address1.ShouldEqual("Address1");
            actual.Address2.ShouldEqual("Address2");
            actual.City.ShouldEqual("City");
            actual.ContactCode.ShouldEqual("ContactCode");
            actual.ContactName.ShouldEqual("ContactName");
            actual.ContactPhoneNbr1.ShouldEqual("ContactPhoneNbr1");
            actual.ContactPhoneNbr2.ShouldEqual("ContactPhoneNbr2");
            actual.ContactPhoneNbr3.ShouldEqual("ContactPhoneNbr3");
            actual.CountryCode.ShouldEqual("CountryCode");
            actual.County.ShouldEqual("County");
            actual.CustType.ShouldEqual("CustType");
            actual.EntityDuns.ShouldEqual("EntityDuns");
            actual.EntityEmail.ShouldBeNull();
            actual.EntityFirstName.ShouldEqual("EntityFirstName");
            actual.EntityIdCode.ShouldEqual("EntityIdCode");
            actual.EntityIdType.ShouldEqual("EntityIdType");
            actual.EntityLastName.ShouldEqual("EntityLastName");
            actual.EntityMiddleName.ShouldEqual("EntityMiddleName");
            actual.EntityName.ShouldEqual("EntityName");
            actual.EntityName2.ShouldEqual("EntityName2");
            actual.EntityName3.ShouldEqual("EntityName3");
            actual.PostalCode.ShouldEqual("PostalCode");
            actual.State.ShouldEqual("State");
            actual.TaxingDistrict.ShouldEqual("TaxingDistrict");
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
                    <ActionCode>ActionCode</ActionCode>
                    <AirConditioners>AirConditioners</AirConditioners>
                    <ApprovalCodeIndicator>ApprovalCodeIndicator</ApprovalCodeIndicator>
                    <ArrearsBalance>ArrearsBalance</ArrearsBalance>
                    <AssignId>AssignId</AssignId>
                    <BillCalculator>BillCalculator</BillCalculator>
                    <BillCycleCodeDesc>BillCycleCodeDesc</BillCycleCodeDesc>
                    <BillType>BillType</BillType>
                    <BudgetBillingStatus>BudgetBillingStatus</BudgetBillingStatus>
                    <BudgetInstallment>BudgetInstallment</BudgetInstallment>
                    <BudgetPlan>BudgetPlan</BudgetPlan>
                    <CapacityObligation>CapacityObligation</CapacityObligation>
                    <CommodityPrice>CommodityPrice</CommodityPrice>
                    <CurrentBalance>CurrentBalance</CurrentBalance>
                    <CustomerAuthorization>CustomerAuthorization</CustomerAuthorization>
                    <CustomerReferenceNumber>CustomerReferenceNumber</CustomerReferenceNumber>
                    <Deposit>Deposit</Deposit>
                    <DFIAccountNumber>DFIAccountNumber</DFIAccountNumber>
                    <DFIIdentificationNumber>DFIIdentificationNumber</DFIIdentificationNumber>
                    <DFIIndicator1>DFIIndicator1</DFIIndicator1>
                    <DFIIndicator2>DFIIndicator2</DFIIndicator2>
                    <DFIQualifier>DFIQualifier</DFIQualifier>
                    <DFIRoutingNumber>DFIRoutingNumber</DFIRoutingNumber>
                    <DisputedAmount>DisputedAmount</DisputedAmount>
                    <DistributionLossFactorCode>DistributionLossFactorCode</DistributionLossFactorCode>
                    <EligibleLoadPercentage>EligibleLoadPercentage</EligibleLoadPercentage>
                    <EnergizedFlag>EnergizedFlag</EnergizedFlag>
                    <EsiId>EsiId</EsiId>
                    <EsiIdEligibilityDate>EsiIdEligibilityDate</EsiIdEligibilityDate>
                    <EsiIdEndDate>EsiIdEndDate</EsiIdEndDate>
                    <EsiIdStartDate>EsiIdStartDate</EsiIdStartDate>
                    <ESPAccountNumber>ESPAccountNumber</ESPAccountNumber>
                    <ESPChargesCommTaxRate>ESPChargesCommTaxRate</ESPChargesCommTaxRate>
                    <ESPChargesResTaxRate>ESPChargesResTaxRate</ESPChargesResTaxRate>
                    <ESPCommodityPrice>ESPCommodityPrice</ESPCommodityPrice>
                    <ESPFixedCharge>ESPFixedCharge</ESPFixedCharge>
                    <ESPTransactionNumber>ESPTransactionNumber</ESPTransactionNumber>
                    <FeeApprovedApplied>FeeApprovedApplied</FeeApprovedApplied>
                    <FixedMonthlyCharge>FixedMonthlyCharge</FixedMonthlyCharge>
                    <FundsAuthorization>FundsAuthorization</FundsAuthorization>
                    <GasPoolId>GasPoolId</GasPoolId>
                    <GasSupplyServiceOption>GasSupplyServiceOption</GasSupplyServiceOption>
                    <GasSupplyServiceOptionCode>GasSupplyServiceOptionCode</GasSupplyServiceOptionCode>
                    <HumanNeeds>HumanNeeds</HumanNeeds>
                    <IntervalStatusType>IntervalStatusType</IntervalStatusType>
                    <LBMPZone>LBMPZone</LBMPZone>
                    <LDCAccountBalance>LDCAccountBalance</LDCAccountBalance>
                    <LDCBillingCycle>LDCBillingCycle</LDCBillingCycle>
                    <LDCBudgetBillingCycle>LDCBudgetBillingCycle</LDCBudgetBillingCycle>
                    <LDCBudgetBillingStatus>LDCBudgetBillingStatus</LDCBudgetBillingStatus>
                    <LDCSupplierBalance>LDCSupplierBalance</LDCSupplierBalance>
                    <MaintenanceTypeCode>MaintenanceTypeCode</MaintenanceTypeCode>
                    <MarketerCustomerAccountNumber>MarketerCustomerAccountNumber</MarketerCustomerAccountNumber>
                    <MaxDailyAmt>MaxDailyAmt</MaxDailyAmt>
                    <MaximumGeneration>MaximumGeneration</MaximumGeneration>
                    <DaysInArrears>79</DaysInArrears>
                    <MembershipID>MembershipID</MembershipID>
                    <MeterAccessNote>MeterAccessNote</MeterAccessNote>
                    <MeterCycleCode>MeterCycleCode</MeterCycleCode>
                    <MeterCycleCodeDesc>MeterCycleCodeDesc</MeterCycleCodeDesc>
                    <NewCustomerIndicator>NewCustomerIndicator</NewCustomerIndicator>
                    <NewPremiseIndicator>NewPremiseIndicator</NewPremiseIndicator>
                    <NextMeterReadDate>NextMeterReadDate</NextMeterReadDate>
                    <NotificationWaiver>NotificationWaiver</NotificationWaiver>
                    <NumberOfMonthsHistory>NumberOfMonthsHistory</NumberOfMonthsHistory>
                    <OldESPAccountNumber>OldESPAccountNumber</OldESPAccountNumber>
                    <ParticipatingInterest>ParticipatingInterest</ParticipatingInterest>
                    <PaymentArrangement>PaymentArrangement</PaymentArrangement>
                    <PaymentCategory>PaymentCategory</PaymentCategory>
                    <PeakDemandHistory>PeakDemandHistory</PeakDemandHistory>
                    <PermitIndicator>PermitIndicator</PermitIndicator>
                    <PowerRegion>PowerRegion</PowerRegion>
                    <PremiseType>PremiseType</PremiseType>
                    <PreviousEsiId>PreviousEsiId</PreviousEsiId>
                    <PreviousESPAccountNumber>PreviousESPAccountNumber</PreviousESPAccountNumber>
                    <PriorityCode>PriorityCode</PriorityCode>
                    <ReinstatementDate>ReinstatementDate</ReinstatementDate>
                    <RemainingUtilBalanceBucket1>RemainingUtilBalanceBucket1</RemainingUtilBalanceBucket1>
                    <RemainingUtilBalanceBucket2>RemainingUtilBalanceBucket2</RemainingUtilBalanceBucket2>
                    <RemainingUtilBalanceBucket3>RemainingUtilBalanceBucket3</RemainingUtilBalanceBucket3>
                    <RemainingUtilBalanceBucket4>RemainingUtilBalanceBucket4</RemainingUtilBalanceBucket4>
                    <RemainingUtilBalanceBucket5>RemainingUtilBalanceBucket5</RemainingUtilBalanceBucket5>
                    <RemainingUtilBalanceBucket6>RemainingUtilBalanceBucket6</RemainingUtilBalanceBucket6>
                    <RenewableEnergyCertification>RenewableEnergyCertification</RenewableEnergyCertification>
                    <RenewableEnergyIndicator>RenewableEnergyIndicator</RenewableEnergyIndicator>
                    <ResidentialTaxPortion>ResidentialTaxPortion</ResidentialTaxPortion>
                    <RTODate>RTODate</RTODate>
                    <RTOTime>RTOTime</RTOTime>
                    <SalesResponsibility>SalesResponsibility</SalesResponsibility>
                    <ServiceDeliveryPoint>ServiceDeliveryPoint</ServiceDeliveryPoint>
                    <ServiceType1>ServiceType1</ServiceType1>
                    <ServiceType2>ServiceType2</ServiceType2>
                    <ServiceType3>ServiceType3</ServiceType3>
                    <ServiceType4>ServiceType4</ServiceType4>
                    <ServiceTypeCode1>ServiceTypeCode1</ServiceTypeCode1>
                    <ServiceTypeCode2>ServiceTypeCode2</ServiceTypeCode2>
                    <ServiceTypeCode3>ServiceTypeCode3</ServiceTypeCode3>
                    <ServiceTypeCode4>ServiceTypeCode4</ServiceTypeCode4>
                    <SICCode>SICCode</SICCode>
                    <SpecialMeterConfig>SpecialMeterConfig</SpecialMeterConfig>
                    <SpecialNeedsExpirationDate>SpecialNeedsExpirationDate</SpecialNeedsExpirationDate>
                    <SpecialNeedsIndicator>SpecialNeedsIndicator</SpecialNeedsIndicator>
                    <SpecialReadSwitchDate>SpecialReadSwitchDate</SpecialReadSwitchDate>
                    <SpecialReadSwitchTime>SpecialReadSwitchTime</SpecialReadSwitchTime>
                    <StateLicenseNumber>StateLicenseNumber</StateLicenseNumber>
                    <StationId>StationId</StationId>
                    <SupplementalAccountNumber>SupplementalAccountNumber</SupplementalAccountNumber>
                    <SystemNumber>SystemNumber</SystemNumber>
                    <TaxExemptionPercent>TaxExemptionPercent</TaxExemptionPercent>
                    <TaxRate>TaxRate</TaxRate>
                    <TotalKWHHistory>TotalKWHHistory</TotalKWHHistory>
                    <TransactionReferenceNumber>TransactionReferenceNumber</TransactionReferenceNumber>
                    <TransmissionObligation>TransmissionObligation</TransmissionObligation>
                    <UnmeteredAcct>UnmeteredAcct</UnmeteredAcct>
                    <WaterHeaters>WaterHeaters</WaterHeaters>
                    <CSAFlag>Y</CSAFlag>
                    <PaymentOption>Y</PaymentOption>
                    <SwitchHoldStatusIndicator>Y</SwitchHoldStatusIndicator>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseService(element, namespaces);

            // assert
            actual.ActionCode.ShouldEqual("ActionCode");
            actual.AirConditioners.ShouldEqual("AirConditioners");
            actual.ApprovalCodeIndicator.ShouldBeNull();
            actual.ArrearsBalance.ShouldEqual("ArrearsBalance");
            actual.AssignId.ShouldEqual("AssignId");
            actual.BillCalculator.ShouldEqual("BillCalculator");
            actual.BillCycleCodeDesc.ShouldEqual("BillCycleCodeDesc");
            actual.BillType.ShouldEqual("BillType");
            actual.BudgetBillingStatus.ShouldEqual("BudgetBillingStatus");
            actual.BudgetInstallment.ShouldEqual("BudgetInstallment");
            actual.BudgetPlan.ShouldEqual("BudgetPlan");
            actual.CapacityObligation.ShouldEqual("CapacityObligation");
            actual.CommodityPrice.ShouldBeNull();
            actual.CurrentBalance.ShouldEqual("CurrentBalance");
            actual.CustomerAuthorization.ShouldEqual("CustomerAuthorization");
            actual.CustomerReferenceNumber.ShouldBeNull();
            actual.Deposit.ShouldEqual("Deposit");
            actual.DfiAccountNumber.ShouldBeNull();
            actual.DfiIdentificationNumber.ShouldBeNull();
            actual.DfiIndicator1.ShouldBeNull();
            actual.DfiIndicator2.ShouldBeNull();
            actual.DfiQualifier.ShouldBeNull();
            actual.DfiRoutingNumber.ShouldBeNull();
            actual.DisputedAmount.ShouldEqual("DisputedAmount");
            actual.DistributionLossFactorCode.ShouldEqual("DistributionLossFactorCode");
            actual.EligibleLoadPercentage.ShouldEqual("EligibleLoadPercentage");
            actual.EnergizedFlag.ShouldEqual("EnergizedFlag");
            actual.EsiId.ShouldEqual("EsiId");
            //actual.EsiIdEligibilityDate.ShouldEqual("EsiIdEligibilityDate");
            actual.EsiIdEndDate.ShouldEqual("EsiIdEndDate");
            actual.EsiIdStartDate.ShouldEqual("EsiIdStartDate");
            actual.EspAccountNumber.ShouldEqual("ESPAccountNumber");
            actual.EspChargesCommTaxRate.ShouldEqual("ESPChargesCommTaxRate");
            actual.EspChargesResTaxRate.ShouldEqual("ESPChargesResTaxRate");
            actual.EspCommodityPrice.ShouldEqual("ESPCommodityPrice");
            actual.EspFixedCharge.ShouldEqual("ESPFixedCharge");
            actual.EspTransactionNumber.ShouldBeNull();
            actual.FeeApprovedApplied.ShouldEqual("FeeApprovedApplied");
            actual.FixedMonthlyCharge.ShouldEqual("FixedMonthlyCharge");
            actual.FundsAuthorization.ShouldEqual("FundsAuthorization");
            actual.GasPoolId.ShouldEqual("GasPoolId");
            actual.GasSupplyServiceOption.ShouldEqual("GasSupplyServiceOption");
            actual.GasSupplyServiceOptionCode.ShouldEqual("GasSupplyServiceOptionCode");
            actual.HumanNeeds.ShouldEqual("HumanNeeds");
            actual.IntervalStatusType.ShouldEqual("IntervalStatusType");
            actual.LbmpZone.ShouldEqual("LBMPZone");
            actual.LdcAccountBalance.ShouldEqual("LDCAccountBalance");
            actual.LdcBillingCycle.ShouldEqual("LDCBillingCycle");
            actual.LdcBudgetBillingCycle.ShouldEqual("LDCBudgetBillingCycle");
            actual.LdcBudgetBillingStatus.ShouldEqual("LDCBudgetBillingStatus");
            actual.LdcSupplierBalance.ShouldEqual("LDCSupplierBalance");
            actual.MaintenanceTypeCode.ShouldEqual("MaintenanceTypeCode");
            actual.MarketerCustomerAccountNumber.ShouldEqual("MarketerCustomerAccountNumber");
            actual.MaxDailyAmt.ShouldEqual("MaxDailyAmt");
            actual.MaximumGeneration.ShouldEqual("MaximumGeneration");
            actual.DaysInArrears.ShouldEqual("79");
            actual.MembershipId.ShouldEqual("MembershipID");
            actual.MeterAccessNote.ShouldBeNull();
            actual.MeterCycleCode.ShouldEqual("MeterCycleCode");
            actual.MeterCycleCodeDesc.ShouldEqual("MeterCycleCodeDesc");
            actual.NewCustomerIndicator.ShouldEqual("NewCustomerIndicator");
            actual.NewPremiseIndicator.ShouldEqual("NewPremiseIndicator");
            actual.NextMeterReadDate.ShouldEqual("NextMeterReadDate");
            actual.NotificationWaiver.ShouldEqual("NotificationWaiver");
            actual.NumberOfMonthsHistory.ShouldEqual("NumberOfMonthsHistory");
            actual.OldEspAccountNumber.ShouldBeNull();
            actual.ParticipatingInterest.ShouldEqual("ParticipatingInterest");
            actual.PaymentArrangement.ShouldEqual("PaymentArrangement");
            actual.PaymentCategory.ShouldBeNull();
            actual.PeakDemandHistory.ShouldEqual("PeakDemandHistory");
            actual.PermitIndicator.ShouldEqual("PermitIndicator");
            actual.PowerRegion.ShouldEqual("PowerRegion");
            actual.PremiseType.ShouldEqual("PremiseType");
            actual.PreviousEsiId.ShouldEqual("PreviousEsiId");
            actual.PreviousEspAccountNumber.ShouldBeNull();
            actual.PriorityCode.ShouldEqual("PriorityCode");
            actual.ReinstatementDate.ShouldEqual("ReinstatementDate");
            actual.RemainingUtilBalanceBucket1.ShouldEqual("RemainingUtilBalanceBucket1");
            actual.RemainingUtilBalanceBucket2.ShouldEqual("RemainingUtilBalanceBucket2");
            actual.RemainingUtilBalanceBucket3.ShouldEqual("RemainingUtilBalanceBucket3");
            actual.RemainingUtilBalanceBucket4.ShouldEqual("RemainingUtilBalanceBucket4");
            actual.RemainingUtilBalanceBucket5.ShouldEqual("RemainingUtilBalanceBucket5");
            actual.RemainingUtilBalanceBucket6.ShouldEqual("RemainingUtilBalanceBucket6");
            actual.RenewableEnergyCertification.ShouldBeNull();
            actual.RenewableEnergyIndicator.ShouldBeNull();
            actual.ResidentialTaxPortion.ShouldEqual("ResidentialTaxPortion");
            actual.RtoDate.ShouldEqual("RTODate");
            actual.RtoTime.ShouldEqual("RTOTime");
            actual.SalesResponsibility.ShouldBeNull();
            actual.ServiceDeliveryPoint.ShouldEqual("ServiceDeliveryPoint");
            actual.ServiceType1.ShouldEqual("ServiceType1");
            actual.ServiceType2.ShouldEqual("ServiceType2");
            actual.ServiceType3.ShouldEqual("ServiceType3");
            actual.ServiceType4.ShouldEqual("ServiceType4");
            actual.ServiceTypeCode1.ShouldEqual("ServiceTypeCode1");
            actual.ServiceTypeCode2.ShouldEqual("ServiceTypeCode2");
            actual.ServiceTypeCode3.ShouldEqual("ServiceTypeCode3");
            actual.ServiceTypeCode4.ShouldEqual("ServiceTypeCode4");
            actual.SicCode.ShouldBeNull();
            actual.SpecialMeterConfig.ShouldEqual("SpecialMeterConfig");
            actual.SpecialNeedsExpirationDate.ShouldBeNull();
            actual.SpecialNeedsIndicator.ShouldEqual("SpecialNeedsIndicator");
            actual.SpecialReadSwitchDate.ShouldEqual("SpecialReadSwitchDate");
            actual.SpecialReadSwitchTime.ShouldEqual("SpecialReadSwitchTime");
            actual.StateLicenseNumber.ShouldBeNull();
            actual.StationId.ShouldEqual("StationId");
            actual.SupplementalAccountNumber.ShouldBeNull();
            actual.SystemNumber.ShouldEqual("SystemNumber");
            //actual.TaxExemptionPercent.ShouldEqual("TaxExemptionPercent");
            actual.TaxRate.ShouldEqual("TaxRate");
            actual.TotalKwhHistory.ShouldEqual("TotalKWHHistory");
            actual.TransactionReferenceNumber.ShouldBeNull();
            actual.TransmissionObligation.ShouldEqual("TransmissionObligation");
            actual.UnmeteredAcct.ShouldBeNull();
            actual.WaterHeaters.ShouldEqual("WaterHeaters");
            actual.CsaFlag.ShouldEqual("Y");
            actual.PaymentOption.ShouldEqual("Y");
            actual.SwitchHoldStatusIndicator.ShouldBeNull();
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
                    <ServiceAccountChangeLoop><ServiceAccountChange><something /></ServiceAccountChange></ServiceAccountChangeLoop>
                    <ServiceDateLoop><ServiceDate><something /></ServiceDate></ServiceDateLoop>
                    <ServiceMeterLoop><ServiceMeter><something /></ServiceMeter></ServiceMeterLoop>
                    <ServiceRejectLoop><ServiceReject><something /></ServiceReject></ServiceRejectLoop>
                    <ServiceStatusLoop><ServiceStatus><something /></ServiceStatus></ServiceStatusLoop>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseService(element, namespaces);

            // assert
            actual.ShouldNotBeNull();

            //actual.AccountChanges.Count().ShouldEqual(1);
            actual.Dates.Count().ShouldEqual(1);
            actual.Meters.Count().ShouldEqual(1);
            actual.Rejects.Count().ShouldEqual(1);
            actual.Statuses.Count().ShouldEqual(1);
        }

        [TestMethod]
        public void ParseServiceAccountChange_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <ChangeDescription>ChangeDescription</ChangeDescription>
                    <ChangeReason>ChangeReason</ChangeReason>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceAccountChange(element, namespaces);

            // assert
            actual.ChangeDescription.ShouldEqual("ChangeDescription");
            actual.ChangeReason.ShouldEqual("ChangeReason");
        }

        [TestMethod]
        public void ParseServiceAccountChange_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            const string xml = @"<root></root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceAccountChange(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        public void ParseServiceDate_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <Date>Date</Date>
                    <NotesDate>NotesDate</NotesDate>
                    <Period>Period</Period>
                    <PeriodFormat>PeriodFormat</PeriodFormat>
                    <Qualifier>Qualifier</Qualifier>
                    <Time>Time</Time>
                    <TimeCode>TimeCode</TimeCode>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceDate(element, namespaces);

            // assert
            actual.Date.ShouldEqual("Date");
            actual.NotesDate.ShouldEqual("NotesDate");
            actual.Period.ShouldEqual("Period");
            actual.PeriodFormat.ShouldEqual("PeriodFormat");
            actual.Qualifier.ShouldEqual("Qualifier");
            actual.Time.ShouldEqual("Time");
            actual.TimeCode.ShouldEqual("TimeCode");
        }

        [TestMethod]
        public void ParseServiceDate_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            const string xml = @"<root></root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceDate(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        public void ParseServiceMeter_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <Address1>Address1</Address1>
                    <Address2>Address2</Address2>
                    <AffiliationNumber>AffiliationNumber</AffiliationNumber>
                    <AMSIndicator>AMSIndicator</AMSIndicator>
                    <Area>Area</Area>
                    <City>City</City>
                    <CollectorIdentification>CollectorIdentification</CollectorIdentification>
                    <CostElement>CostElement</CostElement>
                    <CountryCode>CountryCode</CountryCode>
                    <County>County</County>
                    <CoverageCode>CoverageCode</CoverageCode>
                    <EntityIdCode>EntityIdCode</EntityIdCode>
                    <EntityName2>EntityName2</EntityName2>
                    <EntityName3>EntityName3</EntityName3>
                    <EntityType>EntityType</EntityType>
                    <ESPRateCode>ESPRateCode</ESPRateCode>
                    <FirstName>FirstName</FirstName>
                    <GeographicNumber>GeographicNumber</GeographicNumber>
                    <IdentificationCode>IdentificationCode</IdentificationCode>
                    <ItemNumber>ItemNumber</ItemNumber>
                    <LoadProfile>LoadProfile</LoadProfile>
                    <LocationNumber>LocationNumber</LocationNumber>
                    <LossFactor>LossFactor</LossFactor>
                    <LossReportNumber>LossReportNumber</LossReportNumber>
                    <MeterCode>MeterCode</MeterCode>
                    <MeterCycle>MeterCycle</MeterCycle>
                    <MeterCycleDayOfMonth>MeterCycleDayOfMonth</MeterCycleDayOfMonth>
                    <MeterDataManagementAgent>MeterDataManagementAgent</MeterDataManagementAgent>
                    <MeterDataManagementAgentDUNS>MeterDataManagementAgentDUNS</MeterDataManagementAgentDUNS>
                    <MeterInstaller>MeterInstaller</MeterInstaller>
                    <MeterInstallerDUNS>MeterInstallerDUNS</MeterInstallerDUNS>
                    <MeterMaintenanceProvider>MeterMaintenanceProvider</MeterMaintenanceProvider>
                    <MeterMaintenanceProviderDUNS>MeterMaintenanceProviderDUNS</MeterMaintenanceProviderDUNS>
                    <MeterNumber>MeterNumber</MeterNumber>
                    <MeterOwner>MeterOwner</MeterOwner>
                    <MeterOwnerDUNS>MeterOwnerDUNS</MeterOwnerDUNS>
                    <MeterOwnerIndicator>MeterOwnerIndicator</MeterOwnerIndicator>
                    <MeterReader>MeterReader</MeterReader>
                    <MeterReaderDUNS>MeterReaderDUNS</MeterReaderDUNS>
                    <MeterServiceVoltage>MeterServiceVoltage</MeterServiceVoltage>
                    <MeterType>MeterType</MeterType>
                    <MiddleName>MiddleName</MiddleName>
                    <NamePrefix>NamePrefix</NamePrefix>
                    <NameSuffix>NameSuffix</NameSuffix>
                    <OldMeterNumber>OldMeterNumber</OldMeterNumber>
                    <OrganizationName>OrganizationName</OrganizationName>
                    <PackageOption>PackageOption</PackageOption>
                    <PlanNumber>PlanNumber</PlanNumber>
                    <PriceListNumber>PriceListNumber</PriceListNumber>
                    <PricingStructureCode>PricingStructureCode</PricingStructureCode>
                    <ProductType>ProductType</ProductType>
                    <QualityInspectionArea>QualityInspectionArea</QualityInspectionArea>
                    <RateClass>RateClass</RateClass>
                    <RateSubClass>RateSubClass</RateSubClass>
                    <ReportIdentification>ReportIdentification</ReportIdentification>
                    <SchedulingCoordinator>SchedulingCoordinator</SchedulingCoordinator>
                    <SchedulingCoordinatorDUNS>SchedulingCoordinatorDUNS</SchedulingCoordinatorDUNS>
                    <ServicesReferenceNumber>ServicesReferenceNumber</ServicesReferenceNumber>
                    <ShipperCarOrderNumber>ShipperCarOrderNumber</ShipperCarOrderNumber>
                    <SpecialNeedsIndicator>SpecialNeedsIndicator</SpecialNeedsIndicator>
                    <StandardPointLocation>StandardPointLocation</StandardPointLocation>
                    <State>State</State>
                    <SummaryInterval>SummaryInterval</SummaryInterval>
                    <Supplier>Supplier</Supplier>
                    <TimeOFUse>TimeOFUse</TimeOFUse>
                    <VendorAbbreviation>VendorAbbreviation</VendorAbbreviation>
                    <VendorAgentNumber>VendorAgentNumber</VendorAgentNumber>
                    <VendorIdNumber>VendorIdNumber</VendorIdNumber>
                    <VendorOrderNumber>VendorOrderNumber</VendorOrderNumber>
                    <Zip>Zip</Zip>
                    <MeterInstallPending>Y</MeterInstallPending> = element.GetChildText(empty + 
                    <UsageCode>Y</UsageCode>
                </root>";


            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceMeter(element, namespaces);

            // assert
            actual.EntityIdCode.ShouldEqual("EntityIdCode");
            actual.MeterNumber.ShouldEqual("MeterNumber");
            actual.MeterCode.ShouldEqual("MeterCode");
            actual.MeterType.ShouldEqual("MeterType");
            actual.LoadProfile.ShouldEqual("LoadProfile");
            actual.RateClass.ShouldEqual("RateClass");
            actual.RateSubClass.ShouldEqual("RateSubClass");
            actual.MeterCycle.ShouldEqual("MeterCycle");
            actual.MeterCycleDayOfMonth.ShouldEqual("MeterCycleDayOfMonth");
            actual.SpecialNeedsIndicator.ShouldEqual("SpecialNeedsIndicator");
            actual.OldMeterNumber.ShouldEqual("OldMeterNumber");
            actual.MeterOwnerIndicator.ShouldEqual("MeterOwnerIndicator");
            actual.TimeOfUse.ShouldEqual("TimeOFUse");
            actual.EspRateCode.ShouldEqual("ESPRateCode");
            actual.PricingStructureCode.ShouldEqual("PricingStructureCode");
            actual.MeterServiceVoltage.ShouldEqual("MeterServiceVoltage");
            actual.SummaryInterval.ShouldEqual("SummaryInterval");
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
        public void ParseServiceMeter_ShouldReturnChildren_WhenElementsSupplied()
        {
            // arrange
            const string xml = @"
                <root>
                    <ServiceMeterChangeLoop><ServiceMeterChange><something /></ServiceMeterChange></ServiceMeterChangeLoop>
                    <ServiceMeterRejectLoop><ServiceMeterReject><something /></ServiceMeterReject></ServiceMeterRejectLoop>
                    <ServiceMeterTOULoop><ServiceMeterTOU><something /></ServiceMeterTOU></ServiceMeterTOULoop>
                    <ServiceMeterTypeLoop><ServiceMeterType><something /></ServiceMeterType></ServiceMeterTypeLoop>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceMeter(element, namespaces);

            // assert
            actual.ShouldNotBeNull();

            actual.Changes.Count().ShouldEqual(1);
            //actual.Rejects.Count().ShouldEqual(1);
            actual.Tous.Count().ShouldEqual(1);
            actual.Types.Count().ShouldEqual(1);
        }

        [TestMethod]
        public void ParseServiceMeterChange_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <ChangeDescription>ChangeDescription</ChangeDescription>
                    <ChangeReason>ChangeReason</ChangeReason>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceMeterChange(element, namespaces);

            // assert
            actual.ChangeDescription.ShouldEqual("ChangeDescription");
            actual.ChangeReason.ShouldEqual("ChangeReason");
        }

        [TestMethod]
        public void ParseServiceMeterChange_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            const string xml = @"<root></root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceMeterChange(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }

        /*
        [TestMethod]
        public void ParseServiceMeterReject_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <RejectCode>RejectCode</RejectCode>
                    <RejectReason>RejectReason</RejectReason>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceMeterReject(element, namespaces);

            // assert
            actual.RejectCode.ShouldEqual("RejectCode");
            actual.RejectReason.ShouldEqual("RejectReason");
        }

        [TestMethod]
        public void ParseServiceMeterReject_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            const string xml = @"<root></root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceMeterReject(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }
        */

        [TestMethod]
        public void ParseServiceMeterTOU_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <MeasurementType>MeasurementType</MeasurementType>
                    <TOUCode>TOUCode</TOUCode>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceMeterTou(element, namespaces);

            // assert
            actual.MeasurementType.ShouldEqual("MeasurementType");
            actual.TouCode.ShouldEqual("TOUCode");
        }

        [TestMethod]
        public void ParseServiceMeterTOU_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            const string xml = @"<root></root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceMeterTou(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        public void ParseServiceMeterType_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <ChangeReason>ChangeReason</ChangeReason>
                    <EndMeterRead>EndMeterRead</EndMeterRead>
                    <MeterMultiplier>MeterMultiplier</MeterMultiplier>
                    <MeterType>MeterType</MeterType>
                    <NumberOfDials>NumberOfDials</NumberOfDials>
                    <ProductType>ProductType</ProductType>
                    <StartMeterRead>StartMeterRead</StartMeterRead>
                    <TimeOfUse>TimeOfUse</TimeOfUse>
                    <TimeOfUse2>TimeOfUse2</TimeOfUse2>
                    <UnmeteredDescription>UnmeteredDescription</UnmeteredDescription>
                    <UnmeteredNumberOfDevices>UnmeteredNumberOfDevices</UnmeteredNumberOfDevices>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceMeterType(element, namespaces);

            // assert
            actual.ChangeReason.ShouldEqual("ChangeReason");
            actual.EndMeterRead.ShouldBeNull();
            actual.MeterMultiplier.ShouldEqual("MeterMultiplier");
            actual.MeterType.ShouldEqual("MeterType");
            actual.NumberOfDials.ShouldEqual("NumberOfDials");
            actual.ProductType.ShouldEqual("ProductType");
            actual.StartMeterRead.ShouldEqual("StartMeterRead");
            actual.TimeOfUse.ShouldEqual("TimeOfUse");
            actual.TimeOfUse2.ShouldEqual("TimeOfUse2");
            actual.UnmeteredDescription.ShouldEqual("UnmeteredDescription");
            actual.UnmeteredNumberOfDevices.ShouldEqual("UnmeteredNumberOfDevices");
        }

        [TestMethod]
        public void ParseServiceMeterType_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            const string xml = @"<root></root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceMeterType(element, namespaces);

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
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceReject(element, namespaces);

            // assert
            actual.RejectCode.ShouldEqual("RejectCode");
            actual.RejectReason.ShouldEqual("RejectReason");
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

        [TestMethod]
        public void ParseServiceStatus_ShouldReturnObjectWithValidValues_WhenXmlIsValid()
        {
            // arrange
            const string xml = @"
                <root>
                    <StatusCode>StatusCode</StatusCode>
                    <StatusReason>StatusReason</StatusReason>
                    <StatusType>StatusType</StatusType>
                </root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceStatus(element, namespaces);

            // assert
            actual.StatusCode.ShouldEqual("StatusCode");
            actual.StatusReason.ShouldEqual("StatusReason");
            actual.StatusType.ShouldEqual("StatusType");
        }

        [TestMethod]
        public void ParseServiceStatus_ShouldParse_EvenIfNodeIsEmpty()
        {
            // arrange
            const string xml = @"<root></root>";

            // act
            
            var element = XElement.Parse(xml);
            var actual = concern.ParseServiceStatus(element, namespaces);

            // assert
            actual.ShouldNotBeNull();
        }
    }
}

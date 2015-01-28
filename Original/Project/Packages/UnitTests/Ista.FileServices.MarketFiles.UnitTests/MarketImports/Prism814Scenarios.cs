using System.IO;
using System.Linq;
using System.Text;
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
    public class Prism814Scenarios
    {
        private IClientDataAccess clientDataAccess;
        private ILogger logger;

        private Import814Prism concern;

        [TestInitialize]
        public void StartUp()
        {
            clientDataAccess = MockRepository.GenerateMock<IClientDataAccess>();
            clientDataAccess
                .Stub(x => x.IdentifyMarket(Arg<string>.Is.Anything))
                .Return(1);
            logger = MockRepository.GenerateStub<ILogger>();

            concern = new Import814Prism(clientDataAccess, logger);
        }
        
        [TestMethod, TestCategory("Scenario")]
        public void TX_SampleFile_ActionCode_PD()
        {
            // arrange
            const string file = @"SH|SELTX007923311ENR|STAR3552730B362893786|C|I|
01|SELTX007923311ENR|TX|S|STAR3552730B362893786|2234364|007923311|AEP TEXAS NORTH (ERCOT)|148055531|STARTEX POWER||20130110|||20130110|181639|PD|
10|SELTX007923311ENR|EL|MCI||PD20130110184806001|C|A||||||||||||||||||||||||||||||||||||10204049739986221|
SH|SELTX007923311ENR|STAR3552730B362893787|C|I|
01|SELTX007923311ENR|TX|S|STAR3552730B362893787|2234363|007923311|AEP TEXAS NORTH (ERCOT)|148055531|STARTEX POWER||20130110|||20130110|181639|PD|
10|SELTX007923311ENR|EL|MCI||PD20130110184806002|C|A||||||||||||||||||||||||||||||||||||10204049751062982|
TL|2";

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = concern.Parse(stream);

            // assert
            actual.TransactionActualCount.ShouldEqual(2);
            actual.TransactionAuditCount.ShouldEqual(2);

            var header = actual.Headers.FirstOrDefault() as Type814Header;

            header.TransactionSetPurposeCode.ShouldEqual("S");
            header.TransactionNbr.ShouldEqual("STAR3552730B362893786");
            header.ReferenceNbr.ShouldEqual("2234364");
            header.TdspDuns.ShouldEqual("007923311");
            header.TdspName.ShouldEqual("AEP TEXAS NORTH (ERCOT)");
            header.CrDuns.ShouldEqual("148055531");
            header.CrName.ShouldEqual("STARTEX POWER");
            header.TransactionDate.ShouldEqual("20130110");
            header.TransactionSetId.ShouldEqual("PD");
            header.ActionCode.ShouldEqual("PD");
            //header.POLRClass.ShouldBeNull();

            header.Services.Count().ShouldEqual(1);
            var service = header.Services[0];

            service.ServiceType1.ShouldEqual("EL");
            service.ServiceTypeCode1.ShouldEqual("SH");
            service.ServiceType2.ShouldEqual("MCI");
            service.ServiceTypeCode2.ShouldEqual("SH");
            service.ServiceType3.ShouldBeNull();
            service.ServiceTypeCode3.ShouldBeEmpty();
            service.AssignId.ShouldEqual("PD20130110184806001");
            service.ActionCode.ShouldEqual("A");
            service.EsiId.ShouldEqual("10204049739986221");
        }

        [TestMethod, TestCategory("Scenario")]
        public void TX_SampleFile_Action_A()
        {
            // arrange
            const string file = @"SH|SELTX183529049ENR|230110410201301021325371480555|E|I|
01|SELTX183529049ENR|TX|S|230110410201301021325371480555|2227595|957877905|CENTERPOINT ENERGY|148055531|STAR ELECTRICITY INC DBA STARTEX POWER (LSE)||20130102|||20130102|133747|5|ERCOT|183529049|
05|SELTX183529049ENR|8R|WYNNEWOOD AT WORTHAM||||10225 WORTHAM BLVD #3205||HOUSTON|TX|77065|
10|SELTX183529049ENR|EL|CE|MVI|002013002M1002529000|E|A||||||||||||||||||||||||||SA|||||||||HU|1008901023807666160100|D|||01|||||||N|
11|SELTX183529049ENR||||20130102|
30|SELTX183529049ENR|I66337568|||KHMON|RESIDENTIAL||RESLOWR_COAST_IDR_WS_NOTOU||10|||||||||||||||||||||||||||||||||||||||||AMSR|
35|SELTX183529049ENR|KHMON|1|5.0|51|
TL|1";

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = concern.Parse(stream);

            // assert
            actual.TransactionActualCount.ShouldEqual(1);
            actual.TransactionAuditCount.ShouldEqual(1);

            var header = actual.Headers.FirstOrDefault() as Type814Header;

            header.TransactionSetPurposeCode.ShouldEqual("S");
            header.TransactionNbr.ShouldEqual("230110410201301021325371480555");
            header.ReferenceNbr.ShouldEqual("2227595");
            header.TdspDuns.ShouldEqual("957877905");
            header.TdspName.ShouldEqual("CENTERPOINT ENERGY");
            header.CrDuns.ShouldEqual("148055531");
            header.CrName.ShouldEqual("STAR ELECTRICITY INC DBA STARTEX POWER (LSE)");
            header.TransactionDate.ShouldEqual("20130102");
            header.TransactionSetId.ShouldEqual("5");
            header.ActionCode.ShouldEqual("5");
            //header.POLRClass.ShouldBeNull();

            header.Names.Count().ShouldEqual(1);
            var name = header.Names[0];

            name.EntityIdType.ShouldEqual("8R");
            name.EntityName.ShouldEqual("WYNNEWOOD AT WORTHAM");
            name.EntityName2.ShouldBeNull();
            name.EntityName3.ShouldBeNull();
            name.Address1.ShouldEqual("10225 WORTHAM BLVD #3205");
            name.Address2.ShouldBeNull();
            name.City.ShouldEqual("HOUSTON");
            name.State.ShouldEqual("TX");
            name.PostalCode.ShouldEqual("77065");

            header.Services.Count().ShouldEqual(1);
            var service = header.Services[0];

            // line 10
            service.ServiceType1.ShouldEqual("EL");
            service.ServiceTypeCode1.ShouldEqual("SH");
            service.ServiceType2.ShouldEqual("CE");
            service.ServiceTypeCode2.ShouldEqual("SH");
            service.ServiceType3.ShouldEqual("MVI");
            service.ServiceTypeCode3.ShouldEqual("SH");
            service.AssignId.ShouldEqual("002013002M1002529000");
            service.ActionCode.ShouldEqual("A");
            service.StationId.ShouldEqual("SA");
            service.ServiceType4.ShouldEqual("HU");
            service.ServiceTypeCode4.ShouldEqual("SH");
            service.EsiId.ShouldEqual("1008901023807666160100");
            service.DistributionLossFactorCode.ShouldEqual("D");
            service.MaintenanceTypeCode.ShouldBeNull();
            service.PremiseType.ShouldEqual("01");

            // line 11
            service.EsiIdStartDate.ShouldBeNull();
            service.SpecialReadSwitchDate.ShouldEqual("20130102");

            service.Meters.Count().ShouldEqual(1);
            var meter = service.Meters[0];

            meter.MeterNumber.ShouldEqual("I66337568");
            meter.MeterType.ShouldEqual("KHMON");
            meter.RateClass.ShouldEqual("RESIDENTIAL");
            meter.LoadProfile.ShouldEqual("RESLOWR_COAST_IDR_WS_NOTOU");
            meter.MeterCycle.ShouldEqual("10");
            meter.SpecialNeedsIndicator.ShouldBeNull();
            meter.MeterOwnerIndicator.ShouldBeNull();
            meter.MeterCycleDayOfMonth.ShouldBeNull();

            meter.Types.Count().ShouldEqual(1);
            var meterType = meter.Types[0];

            // line 30
            meterType.ProductType.ShouldBeEmpty();
            meterType.UnmeteredNumberOfDevices.ShouldBeEmpty();
            meterType.UnmeteredDescription.ShouldBeEmpty();

            // line 35
            meterType.MeterType.ShouldEqual("KHMON");
            meterType.MeterMultiplier.ShouldEqual("1");
            meterType.NumberOfDials.ShouldEqual("5.0");
            meterType.TimeOfUse.ShouldEqual("51");
        }

        [TestMethod, TestCategory("Scenario")]
        public void TX_Nueces_SampleFile()
        {
            // arrange
            const string file = @"SH|TXUTX183529049ENR|467127192013011416473817333702|E|I|
01|TXUTX183529049ENR|TX|S|467127192013011416473817333702|MOU38DAY1TXUNUEB|0088288574800|NUECES|1733370281400|TXU SESCO ENERGY SERVICES COMPANY||20130114|||20130213|170506|5|ERCOT|183529049|
05|TXUTX183529049ENR|8R|MIDAS||||133 CLEARWATER XING||SINTON|TX|78387|
10|TXUTX183529049ENR|EL|CE|MVI|1|E|A||28282501||||||||||||||||||||||||ORNGRVS||||||||||101383000282825|B|||02|||||||N|
11|TXUTX183529049ENR||||20130125|
30|TXUTX183529049ENR|UNMETERED||||DEV07|6|NMLIGHT_SOUTH_NIDR_NWS_NOTOU||04|SD||1||||||||||||||||||||||||100W HPS|
SH|TXUTX183529049ENR|467127212013011416473817333702|E|I|
01|TXUTX183529049ENR|TX|S|467127212013011416473817333702|MOU28DAY1TXUNUEB|0088288574800|NUECES|1733370281400|TXU SESCO ENERGY SERVICES COMPANY||20130114|||20130213|170506|5|ERCOT|183529049|
05|TXUTX183529049ENR|8R|SERENA WILLIAMS||||529 E KLEBERG||SINTON|TX|78387|
10|TXUTX183529049ENR|EL|CE|MVI|1|E|A||28722402||||||||||||||||||||||||FREERS||||||||||101383000287224|B|||01|||||||N|
11|TXUTX183529049ENR||||20130205|
30|TXUTX183529049ENR|NONE||||||RESHIWR_SOUTH_NIDR_NWS_NOTOU||07|
SH|TXUTX183529049ENR|467127312013011416474617333702|E|I|
01|TXUTX183529049ENR|TX|S|467127312013011416474617333702|MOU28DAY1TXUNUEC|0088288574800|NUECES|1733370281400|TXU SESCO ENERGY SERVICES COMPANY||20130114|||20130213|170506|5|ERCOT|183529049|
05|TXUTX183529049ENR|8R|JOHN MCENROE||||529 E KLEBERG||SINTON|TX|78387|
10|TXUTX183529049ENR|EL|CE|MVI|1|E|A||28722403||||||||||||||||||||||||FREERS||||||||||101383000287224|B|||01|||||||N|
11|TXUTX183529049ENR||||20130220|
30|TXUTX183529049ENR|NONE||||||RESHIWR_SOUTH_NIDR_NWS_NOTOU||07|
SH|TXUTX183529049ENR|467127352013011416474717333702|E|I|
01|TXUTX183529049ENR|TX|S|467127352013011416474717333702|MOU28DAY1TXUNUEA|0088288574800|NUECES|1733370281400|TXU SESCO ENERGY SERVICES COMPANY||20130114|||20130213|170506|5|ERCOT|183529049|
05|TXUTX183529049ENR|8R|BRUCE JENNER||||529 E KLEBERG||SINTON|TX|78387|
10|TXUTX183529049ENR|EL|CE|MVI|1|E|A||28722401||||||||||||||||||||||||FREERS||||||||||101383000287224|B|||01|||||||N|
11|TXUTX183529049ENR||||20130129|
30|TXUTX183529049ENR|NONE||||||RESHIWR_SOUTH_NIDR_NWS_NOTOU||07|
TL|4";

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = concern.Parse(stream);

            // assert
            actual.TransactionActualCount.ShouldEqual(4);
            actual.TransactionAuditCount.ShouldEqual(4);

            var header = actual.Headers.FirstOrDefault() as Type814Header;

            header.TransactionSetPurposeCode.ShouldEqual("S");
            header.TransactionNbr.ShouldEqual("467127192013011416473817333702");
            header.ReferenceNbr.ShouldEqual("MOU38DAY1TXUNUEB");
            header.TdspDuns.ShouldEqual("0088288574800");
            header.TdspName.ShouldEqual("NUECES");
            header.CrDuns.ShouldEqual("1733370281400");
            header.CrName.ShouldEqual("TXU SESCO ENERGY SERVICES COMPANY");
            header.TransactionDate.ShouldEqual("20130114");
            header.TransactionSetId.ShouldEqual("5");
            header.ActionCode.ShouldEqual("5");
            //header.POLRClass.ShouldBeNull();

            header.Names.Count().ShouldEqual(1);
            var name = header.Names[0];

            name.EntityIdType.ShouldEqual("8R");
            name.EntityName.ShouldEqual("MIDAS");
            name.Address1.ShouldEqual("133 CLEARWATER XING");
            name.City.ShouldEqual("SINTON");
            name.State.ShouldEqual("TX");
            name.PostalCode.ShouldEqual("78387");

            header.Services.Count().ShouldEqual(1);
            var service = header.Services[0];

            service.AssignId.ShouldEqual("1");
            service.ActionCode.ShouldEqual("A");
            service.DistributionLossFactorCode.ShouldEqual("B");
            service.EsiId.ShouldEqual("101383000282825");
            //service.MembershipID.ShouldEqual("28282501"); // TODO: verify that this is the membership id
            service.PremiseType.ShouldEqual("02");
            service.ServiceType1.ShouldEqual("EL");
            service.ServiceType2.ShouldEqual("CE");
            service.ServiceType3.ShouldEqual("MVI");
            service.ServiceType4.ShouldBeNull();
            service.ServiceTypeCode1.ShouldEqual("SH");
            service.ServiceTypeCode2.ShouldEqual("SH");
            service.ServiceTypeCode3.ShouldEqual("SH");
            service.ServiceTypeCode4.ShouldBeEmpty();
            service.SpecialReadSwitchDate.ShouldEqual("20130125");
            service.StationId.ShouldEqual("ORNGRVS");

            service.Meters.Count().ShouldEqual(1);
            var meter = service.Meters[0];

            meter.LoadProfile.ShouldEqual("NMLIGHT_SOUTH_NIDR_NWS_NOTOU");
            meter.MeterCycle.ShouldEqual("04");
            meter.MeterNumber.ShouldEqual("UNMETERED");
            meter.RateClass.ShouldEqual("DEV07");
            meter.RateSubClass.ShouldEqual("6");

            // verifying the membership ids
            //((Type814Header)actual.Headers[0]).Services[0].MembershipID.ShouldEqual("28282501");
            //((Type814Header)actual.Headers[1]).Services[0].MembershipID.ShouldEqual("28722402");
            //((Type814Header)actual.Headers[2]).Services[0].MembershipID.ShouldEqual("28722403");
            //((Type814Header)actual.Headers[3]).Services[0].MembershipID.ShouldEqual("28722401");
        }

        [TestMethod, TestCategory("Scenario")]
        public void NJ_WithArrearage_SampleFile()
        {
            // arrange
            const string file = @"SH|TXUTX183529049ENR|467127192013011416473817333702|E|I|
01|TXUTX183529049ENR|TX|S|467127192013011416473817333702|MOU38DAY1TXUNUEB|0088288574800|NUECES|1733370281400|TXU SESCO ENERGY SERVICES COMPANY||20130114|||20130213|170506|5|ERCOT|183529049|
05|TXUTX183529049ENR|8R|MIDAS||||133 CLEARWATER XING||SINTON|TX|78387|
10|TXUTX183529049ENR|EL|CE||768158|E|A||PE000009458043846616|6278132729||304642||||||||||||||||99||LDC|DUAL|N|||||||||||||||||||||||||||||||||||||NETMETER|777|87|
11|TXUTX183529049ENR||||20130125|
30|TXUTX183529049ENR|UNMETERED||||DEV07|6|NMLIGHT_SOUTH_NIDR_NWS_NOTOU||04|SD||1||||||||||||||||||||||||100W HPS|
SH|TXUTX183529049ENR|467127212013011416473817333702|E|I|
01|TXUTX183529049ENR|TX|S|467127212013011416473817333702|MOU28DAY1TXUNUEB|0088288574800|NUECES|1733370281400|TXU SESCO ENERGY SERVICES COMPANY||20130114|||20130213|170506|5|ERCOT|183529049|
05|TXUTX183529049ENR|8R|SERENA WILLIAMS||||529 E KLEBERG||SINTON|TX|78387|
10|TXUTX183529049ENR|EL|CE||768158|E|A||PE000009458043846616|6278132729||304642||||||||||||||||99||LDC|DUAL|N|||||||||||||||||||||||||||||||||||||NETMETER|777|87|
11|TXUTX183529049ENR||||20130205|
30|TXUTX183529049ENR|NONE||||||RESHIWR_SOUTH_NIDR_NWS_NOTOU||07|
SH|TXUTX183529049ENR|467127312013011416474617333702|E|I|
01|TXUTX183529049ENR|TX|S|467127312013011416474617333702|MOU28DAY1TXUNUEC|0088288574800|NUECES|1733370281400|TXU SESCO ENERGY SERVICES COMPANY||20130114|||20130213|170506|5|ERCOT|183529049|
05|TXUTX183529049ENR|8R|JOHN MCENROE||||529 E KLEBERG||SINTON|TX|78387|
10|TXUTX183529049ENR|EL|CE||768158|E|A||PE000009458043846616|6278132729||304642||||||||||||||||99||LDC|DUAL|N|||||||||||||||||||||||||||||||||||||NETMETER|777|87|
11|TXUTX183529049ENR||||20130220|
30|TXUTX183529049ENR|NONE||||||RESHIWR_SOUTH_NIDR_NWS_NOTOU||07|
SH|TXUTX183529049ENR|467127352013011416474717333702|E|I|
01|TXUTX183529049ENR|TX|S|467127352013011416474717333702|MOU28DAY1TXUNUEA|0088288574800|NUECES|1733370281400|TXU SESCO ENERGY SERVICES COMPANY||20130114|||20130213|170506|5|ERCOT|183529049|
05|TXUTX183529049ENR|8R|BRUCE JENNER||||529 E KLEBERG||SINTON|TX|78387|
10|TXUTX183529049ENR|EL|CE||768158|E|A||PE000009458043846616|6278132729||304642||||||||||||||||99||LDC|DUAL|N|||||||||||||||||||||||||||||||||||||NETMETER|777|87|
11|TXUTX183529049ENR||||20130129|
30|TXUTX183529049ENR|NONE||||||RESHIWR_SOUTH_NIDR_NWS_NOTOU||07|
TL|4";
            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = concern.Parse(stream);

            // assert
            actual.TransactionActualCount.ShouldEqual(4);
            actual.TransactionAuditCount.ShouldEqual(4);

            var header = actual.Headers.FirstOrDefault() as Type814Header;

            header.TransactionSetPurposeCode.ShouldEqual("S");
            header.TransactionNbr.ShouldEqual("467127192013011416473817333702");
            header.ReferenceNbr.ShouldEqual("MOU38DAY1TXUNUEB");
            header.TdspDuns.ShouldEqual("0088288574800");
            header.TdspName.ShouldEqual("NUECES");
            header.CrDuns.ShouldEqual("1733370281400");
            header.CrName.ShouldEqual("TXU SESCO ENERGY SERVICES COMPANY");
            header.TransactionDate.ShouldEqual("20130114");
            header.TransactionSetId.ShouldEqual("5");
            header.ActionCode.ShouldEqual("5");
            //header.POLRClass.ShouldBeNull();

            header.Names.Count().ShouldEqual(1);
            var name = header.Names[0];

            name.EntityIdType.ShouldEqual("8R");
            name.EntityName.ShouldEqual("MIDAS");
            name.Address1.ShouldEqual("133 CLEARWATER XING");
            name.City.ShouldEqual("SINTON");
            name.State.ShouldEqual("TX");
            name.PostalCode.ShouldEqual("78387");

            header.Services.Count().ShouldEqual(1);
            var service = header.Services[0];

            service.AssignId.ShouldEqual("768158");
            service.ActionCode.ShouldEqual("A");
            service.DistributionLossFactorCode.ShouldBeNull();
            service.EsiId.ShouldBeNull();
            //service.MembershipID.ShouldEqual("28282501"); // TODO: verify that this is the membership id
            service.PremiseType.ShouldBeNull();
            service.ServiceType1.ShouldEqual("EL");
            service.ServiceType2.ShouldEqual("CE");
            service.ServiceType3.ShouldBeNull();
            service.ServiceType4.ShouldBeNull();
            service.ServiceTypeCode1.ShouldEqual("SH");
            service.ServiceTypeCode2.ShouldEqual("SH");
            service.ServiceTypeCode3.ShouldBeEmpty();
            service.DaysInArrears.ShouldEqual("87");
            service.ServiceTypeCode4.ShouldBeEmpty();
            service.SpecialReadSwitchDate.ShouldEqual("20130125");
            service.StationId.ShouldBeNull();

            service.Meters.Count().ShouldEqual(1);
            var meter = service.Meters[0];

            meter.LoadProfile.ShouldEqual("NMLIGHT_SOUTH_NIDR_NWS_NOTOU");
            meter.MeterCycle.ShouldEqual("04");
            meter.MeterNumber.ShouldEqual("UNMETERED");
            meter.RateClass.ShouldEqual("DEV07");
            meter.RateSubClass.ShouldEqual("6");
        }
    }
}

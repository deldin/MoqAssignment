using System.Threading;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Models;
using Ista.FileServices.MarketFiles.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Ista.FileServices.MarketFiles.UnitTests.MarketExports
{
    [TestClass]
    public class Prism814Scenarios
    {
        private IClientDataAccess clientDataAccess;
        private IMarket814Export exportDataAccess;
        private ILogger logger;

        private Export814Prism concern;

        [TestInitialize]
        public void SetUp()
        {
            clientDataAccess = MockRepository.GenerateMock<IClientDataAccess>();
            exportDataAccess = MockRepository.GenerateMock<IMarket814Export>();
            logger = MockRepository.GenerateStub<ILogger>();

            concern = new Export814Prism(clientDataAccess, exportDataAccess, logger);
        }

        [TestMethod]
        public void When_Exporting_814_For_Texas_Market_Then_Tdsp_Duns_Is_Ercot()
        {
            var cspDunsPorts = new[]
            {
                new CspDunsPortModel
                {
                    CspDunsId = 1,
                    CspDunsPortId = 1,
                    LdcDuns = "MockTdspDuns_One",
                    LdcShortName = "MockTdspDuns_One",
                    Duns = "MockDuns_One",
                    TradingPartnerId = "Mock_Trade_{DUNS}",
                    ProviderId = 1
                }
            };

            var headers = new[]
            {
                new Type814Header {HeaderKey = 1, ActionCode = "01", TdspDuns = "Not_Used", TdspName = "Mock", CrName = "Mock", },
                new Type814Header {HeaderKey = 2, ActionCode = "01", TdspDuns = "Not_Used", TdspName = "Mock", CrName = "Mock", },
                new Type814Header {HeaderKey = 3, ActionCode = "01", TdspDuns = "Not_Used", TdspName = "Mock", CrName = "Mock", }
            };

            // arrange
            clientDataAccess.Expect(x => x.ListCspDunsPort())
                .Return(cspDunsPorts);

            clientDataAccess.Expect(x => x.IdentifyMarket(Arg<string>.Is.Anything))
                .Return(1);

            exportDataAccess.Expect(x => x.ListUnprocessed(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg.Is(1)))
                .Return(headers);

            exportDataAccess.Expect(x => x.ListServices(Arg<int>.Is.Anything))
                .Return(new Type814Service[0]);

            exportDataAccess.Expect(x => x.ListNames(Arg<int>.Is.Anything))
                .Return(new Type814Name[0]);

            // act
            var results = concern.Export(CancellationToken.None);

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Length);

            var firstResult = results[0];
            Assert.IsNotNull(firstResult);
            Assert.AreEqual("Mock_Trade_183529049ENR", firstResult.TradingPartnerId);
            Assert.AreEqual("MockTdspDuns_One", firstResult.LdcShortName);

            Assert.AreEqual(3, firstResult.HeaderCount);
            CollectionAssert.Contains(firstResult.HeaderKeys, 1);
            CollectionAssert.Contains(firstResult.HeaderKeys, 2);
            CollectionAssert.Contains(firstResult.HeaderKeys, 3);
        }

        [TestMethod]
        public void Should_Produce_The_Same_Content_As_A_Sample_File()
        {
            const string expectedContent =
                @"SH|SELTX183529049ENR|2276133|D|O|
01|SELTX183529049ENR|TX|Q|2276133||957877905|CENTERPOINT ENERGY|148055531|STARTEX POWER||20130309|||||24|ERCOT|183529049|
05|SELTX183529049ENR|8R|COTTONWOOD CAPITAL PROPERTY MANAGEMENT II LLC AS AGENT FOR T||||12700 STAFFORD RD BS2||STAFFORD|TX|77477|||2815648277|2815648277|FRANK MILLER||||||
10|SELTX183529049ENR|EL|CE|MVO|1|D|||||||||||||||||||||||||||||||||||||1008901023817063480105||||||||||||||||
11|SELTX183529049ENR|||||||||||||||20130312||||
TL|1";

            // arrange
            var port = new CspDunsPortModel
            {
                CspDunsId = 1,
                CspDunsPortId = 1,
                Duns = "148055531",
                LdcDuns = string.Empty,
                LdcShortName = string.Empty,
                TradingPartnerId = "SELTX{DUNS}",
                ProviderId = 1,
                FileType = string.Empty,
            };

            var header = new Type814Header
            {
                HeaderKey = 3893482,
                TransactionSetPurposeCode = "Q",
                TransactionNbr = "2276133",
                TransactionDate = "20130309",
                ActionCode = "24",
                TdspDuns = "957877905",
                TdspName = "Centerpoint Energy",
                CrDuns = "148055531",
                CrName = "StarTex Power",
                TransactionTypeId = 13,
                MarketId = 1,
                ProviderId = 1,
                TransactionQueueTypeId = 1,
                ReferenceNbr = string.Empty,
            };

            var name = new Type814Name
            {
                HeaderKey = 3893482,
                NameKey = 2470182,
                EntityIdType = "8R",
                EntityName = "COTTONWOOD CAPITAL PROPERTY MANAGEMENT II LLC AS AGENT FOR T",
                EntityName2 = string.Empty,
                EntityName3 = string.Empty,
                Address1 = "12700 STAFFORD RD BS2",
                Address2 = string.Empty,
                City = "STAFFORD",
                State = "TX",
                PostalCode = "77477",
                ContactName = "FRANK MILLER",
                ContactPhoneNbr1 = "281-564-8277",
                ContactPhoneNbr2 = string.Empty,
                EntityFirstName = string.Empty,
                EntityLastName = "Cottonwood Capital Property Management II LLC as ",
                EntityEmail = string.Empty,
            };

            var service = new Type814Service
            {
                HeaderKey = 3893482,
                ServiceKey = 3892992,
                ServiceTypeCode1 = "SH",
                ServiceType1 = "EL",
                ServiceTypeCode2 = "SH",
                ServiceType2 = "CE",
                ServiceTypeCode3 = "SH",
                ServiceType3 = "MVO",
                MaintenanceTypeCode = "002",
                EsiId = "1008901023817063480105",
                SpecialNeedsIndicator = "N",
                SpecialReadSwitchDate = "20130312",
                EspAccountNumber = "291920",
                PaymentOption = "N",
            };

            clientDataAccess.Expect(x => x.ListCspDunsPort())
                .Return(new[] {port});

            clientDataAccess.Expect(x => x.IdentifyMarket(Arg<string>.Is.Anything))
                .Return(1);

            exportDataAccess.Expect(x => x.ListUnprocessed(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg.Is(1)))
                .Return(new[] {header});

            exportDataAccess.Expect(x => x.ListNames(Arg<int>.Is.Anything))
                .Return(new[] {name});

            exportDataAccess.Expect(x => x.ListServices(Arg<int>.Is.Anything))
                .Return(new[] {service});

            exportDataAccess.Stub(x => x.ListServiceStatuses(Arg<int>.Is.Anything))
                .Return(new Type814ServiceStatus[0]);

            exportDataAccess.Stub(x => x.ListServiceRejects(Arg<int>.Is.Anything))
                .Return(new Type814ServiceReject[0]);

            exportDataAccess.Stub(x => x.ListServiceMeters(Arg<int>.Is.Anything))
                .Return(new Type814ServiceMeter[0]);
            
            // act
            var results = concern.Export(CancellationToken.None);

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Length);

            var result = results[0];
            Assert.IsNotNull(result);
            
            Assert.AreEqual(1, result.HeaderCount);
            CollectionAssert.Contains(result.HeaderKeys, 3893482);
            
            result.FinalizeDocument(1);
            Assert.AreEqual(expectedContent, result.Content);
        }
    }
}

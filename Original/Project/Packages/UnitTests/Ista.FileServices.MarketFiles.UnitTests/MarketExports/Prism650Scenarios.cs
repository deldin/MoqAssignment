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
    public class Prism650Scenarios
    {
        private IClientDataAccess clientDataAccess;
        private IMarket650Export exportDataAccess;
        private ILogger logger;

        private Export650Prism concern;

        [TestInitialize]
        public void SetUp()
        {
            clientDataAccess = MockRepository.GenerateMock<IClientDataAccess>();
            exportDataAccess = MockRepository.GenerateMock<IMarket650Export>();
            logger = MockRepository.GenerateStub<ILogger>();

            concern = new Export650Prism(clientDataAccess, exportDataAccess, logger);
        }

        [TestMethod]
        public void Should_Produce_The_Same_Content_As_A_Sample_File()
        {
            const string expectedContent =
                @"SH|ACTTX957877905MTR|8842008|O|
01|ACTTX957877905MTR|TX|13|20130319|8842008|8841948|79|IT|TYRA BROWN|||1800 EL PASEO ST|APT 205|HOUSTON|TX|77054|BROWN TYRA|6824338859|6824338859|CENTERPOINT ENERGY|957877905|ACCENT ENERGY TEXAS|133305370|201303191003|
10|ACTTX957877905MTR|RC001|01|1008901018191437145100|N|20130319||N||||||||||||||||||||||||||||||
TL|1";
            
            // arrange
            var port = new CspDunsPortModel
            {
                CspDunsId = 1,
                CspDunsPortId = 1,
                Duns = "133305370",
                LdcDuns = string.Empty,
                LdcShortName = string.Empty,
                TradingPartnerId = "ACTTX{DUNS}",
                ProviderId = 1,
                FileType = string.Empty,
            };

            var header = new Type650Header
            {
                HeaderKey = 275531,
                TransactionNbr = "8842008",
                TransactionSetPurposeCode = "13",
                TransactionDate = "20130319",
                TransactionType = "79",
                ReferenceNbr = "8841948",
                ActionCode = "IT",
                TdspName = "Centerpoint Energy",
                TdspDuns = "957877905",
                CrName = "Accent Energy Texas",
                CrDuns = "133305370",
                ProcessedReceivedDateTime = "201303191003",
                TransactionTypeId = 18,
                MarketId = 1,
                ProviderId = 1,
            };
                    
            var name = new Type650Name
            {
                HeaderKey = 275531,
                NameKey = 122923,
                EntityName = "Tyra Brown",
                EntityName2 = string.Empty,
                EntityName3 = string.Empty,
                EntityIdType = "8R",
                Address1 = "1800 EL PASEO ST",
                Address2 = "APT 205",
                City = "HOUSTON",
                State = "TX",
                PostalCode = "77054",
                ContactName = "Brown Tyra",
                ContactPhoneNbr1 = "682-433-8859",
                ContactPhoneNbr2 = "682-433-8859"
            };

            var service = new Type650Service
            {
                HeaderKey = 275531,
                ServiceKey = 275393,
                PurposeCode = "RC001",
                PriorityCode = "01",
                EsiId = "1008901018191437145100",
                SpecialProcessCode = "N",
                ServiceReqDate = "20130319",
                CallAhead = "N",
                NotBeforeDate = string.Empty,
                PremLocation = string.Empty,
                ReportRemarks = string.Empty,
                Directions = string.Empty,
                MeterNbr = string.Empty,
                Membership = string.Empty,
                RemarksPermanentSuspend = string.Empty,
                DisconnectAuthorization = string.Empty
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
            
            exportDataAccess.Stub(x => x.ListServiceRejects(Arg<int>.Is.Anything))
                .Return(new Type650ServiceReject[0]);

            exportDataAccess.Stub(x => x.ListServiceMeters(Arg<int>.Is.Anything))
                .Return(new Type650ServiceMeter[0]);
            
            // act
            var results = concern.Export(CancellationToken.None);

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Length);

            var result = results[0];
            Assert.IsNotNull(result);
            
            Assert.AreEqual(1, result.HeaderCount);
            CollectionAssert.Contains(result.HeaderKeys, 275531);
            
            result.FinalizeDocument(1);
            Assert.AreEqual(expectedContent, result.Content);
        }
    }
}

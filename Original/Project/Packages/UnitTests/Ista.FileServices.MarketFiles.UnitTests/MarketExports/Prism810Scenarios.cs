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
    public class Prism810Scenarios
    {
        private IClientDataAccess clientDataAccess;
        private IMarket810Export exportDataAccess;
        private ILogger logger;

        private Export810Prism concern;

        [TestInitialize]
        public void SetUp()
        {
            clientDataAccess = MockRepository.GenerateMock<IClientDataAccess>();
            exportDataAccess = MockRepository.GenerateMock<IMarket810Export>();
            logger = MockRepository.GenerateStub<ILogger>();

            concern = new Export810Prism(clientDataAccess, exportDataAccess, logger);
        }

        [TestMethod]
        public void When_Exporting_810_Then_LDC_Is_Loaded_By_Tdsp_Duns_From_Header()
        {
            // arrange
            var cspDunsPorts = new[]
            {
                new CspDunsPortModel
                {
                    CspDunsId = 17,
                    CspDunsPortId = 117,
                    LdcDuns = "MockTdspDuns_One",
                    LdcShortName = "MockTdspDuns_One",
                    Duns = "MockDuns_One",
                    TradingPartnerId = "Mock_Trade_{DUNS}",
                    ProviderId = 1
                }
            };

            var headers = new[]
            {
                new Type810Header
                {
                    HeaderKey = 1,
                    TdspDuns = "Mock_Tdsp_Duns",
                    TdspName = "Mock_Tdsp",
                    CrDuns = "Mock_Cr_Duns",
                    CrName = "Mock_Cr",
                    TotalAmount = "12.33",
                }
            };

            clientDataAccess.Stub(x => x.ListCspDunsPort())
                .Return(cspDunsPorts);

            exportDataAccess.Stub(x => x.ListUnprocessed(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg.Is(1)))
                .Return(headers);

            clientDataAccess.Stub(x => x.LoadDunsByCspDunsId(Arg.Is(17)))
                .Return("Mock_Csp_Duns");

            clientDataAccess.Expect(x => x.LoadLdcByTdspDuns(Arg.Is("Mock_Tdsp_Duns")))
                .Return(new LdcModel
                {
                    LdcId = 23,
                    MarketId = 33
                });

            // act
            var results = concern.Export(CancellationToken.None);

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Length);

            var result = results[0];
            Assert.IsNotNull(result);
            Assert.AreEqual("Mock_Trade_Mock_Tdsp_DunsINV", result.TradingPartnerId);

            clientDataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Exporting_810_Then_DUNS_For_CSP_Is_Loaded_By_Identifier()
        {
            // arrange
            var cspDunsPorts = new[]
            {
                new CspDunsPortModel
                {
                    CspDunsId = 17,
                    CspDunsPortId = 117,
                    LdcDuns = "MockTdspDuns_One",
                    LdcShortName = "MockTdspDuns_One",
                    Duns = "MockDuns_One",
                    TradingPartnerId = "Mock_Trade_{DUNS}",
                    ProviderId = 1
                }
            };

            var headers = new[]
            {
                new Type810Header
                {
                    HeaderKey = 1,
                    TdspDuns = "Mock_Tdsp_Duns",
                    TdspName = "Mock_Tdsp",
                    CrDuns = "Mock_Cr_Duns",
                    CrName = "Mock_Cr",
                    TotalAmount = "12.33",
                }
            };

            clientDataAccess.Stub(x => x.ListCspDunsPort())
                .Return(cspDunsPorts);

            exportDataAccess.Stub(x => x.ListUnprocessed(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg.Is(1)))
                .Return(headers);

            clientDataAccess.Expect(x => x.LoadDunsByCspDunsId(Arg.Is(17)))
                .Return("Mock_Csp_Duns");

            clientDataAccess.Stub(x => x.LoadLdcByTdspDuns(Arg.Is("Mock_Tdsp_Duns")))
                .Return(new LdcModel
                {
                    LdcId = 23,
                    MarketId = 33
                });

            // act
            var results = concern.Export(CancellationToken.None);

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Length);

            var result = results[0];
            Assert.IsNotNull(result);
            Assert.AreEqual("Mock_Trade_Mock_Tdsp_DunsINV", result.TradingPartnerId);

            clientDataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Exporting_810_Then_For_Ryder_Csp_Duns_File_Name_Is_Prefixed_With_ECPCUSTINVRYDER()
        {
            // arrange
            var cspDunsPorts = new[]
            {
                new CspDunsPortModel
                {
                    CspDunsId = 17,
                    CspDunsPortId = 117,
                    LdcDuns = "MockTdspDuns_One",
                    LdcShortName = "MockTdspDuns_One",
                    Duns = "MockDuns_One",
                    TradingPartnerId = "Mock_Trade_{DUNS}",
                    ProviderId = 1
                }
            };

            var headers = new[]
            {
                new Type810Header
                {
                    HeaderKey = 1,
                    TdspDuns = "Mock_Tdsp_Duns",
                    TdspName = "Mock_Tdsp",
                    CrDuns = "Mock_Cr_Duns",
                    CrName = "Mock_Cr",
                    TotalAmount = "12.33",
                }
            };

            clientDataAccess.Stub(x => x.ListCspDunsPort())
                .Return(cspDunsPorts);

            exportDataAccess.Stub(x => x.ListUnprocessed(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg.Is(1)))
                .Return(headers);

            clientDataAccess.Stub(x => x.LoadDunsByCspDunsId(Arg.Is(17)))
                .Return("RYDER0000");

            clientDataAccess.Stub(x => x.LoadLdcByTdspDuns(Arg.Is("Mock_Tdsp_Duns")))
                .Return(new LdcModel
                {
                    LdcId = 23,
                    MarketId = 33
                });

            // act
            var results = concern.Export(CancellationToken.None);

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Length);

            var result = results[0];
            Assert.IsNotNull(result);

            var fileName = result.GenerateFileName("810", "TXT");
            Assert.IsTrue(fileName.StartsWith("ECPCUSTINVRYDER_810"));
        }
    }
}

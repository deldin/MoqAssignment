using System.Linq;
using System.Threading;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Contexts;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Ista.FileServices.MarketFiles.UnitTests
{
    [TestClass]
    public class TransmitFileTaskTests
    {
        private int clientId;
        private ILogger logger;
        private IAdminDataAccess adminDataAccess;
        private IClientDataAccess clientDataAccess;
        private IMarketFile marketDataAccess;
        private TransmitFileContext transmitFileContext;

        private TransmitFileTask task;

        [TestInitialize]
        public void Initialize()
        {
            clientId = 1;
            logger = MockRepository.GenerateMock<ILogger>();
            adminDataAccess = MockRepository.GenerateMock<IAdminDataAccess>();
            clientDataAccess = MockRepository.GenerateMock<IClientDataAccess>();
            marketDataAccess = MockRepository.GenerateMock<IMarketFile>();
            transmitFileContext = new TransmitFileContext
            {
                DirectoryArchive = "archive",
                DirectoryDecrypted = "decrypted",
                DirectoryEncrypted = "encrypted",
                DirectoryException = "exception"
            };

            task = new TransmitFileTask(adminDataAccess, clientDataAccess, marketDataAccess, logger, clientId);
        }

        [TestMethod]
        public void CbfFilesShouldSkipProcessing()
        {
            //arrange
            var marketFileModelArray = new[]
            {
                new MarketFileModel {FileType = "CBF"}
            };

            marketDataAccess.Stub(x => x.ListEncryptedOutboundMarketFiles()).Return(marketFileModelArray);

            //act
            task.Execute(transmitFileContext, CancellationToken.None);

            //assert
            clientDataAccess.AssertWasNotCalled(x => x.ListCspDunsPort());
        }

        [TestMethod]
        public void UpdateMarketFileWillBeCalledWhenNoPortsAreFound()
        {
            //arrange
            var marketFileModelArray = new[]
            {
                new MarketFileModel {FileType = "814", LdcId = 1,}
            };

            marketDataAccess.Stub(x => x.ListEncryptedOutboundMarketFiles())
                .Return(marketFileModelArray);

            clientDataAccess.Stub(x => x.ListCspDunsPort())
                .Return(Enumerable.Empty<CspDunsPortModel>().ToArray());

            //act
            task.Execute(transmitFileContext, CancellationToken.None);

            //assert
            marketDataAccess.AssertWasCalled(x => x.UpdateMarketFile(Arg<MarketFileModel>.Is.Anything));
        }

        [TestMethod]
        public void UpdateMarketFileWillBeCalledWhenTransportEnabledFlagIsFalse()
        {
            //arrange
            var marketFileModelArray = new[]
            {
                new MarketFileModel {FileType = "814", LdcId = 1,}
            };

            var cspDunsPortModelArray = new[]
            {
                new CspDunsPortModel {FileType = "814", LdcId = 1, TransportEnabledFlag = false, DirectoryOut = "mock"}
            };

            marketDataAccess.Stub(x => x.ListEncryptedOutboundMarketFiles())
                .Return(marketFileModelArray);

            clientDataAccess.Stub(x => x.ListCspDunsPort())
                .Return(cspDunsPortModelArray);

            //act
            task.Execute(transmitFileContext, CancellationToken.None);

            //assert
            marketDataAccess.AssertWasCalled(x => x.UpdateMarketFile(Arg<MarketFileModel>.Is.Anything));
        }

        [TestMethod]
        public void UpdateMarketFileWillBeCalledWhenTransportEnabledFlagIsTrue()
        {
            //arrange
            var marketFileModelArray = new[]
            {
                new MarketFileModel {FileType = "814", LdcId = 1,}
            };

            var cspDunsPortModelArray = new[]
            {
                new CspDunsPortModel {FileType = "814", LdcId = 1, TransportEnabledFlag = true, DirectoryOut = "mock"}
            };

            marketDataAccess.Stub(x => x.ListEncryptedOutboundMarketFiles())
                .Return(marketFileModelArray);
            
            clientDataAccess.Stub(x => x.ListCspDunsPort())
                .Return(cspDunsPortModelArray);

            //act
            task.Execute(transmitFileContext, CancellationToken.None);

            //assert
            marketDataAccess.AssertWasCalled(x => x.UpdateMarketFile(Arg<MarketFileModel>.Is.Anything));
        }
    }
}
using System;
using System.Linq;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ista.FileServices.MarketFiles.UnitTests.MarketExports
{
    [TestClass]
    public class DuplicateFileScenarios
    {
        [TestMethod]
        public void When_Exporting_Transactions_With_Duplicate_Ldc_Short_Name_And_Trading_Partner_Then_Files_Are_Appended_With_Unique_Identifier()
        {
            // arrange
            var results = new IMarketFileExportResult[]
            {
                new Export814Model(false) {LdcShortName = "DUP", TradingPartnerId = "Partner_Dup", CspDunsId = 1},
                new Export814Model(false) {LdcShortName = "DUP", TradingPartnerId = "Partner_Dup", CspDunsId = 2},
                new Export814Model(false) {LdcShortName = "DUP", TradingPartnerId = "Partner_Dup", CspDunsId = 3},
                new Export814Model(false) {LdcShortName = "DUP", TradingPartnerId = "Partner_Dup", CspDunsId = 4},
                new Export814Model(false) {LdcShortName = "UNIQUE", TradingPartnerId = "Partner_Unique", CspDunsId = 5}
            };

            var concern = new ExportMarketFileTask(null, null, null, 0);

            // act
            concern.HandleDuplicateLdcTradingPartners(results);

            // assert
            Assert.AreEqual(4, results.Count(x => x.DuplicateFileIdentifier.HasValue));

            var firstDuplicate = results.First(x => x.CspDunsId.Equals(1));
            var firstFileName = string.Format("814_Par_DUP_{0:yyyyMMddHHmmss}_1.txt", DateTime.Now);
            Assert.AreEqual(firstFileName, firstDuplicate.GenerateFileName("814", "txt"));

            var lastDuplicate = results.First(x => x.CspDunsId.Equals(4));
            var lastFileName = string.Format("814_Par_DUP_{0:yyyyMMddHHmmss}_4.txt", DateTime.Now);
            Assert.AreEqual(lastFileName, lastDuplicate.GenerateFileName("814", "txt"));

            var uniqueModel = results.First(x => x.CspDunsId.Equals(5));
            var uniqueFileName = string.Format("814_Par_UNIQUE_{0:yyyyMMddHHmmss}.txt", DateTime.Now);
            Assert.AreEqual(uniqueFileName, uniqueModel.GenerateFileName("814", "txt"));
        }

        [TestMethod]
        public void When_Exporting_Transactions_The_Trading_Partner_Substring_Is_Consider_For_Duplicate_Check()
        {
            // arrange
            var results = new IMarketFileExportResult[]
            {
                new Export814Model(false) {LdcShortName = "DUP", TradingPartnerId = "Partner_Diff", CspDunsId = 1},
                new Export814Model(false) {LdcShortName = "DUP", TradingPartnerId = "Partner_Fail", CspDunsId = 2},
                new Export814Model(false) {LdcShortName = "DUP", TradingPartnerId = "Partner_Term", CspDunsId = 3},
                new Export814Model(false) {LdcShortName = "DUP", TradingPartnerId = "Partner_Unique", CspDunsId = 4},
                new Export814Model(false) {LdcShortName = "UNIQUE", TradingPartnerId = "Partner_Unique", CspDunsId = 5}
            };

            var concern = new ExportMarketFileTask(null, null, null, 0);

            // act
            concern.HandleDuplicateLdcTradingPartners(results);

            // assert
            Assert.AreEqual(4, results.Count(x => x.DuplicateFileIdentifier.HasValue));

            var firstDuplicate = results.First(x => x.CspDunsId.Equals(1));
            var firstFileName = string.Format("814_Par_DUP_{0:yyyyMMddHHmmss}_1.txt", DateTime.Now);
            Assert.AreEqual(firstFileName, firstDuplicate.GenerateFileName("814", "txt"));

            var lastDuplicate = results.First(x => x.CspDunsId.Equals(4));
            var lastFileName = string.Format("814_Par_DUP_{0:yyyyMMddHHmmss}_4.txt", DateTime.Now);
            Assert.AreEqual(lastFileName, lastDuplicate.GenerateFileName("814", "txt"));

            var uniqueModel = results.First(x => x.CspDunsId.Equals(5));
            var uniqueFileName = string.Format("814_Par_UNIQUE_{0:yyyyMMddHHmmss}.txt", DateTime.Now);
            Assert.AreEqual(uniqueFileName, uniqueModel.GenerateFileName("814", "txt"));
        }
    }
}

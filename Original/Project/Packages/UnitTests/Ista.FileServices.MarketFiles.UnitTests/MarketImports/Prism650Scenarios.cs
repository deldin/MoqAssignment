using System.IO;
using System.Linq;
using System.Text;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.Models;
using Ista.FileServices.MarketFiles.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Should;

namespace Ista.FileServices.MarketFiles.UnitTests.MarketImports
{
    [TestClass]
    public class Prism650Scenarios
    {
        private Import650Prism importer;
        private IClientDataAccess clientDataAccess;
        private ILogger logger;

        [TestInitialize]
        public void Initialize()
        {
            clientDataAccess = MockRepository.GenerateMock<IClientDataAccess>();
            logger = MockRepository.GenerateMock<ILogger>();

            importer = new Import650Prism(clientDataAccess, logger);
        }

        [TestMethod, TestCategory("Scenario")]
        public void TX_SampleFile_CanHandleMultipleTransactions()
        {
            // arrange
            const string sample = @"SH|SELTX103994067MTR|SB7023315679201301230925586979|I|
01|SELTX103994067MTR|TX|11|20130123|SB7023315679201301230925586979|2243908|72|51||||||||||||ONCOR|1039940674000|STARTEX POWER|148055531|201301230936|
10|SELTX103994067MTR|DC001||10443720004425378|N|||||||M|SB7023315679|20130123|0919|
SH|SELTX103994067MTR|SB7023315702201301230926024969|I|
01|SELTX103994067MTR|TX|11|20130123|SB7023315702201301230926024969|2243913|72|51||||||||||||ONCOR|1039940674000|STARTEX POWER|148055531|201301230936|
10|SELTX103994067MTR|DC001||10443720008577794|N|||||||M|SB7023315702|20130123|0919|
SH|SELTX103994067MTR|SB7023315668201301230925589995|I|
01|SELTX103994067MTR|TX|11|20130123|SB7023315668201301230925589995|2243902|72|51||||||||||||ONCOR|1039940674000|STARTEX POWER|148055531|201301230936|
10|SELTX103994067MTR|DC001||10443720004625295|N|||||||M|SB7023315668|20130123|0919|
SH|SELTX103994067MTR|SB7023306373201301230922305291|I|
01|SELTX103994067MTR|TX|11|20130123|SB7023306373201301230922305291|2243879|72|51||||||||||||ONCOR|1039940674000|STARTEX POWER|148055531|201301230936|
10|SELTX103994067MTR|DC001||10443720005283435|N|||||||M|SB7023306373|20130123|0904|
SH|SELTX103994067MTR|SB7023310926201301230924135366|I|
01|SELTX103994067MTR|TX|11|20130123|SB7023310926201301230924135366|2243887|72|51||||||||||||ONCOR|1039940674000|STARTEX POWER|148055531|201301230936|
10|SELTX103994067MTR|DC001||10443720003122057|N|||||||M|SB7023310926|20130123|0911|
SH|SELTX103994067MTR|SB7023315674201301230925559728|I|
01|SELTX103994067MTR|TX|11|20130123|SB7023315674201301230925559728|2243903|72|51||||||||||||ONCOR|1039940674000|STARTEX POWER|148055531|201301230936|
10|SELTX103994067MTR|DC001||10443720001582707|N|||||||M|SB7023315674|20130123|0919|
SH|SELTX103994067MTR|SB7023315675201301230925595068|I|
01|SELTX103994067MTR|TX|11|20130123|SB7023315675201301230925595068|2243905|72|51||||||||||||ONCOR|1039940674000|STARTEX POWER|148055531|201301230936|
10|SELTX103994067MTR|DC001||10443720004975009|N|||||||M|SB7023315675|20130123|0919|
SH|SELTX103994067MTR|SB7023310921201301230924153549|I|
01|SELTX103994067MTR|TX|11|20130123|SB7023310921201301230924153549|2243884|72|51||||||||||||ONCOR|1039940674000|STARTEX POWER|148055531|201301230936|
10|SELTX103994067MTR|DC001||10443720006802834|N|||||||M|SB7023310921|20130123|0911|
SH|SELTX103994067MTR|CSO20130123091926938427|I|
01|SELTX103994067MTR|TX|11|20130123|CSO20130123091926938427|2243904|SH|51||||||||||||ONCOR|1039940674000|STARTEX POWER|148055531|201301230936|
10|SELTX103994067MTR|SH002||10443720005209317|N||||||||2243904|20130123|0919|||||||||||||||||||||||||RPS|TDSP HAS REMOVED THE PAYMENT PLAN SWITCH HOLD|
SH|SELTX103994067MTR|SB7023315692201301230926027392|I|
01|SELTX103994067MTR|TX|11|20130123|SB7023315692201301230926027392|2243911|72|51||||||||||||ONCOR|1039940674000|STARTEX POWER|148055531|201301230936|
10|SELTX103994067MTR|DC001||10443720005035408|N|||||||M|SB7023315692|20130123|0919|
SH|SELTX103994067MTR|SB7023315684201301230925589804|I|
01|SELTX103994067MTR|TX|11|20130123|SB7023315684201301230925589804|2243909|72|51||||||||||||ONCOR|1039940674000|STARTEX POWER|148055531|201301230936|
10|SELTX103994067MTR|DC001||10443720005483110|N|||||||M|SB7023315684|20130123|0919|
SH|SELTX103994067MTR|SB7023310927201301230924140910|I|
01|SELTX103994067MTR|TX|11|20130123|SB7023310927201301230924140910|2243886|72|51||||||||||||ONCOR|1039940674000|STARTEX POWER|148055531|201301230936|
10|SELTX103994067MTR|DC001||10443720004060879|N|||||||M|SB7023310927|20130123|0911|
SH|SELTX103994067MTR|SB7023315699201301230925584376|I|
01|SELTX103994067MTR|TX|11|20130123|SB7023315699201301230925584376|2243912|72|51||||||||||||ONCOR|1039940674000|STARTEX POWER|148055531|201301230936|
10|SELTX103994067MTR|DC001||10443720006812359|N|||||||M|SB7023315699|20130123|0919|
SH|SELTX103994067MTR|443722013012309233174727|I|
01|SELTX103994067MTR|TX|13|20130123|443722013012309233174727|||S2||||||||||||ONCOR|1039940674000|STAR ELECTRICITY LLC|148055531|201301230936|
10|SELTX103994067MTR|||10443720003174727|Y|||||||||||||||||IN001|20130124|1400|20130124|0900|N|||Y|
14|SELTX103994067MTR|104086414LG|
TL|14
";

            Import650Model context;

            // act
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(sample)))
            {
                var result = importer.Parse(stream);
                context = result as Import650Model;
            }
            
            // assert
            Assert.IsNotNull(context);
            context.TransactionActualCount.ShouldEqual(14);
            context.TransactionAuditCount.ShouldEqual(14);
            context.Headers.Count().ShouldEqual(14);

            foreach (var valid in context.TypeHeaders)
            {
                valid.Names.Count().ShouldEqual(0);
            }
        }

        [TestMethod, TestCategory("Scenario")]
        public void TX_SampleFile_ServiceMeter_AreaOutage_SuspensionOfDeliveryService()
        {
            // arrange
            const string sample = @"SH|SELTX103994067MTR|443722013012309233174727|I|
01|SELTX103994067MTR|TX|13|20130123|443722013012309233174727|||S2||||||||||||ONCOR|1039940674000|STAR ELECTRICITY LLC|148055531|201301230936|
10|SELTX103994067MTR|||10443720003174727|Y|||||||||||||||||IN001|20130124|1400|20130124|0900|N|||Y|
14|SELTX103994067MTR|104086414LG|
TL|1";
            Import650Model context;

            // act
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(sample)))
            {
                var result = importer.Parse(stream);
                context = result as Import650Model;
            }

            // assert
            Assert.IsNotNull(context);
            context.TransactionActualCount.ShouldEqual(1);
            context.TransactionAuditCount.ShouldEqual(1);
            
            //context.Market.ShouldEqual(MarketOptions.Texas);

            var header = context.TypeHeaders.FirstOrDefault();

            //header.Direction.ShouldEqual(true);

            //header.TransactionSetId.ShouldEqual("650");
            Assert.IsNotNull(header);
            header.TransactionSetPurposeCode.ShouldEqual("13");
            header.TransactionDate.ShouldEqual("20130123");
            header.TransactionNbr.ShouldEqual("443722013012309233174727");
            header.ReferenceNbr.ShouldBeNull();
            header.TransactionType.ShouldBeNull();
            header.ActionCode.ShouldEqual("S2");
            header.TdspName.ShouldEqual("ONCOR");
            header.TdspDuns.ShouldEqual("1039940674000");
            header.CrName.ShouldEqual("STAR ELECTRICITY LLC");
            header.CrDuns.ShouldEqual("148055531");
            header.ProcessedReceivedDateTime.ShouldEqual("201301230936");

            header.Names.Count().ShouldEqual(0);

            header.Services.Count().ShouldEqual(1);
            var service = header.Services[0];

            service.EsiId.ShouldEqual("10443720003174727");
            service.SpecialProcessCode.ShouldEqual("Y");
            service.IncidentCode.ShouldEqual("IN001");
            service.EstRestoreDate.ShouldEqual("20130124");
            service.EstRestoreTime.ShouldEqual("1400");
            service.IntStartDate.ShouldEqual("20130124");
            service.IntStartTime.ShouldEqual("0900");
            service.RepairRecommended.ShouldEqual("N");
            service.AreaOutage.ShouldEqual("Y");

            service.Meters.Count().ShouldEqual(1);
            var meter = service.Meters[0];
            meter.MeterNumber.ShouldEqual("104086414LG");
        }

        [TestMethod, TestCategory("Scenario")]
        public void TX_SampleFile_ServiceOrder_RemovedNonPaymentSwitchHold_ServiceOrderResponse()
        {
            // arrange
            const string sample = @"SH|SELTX103994067MTR|CSO20130123091926938427|I|
01|SELTX103994067MTR|TX|11|20130123|CSO20130123091926938427|2243904|SH|51||||||||||||ONCOR|1039940674000|STARTEX POWER|148055531|201301230936|
10|SELTX103994067MTR|SH002||10443720005209317|N||||||||2243904|20130123|0919|||||||||||||||||||||||||RPS|TDSP HAS REMOVED THE PAYMENT PLAN SWITCH HOLD|
TL|1
";
            Import650Model context;

            // act
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(sample)))
            {
                var result = importer.Parse(stream);
                context = result as Import650Model;
            }

            // assert
            Assert.IsNotNull(context);
            context.TransactionActualCount.ShouldEqual(1);
            context.TransactionAuditCount.ShouldEqual(1);
            //context.Market.ShouldEqual(MarketOptions.Texas);

            var header = context.TypeHeaders.FirstOrDefault();

            //header.Direction.ShouldEqual(true);
            
            //header.TransactionSetId.ShouldEqual("650");
            Assert.IsNotNull(header);
            header.TransactionSetPurposeCode.ShouldEqual("11");
            header.TransactionDate.ShouldEqual("20130123");
            header.TransactionNbr.ShouldEqual("CSO20130123091926938427");
            header.ReferenceNbr.ShouldEqual("2243904");
            header.TransactionType.ShouldEqual("SH");
            header.ActionCode.ShouldEqual("51");
            header.TdspName.ShouldEqual("ONCOR");
            header.TdspDuns.ShouldEqual("1039940674000");
            header.CrName.ShouldEqual("STARTEX POWER");
            header.CrDuns.ShouldEqual("148055531");
            header.ProcessedReceivedDateTime.ShouldEqual("201301230936");

            header.Names.Count().ShouldEqual(0);

            header.Services.Count().ShouldEqual(1);
            var service = header.Services[0];

            service.PurposeCode.ShouldEqual("SH002");
            service.EsiId.ShouldEqual("10443720005209317");
            service.SpecialProcessCode.ShouldEqual("N");
            service.ServiceOrderNbr.ShouldEqual("2243904");
            service.CompletionDate.ShouldEqual("20130123");
            service.CompletionTime.ShouldEqual("0919");
        }

        [TestMethod, TestCategory("Scenario")]
        public void TX_SampleFile_DC001_ServiceOrder_ServiceOrderResponse()
        {
            // arrange
            const string file = @"SH|SELTX103994067MTR|SB7023315679201301230925586979|I|
01|SELTX103994067MTR|TX|11|20130123|SB7023315679201301230925586979|2243908|72|51||||||||||||ONCOR|1039940674000|STARTEX POWER|148055531|201301230936|
10|SELTX103994067MTR|DC001||10443720004425378|N|||||||M|SB7023315679|20130123|0919|
TL|1";
            Import650Model context;

            // act
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(file)))
            {
                var result = importer.Parse(stream);
                context = result as Import650Model;
            }

            // assert
            Assert.IsNotNull(context);
            context.TransactionActualCount.ShouldEqual(1);
            context.TransactionAuditCount.ShouldEqual(1);
            //context.Market.ShouldEqual(MarketOptions.Texas);

            var header = context.TypeHeaders.FirstOrDefault();

            //header.Direction.ShouldEqual(true);
            
            //header.TransactionSetId.ShouldEqual("650");
            Assert.IsNotNull(header);
            header.TransactionSetPurposeCode.ShouldEqual("11");
            header.TransactionDate.ShouldEqual("20130123");
            header.TransactionNbr.ShouldEqual("SB7023315679201301230925586979");
            header.ReferenceNbr.ShouldEqual("2243908");
            header.TransactionType.ShouldEqual("72");
            header.ActionCode.ShouldEqual("51");
            header.TdspName.ShouldEqual("ONCOR");
            header.TdspDuns.ShouldEqual("1039940674000");
            header.CrName.ShouldEqual("STARTEX POWER");
            header.CrDuns.ShouldEqual("148055531");
            header.ProcessedReceivedDateTime.ShouldEqual("201301230936");

            header.Names.Count().ShouldEqual(0);

            header.Services.Count().ShouldEqual(1);
            var service = header.Services[0];

            service.PurposeCode.ShouldEqual("DC001");
            service.EsiId.ShouldEqual("10443720004425378");
            service.SpecialProcessCode.ShouldEqual("N");
            service.EquipLocation.ShouldEqual("M");
            service.ServiceOrderNbr.ShouldEqual("SB7023315679");
            service.CompletionDate.ShouldEqual("20130123");
            service.CompletionTime.ShouldEqual("0919");
        }

        [TestMethod, TestCategory("Scenario")]
        public void TX_SampleFile_Reject_ServiceOrderResponse()
        {
            // arrange
            const string file = @"SH|JETTX007929441MTR|TNMP101059104|I|
01|JETTX007929441MTR|TX|11|20120622|TNMP101059104|2666273|79|U||||||||||||TEXAS-NEW MEXICO POWER COMPANY|007929441|JUST ENERGY|110569998|201206221132|
10|JETTX007929441MTR|RC001||10400511126510001|
13|JETTX007929441MTR|A13|DUPLICATE RECONNECT REQUEST FOR PREVIOUS DISCONNECT|
TL|1";
            Import650Model context;

            // act
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(file)))
            {
                var result = importer.Parse(stream);
                context = result as Import650Model;
            }

            // assert
            Assert.IsNotNull(context);
            context.TransactionActualCount.ShouldEqual(1);
            context.TransactionAuditCount.ShouldEqual(1);
            //context.Market.ShouldEqual(MarketOptions.Texas);

            var header = context.TypeHeaders.FirstOrDefault();

            //header.Direction.ShouldEqual(true);
            
            //header.TransactionSetId.ShouldEqual("650");
            Assert.IsNotNull(header);
            header.TransactionSetPurposeCode.ShouldEqual("11");
            header.TransactionDate.ShouldEqual("20120622");
            header.TransactionNbr.ShouldEqual("TNMP101059104");
            header.ReferenceNbr.ShouldEqual("2666273");
            header.TransactionType.ShouldEqual("79");
            header.ActionCode.ShouldEqual("U");
            header.TdspName.ShouldEqual("TEXAS-NEW MEXICO POWER COMPANY");
            header.TdspDuns.ShouldEqual("007929441");
            header.CrName.ShouldEqual("JUST ENERGY");
            header.CrDuns.ShouldEqual("110569998");
            header.ProcessedReceivedDateTime.ShouldEqual("201206221132");

            header.Names.Count().ShouldEqual(0);

            header.Services.Count().ShouldEqual(1);
            var service = header.Services[0];

            service.PurposeCode.ShouldEqual("RC001");
            service.EsiId.ShouldEqual("10400511126510001");

            service.Rejects.Count().ShouldEqual(1);
            var reject = service.Rejects[0];

            reject.RejectCode.ShouldEqual("A13");
            reject.RejectReason.ShouldEqual("DUPLICATE RECONNECT REQUEST FOR PREVIOUS DISCONNECT");
            reject.UnexCode.ShouldBeNull();
            reject.UnexReason.ShouldBeNull();
        }

        [TestMethod, TestCategory("Scenario")]
        public void TX_SampleFile_Change()
        {
            // arrange
            const string file = @"SH|JETTX007929441MTR|TNMP100927911|I|
01|JETTX007929441MTR|TX|11|20110628|TNMP100927911|1853597|79|51||||||||||||TEXAS-NEW MEXICO POWER COMPANY|007929441|JUST ENERGY|110569998|201106281412|
10|JETTX007929441MTR|RC001||10400514781250001|N||||||||4519615|20110628|1350|AMS 06-28-2011 13:50 R-12104 UA2525|
TL|1";

            Import650Model context;

            // act
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(file)))
            {
                var result = importer.Parse(stream);
                context = result as Import650Model;
            }

            // assert
            Assert.IsNotNull(context);
            context.TransactionActualCount.ShouldEqual(1);
            context.TransactionAuditCount.ShouldEqual(1);
            //context.Market.ShouldEqual(MarketOptions.Texas);

            var header = context.TypeHeaders.FirstOrDefault();

            //header.Direction.ShouldEqual(true);

            //header.TransactionSetId.ShouldEqual("650");
            Assert.IsNotNull(header);
            header.TransactionSetPurposeCode.ShouldEqual("11");
            header.TransactionDate.ShouldEqual("20110628");
            header.TransactionNbr.ShouldEqual("TNMP100927911");
            header.ReferenceNbr.ShouldEqual("1853597");
            header.TransactionType.ShouldEqual("79");
            header.ActionCode.ShouldEqual("51");
            header.TdspName.ShouldEqual("TEXAS-NEW MEXICO POWER COMPANY");
            header.TdspDuns.ShouldEqual("007929441");
            header.CrName.ShouldEqual("JUST ENERGY");
            header.CrDuns.ShouldEqual("110569998");
            header.ProcessedReceivedDateTime.ShouldEqual("201106281412");
            
            header.Names.Count().ShouldEqual(0);

            header.Services.Count().ShouldEqual(1);
            var service = header.Services[0];

            service.PurposeCode.ShouldEqual("RC001");
            service.EsiId.ShouldEqual("10400514781250001");
            service.SpecialProcessCode.ShouldEqual("N");
            service.ServiceOrderNbr.ShouldEqual("4519615");
            service.CompletionDate.ShouldEqual("20110628");
            service.CompletionTime.ShouldEqual("1350");
            service.ReportRemarks.ShouldEqual("AMS 06-28-2011 13:50 R-12104 UA2525");
        }
    }
}

using System.IO;
using System.Linq;
using System.Text;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.Models;
using Ista.FileServices.MarketFiles.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Ista.FileServices.MarketFiles.UnitTests.MarketImports
{
    [TestClass]
    public class Prism867Scenarios
    {
        private IClientDataAccess _clientDataAccess;
        private ILogger _logger;

        private Import867Prism _concern;

        [TestInitialize]
        public void StartUp()
        {
            _clientDataAccess = MockRepository.GenerateMock<IClientDataAccess>();
            _logger = MockRepository.GenerateStub<ILogger>();

            _concern = new Import867Prism(_clientDataAccess, _logger);
        }

        [TestMethod]
        public void WhenParsingHeader_TransactionDate_OnlyGets_First_8_Characters()
        {
            // arrange
            const string file = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|DD||CDB1381148122012213942000||1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317||||||||||||||||||||||10443720001381148|ERCOT|183529049|ERCOT|
07|ACTTX183529049USE|||M||KH|MON|
08|ACTTX183529049USE|QD|1475|
09|ACTTX183529049USE|1475|51|20121120|20121220|
15|ACTTX183529049USE|20121120|20121220|104137265LG||KH|MON|||A|||||||||M|
16|ACTTX183529049USE|QD|1475|KH||AA|51|19665|21140|1|
TL|1";

            _clientDataAccess.Stub(x => x.IdentifyMarket(Arg<string>.Is.Anything)).Return(1);


            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = _concern.Parse(stream) as Import867Model;
            

            // assert
            Assert.IsNotNull(actual);
            var header = actual.TypeHeaders.FirstOrDefault();
            Assert.IsNotNull(header);
            Assert.AreEqual("20121220", header.TransactionDate);

            
        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void WhenParsingHeader_EsiId_LoadsFromPosition_35_OrIfNothingAt35_Then_LoadsFromPosition_16() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string fileTestWithPosition35 = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|DD||CDB1381148122012213942000||1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||EsiIDPosition16|||||||||||||||||||EsiIDPosition35|ERCOT|183529049|ERCOT|
07|ACTTX183529049USE|||M||KH|MON|
08|ACTTX183529049USE|QD|1475|
09|ACTTX183529049USE|1475|51|20121120|20121220|
15|ACTTX183529049USE|20121120|20121220|104137265LG||KH|MON|||A|||||||||M|
16|ACTTX183529049USE|QD|1475|KH||AA|51|19665|21140|1|
TL|1";

            const string fileTestWithoutPosition35 = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|DD||CDB1381148122012213942000||1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||EsiIDPosition16||||||||||||||||||||ERCOT|183529049|ERCOT|
07|ACTTX183529049USE|||M||KH|MON|
08|ACTTX183529049USE|QD|1475|
09|ACTTX183529049USE|1475|51|20121120|20121220|
15|ACTTX183529049USE|20121120|20121220|104137265LG||KH|MON|||A|||||||||M|
16|ACTTX183529049USE|QD|1475|KH||AA|51|19665|21140|1|
TL|1";
            _clientDataAccess.Stub(x => x.IdentifyMarket(Arg<string>.Is.Anything)).Return(1);

            
            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileTestWithPosition35));
            var actual = _concern.Parse(stream) as Import867Model;

            var streamWithout35 = new MemoryStream(Encoding.UTF8.GetBytes(fileTestWithoutPosition35));
            var actualWithout35 = _concern.Parse(streamWithout35) as Import867Model;

           
            // assert
            Assert.IsNotNull(actual);
            var headerWithPosition35 = actual.TypeHeaders.FirstOrDefault();
            Assert.IsNotNull(headerWithPosition35);
            Assert.AreEqual("EsiIDPosition35", headerWithPosition35.EsiId);


            Assert.IsNotNull(actualWithout35);
            var headerWithoutPosition35 = actualWithout35.TypeHeaders.FirstOrDefault();
            Assert.IsNotNull(headerWithoutPosition35);
            Assert.AreEqual("EsiIDPosition16", headerWithoutPosition35.EsiId);


        }


        [TestMethod]
// ReSharper disable InconsistentNaming
        public void WhenParsingHeader_CheckAllAssignedFields() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string file = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|DD|HeaderActionCode|CDB1381148122012213942000|HeaderPosition7|1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||HeaderPosition16|||||||||||||||||||10443720001381148|ERCOT|183529049|ERCOT|HeaderPosition39|Pos40|Pos41|Pos42|Pos43|Pos44|Pos45|Pos46|Pos47|Pos48|Pos49|Pos50|Pos51|Pos52
07|ACTTX183529049USE|||M||KH|MON|
08|ACTTX183529049USE|QD|1475|
09|ACTTX183529049USE|1475|51|20121120|20121220|
15|ACTTX183529049USE|20121120|20121220|104137265LG||KH|MON|||A|||||||||M|
16|ACTTX183529049USE|QD|1475|KH||AA|51|19665|21140|1|
TL|1";

            _clientDataAccess.Stub(x => x.IdentifyMarket(Arg<string>.Is.Anything)).Return(1);

            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = _concern.Parse(stream) as Import867Model;
            

            // assert
            Assert.IsNotNull(actual);
            var header = actual.TypeHeaders.FirstOrDefault();
            Assert.IsNotNull(header);
            Assert.AreEqual(1, header.MarketID);
            Assert.AreEqual("I",header.TransactionSetId);
            Assert.AreEqual("00",header.TransactionSetPurposeCode);
            Assert.AreEqual("CDB1381148122012213942000",header.TransactionNbr);
            Assert.AreEqual("20121220", header.TransactionDate);
            Assert.AreEqual("DD", header.ReportTypeCode);
            Assert.AreEqual("10443720001381148", header.EsiId);
            Assert.AreEqual("HeaderActionCode", header.ActionCode);// = marketFields.AtIndex(5),
            Assert.AreEqual("ERCOT", header.PowerRegion);// = marketFields.AtIndex(38),
            Assert.AreEqual("HeaderPosition7", header.OriginalTransactionNbr);// = marketFields.AtIndex(7),
            Assert.AreEqual("HeaderPosition39", header.ReferenceNbr);// = marketFields.AtIndex(39),
            Assert.AreEqual("1039940674000", header.TdspDuns);// = marketFields.AtIndex(8),
            Assert.AreEqual("ONCOR", header.TdspName);// = marketFields.AtIndex(9),
            Assert.AreEqual("133305370", header.CrDuns);// = marketFields.AtIndex(10),
            Assert.AreEqual("ACCENT ENERGY TEXAS, LLC", header.CrName);// = marketFields.AtIndex(11),
            Assert.AreEqual(true, header.DirectionFlag);// = true, //For imports it is true
            Assert.AreEqual("HeaderPosition16", header.UtilityAccountNumber);// = marketFields.AtIndex(16),
            Assert.AreEqual("Pos48", header.EstimationReason);// = marketFields.AtIndex(48),
            Assert.AreEqual("Pos49", header.EstimationDescription);// = marketFields.AtIndex(49),
            Assert.AreEqual("Pos50", header.DoorHangerFlag);// = marketFields.AtIndex(50),
            Assert.AreEqual("Pos51", header.EsnCount);// = marketFields.AtIndex(51),
            Assert.AreEqual("Pos52", header.QoCount);// = marketFields.AtIndex(52)

        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void WhenParsingAccountBillQuantity_CheckAllAssignedFields() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string file = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|DD|HeaderActionCode|CDB1381148122012213942000|HeaderPosition7|1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||HeaderPosition16|||||||||||||||||||10443720001381148|ERCOT|183529049|ERCOT|HeaderPosition39|Pos40|Pos41|Pos42|Pos43|Pos44|Pos45|Pos46|Pos47|Pos48|Pos49|Pos50|Pos51|Pos52
05|ACTTX183529049USE|20121220|20131220
06|ACTTX183529049USE|QD|99|KH
07|ACTTX183529049USE|||M||KH|MON|
08|ACTTX183529049USE|QD|1475|
09|ACTTX183529049USE|1475|51|20121120|20121220|
15|ACTTX183529049USE|20121120|20121220|104137265LG||KH|MON|||A|||||||||M|
16|ACTTX183529049USE|QD|1475|KH||AA|51|19665|21140|1|
TL|1";
            _clientDataAccess.Stub(x => x.IdentifyMarket(Arg<string>.Is.Anything)).Return(1);

           
            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = _concern.Parse(stream) as Import867Model;
            

            // assert
            Assert.IsNotNull(actual);
            var header = actual.TypeHeaders.FirstOrDefault();
            Assert.IsNotNull(header);
            var accountBillQty = header.AccountBillQtys.First();

            Assert.AreEqual("QD", accountBillQty.Qualifier);
            Assert.AreEqual("99", accountBillQty.Quantity);
            Assert.AreEqual("KH", accountBillQty.UOM);
            Assert.AreEqual("20121220", accountBillQty.BeginDate);
            Assert.AreEqual("20131220", accountBillQty.EndDate);
        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void WhenParsingNonIntervalSummary_ForTexasMarket_CheckAllAssignedFields() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string file = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|DD|HeaderActionCode|CDB1381148122012213942000|HeaderPosition7|1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||HeaderPosition16|||||||||||||||||||10443720001381148|ERCOT|183529049|ERCOT|HeaderPosition39|Pos40|Pos41|Pos42|Pos43|Pos44|Pos45|Pos46|Pos47|Pos48|Pos49|Pos50|Pos51|Pos52
05|ACTTX183529049USE|20121220|20131220
06|ACTTX183529049USE|QD|99|KH
07|ACTTX183529049USE|||M||KH|MON|
08|ACTTX183529049USE|QD|1475|
09|ACTTX183529049USE|1475|51|20121120|20121220|
15|ACTTX183529049USE|20121120|20121220|104137265LG||KH|MON|||A|||||||||M|
16|ACTTX183529049USE|QD|1475|KH||AA|51|19665|21140|1|
TL|1";

            
            _clientDataAccess.Stub(x => x.IdentifyMarket("1039940674000")).Return(1);


            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = _concern.Parse(stream) as Import867Model;
            

            // assert
            Assert.IsNotNull(actual);
            var header = actual.TypeHeaders.FirstOrDefault();
            Assert.IsNotNull(header);
            var nonIntervalSummary = header.NonIntervalSummaries.First();
            var nonIntervalSummaryQty = nonIntervalSummary.NonIntervalSummaryQtys.First();

            Assert.AreEqual("SU", nonIntervalSummary.TypeCode);
            Assert.AreEqual("MON", nonIntervalSummary.MeterInterval);
            Assert.AreEqual("KH", nonIntervalSummary.MeterUOM);
            
            //Qty Fields
            Assert.AreEqual("QD", nonIntervalSummaryQty.Qualifier);
            Assert.AreEqual("1475", nonIntervalSummaryQty.Quantity);
            Assert.AreEqual("51", nonIntervalSummaryQty.MeasurementSignificanceCode);
            Assert.AreEqual("20121120", nonIntervalSummaryQty.ServicePeriodStart);
            Assert.AreEqual("20121220", nonIntervalSummaryQty.ServicePeriodEnd);


        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void WhenParsingNonIntervalSummary_ForMarylandMarket_CheckAllAssignedFields() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string file = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|XX|HeaderActionCode|CDB1381148122012213942000|HeaderPosition7|1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||HeaderPosition16|||||||||||||||||||10443720001381148|ERCOT|183529049|ERCOT|HeaderPosition39|Pos40|Pos41|Pos42|Pos43|Pos44|Pos45|Pos46|Pos47|Pos48|Pos49|Pos50|Pos51|Pos52
05|ACTTX183529049USE|20121220|20131220
06|ACTTX183529049USE|QD|99|FM
07|ACTTX183529049USE|07StartDate|07EndDate|M||KH|MON|
08|ACTTX183529049USE|QD|1475|KHFM
15|ACTTX183529049USE|20121120|20121220|104137265LG||KH|MON|||A|||||||||M|
16|ACTTX183529049USE|QD|1475|KH||AA|51|19665|21140|1|
TL|1";

            
            _clientDataAccess.Stub(x => x.IdentifyMarket("1039940674000")).Return(2); // Forcing to return Maryland as Market


            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = _concern.Parse(stream) as Import867Model;
 

            // assert
            Assert.IsNotNull(actual);
            var header = actual.TypeHeaders.FirstOrDefault();
            Assert.IsNotNull(header);
            var nonIntervalSummary = header.NonIntervalSummaries.First();
            var nonIntervalSummaryQty = nonIntervalSummary.NonIntervalSummaryQtys.First();

            Assert.AreEqual("SU", nonIntervalSummary.TypeCode);
            Assert.AreEqual("FM", nonIntervalSummary.MeterInterval);
            Assert.AreEqual("KH", nonIntervalSummary.MeterUOM);

            //Qty Fields
            Assert.AreEqual("QD", nonIntervalSummaryQty.Qualifier);
            Assert.AreEqual("1475", nonIntervalSummaryQty.Quantity);
            Assert.AreEqual("51", nonIntervalSummaryQty.MeasurementSignificanceCode);
            Assert.AreEqual("07StartDate", nonIntervalSummaryQty.ServicePeriodStart);
            Assert.AreEqual("07EndDate", nonIntervalSummaryQty.ServicePeriodEnd);


        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void WhenParsingNonIntervalDetail_ForTexasMarket_CheckAllAssignedFields() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string file = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|XX|HeaderActionCode|CDB1381148122012213942000|HeaderPosition7|1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||HeaderPosition16|||||||||||||||||||10443720001381148|ERCOT|183529049|ERCOT|HeaderPosition39|Pos40|Pos41|Pos42|Pos43|Pos44|Pos45|Pos46|Pos47|Pos48|Pos49|Pos50|Pos51|Pos52
05|ACTTX183529049USE|20121220|20131220
06|ACTTX183529049USE|QD|99|FM
07|ACTTX183529049USE|07StartDate|07EndDate|M||KH|MON|
08|ACTTX183529049USE|QD|1475|KHFM
15|ACTTX183529049USE|20121120|20121220|104137265LG|Pos5|KH|MON|||A||||||||Pos18|M|
16|ACTTX183529049USE|QD|1475|KH||AA|51|19665|21140|1|Pos11|Pos12
TL|1";

            _clientDataAccess.Stub(x => x.IdentifyMarket("1039940674000")).Return(1); 


            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = _concern.Parse(stream) as Import867Model;


            // assert
            Assert.IsNotNull(actual);
            var header = actual.TypeHeaders.FirstOrDefault();
            Assert.IsNotNull(header);
            var nonIntervalDetail = header.NonIntervalDetails.First();
            var nonIntervalDetailQty = nonIntervalDetail.NonIntervalDetailQtys.First();

            Assert.AreEqual("PL", nonIntervalDetail.TypeCode);
            Assert.AreEqual("104137265LG", nonIntervalDetail.MeterNumber);// = marketFields.AtIndex(4),
            Assert.AreEqual("Pos18", nonIntervalDetail.MovementTypeCode);// = marketFields.AtIndex(18),
            Assert.AreEqual("20121120", nonIntervalDetail.ServicePeriodStart);// = marketFields.AtIndex(2),
            Assert.AreEqual("20121220", nonIntervalDetail.ServicePeriodEnd);// = marketFields.AtIndex(3),
            Assert.AreEqual("Pos5", nonIntervalDetail.ExchangeDate);// = marketFields.AtIndex(5),
            Assert.AreEqual("A", nonIntervalDetail.MeterRole);// = marketFields.AtIndex(10),
            Assert.AreEqual("KH", nonIntervalDetail.MeterUom);// = meterType.Length > 1 ? meterType.Substring(0, 2) : string.Empty,
            Assert.AreEqual("MON", nonIntervalDetail.MeterInterval);// = meterType.Length > 2 ? meterType.Substring(2) : string.Empty,
            Assert.AreEqual(null, nonIntervalDetail.RatchetDateTime);// = ratchetDateTime

            //Qty Fields
            
            Assert.AreEqual("QD", nonIntervalDetailQty.Qualifier);// = marketFields.AtIndex(2),
            Assert.AreEqual("1475", nonIntervalDetailQty.Quantity);// = marketFields.AtIndex(3),
            Assert.AreEqual("AA", nonIntervalDetailQty.MeasurementCode);// = marketFields.AtIndex(6),
            Assert.AreEqual("KH", nonIntervalDetailQty.Uom);// = marketFields.AtIndex(4),
            Assert.AreEqual("19665", nonIntervalDetailQty.BeginRead);// = marketFields.AtIndex(8),
            Assert.AreEqual("21140", nonIntervalDetailQty.EndRead);// = marketFields.AtIndex(9),
            Assert.AreEqual("51", nonIntervalDetailQty.MeasurementSignificanceCode);// = (measurementCode == string.Empty) ? null : measurementCode,
            Assert.AreEqual("Pos12", nonIntervalDetailQty.TransformerLossFactor);// = marketFields.AtIndex(12),
            Assert.AreEqual("1", nonIntervalDetailQty.MeterMultiplier);// = marketFields.AtIndex(10),
            Assert.AreEqual("Pos11", nonIntervalDetailQty.PowerFactor);// = marketFields.AtIndex(11)


        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void WhenParsingNonIntervalDetail_ForMaryland_CheckAllAssignedFields() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string file = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|XX|HeaderActionCode|CDB1381148122012213942000|HeaderPosition7|1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||HeaderPosition16|||||||||||||||||||10443720001381148|ERCOT|183529049|ERCOT|HeaderPosition39|Pos40|Pos41|Pos42|Pos43|Pos44|Pos45|Pos46|Pos47|Pos48|Pos49|Pos50|Pos51|Pos52
05|ACTTX183529049USE|20121220|20131220
06|ACTTX183529049USE|QD|99|FM
07|ACTTX183529049USE|07StartDate|07EndDate|M||KH|MON|
08|ACTTX183529049USE|QD|1475|KHFM
15|ACTTX183529049USE|20121120|20121220|104137265LG|Pos5|KH|MON|||A||||||||Pos18|M|
16|ACTTX183529049USE|QD|1475|KH||AA|51|19665|21140|1|Pos11|Pos12|||||||||||||||||||Pos31
TL|1";

           
            _clientDataAccess.Stub(x => x.IdentifyMarket("1039940674000")).Return(2); // Forcing to return Maryland as Market


            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = _concern.Parse(stream) as Import867Model;


            // assert
            Assert.IsNotNull(actual);
            var header = actual.TypeHeaders.FirstOrDefault();
            Assert.IsNotNull(header);
            var nonIntervalDetail = header.NonIntervalDetails.First();
            var nonIntervalDetailQty = nonIntervalDetail.NonIntervalDetailQtys.First();

            Assert.AreEqual("PL", nonIntervalDetail.TypeCode);
            Assert.AreEqual("104137265LG", nonIntervalDetail.MeterNumber);// = marketFields.AtIndex(4),
            Assert.AreEqual("Pos18", nonIntervalDetail.MovementTypeCode);// = marketFields.AtIndex(18),
            Assert.AreEqual("20121120", nonIntervalDetail.ServicePeriodStart);// = marketFields.AtIndex(2),
            Assert.AreEqual("20121220", nonIntervalDetail.ServicePeriodEnd);// = marketFields.AtIndex(3),
            Assert.AreEqual("Pos5", nonIntervalDetail.ExchangeDate);// = marketFields.AtIndex(5),
            Assert.AreEqual("A", nonIntervalDetail.MeterRole);// = marketFields.AtIndex(10),
            Assert.AreEqual("KH", nonIntervalDetail.MeterUom);// = meterType.Length > 1 ? meterType.Substring(0, 2) : string.Empty,
            Assert.AreEqual("FM", nonIntervalDetail.MeterInterval);// = meterType.Length > 2 ? meterType.Substring(2) : string.Empty,
            Assert.AreEqual(null, nonIntervalDetail.RatchetDateTime);// = ratchetDateTime

            //Qty Fields

            Assert.AreEqual("QD", nonIntervalDetailQty.Qualifier);// = marketFields.AtIndex(2),
            Assert.AreEqual("1475", nonIntervalDetailQty.Quantity);// = marketFields.AtIndex(3),
            Assert.AreEqual("AA", nonIntervalDetailQty.MeasurementCode);// = marketFields.AtIndex(6),
            Assert.AreEqual("KH", nonIntervalDetailQty.Uom);// = marketFields.AtIndex(4),
            Assert.AreEqual("19665", nonIntervalDetailQty.BeginRead);// = marketFields.AtIndex(8),
            Assert.AreEqual("21140", nonIntervalDetailQty.EndRead);// = marketFields.AtIndex(9),
            Assert.AreEqual("51", nonIntervalDetailQty.MeasurementSignificanceCode);// = (measurementCode == string.Empty) ? null : measurementCode,
            Assert.AreEqual("Pos12", nonIntervalDetailQty.TransformerLossFactor);// = marketFields.AtIndex(12),
            Assert.AreEqual("1", nonIntervalDetailQty.MeterMultiplier);// = marketFields.AtIndex(10),
            Assert.AreEqual("Pos11", nonIntervalDetailQty.PowerFactor);// = marketFields.AtIndex(11)


        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void WhenParsingNonIntervalDetail_ForGeorgia_CheckAllAssignedFields() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string file = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|XX|HeaderActionCode|CDB1381148122012213942000|HeaderPosition7|1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||HeaderPosition16|||||||||||||||||||10443720001381148|ERCOT|183529049|ERCOT|HeaderPosition39|Pos40|Pos41|Pos42|Pos43|Pos44|Pos45|Pos46|Pos47|Pos48|Pos49|Pos50|Pos51|Pos52
05|ACTTX183529049USE|20121220|20131220
06|ACTTX183529049USE|QD|99|FM
07|ACTTX183529049USE|07StartDate|07EndDate|M||KH|MON|
08|ACTTX183529049USE|QD|1475|KHFM
15|ACTTX183529049USE|20121120|20121220|104137265LG|Pos5|KH|MON|||A||||||||Pos18|M||||||||||||Pos31
16|ACTTX183529049USE|QD|1475|KH||AA|51|19665|21140|1|Pos11|Pos12
TL|1";

            _clientDataAccess.Stub(x => x.IdentifyMarket("1039940674000")).Return(17); // Forcing to return Georgia as Market


            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = _concern.Parse(stream) as Import867Model;

            // assert
            Assert.IsNotNull(actual);
            var header = actual.TypeHeaders.FirstOrDefault();
            Assert.IsNotNull(header);
            
            var nonIntervalDetail = header.NonIntervalDetails.First();
            var nonIntervalDetailQty = nonIntervalDetail.NonIntervalDetailQtys.First();

            Assert.AreEqual("PL", nonIntervalDetail.TypeCode);
            Assert.AreEqual("104137265LG", nonIntervalDetail.MeterNumber);// = marketFields.AtIndex(4),
            Assert.AreEqual("Pos18", nonIntervalDetail.MovementTypeCode);// = marketFields.AtIndex(18),
            Assert.AreEqual("20121120", nonIntervalDetail.ServicePeriodStart);// = marketFields.AtIndex(2),
            Assert.AreEqual("20121220", nonIntervalDetail.ServicePeriodEnd);// = marketFields.AtIndex(3),
            Assert.AreEqual("Pos5", nonIntervalDetail.ExchangeDate);// = marketFields.AtIndex(5),
            Assert.AreEqual("A", nonIntervalDetail.MeterRole);// = marketFields.AtIndex(10),
            Assert.AreEqual("", nonIntervalDetail.MeterUom);// = meterType.Length > 1 ? meterType.Substring(0, 2) : string.Empty,
            Assert.AreEqual("", nonIntervalDetail.MeterInterval);// = meterType.Length > 2 ? meterType.Substring(2) : string.Empty,
            Assert.AreEqual("Pos31", nonIntervalDetail.RatchetDateTime);// = ratchetDateTime

            //Qty Fields

            Assert.AreEqual("QD", nonIntervalDetailQty.Qualifier);// = marketFields.AtIndex(2),
            Assert.AreEqual("1475", nonIntervalDetailQty.Quantity);// = marketFields.AtIndex(3),
            Assert.AreEqual("AA", nonIntervalDetailQty.MeasurementCode);// = marketFields.AtIndex(6),
            Assert.AreEqual("KH", nonIntervalDetailQty.Uom);// = marketFields.AtIndex(4),
            Assert.AreEqual("19665", nonIntervalDetailQty.BeginRead);// = marketFields.AtIndex(8),
            Assert.AreEqual("21140", nonIntervalDetailQty.EndRead);// = marketFields.AtIndex(9),
            Assert.AreEqual("51", nonIntervalDetailQty.MeasurementSignificanceCode);// = (measurementCode == string.Empty) ? null : measurementCode,
            Assert.AreEqual("Pos12", nonIntervalDetailQty.TransformerLossFactor);// = marketFields.AtIndex(12),
            Assert.AreEqual("1", nonIntervalDetailQty.MeterMultiplier);// = marketFields.AtIndex(10),
            Assert.AreEqual("Pos11", nonIntervalDetailQty.PowerFactor);// = marketFields.AtIndex(11)


        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void WhenParsingUnMeterDetail_For15Record_CheckAllAssignedFields() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string file = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|XX|HeaderActionCode|CDB1381148122012213942000|HeaderPosition7|1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||HeaderPosition16|||||||||||||||||||10443720001381148|ERCOT|183529049|ERCOT|HeaderPosition39|Pos40|Pos41|Pos42|Pos43|Pos44|Pos45|Pos46|Pos47|Pos48|Pos49|Pos50|Pos51|Pos52
05|ACTTX183529049USE|20121220|20131220
06|ACTTX183529049USE|QD|99|FM
07|ACTTX183529049USE|07StartDate|07EndDate|M||KH|MON|
08|ACTTX183529049USE|QD|1475|KHFM
15|ACTTX183529049USE|20121120|20121220||Pos5|KH|MON|||A|||Pos13||||Pos17|Pos18|M|
16|ACTTX183529049USE|QD|1475|KH|Pos5|AA|51|19665|21140|1|Pos11|Pos12|||Pos15|Pos16
TL|1";

            _clientDataAccess.Stub(x => x.IdentifyMarket("1039940674000")).Return(1);


            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = _concern.Parse(stream) as Import867Model;
 

            // assert
            Assert.IsNotNull(actual);
            var header = actual.TypeHeaders.FirstOrDefault();
            Assert.IsNotNull(header);
            
            var unMeterDetail = header.UnMeterDetails.First();
            var unMeterDetailQty = unMeterDetail.UnMeterDetailQtys.First();


            Assert.AreEqual("BD", unMeterDetail.TypeCode);// = context.RecordType,
            Assert.AreEqual("20121120", unMeterDetail.ServicePeriodStart);// = servicePeriodStart,
            Assert.AreEqual("20121220", unMeterDetail.ServicePeriodEnd);// = servicePeriodEnd,
            Assert.AreEqual("Pos13", unMeterDetail.ServiceType);// = serviceType,
            Assert.AreEqual("Pos17", unMeterDetail.Description);// = description

           

            //Qty Fields
            Assert.AreEqual("QD", unMeterDetailQty.Qualifier);// = marketFields.AtIndex(2),
            Assert.AreEqual("1475", unMeterDetailQty.Quantity);// = marketFields.AtIndex(3),
            Assert.AreEqual("Pos15", unMeterDetailQty.CompositeUom);// = marketFields.AtIndex(15),
            Assert.AreEqual("Pos5", unMeterDetailQty.NumberOfDevices);// = marketFields.AtIndex(5),
            Assert.AreEqual("Pos16", unMeterDetailQty.ConsumptionPerDevice);// = marketFields.AtIndex(16)

          
        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void WhenParsingUnMeterDetail_For07Record_CheckAllAssignedFields() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string file = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|DD|HeaderActionCode|CDB1381148122012213942000|HeaderPosition7|1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||HeaderPosition16|||||||||||||||||||10443720001381148|ERCOT|183529049|ERCOT|HeaderPosition39|Pos40|Pos41|Pos42|Pos43|Pos44|Pos45|Pos46|Pos47|Pos48|Pos49|Pos50|Pos51|Pos52
05|ACTTX183529049USE|20121220|20131220
06|ACTTX183529049USE|QD|99|FM
07|ACTTX183529049USE|07StartDate|07EndDate|U||KH|MON|
08|ACTTX183529049USE|QD|1475|KHFM|Pos5||||||||||Pos15|Pos16
15|ACTTX183529049USE|20121120|20121220|12345GL|Pos5|KH|MON|||A|||Pos13||||Pos17|Pos18|M|
16|ACTTX183529049USE|QD|1475|KH|Pos5|AA|51|19665|21140|1|Pos11|Pos12|||Pos15|Pos16
TL|1";

            _clientDataAccess.Stub(x => x.IdentifyMarket("1039940674000")).Return(2); // market has to be Maryland


            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = _concern.Parse(stream) as Import867Model;
 

            // assert
            Assert.IsNotNull(actual);
            var header = actual.TypeHeaders.FirstOrDefault();
            Assert.IsNotNull(header);
            
            var unMeterDetail = header.UnMeterDetails.First();
            var unMeterDetailQty = unMeterDetail.UnMeterDetailQtys.First();


            Assert.AreEqual("BD", unMeterDetail.TypeCode);// = context.RecordType,
            Assert.AreEqual("07StartDate", unMeterDetail.ServicePeriodStart);// = servicePeriodStart,
            Assert.AreEqual("07EndDate", unMeterDetail.ServicePeriodEnd);// = servicePeriodEnd,
            Assert.AreEqual("BD", unMeterDetail.ServiceType);// = serviceType,
            Assert.AreEqual("", unMeterDetail.Description);// = description



            //Qty Fields
            Assert.AreEqual("QD", unMeterDetailQty.Qualifier);// = marketFields.AtIndex(2),
            Assert.AreEqual("1475", unMeterDetailQty.Quantity);// = marketFields.AtIndex(3),
            Assert.AreEqual("Pos15", unMeterDetailQty.CompositeUom);// = marketFields.AtIndex(15),
            Assert.AreEqual("Pos5", unMeterDetailQty.NumberOfDevices);// = marketFields.AtIndex(5),
            Assert.AreEqual("Pos16", unMeterDetailQty.ConsumptionPerDevice);// = marketFields.AtIndex(16)


        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void WhenParsingIntervalSummary_For15Record_CheckAllAssignedFields() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string file = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|XX|HeaderActionCode|CDB1381148122012213942000|HeaderPosition7|1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||HeaderPosition16|||||||||||||||||||10443720001381148|ERCOT|183529049|ERCOT|HeaderPosition39|Pos40|Pos41|Pos42|Pos43|Pos44|Pos45|Pos46|Pos47|Pos48|Pos49|Pos50|Pos51|Pos52
05|ACTTX183529049USE|20121220|20131220
06|ACTTX183529049USE|QD|99|FM
07|ACTTX183529049USE|07StartDate|07EndDate|M||KH|MON|
08|ACTTX183529049USE|QD|1475|KHFM
15|ACTTX183529049USE|20121120|20121220|12345GH|Pos5|KH|999|||A|||Pos13|||Pos16|Pos17|Pos18|M|
16|ACTTX183529049USE|QD|1475|KH|Pos5|AA|51|19665|21140|1|Pos11|Pos12|||Pos15|Pos16
TL|1";

            _clientDataAccess.Stub(x => x.IdentifyMarket("1039940674000")).Return(1);


            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = _concern.Parse(stream) as Import867Model;
 

            // assert
            Assert.IsNotNull(actual);
            var header = actual.TypeHeaders.FirstOrDefault();
            Assert.IsNotNull(header);
            
            var intervalSummary = header.IntervalSummaries.First();
            var intervalSummaryQty = intervalSummary.IntervalSummaryQtys.First();

            Assert.AreEqual("BO", intervalSummary.TypeCode);// = context.RecordType;
            Assert.AreEqual("12345GH", intervalSummary.MeterNumber);// = marketFields.AtIndex(4);
            Assert.AreEqual("Pos18", intervalSummary.MovementTypeCode);// = marketFields.AtIndex(18);
            Assert.AreEqual("20121120", intervalSummary.ServicePeriodStart);// = marketFields.AtIndex(2);
            Assert.AreEqual("20121220", intervalSummary.ServicePeriodEnd);// = marketFields.AtIndex(3);
            Assert.AreEqual("Pos5", intervalSummary.ExchangeDate);// = marketFields.AtIndex(5);
            Assert.AreEqual("Pos16", intervalSummary.ChannelNumber);// = marketFields.AtIndex(16);
            Assert.AreEqual("A", intervalSummary.MeterRole);// = marketFields.AtIndex(10);
            Assert.AreEqual("KH", intervalSummary.MeterUOM);// = meterType.Length > 1 ? meterType.Substring(0, 2) : string.Empty;
            Assert.AreEqual("999", intervalSummary.MeterInterval);// = meterType.Length > 2 ? meterType.Substring(2, 3) : string.Empty;
            
          


            //Qty Fields
            Assert.AreEqual("QD", intervalSummaryQty.Qualifier);//  = marketFields.AtIndex(2);
            Assert.AreEqual("1475", intervalSummaryQty.Quantity);//  = marketFields.AtIndex(3);
            Assert.AreEqual("AA", intervalSummaryQty.MeasurementCode);//  = marketFields.AtIndex(6);
            Assert.AreEqual("KH", intervalSummaryQty.Uom);//  = marketFields.AtIndex(4);
            Assert.AreEqual("19665", intervalSummaryQty.BeginRead);//  = marketFields.AtIndex(8);
            Assert.AreEqual("21140", intervalSummaryQty.EndRead);//  = marketFields.AtIndex(9);
            Assert.AreEqual("51", intervalSummaryQty.MeasurementSignificanceCode);//  = marketFields.AtIndex(7);
            Assert.AreEqual("Pos12", intervalSummaryQty.TransformerLossFactor);//  = marketFields.AtIndex(12);
            Assert.AreEqual("1", intervalSummaryQty.MeterMultiplier);//  = marketFields.AtIndex(10);
            Assert.AreEqual("Pos11", intervalSummaryQty.PowerFactor);//  = marketFields.AtIndex(11);
            


        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void WhenParsingIntervalSummary_For07Record_CheckAllAssignedFields() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string file = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|C1|HeaderActionCode|CDB1381148122012213942000|HeaderPosition7|1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||HeaderPosition16|||||||||||||||||||10443720001381148|ERCOT|183529049|ERCOT|HeaderPosition39|Pos40|Pos41|Pos42|Pos43|Pos44|Pos45|Pos46|Pos47|Pos48|Pos49|Pos50|Pos51|Pos52
05|ACTTX183529049USE|20121220|20131220
06|ACTTX183529049USE|QD|99|FM
07|ACTTX183529049USE|07StartDate|07EndDate|M||KH|MON|
08|ACTTX183529049USE|QD|1475|KHFM
15|ACTTX183529049USE|20121120|20121220|12345GH|Pos5|KH|MON|||A|||Pos13|||Pos16|Pos17|Pos18|M|
16|ACTTX183529049USE|QD|1475|KH|Pos5|AA|51|19665|21140|1|Pos11|Pos12|||Pos15|Pos16
TL|1";

            _clientDataAccess.Stub(x => x.IdentifyMarket("1039940674000")).Return(2); // market has to be Maryland


            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = _concern.Parse(stream) as Import867Model;


            // assert
            Assert.IsNotNull(actual);
            var header = actual.TypeHeaders.FirstOrDefault();
            Assert.IsNotNull(header);
            
            var intervalSummary = header.IntervalSummaries.First();
            var intervalSummaryQty = intervalSummary.IntervalSummaryQtys.First();

            Assert.AreEqual("BO", intervalSummary.TypeCode);// = context.RecordType;
            Assert.AreEqual("10443720001381148", intervalSummary.MeterNumber);// = context.EsiId;
            Assert.AreEqual(null, intervalSummary.MovementTypeCode);// = null;
            Assert.AreEqual("07StartDate", intervalSummary.ServicePeriodStart);// = marketFields.AtIndex(2);
            Assert.AreEqual("07EndDate", intervalSummary.ServicePeriodEnd);// = marketFields.AtIndex(3);
            Assert.AreEqual(null, intervalSummary.ExchangeDate);// = marketFields.AtIndex(5);
            Assert.AreEqual(null, intervalSummary.ChannelNumber);// = marketFields.AtIndex(16);
            Assert.AreEqual("A", intervalSummary.MeterRole);// = marketFields.AtIndex(10);
            Assert.AreEqual("KH", intervalSummary.MeterUOM);// = meterType.Length > 1 ? meterType.Substring(0, 2) : string.Empty;
            Assert.AreEqual("MON", intervalSummary.MeterInterval);// = meterType.Length > 2 ? meterType.Substring(2, 3) : string.Empty;




            //Qty Fields
            Assert.AreEqual("QD", intervalSummaryQty.Qualifier);//  = marketFields.AtIndex(2);
            Assert.AreEqual("1475", intervalSummaryQty.Quantity);//  = marketFields.AtIndex(3);
            Assert.AreEqual(null, intervalSummaryQty.MeasurementCode);//  = null;
            Assert.AreEqual("KHFM", intervalSummaryQty.Uom);//  = marketFields.AtIndex(4);
            Assert.AreEqual(null, intervalSummaryQty.BeginRead);//  = null;
            Assert.AreEqual(null, intervalSummaryQty.EndRead);//  = null;
            Assert.AreEqual("51", intervalSummaryQty.MeasurementSignificanceCode);//  = marketFields.AtIndex(7);
            Assert.AreEqual(null, intervalSummaryQty.TransformerLossFactor);//  = null;
            Assert.AreEqual("1", intervalSummaryQty.MeterMultiplier);//  = "1";
            Assert.AreEqual(null, intervalSummaryQty.PowerFactor);//  = null;



        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void WhenParsingNetIntervalSummary_ForTexasMarket_CheckAllAssignedFields() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string file = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|XX|HeaderActionCode|CDB1381148122012213942000|HeaderPosition7|1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||HeaderPosition16|||||||||||||||||||10443720001381148|ERCOT|183529049|ERCOT|HeaderPosition39|Pos40|Pos41|Pos42|Pos43|Pos44|Pos45|Pos46|Pos47|Pos48|Pos49|Pos50|Pos51|Pos52
05|ACTTX183529049USE|20121220|20131220
06|ACTTX183529049USE|QD|99|FM
07|ACTTX183529049USE|07StartDate|07EndDate|I||KH|MON|
08|ACTTX183529049USE|QD|1475|KHFM|Pos5||||||||||Pos15|Pos16
09|ACTTX183529049USE|Pos2|Pos3|Pos4|Pos5
15|ACTTX183529049USE|20121120|20121220|12345GL|Pos5|KH|MON|||A|||Pos13||||Pos17|Pos18|M|
16|ACTTX183529049USE|QD|1475|KH|Pos5|AA|51|19665|21140|1|Pos11|Pos12|||Pos15|Pos16
TL|1";

            _clientDataAccess.Stub(x => x.IdentifyMarket("1039940674000")).Return(1); //Texas Market


            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = _concern.Parse(stream) as Import867Model;

            // assert
            Assert.IsNotNull(actual);
            var header = actual.TypeHeaders.FirstOrDefault();
            Assert.IsNotNull(header);
            
            var netInervalSummary = header.NetIntervalSummaries.First();
            var netInervalSummaryQty = netInervalSummary.NetIntervalSummaryQtys.First();


            Assert.AreEqual("KH", netInervalSummary.MeterUom);// =marketFields.AtIndex(6),
            Assert.AreEqual("MON", netInervalSummary.MeterInterval);// = marketFields.AtIndex(7),
            Assert.AreEqual("IA", netInervalSummary.TypeCode);// = context.RecordType
            



            //Qty Fields
            Assert.AreEqual("QD", netInervalSummaryQty.Qualifier);
            Assert.AreEqual("1475", netInervalSummaryQty.Quantity);
            Assert.AreEqual("Pos4", netInervalSummaryQty.ServicePeriodStart);
            Assert.AreEqual("Pos5", netInervalSummaryQty.ServicePeriodEnd);


        }

        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void WhenParsingNetIntervalSummary_ForNonTexasMarket_CheckAllAssignedFields() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string file = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|XX|HeaderActionCode|CDB1381148122012213942000|HeaderPosition7|1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||HeaderPosition16|||||||||||||||||||10443720001381148|ERCOT|183529049|ERCOT|HeaderPosition39|Pos40|Pos41|Pos42|Pos43|Pos44|Pos45|Pos46|Pos47|Pos48|Pos49|Pos50|Pos51|Pos52
05|ACTTX183529049USE|20121220|20131220
06|ACTTX183529049USE|QD|99|FM
07|ACTTX183529049USE|07StartDate|07EndDate|I||KH|MON|
08|ACTTX183529049USE|QD|1475|KHFM|Pos5||||||||||Pos15|Pos16
09|ACTTX183529049USE|Pos2|Pos3|Pos4|Pos5
15|ACTTX183529049USE|20121120|20121220|12345GL|Pos5|KH|MON|||A|||Pos13||||Pos17|Pos18|M|
16|ACTTX183529049USE|QD|1475|KH|Pos5|AA|51|19665|21140|1|Pos11|Pos12|||Pos15|Pos16
TL|1";

            _clientDataAccess.Stub(x => x.IdentifyMarket("1039940674000")).Return(2); //Maryland


            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = _concern.Parse(stream) as Import867Model;


            // assert
            Assert.IsNotNull(actual);
            var header = actual.TypeHeaders.FirstOrDefault();
            Assert.IsNotNull(header);
            
            var netInervalSummary = header.NetIntervalSummaries.First();
            var netInervalSummaryQty = netInervalSummary.NetIntervalSummaryQtys.First();


            Assert.AreEqual("KH", netInervalSummary.MeterUom);// =marketFields.AtIndex(6),
            Assert.AreEqual("MON", netInervalSummary.MeterInterval);// = marketFields.AtIndex(7),
            Assert.AreEqual("IA", netInervalSummary.TypeCode);// = context.RecordType




            //Qty Fields
            Assert.AreEqual("QD", netInervalSummaryQty.Qualifier);
            Assert.AreEqual("1475", netInervalSummaryQty.Quantity);
            Assert.AreEqual("20121220", netInervalSummaryQty.ServicePeriodStart);
            Assert.AreEqual("20131220", netInervalSummaryQty.ServicePeriodEnd);


        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void WhenParsingIntervalDetail_CheckAllAssignedFields() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string file = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|XX|HeaderActionCode|CDB1381148122012213942000|HeaderPosition7|1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||HeaderPosition16|||||||||||||||||||10443720001381148|ERCOT|183529049|ERCOT|HeaderPosition39|Pos40|Pos41|Pos42|Pos43|Pos44|Pos45|Pos46|Pos47|Pos48|Pos49|Pos50|Pos51|Pos52
05|ACTTX183529049USE|20121220|20131220
06|ACTTX183529049USE|QD|99|FM
07|ACTTX183529049USE|07StartDate|07EndDate|M||KH|MON|
08|ACTTX183529049USE|QD|1475|KHFM
15|ACTTX183529049USE|20121120|20121220|12345GH|Pos5|KH|999|||A|||Pos13|||Pos16|Pos17|Pos18|M|
16|ACTTX183529049USE|QD|1475|KH|Pos5|AA|51|19665|21140|1|Pos11|Pos12|||Pos15|Pos16
17|ACTTX183529049USE|20121120|20121220|12345GH|Pos5|KH|999|Pos8|Pos9|A|||Pos13||||Pos17|Pos18|M|
18|ACTTX183529049USE|QD|1475||Pos5|Pos6
TL|1";

            _clientDataAccess.Stub(x => x.IdentifyMarket("1039940674000")).Return(1);


            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = _concern.Parse(stream) as Import867Model;


            // assert
            Assert.IsNotNull(actual);
            var header = actual.TypeHeaders.FirstOrDefault();
            Assert.IsNotNull(header);
            
            var intervalDetail = header.IntervalDetails.First();
            var intervalDetailQty = intervalDetail.IntervalDetailQtys.First();

            Assert.AreEqual("PM", intervalDetail.TypeCode);// = context.RecordType;
            Assert.AreEqual("12345GH", intervalDetail.MeterNumber);// = marketFields.AtIndex(4);
            Assert.AreEqual("20121120", intervalDetail.ServicePeriodStart);// = marketFields.AtIndex(2);
            Assert.AreEqual("20121220", intervalDetail.ServicePeriodEnd);// = marketFields.AtIndex(3);
            Assert.AreEqual("Pos5", intervalDetail.ExchangeDate);// = marketFields.AtIndex(5);
            Assert.AreEqual("Pos8", intervalDetail.ChannelNumber);// = marketFields.AtIndex(8);
            Assert.AreEqual("Pos9", intervalDetail.MeterRole);// = marketFields.AtIndex(9);
            Assert.AreEqual("KH", intervalDetail.MeterUOM);// = meterType.Length > 1 ? meterType.Substring(0, 2) : string.Empty;
            Assert.AreEqual("999", intervalDetail.MeterInterval);// = meterType.Length > 2 ? meterType.Substring(2, 3) : string.Empty;




            //Qty Fields
            Assert.AreEqual("QD", intervalDetailQty.Qualifier);//  = marketFields.AtIndex(2);
            Assert.AreEqual("1475", intervalDetailQty.Quantity);//  = marketFields.AtIndex(3);
            Assert.AreEqual("Pos5", intervalDetailQty.IntervalEndDate);//  = marketFields.AtIndex(5);
            Assert.AreEqual("Pos6", intervalDetailQty.IntervalEndTime);//  = marketFields.AtIndex(6);
            



        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void WhenParsingScheduledDeterminants_CheckAllAssignedFields() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string file = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|XX|HeaderActionCode|CDB1381148122012213942000|HeaderPosition7|1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||HeaderPosition16|||||||||||||||||||10443720001381148|ERCOT|183529049|ERCOT|HeaderPosition39|Pos40|Pos41|Pos42|Pos43|Pos44|Pos45|Pos46|Pos47|Pos48|Pos49|Pos50|Pos51|Pos52
05|ACTTX183529049USE|20121220|20131220
06|ACTTX183529049USE|QD|99|FM
07|ACTTX183529049USE|07StartDate|07EndDate|M||KH|MON|
08|ACTTX183529049USE|QD|1475|KHFM
15|ACTTX183529049USE|20121120|20121220|12345GH|Pos5|KH|999|||A|||Pos13|||Pos16|Pos17|Pos18|M|
16|ACTTX183529049USE|QD|1475|KH|Pos5|AA|51|19665|21140|1|Pos11|Pos12|||Pos15|Pos16
17|ACTTX183529049USE|20121120|20121220|12345GH|Pos5|KH|999|Pos8|Pos9|A|||Pos13||||Pos17|Pos18|M|
18|ACTTX183529049USE|QD|1475||Pos5|Pos6
20|ACTTX183529049USE|2.434|3.4|Pos4|Pos5|111||||||Pos12|Pos13
TL|1";

            _clientDataAccess.Stub(x => x.IdentifyMarket("1039940674000")).Return(1);


            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = _concern.Parse(stream) as Import867Model;


            // assert
            Assert.IsNotNull(actual);
            var header = actual.TypeHeaders.FirstOrDefault();
            Assert.IsNotNull(header);
            
            var scheduledDeterminant = header.ScheduleDeterminants.First();


            Assert.AreEqual("2.434", scheduledDeterminant.CapacityObligation);//  = marketFields.AtIndex(2),
            Assert.AreEqual("3.4", scheduledDeterminant.TransmissionObligation);//  = marketFields.AtIndex(3),
            Assert.AreEqual("Pos4", scheduledDeterminant.LoadProfile);//  = marketFields.AtIndex(4),
            Assert.AreEqual("Pos5", scheduledDeterminant.LDCRateClass);//  = marketFields.AtIndex(5),
            Assert.AreEqual("Pos12", scheduledDeterminant.SpecialMeterConfig);//  = marketFields.AtIndex(12),
            Assert.AreEqual("Pos13", scheduledDeterminant.MaximumGeneration);//  = marketFields.AtIndex(13)

       



            


        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void WhenParsingSwitch_CheckAllAssignedFields() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string file = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|XX|HeaderActionCode|CDB1381148122012213942000|HeaderPosition7|1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||HeaderPosition16|||||||||||||||||||10443720001381148|ERCOT|183529049|ERCOT|HeaderPosition39|Pos40|Pos41|Pos42|Pos43|Pos44|Pos45|Pos46|Pos47|Pos48|Pos49|Pos50|Pos51|Pos52
05|ACTTX183529049USE|20121220|20131220
06|ACTTX183529049USE|QD|99|FM
07|ACTTX183529049USE|07StartDate|07EndDate|M||KH|MON|
08|ACTTX183529049USE|QD|1475|KHFM
15|ACTTX183529049USE|20121120|20121220|12345GH|Pos5|KH|999|||A|||Pos13|||Pos16|Pos17|Pos18|M|
16|ACTTX183529049USE|QD|1475|KH|Pos5|AA|51|19665|21140|1|Pos11|Pos12|||Pos15|Pos16
17|ACTTX183529049USE|20121120|20121220|12345GH|Pos5|KH|999|Pos8|Pos9|A|||Pos13||||Pos17|Pos18|M|
18|ACTTX183529049USE|QD|1475||Pos5|Pos6
30|ACTTX183529049USE|12345GH|20121120
35|ACTTX183529049USE|QD|KH|1475|Pos5
TL|1";

            _clientDataAccess.Stub(x => x.IdentifyMarket("1039940674000")).Return(1);


            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = _concern.Parse(stream) as Import867Model;


            // assert
            Assert.IsNotNull(actual);
            var header = actual.TypeHeaders.FirstOrDefault();
            Assert.IsNotNull(header);
            
            var tranSwitch = header.Switches.First();
            var tranSwitchQty = tranSwitch.SwitchQtys.First();

            Assert.AreEqual("BJ", tranSwitch.TypeCode);// = context.RecordType;
            Assert.AreEqual("12345GH", tranSwitch.MeterNumber);// = marketFields.AtIndex(2);
            Assert.AreEqual("20121120", tranSwitch.SwitchDate);// = marketFields.AtIndex(3);
            



            //Qty Fields
            Assert.AreEqual("QD", tranSwitchQty.Qualifier);// = marketFields.AtIndex(2),
            Assert.AreEqual("KH", tranSwitchQty.Uom);// = marketFields.AtIndex(3),
            Assert.AreEqual("1475", tranSwitchQty.SwitchRead);// = marketFields.AtIndex(4),
            Assert.AreEqual("Pos5", tranSwitchQty.MeasurementSignificanceCode);// = marketFields.AtIndex(5),
            Assert.AreEqual(null, tranSwitchQty.Message);// = null



        }


        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void WhenParsingTransaction_CheckTransactionCount() // ReSharper restore InconsistentNaming
        {
            // arrange
            const string file = @"SH|ACTTX183529049USE|CDB1381148122012213942000|I|
01|ACTTX183529049USE|TX|00|XX|HeaderActionCode|CDB1381148122012213942000|HeaderPosition7|1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||HeaderPosition16|||||||||||||||||||10443720001381148|ERCOT|183529049|ERCOT|HeaderPosition39|Pos40|Pos41|Pos42|Pos43|Pos44|Pos45|Pos46|Pos47|Pos48|Pos49|Pos50|Pos51|Pos52
05|ACTTX183529049USE|20121220|20131220
06|ACTTX183529049USE|QD|99|FM
07|ACTTX183529049USE|07StartDate|07EndDate|M||KH|MON|
08|ACTTX183529049USE|QD|1475|KHFM
15|ACTTX183529049USE|20121120|20121220|12345GH|Pos5|KH|999|||A|||Pos13|||Pos16|Pos17|Pos18|M|
16|ACTTX183529049USE|QD|1475|KH|Pos5|AA|51|19665|21140|1|Pos11|Pos12|||Pos15|Pos16
17|ACTTX183529049USE|20121120|20121220|12345GH|Pos5|KH|999|Pos8|Pos9|A|||Pos13||||Pos17|Pos18|M|
18|ACTTX183529049USE|QD|1475||Pos5|Pos6
30|ACTTX183529049USE|12345GH|20121120
35|ACTTX183529049USE|QD|KH|1475|Pos5
SH|ACTTX183529049USE|CDB1381148122012213942009|I|
01|ACTTX183529049USE|TX|00|XX|HeaderActionCode|CDB1381148122012213942000|HeaderPosition7|1039940674000|ONCOR|133305370|ACCENT ENERGY TEXAS, LLC|201212200000|20121220224317|||HeaderPosition16|||||||||||||||||||10443720001381148|ERCOT|183529049|ERCOT|HeaderPosition39|Pos40|Pos41|Pos42|Pos43|Pos44|Pos45|Pos46|Pos47|Pos48|Pos49|Pos50|Pos51|Pos52
05|ACTTX183529049USE|20121220|20131220
06|ACTTX183529049USE|QD|99|FM
07|ACTTX183529049USE|07StartDate|07EndDate|M||KH|MON|
08|ACTTX183529049USE|QD|1475|KHFM
15|ACTTX183529049USE|20121120|20121220|12345GH|Pos5|KH|999|||A|||Pos13|||Pos16|Pos17|Pos18|M|
16|ACTTX183529049USE|QD|1475|KH|Pos5|AA|51|19665|21140|1|Pos11|Pos12|||Pos15|Pos16
17|ACTTX183529049USE|20121120|20121220|12345GH|Pos5|KH|999|Pos8|Pos9|A|||Pos13||||Pos17|Pos18|M|
18|ACTTX183529049USE|QD|1475||Pos5|Pos6
30|ACTTX183529049USE|12345GH|20121120
35|ACTTX183529049USE|QD|KH|1475|Pos5
TL|2";

            _clientDataAccess.Stub(x => x.IdentifyMarket("1039940674000")).Return(1);


            // act
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(file));
            var actual = _concern.Parse(stream) as Import867Model;


            // assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Headers.Length);
            Assert.AreEqual(2, actual.TransactionAuditCount);
            Assert.AreEqual(actual.TransactionActualCount, actual.TransactionAuditCount);


        }

        [TestMethod]
        public void Can_Parse_Example_867_And_Have_Headers_Returned_Without_A_TL_Line()
        {
            // arrange
            const string file =
                @"SH|LPTTX183529049USE|979797842020130723203951110439_oldglob|I|
01|LPTTX183529049USE|TX|00|DD||979797842020130723203951110439_oldglob||007923311|AEP TEXAS NORTH (ERCOT)|6116339131000|LPT SP LP|201307240000|20130724090714||||||||||||||||||||||10204049745905620|ERCOT|183529049|ERCOT|
07|LPTTX183529049USE|||M||KH|MON|
08|LPTTX183529049USE|QD|373|
09|LPTTX183529049USE|373|51|20130621|20130723|
15|LPTTX183529049USE|20130621|20130723|110312079||KH|MON|||A|||||||||M|
16|LPTTX183529049USE|QD|373|KH||AA|51|4021|4394|1|

";

            _clientDataAccess.Stub(x => x.IdentifyMarket("007923311")).Return(1);

            // act
            var stream = new MemoryStream((Encoding.UTF8.GetBytes(file)));
            var actual = _concern.Parse(stream) as Import867Model;

            // assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Headers.Length);
        }
    }
}

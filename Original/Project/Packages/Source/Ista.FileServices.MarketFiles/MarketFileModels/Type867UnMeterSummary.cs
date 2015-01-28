using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867UnMeterSummary : IType867Model
    {
        private readonly List<Type867UnMeterSummaryQty> unMeterSummaryQtys;

        public Type867Types ModelType
        {
            get { return Type867Types.UnMeterSummary; }
        }

        public int UnMeterSummaryKey { get; set; }
        public int HeaderKey { get; set; }
        public string TypeCode { get; set; }
        public string MeterUom { get; set; }
        public string MeterInterval { get; set; }
        public string CommodityCode { get; set; }
        public string UtilityRateServiceClass { get; set; }
        public string RateSubClass { get; set; }
        
        public Type867UnMeterSummaryQty[] UnMeterSummaryQtys
        {
            get { return unMeterSummaryQtys.ToArray(); }
        }

        public Type867UnMeterSummary()
        {
            unMeterSummaryQtys = new List<Type867UnMeterSummaryQty>();
        }

        public void AddQuantity(Type867UnMeterSummaryQty item)
        {
            unMeterSummaryQtys.Add(item);
        }
    }
}
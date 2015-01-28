using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867IntervalDetail : IType867Model
    {
        private readonly List<Type867IntervalDetailQty> intervalDetailQtys;

        public Type867Types ModelType
        {
            get { return Type867Types.IntervalDetail; }
        }

        public int IntervalDetailKey { get; set; }
        public int IntervalSummaryKey { get; set; }
        public int HeaderKey { get; set; }
        public string TypeCode { get; set; }
        public string MeterNumber { get; set; }
        public string ServicePeriodStart { get; set; }
        public string ServicePeriodEnd { get; set; }
        public string ExchangeDate { get; set; }
        public string ChannelNumber { get; set; }
        public string MeterUOM { get; set; }
        public string MeterInterval { get; set; }
        public string MeterRole { get; set; }
        public string CommodityCode { get; set; }
        public string NumberOfDials { get; set; }
        public string ServicePointId { get; set; }
        public string UtilityRateServiceClass { get; set; }
        public string RateSubClass { get; set; }
        
        public Type867IntervalDetailQty[] IntervalDetailQtys
        {
            get { return intervalDetailQtys.ToArray(); }
        }

        public Type867IntervalDetail ()
        {
            intervalDetailQtys = new List<Type867IntervalDetailQty>();
        }

        public void AddQuantity(Type867IntervalDetailQty item)
        {
            intervalDetailQtys.Add(item);
        }
    }
}
using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867IntervalSummary: IType867Model
    {
        private readonly List<Type867IntervalSummaryQty> intervalSummaryQtys;

        public Type867Types ModelType
        {
            get { return Type867Types.IntervalSummary; }
        }

        public int IntervalSummaryKey { get; set; }
        public int HeaderKey { get; set; }
        public string TypeCode { get; set; }
        public string MeterNumber { get; set; }
        public string MovementTypeCode { get; set; }
        public string ServicePeriodStart { get; set; }
        public string ServicePeriodEnd { get; set; }
        public string ExchangeDate { get; set; }
        public string ChannelNumber { get; set; }
        public string MeterRole { get; set; }
        public string MeterUOM { get; set; }
        public string MeterInterval { get; set; }
        public string CommodityCode { get; set; }
        public string NumberOfDials { get; set; }
        public string ServicePointId { get; set; }
        public string UtilityRateServiceClass { get; set; }
        public string RateSubClass { get; set; }
        public string LoadProfile { get; set; }
        
        public Type867IntervalSummaryQty[] IntervalSummaryQtys
        {
            get { return intervalSummaryQtys.ToArray(); }
        }

        public Type867IntervalSummary()
        {
            intervalSummaryQtys = new List<Type867IntervalSummaryQty>();
        }

        public void AddQuantity(Type867IntervalSummaryQty item)
        {
            intervalSummaryQtys.Add(item);
        }

    }
}
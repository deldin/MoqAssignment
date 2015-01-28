using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867IntervalSummaryAcrossMeters: IType867Model
    {
        private readonly List<Type867IntervalSummaryAcrossMetersQty> intervalSummaryAcrossMetersQtys;

        public Type867Types ModelType
        {
            get { return Type867Types.IntervalSummaryAcrossMeters; }
        }

        public int IntervalSummaryAcrossMetersKey { get; set; }
        public int HeaderKey { get; set; }
        public string TypeCode { get; set; }
        public string ServicePeriodStart { get; set; }
        public string ServicePeriodStartTime { get; set; }
        public string ServicePeriodEnd { get; set; }
        public string ServicePeriodEndTime { get; set; }
        public string MeterRole { get; set; }
        public string MeterUOM { get; set; }
        public string MeterInterval { get; set; }
        
        public Type867IntervalSummaryAcrossMetersQty[] IntervalSummaryAcrossMetersQtys
        {
            get { return intervalSummaryAcrossMetersQtys.ToArray(); }
        }

        public Type867IntervalSummaryAcrossMeters()
        {
            intervalSummaryAcrossMetersQtys = new List<Type867IntervalSummaryAcrossMetersQty>();
        }

        public void AddQuantity(Type867IntervalSummaryAcrossMetersQty item)
        {
            intervalSummaryAcrossMetersQtys.Add(item);
        }
    }
}
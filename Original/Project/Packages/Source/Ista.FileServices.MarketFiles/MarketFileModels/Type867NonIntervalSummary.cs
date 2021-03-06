using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867NonIntervalSummary : IType867Model
    {
        private readonly List<Type867NonIntervalSummaryQty> nonIntervalSummaryQtys;

        public Type867Types ModelType
        {
            get { return Type867Types.NonIntervalSummary; }
        }

        public int NonIntervalSummaryKey { get; set; }
        public int HeaderKey { get; set; }
        public string TypeCode { get; set; }
        public string MeterUOM { get; set; }
        public string MeterInterval { get; set; }
        public string CommodityCode { get; set; }
        public string NumberOfDials { get; set; }
        public string ServicePointId { get; set; }
        public string UtilityRateServiceClass { get; set; }
        public string RateSubClass { get; set; }
        public string ActualSwitchDate { get; set; }
        public string SequenceNumber { get; set; }
        public string ServiceIndicator { get; set; }
        
        public Type867NonIntervalSummaryQty[] NonIntervalSummaryQtys
        {
            get { return nonIntervalSummaryQtys.ToArray(); }
        }
        
        public Type867NonIntervalSummary()
        {
            nonIntervalSummaryQtys = new List<Type867NonIntervalSummaryQty>();
        }

        public void AddQuantity(Type867NonIntervalSummaryQty item)
        {
            nonIntervalSummaryQtys.Add(item);
        }
    }
}
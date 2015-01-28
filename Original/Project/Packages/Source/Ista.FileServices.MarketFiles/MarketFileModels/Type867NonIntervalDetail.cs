using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867NonIntervalDetail   : IType867Model
    {
        private readonly List<Type867NonIntervalDetailQty> nonIntervalDetailQtys;

        public Type867Types ModelType
        {
            get { return Type867Types.NonIntervalDetail; }
        }

        public int NonIntervalDetailKey { get; set; }
        public int NonIntervalSummaryKey { get; set; }
        public int HeaderKey { get; set; }
        public string TypeCode { get; set; }
        public string MeterNumber { get; set; }
        public string MovementTypeCode { get; set; }
        public string ServicePeriodStart { get; set; }
        public string ServicePeriodEnd { get; set; }
        public string ExchangeDate { get; set; }
        public string MeterRole { get; set; }
        public string MeterUom { get; set; }
        public string MeterInterval { get; set; }
        public string CommodityCode { get; set; }
        public string NumberOfDials { get; set; }
        public string ServicePointId { get; set; }
        public string UtilityRateServiceClass { get; set; }
        public string RateSubClass { get; set; }
        public string SequenceNumber { get; set; }
        public string ServiceIndicator { get; set; }
        public string LoadProfile { get; set; }
        public string RatchetDateTime { get; set; }

        public Type867NonIntervalDetailQty[] NonIntervalDetailQtys
        {
            get { return nonIntervalDetailQtys.ToArray(); }
        }
        
        public Type867NonIntervalDetail()
        {
            nonIntervalDetailQtys = new List<Type867NonIntervalDetailQty>();
        }

        public void AddQuantity(Type867NonIntervalDetailQty item)
        {
            nonIntervalDetailQtys.Add(item);
        }
    }
}
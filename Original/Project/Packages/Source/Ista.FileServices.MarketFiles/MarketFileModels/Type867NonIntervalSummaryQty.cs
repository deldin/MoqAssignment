using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867NonIntervalSummaryQty : IType867Model
    {
        public Type867Types ModelType
        {
            get { return Type867Types.NonIntervalSummaryQty; }
        }

        public int NonIntervalSummaryQtyKey { get; set; }
        public int NonIntervalSummaryKey { get; set; }
        public string Qualifier { get; set; }
        public string Quantity { get; set; }
        public string MeasurementSignificanceCode { get; set; }
        public string ServicePeriodStart { get; set; }
        public string ServicePeriodEnd { get; set; }
        public string RangeMin { get; set; }
        public string RangeMax { get; set; }
        public string ThermFactor { get; set; }
        public string DegreeDayFactor { get; set; }
        public string CompositeUom { get; set; }
        public string MeterMultiplier { get; set; }
    }
}
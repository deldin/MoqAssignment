using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867UnMeterSummaryQty : IType867Model
    {
        public Type867Types ModelType
        {
            get { return Type867Types.UnMeterSummaryQty; }
        }

        public int UnMeterSummaryQtyKey { get; set; }
        public int UnMeterSummaryKey { get; set; }
        public string Qualifier { get; set; }
        public string Quantity { get; set; }
        public string MeasurementSignificanceCode { get; set; }
        public string ServicePeriodStart { get; set; }
        public string ServicePeriodEnd { get; set; }
    }
}
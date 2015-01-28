using System;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867IntervalSummaryQty : IType867Model
    {
        public Type867Types ModelType
        {
            get { return Type867Types.IntervalSummaryQty; }
        }

        public int IntervalSummaryQtyKey { get; set; }
        public int IntervalSummaryKey { get; set; }
        public string Qualifier { get; set; }
        public string Quantity { get; set; }
        public string MeasurementCode { get; set; }
        public string CompositeUom { get; set; }
        public string Uom { get; set; }
        public string BeginRead { get; set; }
        public string EndRead { get; set; }
        public string MeasurementSignificanceCode { get; set; }
        public string TransformerLossFactor { get; set; }
        public string MeterMultiplier { get; set; }
        public string PowerFactor { get; set; }
        public short ProcessFlag { get; set; }
        public DateTime ProcessDate { get; set; }
        public string RangeMin { get; set; }
        public string RangeMax { get; set; }
        public string ThermFactor { get; set; }
        public string DegreeDayFactor { get; set; }
    }
}
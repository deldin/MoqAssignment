using System;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867IntervalDetailQty : IType867Model
    {
        public Type867Types ModelType
        {
            get { return Type867Types.IntervalDetailQty; }
        }

        public int IntervalDetailQtyKey { get; set; }
        public int IntervalDetailKey { get; set; }
        public string Qualifier { get; set; }
        public string Quantity { get; set; }
        public string IntervalEndDate { get; set; }
        public string IntervalEndTime { get; set; }
        public string RangeMin { get; set; }
        public string RangeMax { get; set; }
        public string ThermFactor { get; set; }
        public string DegreeDayFactor { get; set; }
        public short ProcessFlag { get; set; }
        public DateTime ProcessDate { get; set; }
    }
}
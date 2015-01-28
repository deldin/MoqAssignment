using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867IntervalSummaryAcrossMetersQty : IType867Model
    {
        public Type867Types ModelType
        {
            get { return Type867Types.IntervalSummaryAcrossMetersQty; }
        }

        public int IntervalSummaryAcrossMetersQtyKey { get; set; }
        public int IntervalSummaryAcrossMetersKey { get; set; }
        public string Qualifier { get; set; }
        public string Quantity { get; set; }
        public string IntervalEndDate { get; set; }
        public string IntervalEndTime { get; set; }
    }
}
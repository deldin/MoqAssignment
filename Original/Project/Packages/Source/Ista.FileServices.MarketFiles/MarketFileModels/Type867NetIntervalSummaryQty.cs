using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867NetIntervalSummaryQty : IType867Model
    {
        public Type867Types ModelType
        {
            get { return Type867Types.NetIntervalSummaryQty; }
        }

        public int NetIntervalSummaryQtyKey { get; set; }
        public int NetIntervalSummaryKey { get; set; }
        public string Qualifier { get; set; }
        public string Quantity { get; set; }
        public string ServicePeriodStart { get; set; }
        public string ServicePeriodEnd { get; set; }
    }
}
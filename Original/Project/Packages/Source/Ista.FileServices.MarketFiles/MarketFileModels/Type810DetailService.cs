using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type810DetailService : IType810Model
    {
        public Type810Types ModelType
        {
            get { return Type810Types.DetailItemService; }
        }

        public int? DetailServiceKey { get; set; }
        public int DetailKey { get; set; }
        public string Indicator { get; set; }
        public string HandlingCode { get; set; }
        public decimal? Rate { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? Quantity { get; set; }
        public string MeasurementCode { get; set; }
        public string Description { get; set; }
        public string AllowanceCode { get; set; }
    }
}
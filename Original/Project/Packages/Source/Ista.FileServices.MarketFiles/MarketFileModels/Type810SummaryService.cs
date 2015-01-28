using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type810SummaryService : IType810Model
    {
        public Type810Types ModelType
        {
            get { return Type810Types.SummaryService; }
        }

        public int SummaryServiceKey { get; set; }
        public int SummaryKey { get; set; }
        public string Indicator { get; set; }
        public string HandlingCode { get; set; }
        public string Rate { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? Quantity { get; set; }
        public string MeasurementCode { get; set; }
        public string Description { get; set; }
        public string AllowanceCode { get; set; }
    }
}
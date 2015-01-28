using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type810SummaryTax : IType810Model
    {
        public Type810Types ModelType
        {
            get { return Type810Types.SummaryTax; }
        }

        public int SummaryTaxKey { get; set; }
        public int SummaryKey { get; set; }
        public string TaxTypeCode { get; set; }
        public decimal? TaxAmount { get; set; }
    }
}
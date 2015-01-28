using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type810DetailItemTax : IType810Model
    {
        public Type810Types ModelType
        {
            get { return Type810Types.DetailItemTax; }
        }

        public int? TaxKey { get; set; }
        public int ItemKey { get; set; }
        public string TaxTypeCode { get; set; }
        public string TaxAmount { get; set; }
        public string RelationshipCode { get; set; }
    }
}
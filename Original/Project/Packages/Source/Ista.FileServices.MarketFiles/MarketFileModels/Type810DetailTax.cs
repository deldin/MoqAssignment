using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type810DetailTax : IType810Model
    {
        public Type810Types ModelType
        {
            get { return Type810Types.DetailItemTax; }
        }

        public int? TaxKey { get; set; }
        public int DetailKey { get; set; }
        public string AssignedId { get; set; }
        public string MonetaryAmount { get; set; }
        public string Percent { get; set; }
        public string RelationshipCode { get; set; }
        public string DollarBasis { get; set; }
        public string TaxTypeCode { get; set; }
        public string JurisdictionCode { get; set; }
        public string JurisdictionCodeQualifier { get; set; }
        public string ExemptCode { get; set; }
        public string Description { get; set; }
    }
}
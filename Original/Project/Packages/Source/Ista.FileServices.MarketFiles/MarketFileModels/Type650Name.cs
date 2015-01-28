using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type650Name : IType650Model
    {
        public Type650Types ModelType
        {
            get { return Type650Types.Name; }
        }

        public int HeaderKey { get; set; }
        public int? NameKey { get; set; }
        public string EntityIdType { get; set; }
        public string EntityName { get; set; }
        public string EntityName2 { get; set; }
        public string EntityName3 { get; set; }
        public string EntityDuns { get; set; }
        public string EntityIdCode { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string ContactCode { get; set; }
        public string ContactName { get; set; }
        public string ContactPhoneNbr1 { get; set; }
        public string ContactPhoneNbr2 { get; set; }
    }
}
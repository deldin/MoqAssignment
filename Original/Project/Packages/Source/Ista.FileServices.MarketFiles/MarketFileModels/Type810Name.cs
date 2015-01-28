using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type810Name : IType810Model
    {
        public Type810Types ModelType
        {
            get { return Type810Types.Name; }
        }

        public int? NameKey { get; set; }
        public int HeaderKey { get; set; }
        public string EntityIdType { get; set; }
        public string EntityName { get; set; }
        public string EntityDuns { get; set; }
        public string EntityIdCode { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string EntityName2 { get; set; }
        public string EntityName3 { get; set; }
    }
}
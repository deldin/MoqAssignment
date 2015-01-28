using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867Name : IType867Model   
    {
        public Type867Types ModelType
        {
            get { return Type867Types.Name; }
        }

        public int NameKey { get; set; }
        public int HeaderKey { get; set; }
        public string EntityIdType { get; set; }
        public string EntityName { get; set; }
        public string EntityDuns { get; set; }
        public string EntityIdCode { get; set; }
        public string ServiceAddress1 { get; set; }
        public string ServiceAddress2 { get; set; }
        public string ServiceCity { get; set; }
        public string ServiceState { get; set; }
        public string ServiceZipCode { get; set; }
    }
}
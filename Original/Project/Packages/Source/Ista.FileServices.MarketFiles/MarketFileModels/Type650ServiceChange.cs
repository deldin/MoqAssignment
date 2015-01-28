using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type650ServiceChange : IType650Model
    {
        public Type650Types ModelType
        {
            get { return Type650Types.ServiceChange; }
        }

        public int ServiceKey { get; set; }
        public int? ServiceChangeKey { get; set; }
        public string ChangeReason { get; set; }
    }
}
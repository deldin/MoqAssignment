using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type814ServiceAccountChange : IType814Model
    {
        public Type814Types ModelType
        {
            get { return Type814Types.ServiceAccountChange; }
        }

        public int? ChangeKey { get; set; }
        public int ServiceKey { get; set; }
        public string ChangeReason { get; set; }
        public string ChangeDescription { get; set; }
    }
}
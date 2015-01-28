using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type814ServiceStatus : IType814Model
    {
        public Type814Types ModelType
        {
            get { return Type814Types.ServiceStatus; }
        }

        public int? StatusKey { get; set; }
        public int ServiceKey { get; set; }
        public string StatusCode { get; set; }
        public string StatusReason { get; set; }
        public string StatusType { get; set; }
    }
}
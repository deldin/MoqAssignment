using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type814ServiceReject : IType814Model
    {
        public Type814Types ModelType
        {
            get { return Type814Types.ServiceReject; }
        }

        public int? RejectKey { get; set; }
        public int ServiceKey { get; set; }
        public string RejectCode { get; set; }
        public string RejectReason { get; set; }
    }
}
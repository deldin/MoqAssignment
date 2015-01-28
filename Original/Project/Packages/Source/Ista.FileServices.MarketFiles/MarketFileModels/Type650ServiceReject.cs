using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type650ServiceReject : IType650Model
    {
        public Type650Types ModelType
        {
            get { return Type650Types.ServiceReject; }
        }

        public int ServiceKey { get; set; }
        public int? ServiceRejectKey { get; set; }
        public string RejectCode { get; set; }
        public string RejectReason { get; set; }
        public string UnexCode { get; set; }
        public string UnexReason { get; set; }
    }
}
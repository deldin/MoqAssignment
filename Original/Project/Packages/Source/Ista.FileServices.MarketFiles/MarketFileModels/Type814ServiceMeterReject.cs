using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type814ServiceMeterReject : IType814Model
    {
        public Type814Types ModelType
        {
            get { return Type814Types.ServiceMeterReject; }
        }

        public int? RejectKey { get; set; }
        public int MeterKey { get; set; }
        public string RejectCode { get; set; }
        public string RejectReason { get; set; }
    }
}
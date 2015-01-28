using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type814ServiceMeterChange : IType814Model
    {
        public Type814Types ModelType
        {
            get { return Type814Types.ServiceMeterChange; }
        }

        public int? ChangeKey { get; set; }
        public int MeterKey { get; set; }
        public string ChangeReason { get; set; }
        public string ChangeDescription { get; set; }
    }
}
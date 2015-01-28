using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type814ServiceMeterType : IType814Model
    {
        public Type814Types ModelType
        {
            get { return Type814Types.ServiceMeterType; }
        }

        public int? TypeKey { get; set; }
        public int MeterKey { get; set; }
        public string MeterMultiplier { get; set; }
        public string MeterType { get; set; }
        public string ProductType { get; set; }
        public string TimeOfUse { get; set; }
        public string NumberOfDials { get; set; }
        public string UnmeteredNumberOfDevices { get; set; }
        public string UnmeteredDescription { get; set; }
        public string StartMeterRead { get; set; }
        public string EndMeterRead { get; set; }
        public string ChangeReason { get; set; }
        public string TimeOfUse2 { get; set; }
    }
}
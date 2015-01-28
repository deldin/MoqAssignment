using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type814ServiceMeterTou : IType814Model
    {
        public Type814Types ModelType
        {
            get { return Type814Types.ServiceMeterTou; }
        }

        public int? TouKey { get; set; }
        public int MeterKey { get; set; }
        public string TouCode { get; set; }
        public string MeasurementType { get; set; }
    }
}
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type650ServiceMeter : IType650Model
    {
        public Type650Types ModelType
        {
            get { return Type650Types.ServiceMeter; }
        }

        public int ServiceKey { get; set; }
        public int? ServiceMeterKey { get; set; }
        public string MeterNumber { get; set; }
    }
}
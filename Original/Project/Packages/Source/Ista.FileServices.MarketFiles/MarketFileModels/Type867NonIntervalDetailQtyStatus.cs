using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867NonIntervalDetailQtyStatus : IType867Model
    {
        public Type867Types ModelType
        {
            get { return Type867Types.NonIntervalDetailQtyStatus; }
        }

        public int NonIntervalDetailQtyKey { get; set; }
        public int NonIntervalDetailKey { get; set; }
        public short PremiseExists { get; set; }
        public short MeterExists { get; set; }
        public short MeterRegisterExists { get; set; }
        public short MeterUomValid { get; set; }
    }
}
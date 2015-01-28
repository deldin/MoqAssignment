using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type814ServiceMeterContact : IType814Model
    {
        public Type814Types ModelType
        {
            get { return Type814Types.ServiceMeterContact; }
        }

        public int? ContactKey { get; set; }
        public int MeterKey { get; set; }
        public string ContactType { get; set; }
        public string ContactName { get; set; }
        public string ContactPhoneNbr1 { get; set; }
        public string ContactPhoneNbr2 { get; set; }
        public string ContactPhoneNbr3 { get; set; }
    }
}
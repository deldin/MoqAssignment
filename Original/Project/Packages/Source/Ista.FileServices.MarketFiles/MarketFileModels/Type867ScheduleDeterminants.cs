using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867ScheduleDeterminants: IType867Model
    {
        public Type867Types ModelType
        {
            get { return Type867Types.ScheduleDeterminants; }
        }

        public int ScheduleDeterminantsKey { get; set; }
        public int HeaderKey { get; set; }
        public string CapacityObligation { get; set; }
        public string TransmissionObligation { get; set; }
        public string LoadProfile { get; set; }
        public string LDCRateClass { get; set; }
        public string Zone { get; set; }
        public string BillCycle { get; set; }
        public string MeterNumber { get; set; }
        public string EffectiveDate { get; set; }
        public string LossFactor { get; set; }
        public string ServiceVoltage { get; set; }
        public string SpecialMeterConfig { get; set; }
        public string MaximumGeneration { get; set; }
        public string LDCRateSubClass { get; set; }
    }
}
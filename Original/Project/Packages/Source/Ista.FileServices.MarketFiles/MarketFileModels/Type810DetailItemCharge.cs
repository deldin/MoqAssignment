using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type810DetailItemCharge : IType810Model
    {
        public Type810Types ModelType
        {
            get { return Type810Types.DetailItemCharge; }
        }

        public int? ChargeKey { get; set; }
        public int ItemKey { get; set; }
        public string ChargeIndicator { get; set; }
        public string AgencyCode { get; set; }
        public string ChargeCode { get; set; }
        public string Amount { get; set; }
        public string Rate { get; set; }
        public string UOM { get; set; }
        public string Quantity { get; set; }
        public string Description { get; set; }
        public string PrintSeqId { get; set; }
        public string EnergyCharges { get; set; }
    }
}
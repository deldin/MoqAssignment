using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867SwitchQty : IType867Model
    {
        public Type867Types ModelType
        {
            get { return Type867Types.SwitchQty; }
        }

        public int SwitchQtyKey { get; set; }
        public int SwitchKey { get; set; }
        public string Qualifier { get; set; }
        public string CompositeUom { get; set; }
        public string Uom { get; set; }
        public string SwitchRead { get; set; }
        public string MeasurementSignificanceCode { get; set; }
        public string Message { get; set; }
    }
}
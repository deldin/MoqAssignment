using System;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867UnMeterDetailQty : IType867Model
    {
        public Type867Types ModelType
        {
            get { return Type867Types.UnMeterDetailQty; }
        }

        public int UnMeterDetailQtyKey { get; set; }
        public int UnMeterDetailKey { get; set; }
        public string Qualifier { get; set; }
        public string Quantity { get; set; }
        public string CompositeUom { get; set; }
        public string Uom { get; set; }
        public string NumberOfDevices { get; set; }
        public string ConsumptionPerDevice { get; set; }
        public short ProcessFlag { get; set; }
        public DateTime ProcessDate { get; set; }
        public string BackoutCredit { get; set; }
    }
}
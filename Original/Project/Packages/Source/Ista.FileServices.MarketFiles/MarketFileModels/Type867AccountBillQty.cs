using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867AccountBillQty : IType867Model
    {
        public Type867Types ModelType
        {
            get { return Type867Types.AccountBillQty; }
        }

        public int AccountBillQtyKey { get; set; }
        public int HeaderKey { get; set; }
        public string Qualifier { get; set; }
        public string Quantity { get; set; }
        public string UOM { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
    }
}
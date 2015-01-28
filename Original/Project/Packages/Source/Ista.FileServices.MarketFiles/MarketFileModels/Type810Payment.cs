using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type810Payment : IType810Model
    {
        public Type810Types ModelType
        {
            get { return Type810Types.Payment; }
        }

        public int? PaymentKey { get; set; }
        public int HeaderKey { get; set; }
        public string AmountQualifierCode { get; set; }
        public string MonetaryAmount { get; set; }
        public string TimeUnit { get; set; }
        public string DateTimeQualifier { get; set; }
        public string Date { get; set; }
    }
}
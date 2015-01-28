using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867Summary : IType867Model
    {
        public Type867Types ModelType
        {
            get { return Type867Types.Summary; }
        }

        public int SummaryKey { get; set; }
        public int HeaderKey { get; set; }
        public string TotalSegments { get; set; }
        public string TransactionControlNbr { get; set; }
    }
}
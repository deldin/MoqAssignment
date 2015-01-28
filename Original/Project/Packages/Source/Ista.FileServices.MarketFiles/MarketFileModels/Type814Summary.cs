using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type814Summary : IType814Model
    {
        public Type814Types ModelType
        {
            get { return Type814Types.Summary; }
        }

        public int? SummaryKey { get; set; }
        public int HeaderKey { get; set; }
        public string TotalSegments { get; set; }
        public string TransactionControlNbr { get; set; }
    }
}
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type824Reason : IType824Model
    {
        public Type824Types ModelType
        {
            get { return Type824Types.Reason; }
        }

        public int HeaderKey { get; set; }
        public int? ReasonKey { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonText { get; set; }
    }
}

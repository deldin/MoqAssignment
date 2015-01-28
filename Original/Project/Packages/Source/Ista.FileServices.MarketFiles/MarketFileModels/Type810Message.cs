using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type810Message : IType810Model
    {
        public Type810Types ModelType
        {
            get { return Type810Types.Message; }
        }

        public int? MessageKey { get; set; }
        public int HeaderKey { get; set; }
        public string ItemDescType { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public string PositionCode { get; set; }
    }
}
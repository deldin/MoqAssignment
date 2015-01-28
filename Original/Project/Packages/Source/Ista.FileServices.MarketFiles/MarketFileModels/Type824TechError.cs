using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type824TechError : IType824Model
    {
        public Type824Types ModelType
        {
            get { return Type824Types.TechError; }
        }

        public int ReferenceKey { get; set; }
        public int? TechErrorKey { get; set; }
        public string TechErrorCode { get; set; }
        public string BadElementCopy { get; set; }
        public string TechErrorNote { get; set; }
    }
}
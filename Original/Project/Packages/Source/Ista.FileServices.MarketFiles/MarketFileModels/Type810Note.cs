using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type810Note : IType810Model
    {
        public Type810Types ModelType
        {
            get { return Type810Types.Note; }
        }

        public int NoteKey { get; set; }
        public int HeaderKey { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }
}
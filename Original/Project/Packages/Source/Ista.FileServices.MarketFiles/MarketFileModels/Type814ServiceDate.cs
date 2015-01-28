using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type814ServiceDate : IType814Model
    {
        public Type814Types ModelType
        {
            get { return Type814Types.ServiceDate; }
        }

        public int? DateKey { get; set; }
        public int ServiceKey { get; set; }
        public string Qualifier { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string TimeCode { get; set; }
        public string PeriodFormat { get; set; }
        public string Period { get; set; }
        public string NotesDate { get; set; }
    }
}
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type997ResponseNote : IType997Model
    {
        public Type997Types ModelType
        {
            get { return Type997Types.ResponseNote; }
        }

        public int? ResponseNoteKey { get; set; }
        public int ResponseKey { get; set; }
        public string ElementCopy { get; set; }
        public string ElementPosition { get; set; }
        public string ElementReferenceNbr { get; set; }
        public string ElementSyntaxErrorCode { get; set; }
        public string LoopIdentifierCode { get; set; }
        public string SegmentIdCode { get; set; }
        public string SegmentPosition { get; set; }
        public string SegmentSyntaxErrorCode { get; set; }
    }
}
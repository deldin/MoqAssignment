using System.Collections.Generic;
using System.Linq;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type997Response : IType997Model
    {
        private readonly IList<Type997ResponseNote> responseNotes;

        public Type997Types ModelType
        {
            get { return Type997Types.Response; }
        }

        public int? ResponseKey { get; set; }
        public int HeaderKey { get; set; }
        public string AcknowledgementCode { get; set; }
        public string ControlNbr { get; set; }
        public string IdentifierCode { get; set; }
        public string SyntaxErrorCode1 { get; set; }
        public string SyntaxErrorCode2 { get; set; }
        public string SyntaxErrorCode3 { get; set; }
        public string SyntaxErrorCode4 { get; set; }
        public string SyntaxErrorCode5 { get; set; }

        public Type997ResponseNote[] Notes
        {
            get { return responseNotes.ToArray(); }
        }

        public Type997Response()
        {
            responseNotes = new List<Type997ResponseNote>();
        }

        public void AddResponseNote(Type997ResponseNote responseNote)
        {
            responseNotes.Add(responseNote);
        }
    }
}
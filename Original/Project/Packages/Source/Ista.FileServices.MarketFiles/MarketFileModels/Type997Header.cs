using System;
using System.Collections.Generic;
using System.Linq;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type997Header : IType997Model, IMarketHeaderModel
    {
        private readonly IList<Type997Response> responses;

        public Type997Types ModelType
        {
            get { return Type997Types.Header; }
        }

        public int? HeaderKey { get; set; }
        public int MarketFileId { get; set; }
        public string AcknowledgeCode { get; set; }
        public string FunctionalGroup { get; set; }
        public string SegmentCount { get; set; }
        public string SyntaxErrorCode1 { get; set; }
        public string SyntaxErrorCode2 { get; set; }
        public string SyntaxErrorCode3 { get; set; }
        public string SyntaxErrorCode4 { get; set; }
        public string SyntaxErrorCode5 { get; set; }
        public string TransactionNbr { get; set; }
        public string TransactionSetsAccepted { get; set; }
        public string TransactionSetsIncluded { get; set; }
        public string TransactionSetsReceived { get; set; }
        public bool Direction { get; set; }
        public bool ProcessFlag { get; set; }
        public DateTime ProcessDate { get; set; }

        public Type997Response[] Responses
        {
            get { return responses.ToArray(); }
        }

        public Type997Header()
        {
            responses = new List<Type997Response>();
        }

        public void AddResponse(Type997Response response)
        {
            responses.Add(response);
        }
    }
}
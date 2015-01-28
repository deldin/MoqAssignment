using System.Collections.Generic;
using System.Linq;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Models
{
    public class Import997Model : IMarketFileParseResult
    {
        private readonly List<Type997Header> headers;

        public int TransactionAuditCount { get; set; }
        public int TransactionActualCount { get; set; }
        public string InterchangeControlNbr { get; set; }

        public static Import997Model Empty
        {
            get { return new Import997Model(); }
        }

        public IMarketHeaderModel[] Headers
        {
            get
            {
                return headers
                    .Cast<IMarketHeaderModel>()
                    .ToArray();
            }
        }

        public Type997Header[] TypeHeaders
        {
            get { return headers.ToArray(); }
        }

        public Import997Model()
        {
            headers = new List<Type997Header>();
        }

        public void AddHeader(Type997Header item)
        {
            headers.Add(item);
        }
    }
}
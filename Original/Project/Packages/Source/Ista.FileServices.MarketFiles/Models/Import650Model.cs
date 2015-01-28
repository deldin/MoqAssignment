using System.Collections.Generic;
using System.Linq;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Models
{
    public class Import650Model : IMarketFileParseResult
    {
        private readonly List<Type650Header> headers;

        public int TransactionAuditCount { get; set; }
        public int TransactionActualCount { get; set; }
        public string InterchangeControlNbr { get; set; }

        public static Import650Model Empty
        {
            get { return new Import650Model(); }
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

        public Type650Header[] TypeHeaders
        {
            get { return headers.ToArray(); }
        }

        public Import650Model()
        {
            headers = new List<Type650Header>();
        }

        public void AddHeader(Type650Header item)
        {
            headers.Add(item);
        }
    }
}

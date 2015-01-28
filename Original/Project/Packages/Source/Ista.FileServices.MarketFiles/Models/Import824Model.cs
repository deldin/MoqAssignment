using System.Collections.Generic;
using System.Linq;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Models
{
    public class Import824Model : IMarketFileParseResult
    {
        private readonly List<Type824Header> headers;

        public int TransactionAuditCount { get; set; }
        public int TransactionActualCount { get; set; }
        public string InterchangeControlNbr { get; set; }

        public static Import824Model Empty
        {
            get { return new Import824Model(); }
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

        public Type824Header[] TypeHeaders
        {
            get { return headers.ToArray(); }
        }

        public Import824Model()
        {
            headers = new List<Type824Header>();
        }

        public void AddHeader(Type824Header item)
        {
            headers.Add(item);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Models
{
    public class Import820Model : IMarketFileParseResult
    {
        private readonly List<Type820Header> headers;

        public int TransactionAuditCount { get; set; }
        public int TransactionActualCount { get; set; }
        public string InterchangeControlNbr { get; set; }

        public static Import820Model Empty
        {
            get { return new Import820Model(); }
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

        public Type820Header[] TypeHeaders
        {
            get { return headers.ToArray(); }
        }

        public Import820Model()
        {
            headers = new List<Type820Header>();
        }

        public void AddHeader(Type820Header item)
        {
            headers.Add(item);
        }
    }
}

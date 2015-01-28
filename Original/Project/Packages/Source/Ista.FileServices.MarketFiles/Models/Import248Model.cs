using System.Collections.Generic;
using System.Linq;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Models
{
    public class Import248Model : IMarketFileParseResult
    {
        private readonly List<Type248Header> headers;

        public int TransactionAuditCount { get; set; }
        public int TransactionActualCount { get; set; }
        public string InterchangeControlNbr { get; set; }

        public static Import248Model Empty
        {
            get { return new Import248Model(); }
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

        public Type248Header[] TypeHeaders
        {
            get { return headers.ToArray(); }
        }

        public Import248Model()
        {
            headers = new List<Type248Header>();
        }

        public void AddHeader(Type248Header item)
        {
            headers.Add(item);
        }
    }
}

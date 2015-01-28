using System.Collections.Generic;
using System.Linq;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Models
{
    public class Import810Model : IMarketFileParseResult
    {
        private readonly List<Type810Header> headers;

        public int TransactionAuditCount { get; set; }
        public int TransactionActualCount { get; set; }
        public string InterchangeControlNbr { get; set; }

        public static Import810Model Empty
        {
            get { return new Import810Model(); }
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

        public Type810Header[] TypeHeaders
        {
            get { return headers.ToArray(); }
        }

        public Import810Model()
        {
            headers = new List<Type810Header>();
        }

        public void AddHeader(Type810Header item)
        {
            headers.Add(item);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Models
{
    public class Import814Model : IMarketFileParseResult
    {
        private readonly List<Type814Header> headers;

        public int TransactionAuditCount { get; set; }
        public int TransactionActualCount { get; set; }
        public string InterchangeControlNbr { get; set; }

        public static Import814Model Empty
        {
            get { return new Import814Model(); }
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

        public Type814Header[] TypeHeaders
        {
            get { return headers.ToArray(); }
        }

        public Import814Model()
        {
            headers = new List<Type814Header>();
        }

        public void AddHeader(Type814Header item)
        {
            headers.Add(item);
        }
    }
}

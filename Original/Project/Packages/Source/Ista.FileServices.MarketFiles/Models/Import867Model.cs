using System.Collections.Generic;
using System.Linq;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Models
{
    public class Import867Model : IMarketFileParseResult 
    {
        private readonly List<Type867Header> _headers;

        public int TransactionActualCount { get; set; }
        public int TransactionAuditCount { get; set; }
        public string InterchangeControlNbr { get; set; }

        public static Import867Model Empty
        {
            get { return new Import867Model(); }
        }

        public IMarketHeaderModel[] Headers
        {
            get
            {
                return _headers
                    .Cast<IMarketHeaderModel>()
                    .ToArray();
            }
        }

        public Type867Header[] TypeHeaders
        {
            get { return _headers.ToArray(); }
        }

        public Import867Model()
        {
            _headers = new List<Type867Header>();
        }

        public void AddHeader(Type867Header item)
        {
            _headers.Add(item);
        }
    }
}

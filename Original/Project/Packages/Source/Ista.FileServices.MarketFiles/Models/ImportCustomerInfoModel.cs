using System.Collections.Generic;
using System.Linq;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Models
{
    public class ImportCustomerInfoModel : IMarketFileParseResult
    {
        private readonly List<TypeCustomerInfoFile> headers;

        public int TransactionAuditCount { get; set; }
        public int TransactionActualCount { get; set; }
        public string InterchangeControlNbr { get; set; }

        public static ImportCustomerInfoModel Empty
        {
            get { return new ImportCustomerInfoModel(); }
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

        public TypeCustomerInfoFile[] TypeHeaders
        {
            get { return headers.ToArray(); }
        }

        public ImportCustomerInfoModel()
        {
            headers = new List<TypeCustomerInfoFile>();
        }

        public void AddHeader(TypeCustomerInfoFile item)
        {
            headers.Add(item);
        }
    }
}

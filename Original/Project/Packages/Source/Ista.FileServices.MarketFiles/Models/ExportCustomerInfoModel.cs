using System.Collections.Generic;
using System.Text;
using Ista.FileServices.MarketFiles.Interfaces;

namespace Ista.FileServices.MarketFiles.Models
{
    public class ExportCustomerInfoModel : IMarketFileExportResult
    {
        private readonly List<int> keys;
        private readonly string fileName;
        private readonly StringBuilder buffer;

        public string Content { get; set; }
        public string CspDuns { get; set; }
        public string LdcDuns { get; set; }
        public string LdcShortName { get; set; }
        public string TradingPartnerId { get; set; }
        public int LdcId { get; set; }
        public int CspDunsId { get; set; }
        public int? CspDunsTradingPartnerId { get; set; }
        public int? DuplicateFileIdentifier { get; set; }
        
        public StringBuilder Buffer
        {
            get { return buffer; }
        }

        public int HeaderCount
        {
            get { return keys.Count; }
        }

        public int[] HeaderKeys
        {
            get { return keys.ToArray(); }
        }

        public ExportCustomerInfoModel(string fileName)
        {
            this.fileName = fileName;

            keys = new List<int>();
            buffer = new StringBuilder();
        }

        public void AddHeaderKey(int headerKey)
        {
            keys.Add(headerKey);
        }

        public void FinalizeDocument(int marketFileId)
        {
            Content = buffer.ToString();
        }

        public string GenerateFileName(string fileType, string fileExtension)
        {
            return fileName;
        }
    }
}

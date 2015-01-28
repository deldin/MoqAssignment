using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Ista.FileServices.MarketFiles.Interfaces;

namespace Ista.FileServices.MarketFiles.Models
{
    public class Export820Model : IMarketFileExportResult
    {
        private readonly List<int> keys;
        private readonly bool forXmlContent;
 
        public XDocument Document { get; set; }
        public int LdcId { get; set; }
        public int CspDunsId { get; set; }
        public string Content { get; set; }
        public string CspDuns { get; set; }
        public string LdcDuns { get; set; }
        public string LdcShortName { get; set; }
        public string TradingPartnerId { get; set; }
        public int? CspDunsTradingPartnerId { get; set; }
        public int? DuplicateFileIdentifier { get; set; }

        public StringBuilder Buffer { get; private set; }

        public int HeaderCount
        {
            get { return keys.Count; }
        }

        public int[] HeaderKeys
        {
            get { return keys.ToArray(); }
        }

        public Export820Model()
        {
            keys = new List<int>();

            forXmlContent = true;
        }

        public Export820Model(bool forXmlContent)
            : this()
        {
            this.forXmlContent = forXmlContent;
            if (!forXmlContent)
                Buffer = new StringBuilder();
        }

        public void AddHeaderKey(int headerKey)
        {
            keys.Add(headerKey);
        }

        public void AddHeaderKeys(int[] headerKeys)
        {
            keys.AddRange(headerKeys);
        }

        public void FinalizeDocument(int marketFileId)
        {
            if (!forXmlContent || Document == null)
            {
                Content = Buffer
                    .AppendFormat("TL|{0}", HeaderCount)
                    .ToString();

                return;
            }

            var documentElement = Document.Root;
            if (documentElement == null)
                return;

            var marketFileElement = documentElement.Element("MarketFileId");
            if (marketFileElement != null)
                marketFileElement.SetValue(marketFileId);

            var interchangeElement = documentElement.Element("InterchangeControlNbr");
            if (interchangeElement != null)
                interchangeElement.SetValue(marketFileId);

            var functionalElement = documentElement.Element("FunctionalGroupControlNbr");
            if (functionalElement != null)
                functionalElement.SetValue(marketFileId);

            Content = Document.ToString();
        }

        public string GenerateFileName(string fileType, string fileExtension)
        {
            if (DuplicateFileIdentifier.HasValue)
            {
                return string.Format("{0}_{1}_{2}_{3:yyyyMMddHHmmss}_{4}.{5}", fileType,
                    TradingPartnerId.Substring(0, 3), LdcShortName, DateTime.Now, DuplicateFileIdentifier.Value, fileExtension);
            }

            return string.Format("{0}_{1}_{2}_{3:yyyyMMddHHmmss}.{4}", fileType,
                TradingPartnerId.Substring(0, 3), LdcShortName, DateTime.Now, fileExtension);
        }
    }
}
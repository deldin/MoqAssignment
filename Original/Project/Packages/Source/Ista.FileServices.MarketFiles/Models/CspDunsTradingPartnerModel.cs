using System.Collections.Generic;

namespace Ista.FileServices.MarketFiles.Models
{
    public class CspDunsTradingPartnerModel
    {
        private readonly Dictionary<string, string> dictionary;
 
        public int CspDunsTradingPartnerId { get; set; }
        public int CspTradingPartnerId { get; set; }
        public string CspDuns { get; set; }
        public string CspName { get; set; }
        public string CspShortName { get; set; }
        public int TradingPartnerId { get; set; }
        public string TradingPartnerDuns { get; set; }
        public string TradingPartnerName { get; set; }
        public string TradingPartnerShortName { get; set; }

        public CspDunsTradingPartnerModel()
        {
            dictionary = new Dictionary<string, string>();
        }

        public void AddConfig(string name, string value)
        {
            dictionary[name] = value;
        }

        public string GetConfig(string name)
        {
            string value;
            if (!dictionary.TryGetValue(name, out value))
                return string.Empty;

            return value;
        }
    }
}
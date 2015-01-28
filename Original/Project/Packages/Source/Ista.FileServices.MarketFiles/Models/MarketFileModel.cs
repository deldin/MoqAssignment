using System;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.Models
{
    public class MarketFileModel
    {
        public int MarketFileId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string ProcessStatus { get; set; }
        public string ProcessError { get; set; }
        public string SenderTranNum { get; set; }
        public bool DirectionFlag { get; set; }
        public MarketFileStatusOptions Status { get; set; }
        public int? LdcId { get; set; }
        public int? CspDunsId { get; set; }
        public int? CspDunsTradingPartnerId { get; set; }
        public int? RefMarketFileId { get; set; }
        public int? TransactionCount { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ProcessDate { get; set; }
    }
}

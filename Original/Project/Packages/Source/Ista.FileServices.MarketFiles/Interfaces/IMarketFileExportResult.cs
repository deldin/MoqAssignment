
namespace Ista.FileServices.MarketFiles.Interfaces
{
    public interface IMarketFileExportResult
    {
        string Content { get; set; }
        string CspDuns { get; set; }
        string LdcDuns { get; set; }
        string LdcShortName { get; set; }
        string TradingPartnerId { get; set; }
        int LdcId { get; set; }
        int CspDunsId { get; set; }
        int? CspDunsTradingPartnerId { get; set; }
        int? DuplicateFileIdentifier { get; set; }

        int HeaderCount { get; }
        int[] HeaderKeys { get; }

        void FinalizeDocument(int marketFileId);
        string GenerateFileName(string fileType, string fileExtension);
    }
}
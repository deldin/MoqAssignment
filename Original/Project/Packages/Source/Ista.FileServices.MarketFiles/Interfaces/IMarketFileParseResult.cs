
namespace Ista.FileServices.MarketFiles.Interfaces
{
    public interface IMarketFileParseResult
    {
        IMarketHeaderModel[] Headers { get; }
        int TransactionActualCount { get; }
        int TransactionAuditCount { get; }
        string InterchangeControlNbr { get; set; }
    }
}
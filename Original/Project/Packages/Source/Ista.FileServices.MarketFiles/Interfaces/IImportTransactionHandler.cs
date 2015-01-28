
namespace Ista.FileServices.MarketFiles.Interfaces
{
    public interface IImportTransactionHandler
    {
        void ProcessHeader(IMarketHeaderModel header, int marketFileId);
    }
}
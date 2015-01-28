
namespace Ista.FileServices.MarketFiles.Interfaces
{
    public interface IExportTransactionHandler
    {
        void UpdateHeader(int headerKey, int marketFileId, string fileName);
    }
}

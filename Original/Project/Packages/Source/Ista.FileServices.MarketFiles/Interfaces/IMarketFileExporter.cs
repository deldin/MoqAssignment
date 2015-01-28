using System.Threading;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    public interface IMarketFileExporter
    {
        IMarketFileExportResult[] Export(CancellationToken token);
    }
}

using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access methods for importing 820 Transactions to Market database.
    /// </summary>
    public interface IMarket820Import
    {
        /// <summary>
        /// Inserts a 820 Header record.
        /// </summary>
        /// <param name="model">820 Header model</param>
        /// <returns>820 Header Key</returns>
        int InsertHeader(Type820Header model);

        /// <summary>
        /// Inserts a 820 Detail record.
        /// </summary>
        /// <remarks>
        /// Header Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">820 Detail model</param>
        /// <returns>820 Detail Key</returns>
        int InsertDetail(Type820Detail model);
    }
}

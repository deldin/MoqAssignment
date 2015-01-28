using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access methods for importing 248 Transactions to Market database.
    /// </summary>
    public interface IMarket248Import
    {
        /// <summary>
        /// Inserts a 248 Header record.
        /// </summary>
        /// <param name="model">248 Header model</param>
        /// <returns>248 Header Key</returns>
        int InsertHeader(Type248Header model);

        /// <summary>
        /// Inserts a 248 Detail record.
        /// </summary>
        /// <remarks>
        /// Header Key should already be set prior to calling this method
        /// </remarks>
        /// <param name="model">248 Detail model</param>
        /// <returns>248 Detail Key</returns>
        int InsertDetail(Type248Detail model);
    }
}

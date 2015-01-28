using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access methods for importing 568 Transactions to Market database.
    /// </summary>
    public interface IMarket568Import
    {
        /// <summary>
        /// Inserts a 568 Header record.
        /// </summary>
        /// <param name="model">568 Header model</param>
        /// <returns>568 Header Key</returns>
        int InsertHeader(Type568Header model);

        /// <summary>
        /// Inserts a 568 Detail record.
        /// </summary>
        /// <remarks>
        /// Header Key should already be set prior to calling this method
        /// </remarks>
        /// <param name="model">568 Detail model</param>
        /// <returns>568 Detail Key</returns>
        int InsertDetail(Type568Detail model);
    }
}

using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access methods for importing 997 Transactions to Market database.
    /// </summary>
    public interface IMarket997Import
    {
        /// <summary>
        /// Inserts a 997 Header record.
        /// </summary>
        /// <param name="model">997 Header model</param>
        /// <returns>997 Header Key</returns>
        int InsertHeader(Type997Header model);

        /// <summary>
        /// Inserts a 997 Response record.
        /// </summary>
        /// <remarks>
        /// Header Key should already be set prior to calling this method
        /// </remarks>
        /// <param name="model">997 Response model</param>
        /// <returns>997 Response Key</returns>
        int InsertResponse(Type997Response model);

        /// <summary>
        /// Inserts a 997 Response Note record.
        /// </summary>
        /// <remarks>
        /// Response Key should already be set prior to calling this method
        /// </remarks>
        /// <param name="model">997 Response Note model</param>
        /// <returns>997 Response Note Key</returns>
        int InsertResponseNote(Type997ResponseNote model);
    }
}

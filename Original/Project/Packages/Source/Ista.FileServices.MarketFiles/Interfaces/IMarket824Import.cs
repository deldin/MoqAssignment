using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access methods for importing 824 Transactions to Market database.
    /// </summary>
    public interface IMarket824Import
    {
        /// <summary>
        /// Inserts 824 Header record.
        /// </summary>
        /// <param name="model">824 Header model</param>
        /// <returns>824 Header Key</returns>
        int InsertHeader(Type824Header model);

        /// <summary>
        /// Inserts 824 Reason record.
        /// </summary>
        /// <remarks>
        /// Header Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">824 Reason model</param>
        /// <returns>824 Reason Key</returns>
        int InsertReason(Type824Reason model);

        /// <summary>
        /// Inserts a 824 Reference record.
        /// </summary>
        /// <remarks>
        /// Header Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">824 Reference model</param>
        /// <returns>824 Reference Key</returns>
        int InsertReference(Type824Reference model);

        /// <summary>
        /// Inserts a 824 Tech Error record.
        /// </summary>
        /// <remarks>
        /// Reference Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">824 Tech Error model</param>
        /// <returns>824 Tech Error Key</returns>
        int InsertTechError(Type824TechError model);
    }
}

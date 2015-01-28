using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access methods for Market File records from Market database.
    /// </summary>
    public interface IMarketFile
    {
        /// <summary>
        /// Lists Market File records that have a Status of 2 (Encrypted)
        /// and Direction Flag of false (Outbound).
        /// </summary>
        /// <remarks>
        /// Only used for Ercot Customer Billing File exports.
        /// </remarks>
        /// <remarks>
        /// Should be inspected since the code only wants CBF files but
        /// this method returns "all" records that satisfy the criteria.
        /// </remarks>
        /// <returns>array of Market File Models</returns>
        MarketFileModel[] ListEncryptedOutboundMarketFiles();

        /// <summary>
        /// Lists Market File records that have a Status of 0 (Inserted)
        /// and Direction Flag of false (Outbound).
        /// </summary>
        /// <remarks>
        /// Only used for Ercot Customer Billing File exports.
        /// </remarks>
        /// <remarks>
        /// Should be inspected since the code only wants CBF files but
        /// this method returns "all" records that satisfy the criteria.
        /// </remarks>
        /// <returns>array of Market File Models</returns>
        MarketFileModel[] ListInsertedOutboundMarketFiles();
        
        /// <summary>
        /// Inserts a Market File record.
        /// </summary>
        /// <param name="model">Market File model</param>
        /// <returns>Market File Id</returns>
        int InsertMarketFile(MarketFileModel model);

        /// <summary>
        /// Inserts a Market File Audit record.
        /// </summary>
        /// <param name="marketFileId">Market File Id</param>
        /// <param name="auditCount">Count from imported file</param>
        /// <param name="actualCount">Actual count from import process</param>
        void InsertAuditRecord(int marketFileId, int auditCount, int actualCount);

        /// <summary>
        /// Checks whether or not the file already exists in the Market File table.
        /// </summary>
        /// <remarks>
        /// This check is only being performed for Prism files being imported.
        /// </remarks>
        /// <param name="fileName">file name</param>
        /// <returns>True if already exists; otherwise false</returns>
        bool MarketFileExists(string fileName);

        /// <summary>
        /// Updates a Market File record.
        /// </summary>
        /// <param name="model">Market File model</param>
        void UpdateMarketFile(MarketFileModel model);

        /// <summary>
        /// Load Market File record by File Name
        /// where Direction Flag is false (Outbound).
        /// </summary>
        /// <returns>Market File Model</returns>
        MarketFileModel LoadOutboundMarketFileByName(string fileName);
    }
}
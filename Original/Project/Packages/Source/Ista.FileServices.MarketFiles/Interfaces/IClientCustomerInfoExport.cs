using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access for exporting Customer Billing Files from the Client database.
    /// </summary>
    public interface IClientCustomerInfoExport
    {
        /// <summary>
        /// Lists Customer Billing File records with a Status of 3 (ReadyForTransmission).
        /// </summary>
        /// <returns>array of Customer Info File models</returns>
        TypeCustomerInfoFile[] ListCustomerInfoReadyForTransmission();

        /// <summary>
        /// Lists Customer Billing File Record records by Customer Billing File Id.
        /// </summary>
        /// <param name="fileId">Customer Billing File Id</param>
        /// <returns>array of Customer Info Record models</returns>
        TypeCustomerInfoRecord[] ListRecords(int fileId);

        /// <summary>
        /// Updates Customer Billing File record with given Status.
        /// </summary>
        /// <remarks>
        /// The stored procedure requires all fields be passed to perform the update.
        /// This requires the Customer Billing File to first be loaded prior to 
        /// updating the record to the new Status.
        /// </remarks>
        /// <param name="fileId">Customer Billing File Id</param>
        /// <param name="status">Customer Info File Status</param>
        void UpdateCustomerInfoFile(int fileId, CustomerInfoFileStatusOptions status);
    }
}

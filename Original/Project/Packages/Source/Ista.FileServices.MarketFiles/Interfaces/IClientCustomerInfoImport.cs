using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access methods for importing Customer Billing Files to the Client database.
    /// </summary>
    public interface IClientCustomerInfoImport
    {
        /// <summary>
        /// Inserts a Customer Billing File record.
        /// </summary>
        /// <param name="model">Customer Billing File model</param>
        /// <returns>Customer Billing File Id</returns>
        int InsertFile(TypeCustomerInfoFile model);

        /// <summary>
        /// Inserts a Customer Billing File Error Record record.
        /// </summary>
        /// <param name="model">Customer Billing File Error Record model</param>
        /// <returns>Customer Billing File Error Record Id</returns>
        int InsertErrorRecord(TypeCustomerInfoErrorRecord model);
    }
}
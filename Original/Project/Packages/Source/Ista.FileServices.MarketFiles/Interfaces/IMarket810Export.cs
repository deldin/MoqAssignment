using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access methods for exporting 810 Transactions from Market database.
    /// </summary>
    public interface IMarket810Export
    {
        /// <summary>
        /// Lists 810 Header records that have not been processed.
        /// </summary>
        /// <param name="ldcDuns">LDC Duns</param>
        /// <param name="duns">Duns</param>
        /// <param name="providerId">Prism (1) or Xml (2)</param>
        /// <returns>array of 810 Header models</returns>
        Type810Header[] ListUnprocessed(string ldcDuns, string duns, int providerId);

        /// <summary>
        /// Lists 810 Balance records by Header.
        /// </summary>
        /// <remarks>
        /// Utilized only by Xml exports.
        /// </remarks>
        /// <param name="headerKey">810 Header Key</param>
        /// <returns>array of 810 Balance models</returns>
        Type810Balance[] ListBalances(int headerKey);

        /// <summary>
        /// Lists 810 Detail records by Header.
        /// </summary>
        /// <param name="headerKey">810 Header Key</param>
        /// <returns>array of 810 Detail models</returns>
        Type810Detail[] ListDetails(int headerKey);

        /// <summary>
        /// Lists 810 Detail Item records by Detail.
        /// </summary>
        /// <remarks>
        /// Utilized only by Xml exports.
        /// </remarks>
        /// <param name="detailKey">810 Detail Key</param>
        /// <returns>array of 810 Detail Item models</returns>
        Type810DetailItem[] ListDetailItems(int detailKey);

        /// <summary>
        /// Lists 810 Detail Item Charges records by Detail Item.
        /// </summary>
        /// <remarks>
        /// Utilized only by Xml exports.
        /// </remarks>
        /// <param name="detailItemKey">810 Detail Item Key</param>
        /// <returns>array of 810 Detail Item Charge models</returns>
        Type810DetailItemCharge[] ListDetailItemCharges(int detailItemKey);

        /// <summary>
        /// Lists 810 Detail Item Tax records by Detail Item.
        /// </summary>
        /// <remarks>
        /// Utilized only by Xml exports.
        /// </remarks>
        /// <param name="detailItemKey">810 Detail Item Key</param>
        /// <returns>array of 810 Detail Item Tax models</returns>
        Type810DetailItemTax[] ListDetailItemTaxes(int detailItemKey);

        /// <summary>
        /// Lists 810 Detail Tax records by Detail.
        /// </summary>
        /// <remarks>
        /// Utilized only by Xml exports.
        /// </remarks>
        /// <param name="detailKey">810 Detail Key</param>
        /// <returns>array of 810 Detail Tax models</returns>
        Type810DetailTax[] ListDetailTaxes(int detailKey);

        /// <summary>
        /// Lists 810 Message records by Header.
        /// </summary>
        /// <remarks>
        /// Utilized only by Xml exports.
        /// </remarks>
        /// <param name="headerKey">810 Header Key</param>
        /// <returns>array of 810 Message models</returns>
        Type810Message[] ListMessages(int headerKey);

        /// <summary>
        /// Lists 810 Name records by Header.
        /// </summary>
        /// <remarks>
        /// Utilized only by Xml exports.
        /// </remarks>
        /// <param name="headerKey">810 Header Key</param>
        /// <returns>array of 810 Name models</returns>
        Type810Name[] ListNames(int headerKey);

        /// <summary>
        /// Lists 810 Summary records by Header.
        /// </summary>
        /// <remarks>
        /// Utilized only by Xml exports.
        /// </remarks>
        /// <param name="headerKey">810 Header Key</param>
        /// <returns>array of 810 Summary models</returns>
        Type810Summary[] ListSummaries(int headerKey);

        /// <summary>
        /// Lists 810 Detail Item Charge records by Header.
        /// </summary>
        /// <remarks>
        /// Utilized only by Prism exports.
        /// </remarks>
        /// <param name="headerKey">810 Header Key</param>
        /// <returns>array of 810 Detail Item Charge models</returns>
        Type810DetailItemCharge[] ListDetailItemChargesByHeader(int headerKey);

        /// <summary>
        /// Lists 810 Detail Item Tax records by Header.
        /// </summary>
        /// <remarks>
        /// Utilized only by Prism exports.
        /// </remarks>
        /// <param name="headerKey">810 Header Key</param>
        /// <returns>array of 810 Detail Item Tax models</returns>
        Type810DetailItemTax[] ListDetailItemTaxesByHeader(int headerKey);

        /// <summary>
        /// Loads the first 810 Name record by Header.
        /// </summary>
        /// <remarks>
        /// Utilized only by Prism exports.
        /// </remarks>
        /// <param name="headerKey">810 Header Key</param>
        /// <returns>first 810 Name model</returns>
        Type810Name LoadFirstName(int headerKey);

        /// <summary>
        /// Updates the 810 Header with the Market File Id and File Name.
        /// </summary>
        /// <remarks>
        /// Depending on the provider (Prism or Xml) the information
        /// used to update the Header changes.
        /// </remarks>
        /// <param name="headerKey">810 Header Key</param>
        /// <param name="marketFileId">Market File Id</param>
        /// <param name="fileName">Export File Name</param>
        void UpdateHeader(int headerKey, int marketFileId, string fileName);
    }
}
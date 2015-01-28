using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access methods for exporting 824 Transactions from Market database.
    /// </summary>
    public interface IMarket824Export
    {
        /// <summary>
        /// Lists 824 Header records that have not been processed.
        /// </summary>
        /// <param name="ldcDuns">LDC Duns</param>
        /// <param name="duns">Duns</param>
        /// <param name="providerId">Prism (1) or Xml (2)</param>
        /// <returns>array of 824 Header models</returns>
        Type824Header[] ListUnprocessed(string ldcDuns, string duns, int providerId);

        /// <summary>
        /// Lists 824 Reason records by Header.
        /// </summary>
        /// <param name="headerKey">824 Header Key</param>
        /// <returns>array of 824 Reason models</returns>
        Type824Reason[] ListReasons(int headerKey);

        /// <summary>
        /// Lists 814 Reference records by Header.
        /// </summary>
        /// <param name="headerKey">824 Header Key</param>
        /// <returns>array of 824 Reference models.</returns>
        Type824Reference[] ListReferences(int headerKey);

        /// <summary>
        /// Updates the 824 Header with the Market File Id and File Name.
        /// </summary>
        /// <remarks>
        /// Depending on the provider (Prism or Xml) the information
        /// used to update the Header changes.
        /// </remarks>
        /// <param name="headerKey">824 Header Key</param>
        /// <param name="marketFileId">Market File Id</param>
        /// <param name="fileName">Export File Name</param>
        void UpdateHeader(int headerKey, int marketFileId, string fileName);
    }
}
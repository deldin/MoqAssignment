using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access methods for exporting 820 Transactions from Market database.
    /// </summary>
    public interface IMarket820Export
    {
        /// <summary>
        /// Lists 820 Header records that have not been processed.
        /// </summary>
        /// <param name="ldcDuns">LDC Duns</param>
        /// <param name="duns">Duns</param>
        /// <param name="providerId">Prism (1) or Xml (2)</param>
        /// <returns>array of 820 Header models</returns>
        Type820Header[] ListUnprocessed(string ldcDuns, string duns, int providerId);

        /// <summary>
        /// Lists 820 Detail records by Header.
        /// </summary>
        /// <param name="headerKey">820 Header Key</param>
        /// <returns>array of 820 Detail models</returns>
        Type820Detail[] ListDetails(int headerKey);

        /// <summary>
        /// Updates the 820 Header with the Market File Id and File Name.
        /// </summary>
        /// <remarks>
        /// Depending on the provider (Prism or Xml) the information
        /// used to update the Header changes.
        /// </remarks>
        /// <param name="headerKey">820 Header Key</param>
        /// <param name="marketFileId">Market File Id</param>
        /// <param name="fileName">Export File Name</param>
        void UpdateHeader(int headerKey, int marketFileId, string fileName);
    }
}
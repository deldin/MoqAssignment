using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access methods for exporting 650 Transactions from Market database.
    /// </summary>
    public interface IMarket650Export
    {
        /// <summary>
        /// Lists 650 Header records that have not been processed.
        /// </summary>
        /// <param name="ldcDuns">LDC Duns</param>
        /// <param name="duns">Duns</param>
        /// <param name="providerId">Prism (1) or Xml (2)</param>
        /// <returns>array of 650 Header models</returns>
        Type650Header[] ListUnprocessed(string ldcDuns, string duns, int providerId);

        /// <summary>
        /// Lists 650 Name records by Header.
        /// </summary>
        /// <param name="headerKey">650 Header Key</param>
        /// <returns>array of 650 Name models</returns>
        Type650Name[] ListNames(int headerKey);

        /// <summary>
        /// Lists 650 Service records by Header.
        /// </summary>
        /// <param name="headerKey">650 Header Key</param>
        /// <returns>array of 650 Service models</returns>
        Type650Service[] ListServices(int headerKey);

        /// <summary>
        /// Lists 650 Service Pole records by Service.
        /// </summary>
        /// <param name="serviceKey">650 Service Key</param>
        /// <returns>array of 650 Service Pole models</returns>
        Type650ServicePole[] ListServicePoles(int serviceKey);

        /// <summary>
        /// Lists 650 Service Change records by Service.
        /// </summary>
        /// <param name="serviceKey">650 Service Key</param>
        /// <returns>array of 650 Service Change models</returns>
        Type650ServiceChange[] ListServiceChanges(int serviceKey);

        /// <summary>
        /// List 650 Service Reject records by Service.
        /// </summary>
        /// <param name="serviceKey">650 Service Key</param>
        /// <returns>array of 650 Service Reject models</returns>
        Type650ServiceReject[] ListServiceRejects(int serviceKey);

        /// <summary>
        /// List 650 Service Meter records by Service.
        /// </summary>
        /// <param name="serviceKey">650 Service Key</param>
        /// <returns>array of 650 Service Meter models</returns>
        Type650ServiceMeter[] ListServiceMeters(int serviceKey);

        /// <summary>
        /// Updates the 650 Header with the Market File Id and File Name.
        /// </summary>
        /// <remarks>
        /// Depending on the provider (Prism or Xml) the information
        /// used to update the Header changes.
        /// </remarks>
        /// <param name="headerKey">650 Header Key</param>
        /// <param name="marketFileId">Market File Id</param>
        /// <param name="fileName">Export File Name</param>
        void UpdateHeader(int headerKey, int marketFileId, string fileName);
    }
}
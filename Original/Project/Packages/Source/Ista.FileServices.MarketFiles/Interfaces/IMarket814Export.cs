using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access methods for exporting 814 Transactions from Market database.
    /// </summary>
    public interface IMarket814Export
    {
        /// <summary>
        /// Lists 814 Header records that have not been processed.
        /// </summary>
        /// <param name="ldcDuns">LDC Duns</param>
        /// <param name="duns">Duns</param>
        /// <param name="providerId">Prism (1) or Xml (2)</param>
        /// <returns>array of 814 Header models</returns>
        Type814Header[] ListUnprocessed(string ldcDuns, string duns, int providerId);

        /// <summary>
        /// Lists 814 Name records by Header.
        /// </summary>
        /// <param name="headerKey">814 Header Key</param>
        /// <returns>array of 814 Name models</returns>
        Type814Name[] ListNames(int headerKey);

        /// <summary>
        /// Lists 814 Service records by Header.
        /// </summary>
        /// <param name="headerKey">814 Header Key</param>
        /// <returns>array of 814 Service models</returns>
        Type814Service[] ListServices(int headerKey);

        /// <summary>
        /// Lists 814 Service Account Change records by Service.
        /// </summary>
        /// <param name="serviceKey">814 Service Key</param>
        /// <returns>array of 814 Service Account Change models</returns>
        Type814ServiceAccountChange[] ListServiceAccountChanges(int serviceKey);

        /// <summary>
        /// Lists 814 Service Date records by Service.
        /// </summary>
        /// <param name="serviceKey">814 Service Key</param>
        /// <returns>array of 814 Service Date models</returns>
        Type814ServiceDate[] ListServiceDates(int serviceKey);

        /// <summary>
        /// Lists 814 Service Status records by Service.
        /// </summary>
        /// <param name="serviceKey">814 Service Key</param>
        /// <returns>array of 814 Service Status models</returns>
        Type814ServiceStatus[] ListServiceStatuses(int serviceKey);

        /// <summary>
        /// Lists 814 Service Reject records by Service.
        /// </summary>
        /// <param name="serviceKey">814 Service Key</param>
        /// <returns>array of 814 Service Reject models</returns>
        Type814ServiceReject[] ListServiceRejects(int serviceKey);

        /// <summary>
        /// Lists 814 Service Meter records by Service.
        /// </summary>
        /// <param name="serviceKey">814 Service Key</param>
        /// <returns>array of 814 Service Meter models</returns>
        Type814ServiceMeter[] ListServiceMeters(int serviceKey);

        /// <summary>
        /// Lists 814 Service Meter Change records by Service.
        /// </summary>
        /// <param name="serviceKey">814 Service Key</param>
        /// <returns>array of 814 Service Meter Change models</returns>
        Type814ServiceMeterChange[] ListServiceMeterChangesByService(int serviceKey);

        /// <summary>
        /// Lists 814 Service Meter Change records by Meter.
        /// </summary>
        /// <param name="meterKey">814 Service Meter Key</param>
        /// <returns>array of 814 Service meter Change models</returns>
        Type814ServiceMeterChange[] ListServiceMeterChanges(int meterKey);

        /// <summary>
        /// Lists 814 Service Meter TOU records by Meter.
        /// </summary>
        /// <param name="meterKey">814 Service Meter Key</param>
        /// <returns>array of 814 Service Meter Tou models</returns>
        Type814ServiceMeterTou[] ListServiceMeterTous(int meterKey);

        /// <summary>
        /// Lists 814 Service Meter Type records by Meter.
        /// </summary>
        /// <param name="meterKey">814 Service Meter Key</param>
        /// <returns>array of 814 Service Meter Type models</returns>
        Type814ServiceMeterType[] ListServiceMeterTypes(int meterKey);

        /// <summary>
        /// Updates the 814 Header with the Market File Id and File Name.
        /// </summary>
        /// <remarks>
        /// Depending on the provider (Prism or Xml) the information
        /// used to update the Header changes.
        /// </remarks>
        /// <param name="headerKey">814 Header Key</param>
        /// <param name="marketFileId">Market File Id</param>
        /// <param name="fileName">Export File Name</param>
        void UpdateHeader(int headerKey, int marketFileId, string fileName);
    }
}
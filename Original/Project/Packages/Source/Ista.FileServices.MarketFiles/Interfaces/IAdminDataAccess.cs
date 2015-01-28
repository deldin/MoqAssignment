using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access methods for the Billing Admin database.
    /// </summary>
    public interface IAdminDataAccess
    {
        /// <summary>
        /// Loads configuration information for Export by Client.
        /// </summary>
        /// <remarks>
        /// Loads from Global Application Configuration passing Environment.MachineName.
        /// </remarks>
        /// <param name="clientId">client identifier</param>
        /// <returns>Configuration model</returns>
        ExportConfigurationModel LoadExportConfiguration(int clientId);

        /// <summary>
        /// Loads configuration information for Import by Client.
        /// </summary>
        /// <remarks>
        /// Loads Global Application Configuration passing Environment.MachineName.
        /// </remarks>
        /// <param name="clientId">client identifier</param>
        /// <returns>Configuration model</returns>
        ImportConfigurationModel LoadImportConfiguration(int clientId);

        /// <summary>
        /// Lists Client Provider information by Client.
        /// </summary>
        /// <remarks>
        /// Identifies whether or not the Client has been configured
        /// for Prism, Xml or both.
        /// </remarks>
        /// <param name="clientId">client identifier</param>
        /// <returns>array of Provider models</returns>
        ProviderModel[] ListProviders(int clientId);
    }
}

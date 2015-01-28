using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access methods for importing 650 Transactions to Market database.
    /// </summary>
    public interface IMarket650Import
    {
        /// <summary>
        /// Inserts a 650 Header record.
        /// </summary>
        /// <param name="model">650 Header model</param>
        /// <returns>650 Header Key</returns>
        int InsertHeader(Type650Header model);

        /// <summary>
        /// Inserts a 650 Name record.
        /// </summary>
        /// <remarks>
        /// Header Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">650 Name model</param>
        /// <returns>650 Name Key</returns>
        int InsertName(Type650Name model);

        /// <summary>
        /// Inserts a 650 Service record.
        /// </summary>
        /// <remarks>
        /// Header Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">650 Service model</param>
        /// <returns>650 Service Key</returns>
        int InsertService(Type650Service model);

        /// <summary>
        /// Inserts a 650 Service Change record.
        /// </summary>
        /// <remarks>
        /// Service Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">650 Service Change model</param>
        /// <returns>650 Service Change Key</returns>
        int InsertServiceChange(Type650ServiceChange model);

        /// <summary>
        /// Inserts a 650 Service Meter record.
        /// </summary>
        /// <remarks>
        /// Service Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">650 Service Meter model</param>
        /// <returns>650 Service Meter Key</returns>
        int InsertServiceMeter(Type650ServiceMeter model);

        /// <summary>
        /// Inserts a 650 Service Pole record.
        /// </summary>
        /// <remarks>
        /// Service Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">650 Service Pole model</param>
        /// <returns>650 Service Pole Key</returns>
        int InsertServicePole(Type650ServicePole model);

        /// <summary>
        /// Inserts a 650 Service Reject record.
        /// </summary>
        /// <remarks>
        /// Service Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">650 Service Reject model</param>
        /// <returns>650 Service Reject Key</returns>
        int InsertServiceReject(Type650ServiceReject model);
    }
}
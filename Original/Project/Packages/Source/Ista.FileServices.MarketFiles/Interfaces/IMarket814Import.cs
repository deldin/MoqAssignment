using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access methods for importing 814 Transactions to Market database.
    /// </summary>
    public interface IMarket814Import
    {
        /// <summary>
        /// Inserts a 814 Header record.
        /// </summary>
        /// <param name="model">814 Header model</param>
        /// <returns>814 Header Key</returns>
        int InsertHeader(Type814Header model);

        /// <summary>
        /// Inserts a 814 Name record.
        /// </summary>
        /// <remarks>
        /// Header Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">814 Name model</param>
        /// <returns>814 Name Key</returns>
        int InsertName(Type814Name model);

        /// <summary>
        /// Inserts a 814 Service record.
        /// </summary>
        /// <remarks>
        /// Header Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">814 Service model</param>
        /// <returns>814 Service Key</returns>
        int InsertService(Type814Service model);

        /// <summary>
        /// Inserts a 814 Service Meter record.
        /// </summary>
        /// <remarks>
        /// Service Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">814 Service Meter model</param>
        /// <returns>814 Service Meter Key</returns>
        int InsertServiceMeter(Type814ServiceMeter model);

        /// <summary>
        /// Inserts a 814 Service Meter Type record.
        /// </summary>
        /// <remarks>
        /// Service Meter Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">814 Service Meter Type model</param>
        /// <returns>814 Service Meter Type Key</returns>
        int InsertServiceMeterType(Type814ServiceMeterType model);

        /// <summary>
        /// Inserts a 814 Service Meter Change record.
        /// </summary>
        /// <remarks>
        /// Service Meter Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">814 Service Meter Change model</param>
        /// <returns>814 Service Meter Change Key</returns>
        int InsertServiceMeterChange(Type814ServiceMeterChange model);

        /// <summary>
        /// Inserts a 814 Service Meter TOU record.
        /// </summary>
        /// <remarks>
        /// Service Meter Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">814 Service Meter TOU model</param>
        /// <returns>814 Service Meter TOU Key</returns>
        int InsertServiceMeterTou(Type814ServiceMeterTou model);

        /// <summary>
        /// Inserts a 814 Service Date record.
        /// </summary>
        /// <remarks>
        /// Service Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">814 Service Date model</param>
        /// <returns>814 Service Date Key</returns>
        int InsertServiceDate(Type814ServiceDate model);

        /// <summary>
        /// Inserts a 814 Service Status record.
        /// </summary>
        /// <remarks>
        /// Service Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">814 Service Status model</param>
        /// <returns>814 Service Status Key</returns>
        int InsertServiceStatus(Type814ServiceStatus model);

        /// <summary>
        /// Inserts a 814 Service Reject record.
        /// </summary>
        /// <remarks>
        /// Service Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">814 Service Reject model</param>
        /// <returns>814 Service Reject Key</returns>
        int InsertServiceReject(Type814ServiceReject model);

        /// <summary>
        /// Inserts a 814 Service Account Change record.
        /// </summary>
        /// <remarks>
        /// Service Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">814 Service Account Change model</param>
        /// <returns>814 Service Account Change Key</returns>
        int InsertServiceAccountChange(Type814ServiceAccountChange model);
    }
}

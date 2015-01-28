using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access methods for the Market database (non specific function).
    /// </summary>
    public interface IMarketDataAccess
    {
        /// <summary>
        /// Identifies the 814 Transaction Type based on Action code and Service Action code.
        /// </summary>
        /// <remarks>
        /// Only valid for 814 Xml imports.
        /// </remarks>
        /// <param name="actionCode">Action code</param>
        /// <param name="serviceActionCode">Action code from Service</param>
        /// <returns>Transaction Type Id or zero</returns>
        int IdentifyTransactionType(string actionCode, string serviceActionCode);

        /// <summary>
        /// Loads Csp Duns Trading Partner Relationship by Csp Duns and Ldc Duns.
        /// </summary>
        /// <param name="cspDuns">Csp Duns</param>
        /// <param name="ldcDuns">Trading Partner Duns</param>
        /// <returns>Csp Duns Trading Partner model</returns>
        /// <exception cref="System.ArgumentNullException">Csp Duns is null or empty</exception>
        /// <exception cref="System.ArgumentNullException">Ldc Duns is null or empty</exception>
        CspDunsTradingPartnerModel LoadCspDunsTradingPartner(string cspDuns, string ldcDuns);

        /// <summary>
        /// Loads the Csp Duns Trading Partner Configuration
        /// </summary>
        /// <param name="model">Csp Duns Trading Partner model </param>
        /// <exception cref="System.ArgumentNullException">Csp Duns Trading Partner model is null</exception>
        void LoadCspDunsTradingPartnerConfig(CspDunsTradingPartnerModel model);
    }
}
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access methods for importing 810 Transactions to Market database.
    /// </summary>
    public interface IMarket810Import
    {
        /// <summary>
        /// Inserts a 810 Balance record.
        /// </summary>
        /// <remarks>
        /// Header Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">810 Balance model</param>
        /// <returns>810 Balance Key</returns>
        int InsertBalance(Type810Balance model);

        /// <summary>
        /// Inserts a 810 Detail record.
        /// </summary>
        /// <remarks>
        /// Header Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">810 Detail model</param>
        /// <returns>810 Detail Key</returns>
        int InsertDetail(Type810Detail model);

        /// <summary>
        /// Inserts a 810 Detail Item record.
        /// </summary>
        /// <remarks>
        /// Detail Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">810 Detail Item model</param>
        /// <returns>810 Detail Item Key</returns>
        int InsertDetailItem(Type810DetailItem model);

        /// <summary>
        /// Inserts a 810 Detail Item Charge record.
        /// </summary>
        /// <remarks>
        /// Detail Item Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">810 Detail Item Charge model</param>
        /// <returns>810 Detail Item Charge Key</returns>
        int InsertDetailItemCharge(Type810DetailItemCharge model);

        /// <summary>
        /// Inserts a 810 Detail Item Tax record.
        /// </summary>
        /// <remarks>
        /// Detail Item Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">810 Detail Item Tax model</param>
        /// <returns>810 Detail Item Tax Key</returns>
        int InsertDetailItemTax(Type810DetailItemTax model);

        /// <summary>
        /// Inserts a 810 Detail Tax record.
        /// </summary>
        /// <remarks>
        /// Detail Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">810 Detail Tax model</param>
        /// <returns>810 Detail Tax Key</returns>
        int InsertDetailTax(Type810DetailTax model);

        /// <summary>
        /// Inserts a 810 Header record.
        /// </summary>
        /// <param name="model">810 Header model</param>
        /// <returns>810 Header Key</returns>
        int InsertHeader(Type810Header model);

        /// <summary>
        /// Inserts a 810 Message record.
        /// </summary>
        /// <remarks>
        /// Header Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">810 Message model</param>
        /// <returns>810 Message Key</returns>
        int InsertMessage(Type810Message model);

        /// <summary>
        /// Inserts a 810 Name record.
        /// </summary>
        /// <remarks>
        /// Header Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">810 Name model</param>
        /// <returns>810 Name Key</returns>
        int InsertName(Type810Name model);

        /// <summary>
        /// Inserts a 810 Payment record.
        /// </summary>
        /// <remarks>
        /// Header Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">810 Payment model</param>
        /// <returns>810 Payment Key</returns>
        int InsertPayment(Type810Payment model);

        /// <summary>
        /// Inserts a 810 Summary record.
        /// </summary>
        /// <remarks>
        /// Header Key should already be set prior to calling this method.
        /// </remarks>
        /// <param name="model">810 Summary model</param>
        /// <returns>810 Summary Key</returns>
        int InsertSummary(Type810Summary model);
    }
}

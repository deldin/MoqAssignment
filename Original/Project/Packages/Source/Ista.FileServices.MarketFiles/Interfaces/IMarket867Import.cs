using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access methods for importing 867 Transactions to Market database.
    /// </summary>
    public interface IMarket867Import
    {
        /// <summary>
        /// Inserts a 867 Header record.
        /// </summary>
        /// <param name="model">867 Header model</param>
        /// <returns>867 Header Key</returns>
        int InsertHeader(Type867Header model);

        /// <summary>
        /// Inserts a 867 AccountBill Quantity record.
        /// </summary>
        /// <remarks>
        /// This was only found to exist for Liberty and Energy Plus.
        /// This could cause problems.
        /// </remarks>
        /// <param name="model">867 Account Bill Qty model</param>
        /// <returns>867 AccountBill Quantity Key</returns>
        int InsertAccountBillQuanity(Type867AccountBillQty model);

        /// <summary>
        /// Inserts a 867 Non Interval Summary record.
        /// </summary>
        /// <param name="model">867 Non Interval Summary model</param>
        /// <returns>867 Non Interval Summary Key</returns>
        int InsertNonIntervalSummary(Type867NonIntervalSummary model);

        /// <summary>
        /// Inserts a 867 Net Interval Summary record.
        /// </summary>
        /// <param name="model">867 Net Interval Summary model</param>
        /// <returns>867 Net Interval Summary Key</returns>
        int InsertNetIntervalSummary(Type867NetIntervalSummary model);

        /// <summary>
        /// Inserts a 867 Interval Summary record.
        /// </summary>
        /// <param name="model">867 Interval Summary model</param>
        /// <returns>867 Interval Summary Key</returns>
        int InsertIntervalSummary(Type867IntervalSummary model);

        /// <summary>
        /// Inserts a 867 Unmeter Detail record.
        /// </summary>
        /// <param name="model">867 Unmeter Detail model</param>
        /// <returns>867 Unmeter Detail Key</returns>
        int InsertUnMeterDetail(Type867UnMeterDetail model);

        /// <summary>
        /// Inserts a 867 Non Interval Summary Qty record.
        /// </summary>
        /// <param name="model">867 Non Interval Summary Qty model</param>
        void InsertNonIntervalSummaryQty(Type867NonIntervalSummaryQty model);

        /// <summary>
        /// Inserts a 867 Net Interval Summary Qty record.
        /// </summary>
        /// <param name="model">867 Net Interval Summary Qty model</param>
        void InsertNetIntervalSummaryQty(Type867NetIntervalSummaryQty model);

        /// <summary>
        /// Inserts a 867 Interval Summary Qty record.
        /// </summary>
        /// <param name="model">867 Interval Summary Qty model</param>
        void InsertIntervalSummaryQty(Type867IntervalSummaryQty model);

        /// <summary>
        /// Inserts a 867 Unmeter Detail Qty record.
        /// </summary>
        /// <param name="model">867 Unmeter Detail Qty model</param>
        void InsertUnMeterDetailQty(Type867UnMeterDetailQty model);

        /// <summary>
        /// Inserts a 867 Interval Summary Across Meters record.
        /// </summary>
        /// <param name="model">867 Interval Summary Across Meters model</param>
        /// <returns>867 Interval Summary Across Meters Key</returns>
        int InsertIntervalSummaryAcrossMeters(Type867IntervalSummaryAcrossMeters model);

        /// <summary>
        /// Inserts a 867 Interval Summary Across Meters Qty record.
        /// </summary>
        /// <param name="model">867 Interval Summary Across Meters Qty model</param>
        void InsertIntervalSummaryAcrossMetersQty(Type867IntervalSummaryAcrossMetersQty model);

        /// <summary>
        /// Inserts a 867 Non Interval Detail record.
        /// </summary>
        /// <param name="model">867 Non Interval Detail model</param>
        /// <returns>867 Non Interval Detail Key</returns>
        int InsertNonIntervalDetail(Type867NonIntervalDetail model);

        /// <summary>
        /// Inserts a 867 Non Interval Detail Qty record.
        /// </summary>
        /// <param name="model">867 Non Interval Detail Qty model</param>
        void InsertNonIntervalDetailQty(Type867NonIntervalDetailQty model);

        /// <summary>
        /// Inserts a 867 Interval Detail record.
        /// </summary>
        /// <param name="model">867 Interval Detail model</param>
        /// <returns>867 Interval Detail Key</returns>
        int InsertIntervalDetail(Type867IntervalDetail model);

        /// <summary>
        /// Inserts a 867 Interval Detail Qty record.
        /// </summary>
        /// <param name="model">867 Interval Detail Qty model</param>
        void InsertIntervalDetailQty(Type867IntervalDetailQty model);

        /// <summary>
        /// Inserts a 867 Schedule Determinants record.
        /// </summary>
        /// <param name="model">867 Schedule Determinants model</param>
        /// <returns>867 Schedule Determinants Key</returns>
        int InsertScheduleDeterminants(Type867ScheduleDeterminants model);

        /// <summary>
        /// Inserts a 867 Switch record.
        /// </summary>
        /// <remarks>
        /// Utilized by Prism imports only.
        /// </remarks>
        /// <param name="model">867 Switch model</param>
        /// <returns>867 Switch Key</returns>
        int InsertSwitch(Type867Switch model);

        /// <summary>
        /// Inserts a 867 Switch Qty record.
        /// </summary>
        /// <remarks>
        /// Utilized by Prism imports only.
        /// </remarks>
        /// <param name="model">867 Switch Qty model</param>
        void InsertSwitchQty(Type867SwitchQty model);

        /// <summary>
        /// Inserts a 867 Name record.
        /// </summary>
        /// <remarks>
        /// Utilized by Xml imports only.
        /// </remarks>
        /// <param name="model">867 Name model</param>
        void InsertName(Type867Name model);

        /// <summary>
        /// Inserts a 867 Unmeter Summary record.
        /// </summary>
        /// <remarks>
        /// Utilized by Xml imports only.
        /// </remarks>
        /// <param name="model">867 Unmeter Summary model</param>
        /// <returns>867 Unmeter Summary Key</returns>
        int InsertUnMeterSummary(Type867UnMeterSummary model);

        /// <summary>
        /// Inserts a 867 Unmeter Summary Qty record.
        /// </summary>
        /// <remarks>
        /// Utilized by Xml imports only.
        /// </remarks>
        /// <param name="model">867 Unmeter Summary Qty model</param>
        void InsertUnMeterSummaryQty(Type867UnMeterSummaryQty model);

        /// <summary>
        /// Inserts a 867 Gas Profile Factor Evaluation record.
        /// </summary>
        /// <remarks>
        /// Utilized by Xml imports only.
        /// </remarks>
        /// <remarks>
        /// It does not appear that all Market databases have this table.
        /// This could cause problems.
        /// </remarks>
        /// <param name="model">867 Gas Profile Factor Evaluation model</param>
        void InsertGasProfileFactorEvaluation(Type867GasProfileFactorEvaluation model);

        /// <summary>
        /// Inserts a 867 Gas Profile Factor Sample record.
        /// </summary>
        /// <remarks>
        /// Utilized by Xml imports only.
        /// </remarks>
        /// <remarks>
        /// It does not appear that all Market databases have this table.
        /// This could cause problems.
        /// </remarks>
        /// <param name="model">867 Gas Profile Factor Sample model</param>
        void InsertGasProfileFactorSample(Type867GasProfileFactorSample model);
    }
}

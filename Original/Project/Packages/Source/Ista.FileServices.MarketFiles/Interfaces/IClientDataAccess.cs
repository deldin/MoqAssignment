using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    /// <summary>
    /// Data Access methods for the Client database.
    /// </summary>
    public interface IClientDataAccess
    {
        /// <summary>
        /// Returns the Market Id by TDSP Duns if found; otherwise null.
        /// </summary>
        /// <param name="tdspDuns">TDSP Duns</param>
        /// <returns>Market Id or null</returns>
        int? IdentifyMarket(string tdspDuns);

        /// <summary>
        /// Returns the CSP Duns Id by CSP Duns if found; otherwise zero.
        /// </summary>
        /// <remarks>
        /// Actually loads the CSP Duns record returning only the CSP Duns Id.
        /// </remarks>
        /// <param name="cspDuns">CSP Duns</param>
        /// <returns>CSP Duns Id or zero</returns>
        /// <exception cref="System.ArgumentNullException">CSP Duns is null or empty</exception>
        int IdentifyCspDunsId(string cspDuns);

        /// <summary>
        /// Determines whether or not Meter data should be exported with the 810
        /// by looking at the "Export Meter Data for 810s" column within the LDC Detail table.
        /// </summary>
        /// <remarks>
        /// Returns true if the TDSP Duns is null or an empty string.
        /// </remarks>
        /// <param name="tdspDuns">TDSP Duns</param>
        /// <returns>True if Meter Data should be exported; otherwise false</returns>
        bool ShouldExportMeterData(string tdspDuns);

        /// <summary>
        /// Lists CSP Duns.
        /// </summary>
        /// <returns>array of Csp Duns models</returns>
        CspDunsModel[] ListCspDuns();

        /// <summary>
        /// Lists CSP Duns Port.
        /// </summary>
        /// <remarks>
        /// Calls ListCspDunsPort with an empty file type.
        /// This returns any CSP Duns Port record with an empty or null File Type.
        /// </remarks>
        /// <returns>array of Csp Duns Port models</returns>
        CspDunsPortModel[] ListCspDunsPort();

        /// <summary>
        /// Lists CSP Duns Port for a given Market File Type.
        /// </summary>
        /// <remarks>
        /// This returns any CSP Duns Port record with an empty or null File Type
        /// and any record whose File Type matches the given Market File Type. If
        /// a match is found, however, any record with an empty or null File Type
        /// for the same Csp Duns Id and Ldc Id is removed.
        /// </remarks>
        /// <param name="fileType">Market File Type</param>
        /// <returns>array of Csp Duns Port models</returns>
        CspDunsPortModel[] ListCspDunsPort(string fileType);

        /// <summary>
        /// Lists Meter Consumption by Invoice Number.
        /// </summary>
        /// <param name="invoiceNbr">Invoice Number</param>
        /// <returns>array of Meter Consumption models</returns>
        /// <exception cref="System.ArgumentNullException">Invoice Number is null or empty</exception>
        /// <exception cref="System.ArgumentException">Invoice Number is not numeric</exception>
        MeterConsumptionModel[] ListMeterConsumptionByInvoice(string invoiceNbr);

        /// <summary>
        /// Loads Customer Invoice Config record by Customer Duns and Ldc Id; otherwise null.
        /// </summary>
        /// <param name="customerDuns">Customer Duns</param>
        /// <param name="ldcId">Ldc Id</param>
        /// <returns>Customer Invoice Configuration model or null</returns>
        /// <exception cref="System.ArgumentNullException">Customer Duns is null or empty</exception>
        CustomerInvoiceConfigModel LoadCustomerInvoiceConfig(string customerDuns, int ldcId);

        /// <summary>
        /// Loads Premise record by Premise Number (or Esi Id); otherwise null.
        /// </summary>
        /// <param name="esiId">Premise Number (Esi Id)</param>
        /// <returns>Customer Premise model or null</returns>
        /// <exception cref="System.ArgumentNullException">Esi Id is null or empty</exception>
        CustomerPremiseModel LoadPremiseByEsiId(string esiId);

        /// <summary>
        /// Loads Customer Detail by Cust Id.
        /// </summary>
        /// <param name="customerId">Cust Id</param>
        /// <returns>Customer Detail model</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Customer Id is less than or equal to zero</exception>
        CustomerDetailModel LoadCustomerDetail(int customerId);

        /// <summary>
        /// Loads Customer Detail by Premise Number (or Esi Id); otherwise null.
        /// </summary>
        /// <remarks>
        /// Actually loads the Premise record first to identify the Cust Id
        /// and then Loads the Customer Detail by Cust Id.
        /// </remarks>
        /// <param name="esiId">Premise Number (Esi Id)</param>
        /// <returns>Customer Detail model or null</returns>
        /// <exception cref="System.ArgumentNullException">Esi Id is null or empty</exception>
        CustomerDetailModel LoadCustomerDetailByEsiId(string esiId);

        /// <summary>
        /// Loads Accounts Receivable History record by Invoice Number; otherwise null.
        /// </summary>
        /// <remarks>
        /// The underlying stored procedure returns a lot more information but was not 
        /// found to be used or wanted.
        /// </remarks>
        /// <param name="invoiceNbr">Invoice Number</param>
        /// <returns>Customer AR Summary model or null</returns>
        /// <exception cref="System.ArgumentNullException">Invoice Number is null or empty</exception>
        CustomerArSummaryModel LoadArSummaryByInvoice(string invoiceNbr);

        /// <summary>
        /// Loads LDC record by TDSP Duns; otherwise null.
        /// </summary>
        /// <remarks>
        /// Returns both the LDC Id and Market Id.
        /// </remarks>
        /// <param name="tdspDuns">TDSP Duns</param>
        /// <returns>Ldc model or null</returns>
        /// <exception cref="System.ArgumentNullException">TDSP Duns is null or empty</exception>
        LdcModel LoadLdcByTdspDuns(string tdspDuns);

        /// <summary>
        /// Loads LDC record by Ldc Id; otherwise null.
        /// </summary>
        /// <param name="ldcId">Ldc Id</param>
        /// <returns>Ldc model or null</returns>
        LdcModel LoadLdcById(int ldcId);

        /// <summary>
        /// Returns the Duns by CSP Duns Id if found.
        /// </summary>
        /// <remarks>
        /// Loads the CSP Duns record returning only the Duns.
        /// </remarks>
        /// <param name="cspDunsId">CSP Duns Id</param>
        /// <returns>Duns</returns>
        string LoadDunsByCspDunsId(int cspDunsId);

        /// <summary>
        /// Returns the Value for the Global Parameter Configuration
        /// of the given parameter configuration name
        /// </summary>
        /// <param name="parameterConfigurationName">Global Parameter Configuration Name</param>
        /// <returns>string value</returns>
        string LoadParameterConfigurationValue(string parameterConfigurationName);
    }
}
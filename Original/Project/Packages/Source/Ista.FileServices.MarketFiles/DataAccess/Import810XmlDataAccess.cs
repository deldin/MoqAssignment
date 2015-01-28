using System;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class Import810XmlDataAccess : IMarket810Import
    {
        private readonly string connectionString;

        public Import810XmlDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int InsertHeader(Type810Header model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810HeaderInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@MarketFileId", model.MarketFileId)
                    .AddWithValue("@TransactionSetId", model.TransactionSetId)
                    .AddWithValue("@TransactionSetControlNbr", model.TransactionSetControlNbr)
                    .AddIfNotEmptyOrDbNull("@TransactionSetPurposeCode", model.TransactionSetPurposeCode)
                    .AddIfNotEmptyOrDbNull("@InvoiceNbr", model.InvoiceNbr)
                    .AddIfNotEmptyOrDbNull("@TransactionDate", model.TransactionDate)
                    .AddIfNotEmptyOrDbNull("@ReleaseNbr", model.ReleaseNbr)
                    .AddIfNotEmptyOrDbNull("@TransactionTypeCode", model.TransactionTypeCode)
                    .AddIfNotEmptyOrDbNull("@OriginalInvoiceNbr", model.OriginalInvoiceNbr)
                    .AddIfNotEmptyOrDbNull("@EsiId", model.EsiId)
                    .AddWithValue("@CRAccountNumber", model.CrAccountNumber)
                    .AddIfNotEmptyOrDbNull("@PaymentDueDate", model.PaymentDueDate)
                    .AddIfNotEmptyOrDbNull("@TdspDuns", model.TdspDuns)
                    .AddIfNotEmptyOrDbNull("@TdspName", model.TdspName)
                    .AddWithValue("@CrDuns", model.CrDuns)
                    .AddWithValue("@CrName", model.CrName)
                    .AddIfNotEmptyOrDbNull("@TotalAmount", model.TotalAmount)
                    .AddWithValue("@Direction", true)
                    .AddWithValue("@CustNoForESCO", model.CustNoForESCO)
                    .AddWithValue("@PreviousUtilityAccountNumber", model.PreviousUtilityAccountNumber)
                    .AddWithValue("@BillCalculator", model.BillCalculator)
                    .AddWithValue("@BillPresenter", model.BillPresenter)
                    .AddWithValue("@GasPoolId", model.GasPoolId)
                    .AddWithValue("@TransactionTypeID", model.TransasctionTypeId)
                    .AddWithValue("@ProviderID", model.ProviderId)
                    .AddWithValue("@MarketID", model.MarketId)
                    .AddWithValue("@CustomerDUNS", model.CustomerDUNS)
                    .AddWithValue("@ServiceDeliveryPoint", model.ServiceDeliveryPoint)
                    .AddOutParameter("@Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var headerKey = (int)keyParameter.Value;
                model.HeaderKey = headerKey;

                return headerKey;
            }
        }

        public int InsertName(Type810Name model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810NameInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@810_Key", model.HeaderKey)
                    .AddWithValue("@EntityIdType", model.EntityIdType)
                    .AddWithValue("@EntityName", model.EntityName)
                    .AddWithValue("@EntityDuns", model.EntityDuns)
                    .AddWithValue("@EntityIdCode", model.EntityIdCode)
                    .AddWithValue("@EntityName2", model.EntityName2)
                    .AddWithValue("@EntityName3", model.EntityName3)
                    .AddWithValue("@Address1", model.Address1)
                    .AddWithValue("@Address2", model.Address2)
                    .AddWithValue("@City", model.City)
                    .AddWithValue("@State", model.State)
                    .AddWithValue("@PostalCode", model.PostalCode)
                    .AddOutParameter("@Name_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var nameKey = (int)keyParameter.Value;
                model.NameKey = nameKey;

                return nameKey;
            }
        }

        public int InsertDetail(Type810Detail model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810DetailInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@810_Key", model.HeaderKey)
                    .AddWithValue("@AssignedId", model.AssignedId)
                    .AddWithValue("@ServiceTypeCode", model.ServiceTypeCode)
                    .AddWithValue("@ServiceType", model.ServiceType)
                    .AddWithValue("@ServiceClassCode", model.ServiceClassCode)
                    .AddWithValue("@ServiceClass", model.ServiceClass)
                    .AddWithValue("@RateClass", model.RateClass)
                    .AddWithValue("@RateSubClass", model.RateSubClass)
                    .AddWithValue("@ServicePeriodStartDate", model.ServicePeriodStartDate)
                    .AddWithValue("@ServicePeriodEndDate", model.ServicePeriodEndDate)
                    .AddWithValue("@MeterNumber", model.MeterNumber)
                    .AddWithValue("@BillCycle", model.BillCycle)
                    .AddWithValue("@GasPoolId", model.GasPoolId)
                    .AddWithValue("@ServiceAgreement", model.ServiceAgreement)
                    .AddWithValue("@RateCode", model.RateCode)
                    .AddWithValue("@SupplierContractID", model.SupplierContractId)
                    .AddWithValue("@UtilityContractID", model.UtilityContractId)
                    .AddOutParameter("@Detail_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var detailKey = (int)keyParameter.Value;
                model.DetailKey = detailKey;

                return detailKey;
            }
        }

        public int InsertDetailItem(Type810DetailItem model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810DetailItemInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@Detail_Key", model.DetailKey)
                    .AddWithValue("@AssignedId", model.AssignedId)
                    .AddWithValue("@RelationshipCode", model.RelationshipCode)
                    .AddWithValue("@ServiceOrderCompleteDate", model.ServiceOrderCompleteDate)
                    .AddWithValue("@UnmeteredServiceDateRange", model.UnmeteredServiceDateRange)
                    .AddWithValue("@InvoiceNbr", model.InvoiceNbr)
                    .AddWithValue("@ServiceOrderNbr", model.ServiceOrderNbr)
                    .AddWithValue("@Consumption", model.Consumption)
                    .AddWithValue("@EffectiveDate", model.EffectiveDate)
                    .AddOutParameter("@Item_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var detailItemKey = (int)keyParameter.Value;
                model.ItemKey = detailItemKey;

                return detailItemKey;
            }
        }

        public int InsertDetailItemCharge(Type810DetailItemCharge model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810DetailItemChargeInsert"))
            {
                command.AddWithValue("@Item_Key", model.ItemKey)
                    .AddWithValue("@ChargeIndicator", model.ChargeIndicator)
                    .AddWithValue("@AgencyCode", model.AgencyCode)
                    .AddWithValue("@ChargeCode", model.ChargeCode)
                    .AddWithValue("@Amount", model.Amount)
                    .AddWithValue("@Rate", model.Rate)
                    .AddWithValue("@UOM", model.UOM)
                    .AddWithValue("@Quantity", model.Quantity)
                    .AddWithValue("@Description", model.Description);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                model.ChargeKey = 1;
                return 1;
            }
        }

        public int InsertDetailItemTax(Type810DetailItemTax model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810DetailItemTaxInsert"))
            {
                command.AddWithValue("@Item_Key", model.ItemKey)
                    .AddWithValue("@TaxTypeCode", model.TaxTypeCode)
                    .AddWithValue("@TaxAmount", model.TaxAmount)
                    .AddWithValue("@RelationshipCode", model.RelationshipCode);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                model.TaxKey = 1;
                return 1;
            }
        }

        public int InsertDetailTax(Type810DetailTax model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810DetailTaxInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@Detail_Key", model.DetailKey)
                    .AddWithValue("@AssignedId", model.AssignedId)
                    .AddWithValue("@MonetaryAmount", model.MonetaryAmount)
                    .AddWithValue("@Percent", model.Percent)
                    .AddWithValue("@RelationshipCode", model.RelationshipCode)
                    .AddWithValue("@DollarBasis", model.DollarBasis)
                    .AddWithValue("@TaxTypeCode", model.TaxTypeCode)
                    .AddWithValue("@TaxJurisdictionCodeQualifier", model.JurisdictionCodeQualifier)
                    .AddWithValue("@TaxJurisdictionCode", model.JurisdictionCode)
                    .AddWithValue("@ExemptCode", model.ExemptCode)
                    .AddWithValue("@Description", model.Description)
                    .AddOutParameter("@Tax_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var taxKey = (int)keyParameter.Value;
                model.TaxKey = taxKey;

                return taxKey;
            }
        }

        public int InsertSummary(Type810Summary model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810SummaryInsert"))
            {
                command.AddWithValue("@810_Key", model.HeaderKey)
                    .AddWithValue("@TotalAmount", model.TotalAmount)
                    .AddWithValue("@TotalLineItems", model.TotalLineItems);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                model.SummaryKey = 1;
                return 1;
            }
        }

        public int InsertBalance(Type810Balance model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810BalanceInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@810_Key", model.HeaderKey)
                    .AddWithValue("@TotalOutstandingBalance", model.TotalOutstandingBalance)
                    .AddWithValue("@BeginningBalance", model.BeginningBalance)
                    .AddWithValue("@BudgetCumulativeDifference", model.BudgetCumulativeDifference)
                    .AddWithValue("@BudgetMonthDifference", model.BudgetMonthDifference)
                    .AddOutParameter("@Balance_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var balanceKey = (int)keyParameter.Value;
                model.BalanceKey = balanceKey;

                return balanceKey;
            }
        }

        public int InsertMessage(Type810Message model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810MessageInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@810_Key", model.HeaderKey)
                    .AddWithValue("@ItemDescType", model.ItemDescType)
                    .AddWithValue("@ProductCode", model.ProductCode)
                    .AddWithValue("@Description", model.Description)
                    .AddWithValue("@PositionCode", model.PositionCode)
                    .AddOutParameter("@Message_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var messageKey = (int)keyParameter.Value;
                model.MessageKey = messageKey;

                return messageKey;
            }
        }

        public int InsertPayment(Type810Payment model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810PaymentInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@810_Key", model.HeaderKey)
                    .AddWithValue("@AmountQualifierCode", model.AmountQualifierCode)
                    .AddWithValue("@MonetaryAmount", model.MonetaryAmount)
                    .AddWithValue("@TimeUnit", model.TimeUnit)
                    .AddWithValue("@DateTimeQualifier", model.DateTimeQualifier)
                    .AddWithValue("@Date", model.Date)
                    .AddOutParameter("@Payment_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var paymentKey = (int)keyParameter.Value;
                model.PaymentKey = paymentKey;

                return paymentKey;
            }
        }
    }
}
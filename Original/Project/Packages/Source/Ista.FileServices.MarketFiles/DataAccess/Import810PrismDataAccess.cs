using System;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class Import810PrismDataAccess : IMarket810Import
    {
        private readonly string connectionString;

        public Import810PrismDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int InsertHeader(Type810Header model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810HeaderInsert"))
            {
                SqlParameter keyParameter;

                // this differs from other imports as the import
                // specifies the parameters that should be used
                // the stored procedure takes many more parameters but...
                command.AddWithValue("@MarketFileId", model.MarketFileId)
                    .AddIfNotEmptyOrDbNull("@TransactionSetPurposeCode", model.TransactionSetPurposeCode)
                    .AddIfNotEmptyOrDbNull("@InvoiceNbr", model.InvoiceNbr)
                    .AddIfNotEmptyOrDbNull("@TransactionDate", model.TransactionDate)
                    .AddIfNotEmptyOrDbNull("@ReleaseNbr", model.ReleaseNbr)
                    .AddIfNotEmptyOrDbNull("@TransactionTypeCode", model.TransactionTypeCode)
                    .AddIfNotEmptyOrDbNull("@OriginalInvoiceNbr", model.OriginalInvoiceNbr)
                    .AddIfNotEmptyOrDbNull("@EsiId", model.EsiId)
                    .AddIfNotEmptyOrDbNull("@PaymentDueDate", model.PaymentDueDate)
                    .AddIfNotEmptyOrDbNull("@TdspDuns", model.TdspDuns)
                    .AddIfNotEmptyOrDbNull("@TdspName", model.TdspName)
                    .AddWithValue("@CrDuns", model.CrDuns)
                    .AddWithValue("@CrName", model.CrName)
                    .AddIfNotEmptyOrDbNull("@TotalAmount", model.TotalAmount)
                    .AddWithValue("@Direction", true)
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

        public int InsertDetail(Type810Detail model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810DetailInsert"))
            {
                SqlParameter keyParameter;

                // this differs from other imports as the import
                // specifies the parameters that should be used
                // the stored procedure takes many more parameters but...
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
                    .AddIfNotEmptyOrDbNull("@AssignedId", model.AssignedId)
                    .AddIfNotEmptyOrDbNull("@RelationshipCode", model.RelationshipCode)
                    .AddIfNotEmptyOrDbNull("@ServiceOrderCompleteDate", model.ServiceOrderCompleteDate)
                    .AddIfNotEmptyOrDbNull("@UnmeteredServiceDateRange", model.UnmeteredServiceDateRange)
                    .AddIfNotEmptyOrDbNull("@InvoiceNbr", model.InvoiceNbr)
                    .AddIfNotEmptyOrDbNull("@ServiceOrderNbr", model.ServiceOrderNbr)
                    .AddWithValue("@Consumption", model.Consumption)
                    .AddWithValue("@EffectiveDate", model.EffectiveDate)
                    .AddOutParameter("@Item_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var itemKey = (int)keyParameter.Value;
                model.ItemKey = itemKey;

                return itemKey;
            }
        }

        public int InsertDetailItemCharge(Type810DetailItemCharge model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810DetailItemChargeInsert"))
            {
                command.AddWithValue("@Item_Key", model.ItemKey)
                    .AddIfNotEmptyOrDbNull("@ChargeIndicator", model.ChargeIndicator)
                    .AddIfNotEmptyOrDbNull("@AgencyCode", model.AgencyCode)
                    .AddIfNotEmptyOrDbNull("@ChargeCode", model.ChargeCode)
                    .AddIfNotEmptyOrDbNull("@Amount", model.Amount)
                    .AddIfNotEmptyOrDbNull("@Rate", model.Rate)
                    .AddIfNotEmptyOrDbNull("@UOM", model.UOM)
                    .AddIfNotEmptyOrDbNull("@Quantity", model.Quantity)
                    .AddIfNotEmptyOrDbNull("@Description", model.Description);

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
                    .AddIfNotEmptyOrDbNull("@TaxTypeCode", model.TaxTypeCode)
                    .AddIfNotEmptyOrDbNull("@TaxAmount", model.TaxAmount)
                    .AddIfNotEmptyOrDbNull("@RelationshipCode", model.RelationshipCode);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                model.TaxKey = 1;
                return 1;
            }
        }

        public int InsertSummary(Type810Summary model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810SummaryInsert"))
            {
                command.AddWithValue("@810_Key", model.HeaderKey)
                    .AddIfNotEmptyOrDbNull("@TotalAmount", model.TotalAmount)
                    .AddIfNotEmptyOrDbNull("@TotalLineItems", model.TotalLineItems);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                model.SummaryKey = 1;
                return 1;
            }
        }

        public int InsertBalance(Type810Balance model)
        {
            return -1;
        }

        public int InsertDetailTax(Type810DetailTax model)
        {
            return -1;
        }

        public int InsertMessage(Type810Message model)
        {
            return -1;
        }

        public int InsertName(Type810Name model)
        {
            return -1;
        }

        public int InsertPayment(Type810Payment model)
        {
            return -1;
        }
    }
}

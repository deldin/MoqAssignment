using System;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class Import824PrismDataAccess : IMarket824Import
    {
        private readonly string connectionString;

        public Import824PrismDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int InsertHeader(Type824Header model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp824HeaderInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@MarketFileId", model.MarketFileId)
                    .AddWithValue("@TransactionSetId", model.TransactionSetId)
                    .AddWithValue("@TransactionSetControlNbr", model.TransactionSetControlNbr)
                    .AddWithValue("@TransactionSetPurposeCode", model.TransactionSetPurposeCode)
                    .AddWithValue("@TransactionNbr", model.TransactionNbr)
                    .AddWithValue("@TransactionDate", model.TransactionDate)
                    .AddWithValue("@ReportTypeCode", model.ReportTypeCode)
                    .AddWithValue("@ActionCode", model.ActionCode)
                    .AddWithValue("@TdspDuns", model.TdspDuns)
                    .AddWithValue("@TdspName", model.TdspName)
                    .AddWithValue("@CrDuns", model.CrDuns)
                    .AddWithValue("@CrName", model.CrName)
                    .AddWithValue("@AppAckCode", model.AppAckCode)
                    .AddWithValue("@ReferenceNbr", model.ReferenceNbr)
                    .AddWithValue("@TransactionSetNbr", model.TransactionSetNbr)
                    .AddWithValue("@EsiId", model.EsiId)
                    .AddWithValue("@Direction", true)
                    .AddWithValue("@ProviderID", model.ProviderId)
                    .AddWithValue("@TransactionTypeID", model.TransactionTypeId)
                    .AddWithValue("@MarketID", model.MarketId)
                    .AddWithValue("@CrQualifier", model.CrQualifier)
                    .AddWithValue("@TdspQualifier", model.TdspQualifier)
                    .AddWithValue("@ESPUtilityAccountNumber", model.EspUtilityAccountNumber)
                    .AddWithValue("@ESPCustomerAccountNumber", model.EspCustomerAccountNumber)
                    .AddWithValue("@PreviousUtilityAccountNumber", model.PreviousUtilityAccountNumber)
                    .AddWithValue("@CustomerName", model.CustomerName)
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

        public int InsertReason(Type824Reason model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp824ReasonInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@824_Key", model.HeaderKey)
                    .AddIfNotEmptyOrDbNull("@ReasonCode", model.ReasonCode)
                    .AddIfNotEmptyOrDbNull("@ReasonText", model.ReasonText)
                    .AddOutParameter("@Reason_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var reasonKey = (int)keyParameter.Value;
                model.ReasonKey = reasonKey;

                return reasonKey;
            }
        }

        public int InsertReference(Type824Reference model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp824ReferenceInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@824_Key", model.HeaderKey)
                    .AddWithValue("@AppAckCode", model.AppAckCode)
                    .AddWithValue("@ReferenceQualifier", model.ReferenceQualifier)
                    .AddWithValue("@ReferenceNbr", model.ReferenceNbr)
                    .AddWithValue("@TransactionSetId", model.TransactionSetId)
                    .AddWithValue("@CrossReferenceNbr", model.CrossReferenceNbr)
                    .AddWithValue("@PurchaseOrderNbr", model.PurchaseOrderNbr)
                    .AddWithValue("@PaymentsAppliedThroughDate", model.PaymentsAppliedThroughDate)
                    .AddWithValue("@TotalPaymentsApplied", model.TotalPaymentsApplied)
                    .AddWithValue("@PaymentDueDate", model.PaymentDueDate)
                    .AddWithValue("@TotalAmountDue", model.TotalAmountDue)
                    .AddOutParameter("@Reference_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var referenceKey = (int)keyParameter.Value;
                model.ReferenceKey = referenceKey;

                return referenceKey;
            }
        }

        public int InsertTechError(Type824TechError model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp824TechErrorInsert"))
            {
                command.AddWithValue("@Reference_Key", model.ReferenceKey)
                    .AddIfNotEmptyOrDbNull("@TechErrorCode", model.TechErrorCode)
                    .AddIfNotEmptyOrDbNull("@BadElementCopy", model.BadElementCopy)
                    .AddIfNotEmptyOrDbNull("@TechErrorNote", model.TechErrorNote);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                model.TechErrorKey = 1;
                return 1;
            }
        }
    }
}

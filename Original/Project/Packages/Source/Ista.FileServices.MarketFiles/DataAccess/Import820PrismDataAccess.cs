using System;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class Import820PrismDataAccess : IMarket820Import
    {
        private readonly string connectionString;

        public Import820PrismDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }
        
        public int InsertHeader(Type820Header model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp820HeaderInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@MarketFileId", model.MarketFileId)
                    .AddIfNotEmptyOrDbNull("@TransactionNbr", model.TransactionNbr)
                    .AddIfNotEmptyOrDbNull("@TransactionDate", model.TransactionDate)
                    .AddIfNotEmptyOrDbNull("@TotalAmount", model.TotalAmount)
                    .AddWithValue("CreditDebitFlag", model.CreditDebitFlag)
                    .AddIfNotEmptyOrDbNull("@PaymentMethodCode", model.PaymentMethodCode)
                    .AddIfNotEmptyOrDbNull("@TraceReferenceNbr", model.TraceReferenceNbr)
                    .AddIfNotEmptyOrDbNull("@TdspDuns", model.TdspDuns)
                    .AddIfNotEmptyOrDbNull("@TdspName", model.TdspName)
                    .AddWithValue("@CrDuns", model.CrDuns)
                    .AddWithValue("@CrName", model.CrName)
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

        public int InsertDetail(Type820Detail model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp820DetailInsert"))
            {
                command.AddWithValue("@820_Key", model.HeaderKey)
                    .AddWithValue("@AssignedId", model.AssignedId)
                    .AddWithValue("@ReferenceNbr", model.ReferenceNbr)
                    .AddWithValue("@CrossReferenceNbr", model.CrossReferenceNbr)
                    .AddWithValue("@PaymentActionCode", model.PaymentActionCode)
                    .AddIfNotEmptyOrDbNull("@PaymentAmount", model.PaymentAmount)
                    .AddWithValue("@AdjustmentReasonCode", model.AdjustmentReasonCode)
                    .AddWithValue("@AdjustmentAmount", model.AdjustmentAmount)
                    .AddIfNotEmptyOrDbNull("@EsiId", model.EsiId);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                model.DetailKey = 1;
                return 1;
            }
        }
    }
}

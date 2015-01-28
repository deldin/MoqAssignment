using System;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class ImportCustomerInfoDataAccess : IClientCustomerInfoImport
    {
        private readonly string connectionString;

        public ImportCustomerInfoDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int InsertFile(TypeCustomerInfoFile model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_CustomerBillingFileInsert"))
            {
                SqlParameter keyParameter;

                command
                    .AddWithValue("@CustomerBillingFileNumber", model.FileNumber)
                    .AddIfNotEmptyOrDbNull("@CustomerBillingFileReferenceNumber", model.ReferenceNumber)
                    .AddWithValue("@FileName", model.FileName)
                    .AddWithValue("@CSPDUNSID", model.CspDunsId)
                    .AddWithValue("@StatusID", (int)model.Status)
                    .AddWithValue("@TypeID", model.FileTypeId)
                    .AddWithValue("@DirectionFlag", true)
                    .AddWithValue("@CreateDate", DateTime.Now)
                    .AddWithValue("@StatusDate", model.StatusDate)
                    .AddWithValue("@UserID", model.UserId)
                    .AddOutParameter("@CustomerBillingFileID", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var fileId = (int)keyParameter.Value;
                model.FileId = fileId;

                return fileId;
            }
        }

        public int InsertErrorRecord(TypeCustomerInfoErrorRecord model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_CustomerBillingFileErrorRecordInsert"))
            {
                SqlParameter keyParameter;

                command
                    .AddWithValue("@CustomerBillingFileID", model.FileId)
                    .AddWithValue("@CustomerBillingFileRecordID", model.RecordId)
                    .AddWithValue("@RecordTypeID", model.RecordTypeId)
                    .AddIfNotEmptyOrDbNull("@FieldName", model.FieldName)
                    .AddIfNotEmptyOrDbNull("@ErrorMessage", model.ErrorMessage)
                    .AddWithValue("@ClearedFlag", model.IsCleared)
                    .AddWithValue("@ClearedDate", model.ClearedDate)
                    .AddWithValue("@UserId", model.UserId)
                    .AddIfNotEmptyOrDbNull("@EsiId", model.PremNo)
                    .AddOutParameter("@CustomerBillingFileErrorRecordID", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var errorRecordId = (int)keyParameter.Value;
                model.ErrorRecordId = errorRecordId;

                return errorRecordId;
            }
        }
    }
}
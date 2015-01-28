using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class Export824XmlDataAccess : IMarket824Export
    {
        private readonly string connectionString;

        public Export824XmlDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Type824Header[] ListUnprocessed(string ldcDuns, string duns, int providerId)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_824ExportList"))
            {
                command.AddIfNotEmptyOrDbNull("@CrDuns", duns)
                    .AddIfNotEmptyOrDbNull("@TdspDuns", ldcDuns);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type824Header>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type824Header
                        {
                            HeaderKey = reader.GetInt32("824_Key"),
                            Direction = reader.GetBoolean("Direction"),
                        };

                        reader.TryGetString("TransactionSetId", x => item.TransactionSetId = x);
                        reader.TryGetString("TransactionSetControlNbr", x => item.TransactionSetControlNbr = x);
                        reader.TryGetString("TransactionSetPurposeCode", x => item.TransactionSetPurposeCode = x);
                        reader.TryGetString("TransactionNbr", x => item.TransactionNbr = x);
                        reader.TryGetString("TransactionDate", x => item.TransactionDate = x);
                        reader.TryGetString("ReportTypeCode", x => item.ReportTypeCode = x);
                        reader.TryGetString("ActionCode", x => item.ActionCode = x);
                        reader.TryGetString("TdspDuns", x => item.TdspDuns = x);
                        reader.TryGetString("TdspName", x => item.TdspName = x);
                        reader.TryGetString("CrDuns", x => item.CrDuns = x);
                        reader.TryGetString("CrName", x => item.CrName = x);
                        reader.TryGetString("AppAckCode", x => item.AppAckCode = x);
                        reader.TryGetString("ReferenceNbr", x => item.ReferenceNbr = x);
                        reader.TryGetString("TransactionSetNbr", x => item.TransactionSetNbr = x);
                        reader.TryGetString("EsiId", x => item.EsiId = x);
                        reader.TryGetInt32("ProcessFlag", x => item.ProcessFlag = x);
                        reader.TryGetDateTime("ProcessDate", x => item.ProcessDate = x);
                        reader.TryGetInt32("TransactionTypeID", x => item.TransactionTypeId = x);
                        reader.TryGetInt32("MarketID", x => item.MarketId = x);
                        reader.TryGetInt32("ProviderID", x => item.ProviderId = x);
                        reader.TryGetString("CrQualifier", x => item.CrQualifier = x);
                        reader.TryGetString("TdspQualifier", x => item.TdspQualifier = x);
                        reader.TryGetString("ESPUtilityAccountNumber", x => item.EspUtilityAccountNumber = x);
                        reader.TryGetString("CustomerName", x => item.CustomerName = x);
                        reader.TryGetString("ESPCustomerAccountNumber", x => item.EspCustomerAccountNumber = x);
                        reader.TryGetString("PreviousUtilityAccountNumber", x => item.PreviousUtilityAccountNumber = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type824Reason[] ListReasons(int headerKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_824ReasonList"))
            {
                command.AddWithValue("@HeaderKey", headerKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type824Reason>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type824Reason
                        {
                            HeaderKey = headerKey,
                            ReasonKey = reader.GetInt32("Reason_Key"),
                        };

                        reader.TryGetString("ReasonCode", x => item.ReasonCode = x);
                        reader.TryGetString("ReasonText", x => item.ReasonText = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type824Reference[] ListReferences(int headerKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_824ReferenceList"))
            {
                command.AddWithValue("@HeaderKey", headerKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type824Reference>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type824Reference
                        {
                            HeaderKey = headerKey,
                            ReferenceKey = reader.GetInt32("Reference_Key"),
                        };

                        reader.TryGetString("AppAckCode", x => item.AppAckCode = x);
                        reader.TryGetString("ReferenceQualifier", x => item.ReferenceQualifier = x);
                        reader.TryGetString("ReferenceNbr", x => item.ReferenceNbr = x);
                        reader.TryGetString("TransactionSetId", x => item.TransactionSetId = x);
                        reader.TryGetString("CrossReferenceNbr", x => item.CrossReferenceNbr = x);
                        reader.TryGetString("PurchaseOrderNbr", x => item.PurchaseOrderNbr = x);
                        reader.TryGetString("PaymentsAppliedThroughDate", x => item.PaymentsAppliedThroughDate = x);
                        reader.TryGetString("TotalPaymentsApplied", x => item.TotalPaymentsApplied = x);
                        reader.TryGetString("PaymentDueDate", x => item.PaymentDueDate = x);
                        reader.TryGetString("TotalAmountDue", x => item.TotalAmountDue = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public void UpdateHeader(int headerKey, int marketFileId, string fileName)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_824HeaderStatusUpdate"))
            {
                command.AddWithValue("@HeaderKey", headerKey)
                    .AddWithValue("@Status", 1)
                    .AddWithValue("@MarketFileID", marketFileId);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }
    }
}
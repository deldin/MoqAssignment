using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class Export820XmlDataAccess : IMarket820Export
    {
        private readonly string connectionString;

        public Export820XmlDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Type820Header[] ListUnprocessed(string ldcDuns, string duns, int providerId)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_820ExportList"))
            {
                command.AddIfNotEmptyOrDbNull("@CrDuns", duns)
                    .AddIfNotEmptyOrDbNull("@TdspDuns", ldcDuns);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type820Header>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type820Header
                        {
                            HeaderKey = reader.GetInt32("820_Key"),
                            ProcessFlag = reader.GetBoolean("ProcessFlag"),
                            Direction = reader.GetBoolean("Direction"),
                        };

                        reader.TryGetInt32("MarketFileId", x => item.MarketFileId = x);
                        reader.TryGetString("TransactionSetId", x => item.TransactionSetId = x);
                        reader.TryGetString("TransactionSetControlNbr", x => item.TransactionSetControlNbr = x);
                        reader.TryGetString("TransactionTypeCode", x => item.TransactionTypeCode = x);
                        reader.TryGetString("TransactionNbr", x => item.TransactionNbr = x);
                        reader.TryGetString("TransactionDate", x => item.TransactionDate = x);
                        reader.TryGetString("TotalAmount", x => item.TotalAmount = x);
                        reader.TryGetString("CreditDebitFlag", x => item.CreditDebitFlag = x);
                        reader.TryGetString("PaymentMethodCode", x => item.PaymentMethodCode = x);
                        reader.TryGetString("TraceReferenceNbr", x => item.TraceReferenceNbr = x);
                        reader.TryGetString("TdspDunsStructureCode", x => item.TdspDunsStructureCode = x);
                        reader.TryGetString("TdspDuns", x => item.TdspDuns = x);
                        reader.TryGetString("TdspName", x => item.TdspName = x);
                        reader.TryGetString("CrDuns", x => item.CrDuns = x);
                        reader.TryGetString("CrName", x => item.CrName = x);
                        reader.TryGetDateTime("ProcessDate", x => item.ProcessDate = x);
                        reader.TryGetInt32("TransactionTypeID", x => item.TransactionTypeId = x);
                        reader.TryGetInt32("MarketID", x => item.MarketId = x);
                        reader.TryGetInt32("ProviderID", x => item.ProviderId = x);
                        reader.TryGetString("ESPUtilityAccountNumber", x => item.ESPUtilityAccountNumber = x);
                        reader.TryGetString("CreateDate", x => item.CreateDate = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type820Detail[] ListDetails(int headerKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_820DetailList"))
            {
                command.AddWithValue("@HeaderKey", headerKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type820Detail>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type820Detail
                        {
                            HeaderKey = headerKey,
                            DetailKey = reader.GetInt32("Detail_Key"),
                            ProcessFlag = reader.GetInt32("ProcessFlag"),
                        };

                        reader.TryGetString("AssignedId", x => item.AssignedId = x);
                        reader.TryGetString("ReferenceId", x => item.ReferenceId = x);
                        reader.TryGetString("ReferenceNbr", x => item.ReferenceNbr = x);
                        reader.TryGetString("CrossReferenceNbr", x => item.CrossReferenceNbr = x);
                        reader.TryGetString("PaymentActionCode", x => item.PaymentActionCode = x);
                        reader.TryGetString("PaymentAmount", x => item.PaymentAmount = x);
                        reader.TryGetString("AdjustmentReasonCode", x => item.AdjustmentReasonCode = x);
                        reader.TryGetString("AdjustmentAmount", x => item.AdjustmentAmount = x);
                        reader.TryGetString("EsiId", x => item.EsiId = x);
                        reader.TryGetDateTime("ProcessDate", x => item.ProcessDate = x);
                        reader.TryGetString("CommodityCode", x => item.CommodityCode = x);
                        reader.TryGetString("InvoiceAmount", x => item.InvoiceAmount = x);
                        reader.TryGetString("DiscountAmount", x => item.DiscountAmount = x);
                        reader.TryGetString("PrevUtilityAccountNumber", x => item.PrevUtilityAccountNumber = x);
                        reader.TryGetString("ESPAccountNumber", x => item.ESPAccountNumber = x);
                        reader.TryGetString("CustomerName", x => item.CustomerName = x);
                        reader.TryGetString("DatePosted", x => item.DatePosted = x);
                        reader.TryGetString("UnmeteredServiceDesignator", x => item.UnmeteredServiceDesignator = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public void UpdateHeader(int headerKey, int marketFileId, string fileName)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_820HeaderStatusUpdate"))
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
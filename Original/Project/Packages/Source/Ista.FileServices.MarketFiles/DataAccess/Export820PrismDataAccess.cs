using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class Export820PrismDataAccess : IMarket820Export
    {
        private readonly string connectionString;

        public Export820PrismDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Type820Header[] ListUnprocessed(string ldcDuns, string duns, int providerId)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp820ExportList"))
            {
                command.AddIfNotEmptyOrDbNull("@TDSPDuns", ldcDuns)
                    .AddIfNotEmptyOrDbNull("@CrDuns", duns)
                    .AddWithValue("@ProviderID", providerId);

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

                        reader.TryGetString("TransactionTypeCode", x => item.TransactionTypeCode = x);
                        reader.TryGetString("TransactionNbr", x => item.TransactionNbr = x);
                        reader.TryGetString("TransactionDate", x => item.TransactionDate = x);
                        reader.TryGetString("TotalAmount", x => item.TotalAmount = x);
                        reader.TryGetString("CreditDebitFlag", x => item.CreditDebitFlag = x);
                        reader.TryGetString("PaymentMethodCode", x => item.PaymentMethodCode = x);
                        reader.TryGetString("TraceReferenceNbr", x => item.TraceReferenceNbr = x);
                        reader.TryGetString("TdspDuns", x => item.TdspDuns = x);
                        reader.TryGetString("TdspName", x => item.TdspName = x);
                        reader.TryGetString("CrDuns", x => item.CrDuns = x);
                        reader.TryGetString("CrName", x => item.CrName = x);
                        reader.TryGetInt32("ProviderID", x => item.ProviderId = x);
                        reader.TryGetInt32("MarketID", x => item.MarketId = x);
                        reader.TryGetInt32("TransactionTypeID", x => item.TransactionTypeId = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type820Detail[] ListDetails(int headerKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp820ExportListDetailRecords"))
            {
                command.AddWithValue("@820Key", headerKey);

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
                        };

                        reader.TryGetString("ReferenceNbr", x => item.ReferenceNbr = x);
                        reader.TryGetString("CrossReferenceNbr", x => item.CrossReferenceNbr = x);
                        reader.TryGetString("PaymentAmount", x => item.PaymentAmount = x);
                        reader.TryGetString("EsiId", x => item.EsiId = x);
                        
                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public void UpdateHeader(int headerKey, int marketFileId, string fileName)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp820ExportHeaderUpdate"))
            {
                command.AddWithValue("@820Key", headerKey)
                    .AddWithValue("@FileName", fileName);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }
    }
}

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class Export824PrismDataAccess : IMarket824Export
    {
        private readonly string connectionString;

        public Export824PrismDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Type824Header[] ListUnprocessed(string ldcDuns, string duns, int providerId)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp824ExportList"))
            {
                command.AddIfNotEmptyOrDbNull("@TDSPDuns", ldcDuns)
                    .AddIfNotEmptyOrDbNull("@CrDuns", duns)
                    .AddWithValue("@ProviderID", providerId);

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

                        reader.TryGetString("TransactionDate", x => item.TransactionDate = x);
                        reader.TryGetString("TransactionNbr", x => item.TransactionNbr = x);
                        reader.TryGetString("ActionCode", x => item.ActionCode = x);
                        reader.TryGetString("TdspDuns", x => item.TdspDuns = x);
                        reader.TryGetString("TdspName", x => item.TdspName = x);
                        reader.TryGetString("CrDuns", x => item.CrDuns = x);
                        reader.TryGetString("CrName", x => item.CrName = x);
                        reader.TryGetString("AppAckCode", x => item.AppAckCode = x);
                        reader.TryGetString("ReferenceNbr", x => item.ReferenceNbr = x);
                        reader.TryGetString("TransactionSetNbr", x => item.TransactionSetNbr = x);
                        reader.TryGetString("EsiId", x => item.EsiId = x);
                        reader.TryGetString("TransactionSetPurposeCode", x => item.TransactionSetPurposeCode = x);
                        
                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type824Reason[] ListReasons(int headerKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp824ExportReason"))
            {
                command.AddWithValue("@824Key", headerKey);

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
            return new Type824Reference[0];
        }

        public void UpdateHeader(int headerKey, int marketFileId, string fileName)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp824ExportHeaderUpdate"))
            {
                command.AddWithValue("@824Key", headerKey)
                    .AddWithValue("@FileName", fileName);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }
    }
}

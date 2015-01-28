using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class Export650PrismDataAccess : IMarket650Export
    {
        private readonly string connectionString;

        public Export650PrismDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Type650Header[] ListUnprocessed(string ldcDuns, string duns, int providerId)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp650ExportList"))
            {
                command.AddWithValue("@TDSPDuns", ldcDuns)
                    .AddWithValue("@CrDuns", duns)
                    .AddWithValue("@ProviderID", providerId);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type650Header>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type650Header
                        {
                            HeaderKey = reader.GetInt32("650_Key"),
                            Direction = reader.GetBoolean("Direction"),
                            ProcessFlag = reader.GetInt16("ProcessFlag"),
                        };

                        reader.TryGetString("TransactionDate", x => item.TransactionDate = x);
                        reader.TryGetString("TransactionSetPurposeCode", x => item.TransactionSetPurposeCode = x);
                        reader.TryGetString("TransactionNbr", x => item.TransactionNbr = x);
                        reader.TryGetString("ReferenceNbr", x => item.ReferenceNbr = x);
                        reader.TryGetString("TransactionType", x => item.TransactionType = x);
                        reader.TryGetString("ActionCode", x => item.ActionCode = x);
                        reader.TryGetString("TdspName", x => item.TdspName = x);
                        reader.TryGetString("TdspDuns", x => item.TdspDuns = x);
                        reader.TryGetString("CrName", x => item.CrName = x);
                        reader.TryGetString("CrDuns", x => item.CrDuns = x);
                        reader.TryGetString("ProcessedReceivedDateTime", x => item.ProcessedReceivedDateTime = x);
                        reader.TryGetInt32("MarketID", x => item.MarketId = x);
                        reader.TryGetInt32("TransactionTypeID", x => item.TransactionTypeId = x);
                        reader.TryGetInt32("ProviderID", x => item.ProviderId = x);

                        // this is a little concerning since this stored procedure
                        // actually returns a name record as well
                        var name = new Type650Name { HeaderKey = reader.GetInt32("650_Key") };
                        reader.TryGetString("Customer", x => name.EntityName = x);
                        reader.TryGetString("CustomerName1", x => name.EntityName2 = x);
                        reader.TryGetString("CustomerName2", x => name.EntityName3 = x);
                        reader.TryGetString("Address1", x => name.Address1 = x);
                        reader.TryGetString("Address2", x => name.Address2 = x);
                        reader.TryGetString("City", x => name.City = x);
                        reader.TryGetString("State", x => name.State = x);
                        reader.TryGetString("PostalCode", x => name.PostalCode = x);
                        reader.TryGetString("TechnicalContact", x => name.ContactName = x);
                        reader.TryGetString("ContactPhoneNbr1", x => name.ContactPhoneNbr1 = x);
                        reader.TryGetString("ContactPhoneNbr2", x => name.ContactPhoneNbr2 = x);
                        item.AddName(name);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        /// <summary>
        /// This is actually taken from the Xml data access approach.
        /// The Prism data access does not have this method available.
        /// </summary>
        /// <remarks>
        /// It is possible to avoid this call if the Header export
        /// is taken on good faith as it returns a single Name record
        /// if the EntityIdType is '8R'
        /// </remarks>
        /// <param name="headerKey"></param>
        /// <returns></returns>
        public Type650Name[] ListNames(int headerKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_650NameList"))
            {
                command.AddWithValue("@650_Key", headerKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type650Name>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type650Name
                        {
                            HeaderKey = headerKey,
                            NameKey = reader.GetInt32("Name_Key"),
                        };

                        reader.TryGetString("EntityIdType", x => item.EntityIdType = x);
                        reader.TryGetString("EntityName", x => item.EntityName = x);
                        reader.TryGetString("EntityName2", x => item.EntityName2 = x);
                        reader.TryGetString("EntityName3", x => item.EntityName3 = x);
                        reader.TryGetString("EntityDuns", x => item.EntityDuns = x);
                        reader.TryGetString("EntityIdCode", x => item.EntityIdCode = x);
                        reader.TryGetString("Address1", x => item.Address1 = x);
                        reader.TryGetString("Address2", x => item.Address2 = x);
                        reader.TryGetString("City", x => item.City = x);
                        reader.TryGetString("State", x => item.State = x);
                        reader.TryGetString("PostalCode", x => item.PostalCode = x);
                        reader.TryGetString("CountryCode", x => item.CountryCode = x);
                        reader.TryGetString("ContactCode", x => item.ContactCode = x);
                        reader.TryGetString("ContactName", x => item.ContactName = x);
                        reader.TryGetString("ContactPhoneNbr1", x => item.ContactPhoneNbr1 = x);
                        reader.TryGetString("ContactPhoneNbr2", x => item.ContactPhoneNbr2 = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type650Service[] ListServices(int headerKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp650ExportListServiceRecords"))
            {
                command.AddWithValue("@650Key", headerKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type650Service>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type650Service
                        {
                            HeaderKey = headerKey,
                            ServiceKey = reader.GetInt32("Service_Key"),
                        };

                        reader.TryGetString("PurposeCode", x => item.PurposeCode = x);
                        reader.TryGetString("PriorityCode", x => item.PriorityCode = x);
                        reader.TryGetString("ESIID", x => item.EsiId = x);
                        reader.TryGetString("SpecialProcessCode", x => item.SpecialProcessCode = x);
                        reader.TryGetString("ServiceReqDate", x => item.ServiceReqDate = x);
                        reader.TryGetString("NotBeforeDate", x => item.NotBeforeDate = x);
                        reader.TryGetString("CallAhead", x => item.CallAhead = x);
                        reader.TryGetString("PremLocation", x => item.PremLocation = x);
                        reader.TryGetString("ReportRemarks", x => item.ReportRemarks = x);
                        reader.TryGetString("Directions", x => item.Directions = x);
                        reader.TryGetString("MeterNbr", x => item.MeterNbr = x);
                        reader.TryGetString("Membership", x => item.Membership = x);
                        reader.TryGetString("RemarksPermanentSuspend", x => item.RemarksPermanentSuspend = x);
                        reader.TryGetString("DisconnectAuthorization", x => item.DisconnectAuthorization = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type650ServicePole[] ListServicePoles(int serviceKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp650ExportListServicePoleRecords"))
            {
                command.AddWithValue("Service_Key", serviceKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type650ServicePole>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type650ServicePole
                        {
                            ServiceKey = serviceKey,
                            ServicePoleKey = reader.GetInt32("Pole_Key"),
                        };

                        reader.TryGetString("PoleNbr", x => item.PoleNbr = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type650ServiceChange[] ListServiceChanges(int serviceKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp650ExportListServiceChangeRecords"))
            {
                command.AddWithValue("@Service_Key", serviceKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type650ServiceChange>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type650ServiceChange
                        {
                            ServiceKey = serviceKey,
                            ServiceChangeKey = reader.GetInt32("ServiceChange_Key"),
                        };

                        reader.TryGetString("ChangeReason", x => item.ChangeReason = x);
                        
                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type650ServiceReject[] ListServiceRejects(int serviceKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp650ExportListServiceRejectRecords"))
            {
                command.AddWithValue("@Service_Key", serviceKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type650ServiceReject>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type650ServiceReject
                        {
                            ServiceKey = serviceKey,
                            ServiceRejectKey = -1,
                        };

                        reader.TryGetString("RejectCode", x => item.RejectCode = x);
                        reader.TryGetString("RejectReason", x => item.RejectReason = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type650ServiceMeter[] ListServiceMeters(int serviceKey)
        {
            return new Type650ServiceMeter[0];
        }

        public void UpdateHeader(int headerKey, int marketFileId, string fileName)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp650ExportHeaderUpdate"))
            {
                command.AddWithValue("@650Key", headerKey)
                    .AddWithValue("@FileName", fileName);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }
    }
}

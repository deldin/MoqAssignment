using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class MarketDataAccess : IMarketDataAccess, IMarketFile
    {
        private readonly string connectionString;

        public MarketDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int IdentifyTransactionType(string actionCode, string serviceActionCode)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_TransactionTypeLoad"))
            {
                SqlParameter outputParameter;

                command.AddWithValue("@TransType", "814")
                    .AddWithValue("@ActionCode", actionCode)
                    .AddWithValue("@ServiceActionCode", serviceActionCode)
                    .AddOutParameter("@TransactionTypeID", SqlDbType.Int, out outputParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (outputParameter.Value == null)
                    return 0;

                return (int)outputParameter.Value;
            }
        }

        public MarketFileModel[] ListEncryptedOutboundMarketFiles()
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp_MarketFileList"))
            {
                command
                    .AddWithValue("@Status", (int)MarketFileStatusOptions.Encrypted)
                    .AddWithValue("@DirectionFlag", false);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<MarketFileModel>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new MarketFileModel
                        {
                            DirectionFlag = false,
                            MarketFileId = reader.GetInt32("MarketFileId"),
                            Status = (MarketFileStatusOptions)reader.GetInt16("Status"),
                        };

                        reader.TryGetString("FileName", x => item.FileName = x);
                        reader.TryGetString("FileType", x => item.FileType = x);
                        reader.TryGetString("ProcessStatus", x => item.ProcessStatus = x);
                        reader.TryGetDateTime("ProcessDate", x => item.ProcessDate = x);
                        reader.TryGetString("ProcessError", x => item.ProcessError = x);
                        reader.TryGetString("SenderTranNum", x => item.SenderTranNum = x);
                        reader.TryGetInt32("LDCID", x => item.LdcId = x);
                        reader.TryGetInt32("CSPDUNSID", x => item.CspDunsId = x);
                        reader.TryGetInt32("RefMarketFileId", x => item.RefMarketFileId = x);
                        reader.TryGetDateTime("CreateDate", x => item.CreateDate = x);
                        reader.TryGetInt32("CspDunsTradingPartnerID", x => item.CspDunsTradingPartnerId = x);
                        reader.TryGetInt32("TransactionCount", x => item.TransactionCount = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public MarketFileModel[] ListInsertedOutboundMarketFiles()
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp_MarketFileList"))
            {
                command
                    .AddWithValue("@Status", (int)MarketFileStatusOptions.Inserted)
                    .AddWithValue("@DirectionFlag", false);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<MarketFileModel>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new MarketFileModel
                        {
                            DirectionFlag = false,
                            MarketFileId = reader.GetInt32("MarketFileId"),
                            Status = (MarketFileStatusOptions)reader.GetInt16("Status"),
                        };

                        reader.TryGetString("FileName", x => item.FileName = x);
                        reader.TryGetString("FileType", x => item.FileType = x);
                        reader.TryGetString("ProcessStatus", x => item.ProcessStatus = x);
                        reader.TryGetDateTime("ProcessDate", x => item.ProcessDate = x);
                        reader.TryGetString("ProcessError", x => item.ProcessError = x);
                        reader.TryGetString("SenderTranNum", x => item.SenderTranNum = x);
                        reader.TryGetInt32("LDCID", x => item.LdcId = x);
                        reader.TryGetInt32("CSPDUNSID", x => item.CspDunsId = x);
                        reader.TryGetInt32("RefMarketFileId", x => item.RefMarketFileId = x);
                        reader.TryGetDateTime("CreateDate", x => item.CreateDate = x);
                        reader.TryGetInt32("CspDunsTradingPartnerID", x => item.CspDunsTradingPartnerId = x);
                        reader.TryGetInt32("TransactionCount", x => item.TransactionCount = x);
                        
                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public CspDunsTradingPartnerModel LoadCspDunsTradingPartner(string cspDuns, string ldcDuns)
        {
            if (string.IsNullOrWhiteSpace(cspDuns))
                throw new ArgumentNullException("cspDuns");

            if (string.IsNullOrWhiteSpace(ldcDuns))
                throw new ArgumentNullException("ldcDuns");

            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_CspDunsTradingPartnerLoadByRelationship"))
            {
                command
                    .AddWithValue("@CspDuns", cspDuns)
                    .AddWithValue("@TradingPartnerDuns", ldcDuns);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    var model = new CspDunsTradingPartnerModel
                    {
                        CspDunsTradingPartnerId = reader.GetInt32("CspDunsTradingPartnerID"),
                        CspTradingPartnerId = reader.GetInt32("CspTradingPartnerID"),
                        CspDuns = reader.GetString("CspDuns"),
                        CspName = reader.GetString("CspName"),
                        CspShortName = reader.GetString("CspShortName"),
                        TradingPartnerId = reader.GetInt32("TradingPartnerID"),
                        TradingPartnerDuns = reader.GetString("TradingPartnerDuns"),
                        TradingPartnerName = reader.GetString("TradingPartnerName"),
                        TradingPartnerShortName = reader.GetString("TradingPartnerShortName"),
                    };

                    return model;
                }
            }
        }

        public void LoadCspDunsTradingPartnerConfig(CspDunsTradingPartnerModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_CspDunsTradingPartnerConfigList"))
            {
                command.AddWithValue("@CspDunsTradingPartnerID", model.CspDunsTradingPartnerId);
                    
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString("Artifact");
                        
                        // at some point, the Value column was
                        // made to be nullable causing the original
                        // approach to fail
                        var value = string.Empty;
                        reader.TryGetString("Value", x => value = x);
                        
                        model.AddConfig(name, value);
                    }
                }
            }
        }

        public bool MarketFileExists(string fileName)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp_MarketFileExists"))
            {
                command.AddWithValue("@MarketFileName", fileName)
                    .AddWithValue("@DirectionFlag", true);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var result = command.ExecuteScalar();
                if (result == null)
                    throw new Exception();

                var count = (int)result;
                return (count > 0);
            }
        }

        public int InsertMarketFile(MarketFileModel model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp_MarketFileInsert"))
            {
                SqlParameter keyParameter;

                command.AddOutParameter("@MarketFileId", SqlDbType.Int, out keyParameter)
                    .AddIfNotEmptyOrDbNull("@FileName", model.FileName)
                    .AddIfNotEmptyOrDbNull("@FileType", model.FileType)
                    .AddWithValue("@ProcessStatus", model.ProcessStatus)
                    .AddIfNotEmptyOrDbNull("@ProcessError", model.ProcessError)
                    .AddWithValue("@ProcessDate", model.ProcessDate ?? DateTime.Now)
                    .AddIfNotEmptyOrDbNull("@SenderTranNum", model.SenderTranNum)
                    .AddWithValue("@DirectionFlag", model.DirectionFlag)
                    .AddWithValue("@Status", (short)model.Status)
                    .AddWithValue("@LDCID", model.LdcId ?? 0)
                    .AddWithValue("@CSPDUNSID", model.CspDunsId ?? 0)
                    .AddWithValueOrDbNull("@CspDunsTradingPartnerID", model.CspDunsTradingPartnerId)
                    .AddWithValueOrDbNull("@RefMarketFileId", model.RefMarketFileId)
                    .AddWithValueOrDbNull("@TransactionCount", model.TransactionCount);
                
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                return (int)keyParameter.Value;
            }
        }

        public void InsertAuditRecord(int marketFileId, int auditCount, int actualCount)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("cspMarketFileAuditInsert"))
            {
                command.AddWithValue("@MarketFileID", marketFileId)
                    .AddWithValue("@RecordCountAudit", auditCount)
                    .AddWithValue("@RecordCountActual", actualCount);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void UpdateMarketFile(MarketFileModel model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp_MarketFileUpdate"))
            {
                command.AddWithValue("@MarketFileId", model.MarketFileId)
                    .AddIfNotEmptyOrDbNull("@FileName", model.FileName)
                    .AddIfNotEmptyOrDbNull("@FileType", model.FileType)
                    .AddIfNotEmptyOrDbNull("@ProcessStatus", model.ProcessStatus)
                    .AddIfNotEmptyOrDbNull("@ProcessError", model.ProcessError)
                    .AddWithValue("@ProcessDate", model.ProcessDate ?? DateTime.Now)
                    .AddIfNotEmptyOrDbNull("@SenderTranNum", model.SenderTranNum)
                    .AddWithValue("@DirectionFlag", model.DirectionFlag)
                    .AddWithValue("@Status", (short)model.Status)
                    .AddWithValue("@LDCID", model.LdcId ?? 0)
                    .AddWithValue("@CSPDUNSID", model.CspDunsId ?? 0)
                    .AddWithValueOrDbNull("@CspDunsTradingPartnerID", model.CspDunsTradingPartnerId)
                    .AddWithValueOrDbNull("@RefMarketFileId", model.RefMarketFileId)
                    .AddWithValueOrDbNull("@TransactionCount", model.TransactionCount);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public MarketFileModel LoadOutboundMarketFileByName(string fileName)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp_MarketFileLoadByFileName"))
            {
                command
                    .AddWithValue("@FileName", fileName)
                    .AddWithValue("@DirectionFlag", false);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var item = new MarketFileModel
                        {
                            DirectionFlag = false,
                            MarketFileId = reader.GetInt32("MarketFileId"),
                            Status = (MarketFileStatusOptions)reader.GetInt16("Status"),
                        };

                        reader.TryGetString("FileName", x => item.FileName = x);
                        reader.TryGetString("FileType", x => item.FileType = x);
                        reader.TryGetString("ProcessStatus", x => item.ProcessStatus = x);
                        reader.TryGetDateTime("ProcessDate", x => item.ProcessDate = x);
                        reader.TryGetString("ProcessError", x => item.ProcessError = x);
                        reader.TryGetString("SenderTranNum", x => item.SenderTranNum = x);
                        reader.TryGetInt32("LDCID", x => item.LdcId = x);
                        reader.TryGetInt32("CSPDUNSID", x => item.CspDunsId = x);
                        reader.TryGetInt32("RefMarketFileId", x => item.RefMarketFileId = x);
                        reader.TryGetDateTime("CreateDate", x => item.CreateDate = x);
                        reader.TryGetInt32("CspDunsTradingPartnerID", x => item.CspDunsTradingPartnerId = x);
                        reader.TryGetInt32("TransactionCount", x => item.TransactionCount = x);

                        return item;
                    }
                }
            }

            return null;
        }
    }
}
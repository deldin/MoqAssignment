using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class AdminDataAccess : IAdminDataAccess
    {
        private readonly string connectionString;

        public AdminDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public ExportConfigurationModel LoadExportConfiguration(int clientId)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp_GlobalApplicationConfigurationLoad"))
            {
                command.AddWithValue("@ClientID", clientId)
                    .AddWithValue("@MachineName", Environment.MachineName);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    var model = new ExportConfigurationModel
                    {
                        ClientId = clientId,
                        Client = reader.GetString("ClientAbbreviation"),
                        ClientConnectionString = reader.GetString("ConnectionCsr"),
                        MarketConnectionString = reader.GetString("ConnectionMarket"),
                        DirectoryEncrypted = reader.GetString("ExportDirectoryEncrypted"),
                        DirectoryDecrypted = reader.GetString("ExportDirectoryDecrypted"),
                        DirectoryException = reader.GetString("ExportDirectoryException"),
                        DirectoryArchive = reader.GetString("ExportDirectoryArchive"),
                    };

                    reader.TryGetString("ExportDirectoryFtpOut", x => model.FtpDirectory = x);
                    reader.TryGetString("ExportFtpRemoteDirectory", x => model.FtpRemoteDirectory = x);
                    reader.TryGetString("FtpRemoteServer", x => model.FtpRemoteServer = x);
                    reader.TryGetString("FtpUserName", x => model.FtpUsername = x);
                    reader.TryGetString("FtpUserPassword", x => model.FtpPassword = x);
                    reader.TryGetString("PgpPassPhrase", x => model.PgpPassPhrase = x);
                    reader.TryGetString("PgpEncryptionKey", x => model.PgpEncryptionKey = x);
                    reader.TryGetString("PgpSignatureKey", x => model.PgpSignatureKey = x);

                    return model;
                }
            }
        }

        public ImportConfigurationModel LoadImportConfiguration(int clientId)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp_GlobalApplicationConfigurationLoad"))
            {
                command.AddWithValue("@ClientID", clientId)
                    .AddWithValue("@MachineName", Environment.MachineName);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    var model = new ImportConfigurationModel
                    {
                        ClientId = clientId,
                        Client = reader.GetString("ClientAbbreviation"),
                        ClientConnectionString = reader.GetString("ConnectionCsr"),
                        MarketConnectionString = reader.GetString("ConnectionMarket"),
                        DirectoryEncrypted = reader.GetString("ImportDirectoryEncrypted"),
                        DirectoryDecrypted = reader.GetString("ImportDirectoryDecrypted"),
                        DirectoryException = reader.GetString("ImportDirectoryException"),
                        DirectoryArchive = reader.GetString("ImportDirectoryArchive"),
                    };

                    reader.TryGetString("ImportDirectoryFtpIn", x => model.FtpDirectory = x);
                    reader.TryGetString("ImportFtpRemoteDirectory", x => model.FtpRemoteDirectory = x);
                    reader.TryGetString("FtpRemoteServer", x => model.FtpRemoteServer = x);
                    reader.TryGetString("FtpUserName", x => model.FtpUsername = x);
                    reader.TryGetString("FtpUserPassword", x => model.FtpPassword = x);
                    reader.TryGetString("PgpPassPhrase", x => model.PgpPassPhrase = x);
                    reader.TryGetString("PgpEncryptionKey", x => model.PgpEncryptionKey = x);
                    reader.TryGetString("PgpSignatureKey", x => model.PgpSignatureKey = x);

                    return model;
                }
            }
        }

        public ProviderModel[] ListProviders(int clientId)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp_ProviderListByClient"))
            {
                command.AddWithValue("@ClientID", clientId);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<ProviderModel>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new ProviderModel
                        {
                            ProviderId = reader.GetInt32("ProviderID"),
                            ProviderName = reader.GetString("ProviderName"),
                        };

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }
    }
}

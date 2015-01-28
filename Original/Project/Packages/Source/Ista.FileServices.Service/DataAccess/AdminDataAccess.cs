using System;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.Service.Interfaces;
using Ista.FileServices.Service.Models;
using Ista.Miramar.Interfaces;

namespace Ista.FileServices.Service.DataAccess
{
    public class AdminDataAccess : IAdminDataAccess
    {
        private readonly string connectionString;

        public AdminDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IMiramarClientInfo LoadClientInfo(int clientId)
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

                    var model = new ClientInfoModel
                    {
                        ClientId = clientId,
                        AdminConnection = connectionString,
                        Client = reader.GetString("ClientAbbreviation"),
                        ClientConnection = reader.GetString("ConnectionCsr"),
                        MarketConnection = reader.GetString("ConnectionMarket"),
                    };

                    return model;
                }
            }
        }
    }
}

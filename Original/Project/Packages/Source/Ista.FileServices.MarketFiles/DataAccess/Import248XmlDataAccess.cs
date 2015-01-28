using System;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class Import248XmlDataAccess : IMarket248Import
    {
        private readonly string connectionString;

        public Import248XmlDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int InsertHeader(Type248Header model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp248HeaderInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@MarketFileId", model.MarketFileId)
                    .AddIfNotEmptyOrDbNull("@ControlNbr", model.ControlNbr)
                    .AddIfNotEmptyOrDbNull("@StructureCode", model.StructureCode)
                    .AddIfNotEmptyOrDbNull("@TransactionSetPurposeCode", model.TransactionSetPurposeCode)
                    .AddIfNotEmptyOrDbNull("@TransactionReferenceNbr", model.TransactionReferenceNbr)
                    .AddIfNotEmptyOrDbNull("@TransactionDate", model.TransactionDate)
                    .AddIfNotEmptyOrDbNull("@TransactionTypeCode", model.TransactionTypeCode)
                    .AddIfNotEmptyOrDbNull("@CrDuns", model.CrDuns)
                    .AddIfNotEmptyOrDbNull("@CrName", model.CrName)
                    .AddIfNotEmptyOrDbNull("@LDCDuns", model.LDCDuns)
                    .AddIfNotEmptyOrDbNull("@LDCName", model.LDCName)
                    .AddIfNotEmptyOrDbNull("@SegmentCount", model.SegmentCount)
                    .AddWithValue("@Direction", true)
                    .AddWithValue("@ProcessFlag", DBNull.Value)
                    .AddWithValue("@ProcessDate", DBNull.Value)
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

        public int InsertDetail(Type248Detail model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp248DetailInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@248_Key", model.HeaderKey)
                    .AddIfNotEmptyOrDbNull("@HierarchicalID", model.HierarchicalID)
                    .AddIfNotEmptyOrDbNull("@HierarchicalLevelCode", model.HierarchicalLevelCode)
                    .AddIfNotEmptyOrDbNull("@CustomerName", model.CustomerName)
                    .AddIfNotEmptyOrDbNull("@ESPAccountNumber", model.ESPAccountNbr)
                    .AddIfNotEmptyOrDbNull("@OldLdcAccountNumber", model.OldLdcAccountNbr)
                    .AddIfNotEmptyOrDbNull("@WriteOffAccountNbr", model.WriteOffAccountNbr)
                    .AddIfNotEmptyOrDbNull("@MarketerCustomerAccountNumber", model.MarketerCustomerAccountNumber)
                    .AddIfNotEmptyOrDbNull("@ServiceTypeCode", model.ServiceTypeCode)
                    .AddIfNotEmptyOrDbNull("@CustomerTelephone1", model.CustomerTelephone1)
                    .AddIfNotEmptyOrDbNull("@CustomerTelephone2", model.CustomerTelephone2)
                    .AddIfNotEmptyOrDbNull("@BalanceAmount", model.BalanceAmount)
                    .AddIfNotEmptyOrDbNull("@WriteOffDate", model.WriteOffDate)
                    .AddIfNotEmptyOrDbNull("@ReinstatementDate", model.ReinstatementDate)
                    .AddIfNotEmptyOrDbNull("@InvoiceDate", model.InvoiceDate)
                    .AddIfNotEmptyOrDbNull("@InvoiceAmount", model.InvoiceAmount)
                    .AddIfNotEmptyOrDbNull("@InvoiceNbr", model.InvoiceNbr)
                    .AddOutParameter("@Detail_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var detailKey = (int)keyParameter.Value;
                model.DetailKey = detailKey;

                return detailKey;
            }
        }
    }
}

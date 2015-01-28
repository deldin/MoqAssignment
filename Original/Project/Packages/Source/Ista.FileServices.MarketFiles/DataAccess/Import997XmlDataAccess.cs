using System;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class Import997XmlDataAccess : IMarket997Import
    {
        private readonly string connectionString;

        public Import997XmlDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }
        
        public int InsertHeader(Type997Header model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp997HeaderInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("MarketFileId", model.MarketFileId)
                    .AddIfNotEmptyOrDbNull("@FunctionalGroup", model.FunctionalGroup)
                    .AddIfNotEmptyOrDbNull("@TransactionNbr", model.TransactionNbr)
                    .AddIfNotEmptyOrDbNull("@AcknowledgeCode", model.AcknowledgeCode)
                    .AddIfNotEmptyOrDbNull("@TransactionSetsIncluded", model.TransactionSetsIncluded)
                    .AddIfNotEmptyOrDbNull("@TransactionSetsReceived", model.TransactionSetsReceived)
                    .AddIfNotEmptyOrDbNull("@TransactionSetsAccepted", model.TransactionSetsAccepted)
                    .AddIfNotEmptyOrDbNull("@SyntaxErrorCode1", model.SyntaxErrorCode1)
                    .AddIfNotEmptyOrDbNull("@SyntaxErrorCode2", model.SyntaxErrorCode2)
                    .AddIfNotEmptyOrDbNull("@SyntaxErrorCode3", model.SyntaxErrorCode3)
                    .AddIfNotEmptyOrDbNull("@SyntaxErrorCode4", model.SyntaxErrorCode4)
                    .AddIfNotEmptyOrDbNull("@SyntaxErrorCode5", model.SyntaxErrorCode5)
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

        public int InsertResponse(Type997Response model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp997ResponseInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@997_Key", model.HeaderKey)
                    .AddIfNotEmptyOrDbNull("@IdentifierCode", model.IdentifierCode)
                    .AddIfNotEmptyOrDbNull("@ControlNbr", model.ControlNbr)
                    .AddIfNotEmptyOrDbNull("@AcknowledgementCode", model.AcknowledgementCode)
                    .AddIfNotEmptyOrDbNull("@SyntaxErrorCode1", model.SyntaxErrorCode1)
                    .AddIfNotEmptyOrDbNull("@SyntaxErrorCode2", model.SyntaxErrorCode2)
                    .AddIfNotEmptyOrDbNull("@SyntaxErrorCode3", model.SyntaxErrorCode3)
                    .AddIfNotEmptyOrDbNull("@SyntaxErrorCode4", model.SyntaxErrorCode4)
                    .AddIfNotEmptyOrDbNull("@SyntaxErrorCode5", model.SyntaxErrorCode5)
                    .AddOutParameter("@Response_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var responseKey = (int)keyParameter.Value;
                model.ResponseKey = responseKey;

                return responseKey;
            }
        }

        public int InsertResponseNote(Type997ResponseNote model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp997ResponseNoteInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@Response_Key", model.ResponseKey)
                    .AddIfNotEmptyOrDbNull("@SegmentIdCode", model.SegmentIdCode)
                    .AddIfNotEmptyOrDbNull("@SegmentPosition", model.SegmentPosition)
                    .AddIfNotEmptyOrDbNull("@LoopIdentifierCode", model.LoopIdentifierCode)
                    .AddIfNotEmptyOrDbNull("@SegmentSyntaxErrorCode", model.SegmentSyntaxErrorCode)
                    .AddIfNotEmptyOrDbNull("@ElementPosition", model.ElementPosition)
                    .AddIfNotEmptyOrDbNull("@ElementReferenceNbr", model.ElementReferenceNbr)
                    .AddIfNotEmptyOrDbNull("@ElementSyntaxErrorCode", model.ElementSyntaxErrorCode)
                    .AddIfNotEmptyOrDbNull("@ElementCopy", model.ElementCopy)
                    .AddOutParameter("@Note_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var noteKey = (int)keyParameter.Value;
                model.ResponseNoteKey = noteKey;

                return noteKey;
            }
        }
    }
}
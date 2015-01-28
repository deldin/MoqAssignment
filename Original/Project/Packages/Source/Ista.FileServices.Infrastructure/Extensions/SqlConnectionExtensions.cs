using System.Data;
using System.Data.SqlClient;

namespace Ista.FileServices.Infrastructure.Extensions
{
    public static class SqlConnectionExtensions
    {
        public static SqlCommand CreateCommand(this SqlConnection connection, string procedureName)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = procedureName;

            return command;
        }
    }
}

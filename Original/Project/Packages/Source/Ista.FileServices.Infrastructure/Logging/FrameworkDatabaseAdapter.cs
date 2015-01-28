using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using NLog;
using NLog.Targets;

namespace Ista.FileServices.Infrastructure.Logging
{
    [Target("FrameworkDatabaseAdapter")]
    public class FrameworkDatabaseAdapter : TargetWithLayout
    {
        protected override void Write(LogEventInfo logEvent)
        {
            if (logEvent.Exception == null)
                return;

            var message = Layout.Render(logEvent);
            PublishMessage(logEvent.Exception, message, logEvent.Properties);
        }

        public void PublishMessage(Exception exception, string message, IDictionary<object, object> meta)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["AppExceptionLog"].ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
                return;

            if (exception is AggregateException)
                exception = exception.InnerException;

            var clientId = 1;
            var formattedMessage = ExceptionFormatter.FormatText(exception, meta);
            var stackHash = ExceptionFormatter.ComputeHash(exception.StackTrace);

            object clientIdEntry;
            if (meta.TryGetValue("ClientId", out clientIdEntry))
            {
                if (clientIdEntry != null)
                    if (!int.TryParse(clientIdEntry.ToString(), out clientId))
                        clientId = 1;
            }

            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("cspAppExceptionInsert"))
            {
                command.AddWithValue("@Environment", Environment.MachineName)
                    .AddWithValue("@ApplicationName", "File Service")
                    .AddWithValue("@ApplicationType", "File Service")
                    .AddWithValue("@UserName", Environment.UserName)
                    .AddIfNotEmptyOrDbNull("@Message", message)
                    .AddIfNotEmptyOrDbNull("@StackTrace", exception.StackTrace)
                    .AddIfNotEmptyOrDbNull("@Source", exception.Source)
                    .AddIfNotEmptyOrDbNull("@FormattedMessage", formattedMessage)
                    .AddIfNotEmptyOrDbNull("@StackHash", stackHash)
                    .AddWithValue("@PostDate", DateTime.Now)
                    .AddWithValue("@ClientID", clientId);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }
    }
}

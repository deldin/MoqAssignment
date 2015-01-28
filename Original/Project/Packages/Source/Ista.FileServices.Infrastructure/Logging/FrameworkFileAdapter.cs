using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Ista.FileServices.Infrastructure.Logging
{
    [Target("FrameworkFileAdapter")]
    public class FrameworkFileAdapter : TargetWithLayout
    {
        [RequiredParameter]
        public string newLog { get; set; }

        [RequiredParameter]
        public string filePattern { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            if (logEvent.Exception == null)
                return;

            var message = Layout.Render(logEvent);
            PublishMessage(logEvent.Exception, message, logEvent.Properties);
        }

        public void PublishMessage(Exception exception, string message, IDictionary<object, object> meta)
        {
            var directoryPath = ConfigurationManager.AppSettings["ExceptionLogDirectory"];
            if (string.IsNullOrEmpty(directoryPath))
                return;

            var directoryInfo = new DirectoryInfo(directoryPath);
            if (!directoryInfo.Exists)
                directoryInfo.Create();

            if (exception is AggregateException)
                exception = exception.InnerException;

            var clientId = 1;
            var clientName = "IH";
            var formattedMessage = ExceptionFormatter.FormatText(exception, meta);

            object clientIdEntry;
            if (meta.TryGetValue("ClientId", out clientIdEntry))
            {
                if (clientIdEntry != null)
                    if (!int.TryParse(clientIdEntry.ToString(), out clientId))
                        clientId = 1;
            }

            object clientNameEntry;
            if (meta.TryGetValue("ClientName", out clientNameEntry))
            {
                if (clientNameEntry != null)
                    clientName = clientNameEntry.ToString();
            }

            var clientFileName = filePattern
                .Replace("{ClientId}", clientId.ToString())
                .Replace("{ClientName}", clientName);

            var fileExtension = Path.GetExtension(clientFileName);
            var fileName = Path.GetFileNameWithoutExtension(clientFileName);
            var fileDate = GetFileDate();

            fileName = (string.IsNullOrEmpty(fileDate))
                           ? string.Concat(fileName, fileExtension)
                           : string.Concat(fileName, "_", fileDate, fileExtension);

            var filePath = Path.Combine(directoryInfo.FullName, fileName);
            var fileInfo = new FileInfo(filePath);

            using (var stream = fileInfo.Open(FileMode.Append, FileAccess.Write, FileShare.Read))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(formattedMessage);
            }
        }

        public string GetFileDate()
        {
            switch (newLog)
            {
                case "daily":
                    return DateTime.Now.ToString("yyyyMMdd");
                case "monthly":
                    return DateTime.Now.ToString("yyyyMM");
            }

            return string.Empty;
        }
    }
}

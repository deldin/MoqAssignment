using System;
using System.Collections.Generic;
using Ista.FileServices.Infrastructure.Interfaces;

namespace Ista.FileServices.Infrastructure.Logging
{
    public class Logger : ILogger
    {
        private readonly int clientId;
        private readonly string loggerName;
        private readonly string loggerTaskId;
        private readonly NLog.Logger logger;
        
        public Logger()
            : this(1, "Client-IH")
        {
        }

        public Logger(string name)
            : this (1, name)
        {
        }

        public Logger(int clientId, string loggerName)
        {
            this.clientId = clientId;
            this.loggerName = loggerName;
            
            logger = NLog.LogManager.GetLogger(loggerName);
            loggerTaskId = string.Empty;
        }

        public Logger(int clientId, string loggerName, string loggerTaskId)
            : this(clientId, loggerName)
        {
            this.loggerTaskId = loggerTaskId;
        }

        public void Trace(string message)
        {
            TraceFormat(null, message, null);
        }

        public void Trace(string message, Dictionary<string, object> meta)
        {
            TraceFormat(meta, message, null);
        }

        public void TraceFormat(string format, params object[] args)
        {
            TraceFormat(null, format, args);
        }

        public void TraceFormat(Dictionary<string, object> meta, string format, params object[] args)
        {
            if (!logger.IsTraceEnabled)
                return;

            var entry = new NLog.LogEventInfo(NLog.LogLevel.Trace, loggerName, null, format, args);

            if (meta != null)
                foreach (var item in meta)
                    entry.Properties[item.Key] = item.Value;

            logger.Log(entry);
        }

        public void Debug(string message)
        {
            DebugFormat(null, message, null);
        }

        public void Debug(string message, Dictionary<string, object> meta)
        {
            DebugFormat(meta, message, null);
        }

        public void DebugFormat(string format, params object[] args)
        {
            DebugFormat(null, format, args);
        }

        public void DebugFormat(Dictionary<string, object> meta, string format, params object[] args)
        {
            if (!logger.IsDebugEnabled)
                return;

            var entry = new NLog.LogEventInfo(NLog.LogLevel.Debug, loggerName, null, format, args);

            SetLogEntryProperties(meta, entry);
            logger.Log(entry);
        }

        public void Info(string message)
        {
            InfoFormat(null, message, null);
        }

        public void Info(string message, Dictionary<string, object> meta)
        {
            InfoFormat(meta, message, null);
        }

        public void InfoFormat(string format, params object[] args)
        {
            InfoFormat(null, format, args);
        }

        public void InfoFormat(Dictionary<string, object> meta, string format, params object[] args)
        {
            if (!logger.IsInfoEnabled)
                return;

            var entry = new NLog.LogEventInfo(NLog.LogLevel.Info, loggerName, null, format, args);

            SetLogEntryProperties(meta, entry);
            logger.Log(entry);
        }

        public void Warn(string message)
        {
            WarnFormat(null, null, message, null);
        }

        public void Warn(string message, Dictionary<string, object> meta)
        {
            WarnFormat(null, meta, message, null);
        }

        public void WarnFormat(string format, params object[] args)
        {
            WarnFormat(null, null, format, args);
        }

        public void WarnFormat(Exception ex, string format, params object[] args)
        {
            WarnFormat(ex, null, format, args);
        }

        public void WarnFormat(Dictionary<string, object> meta, string format, params object[] args)
        {
            WarnFormat(null, meta, format, args);
        }

        public void WarnFormat(Exception ex, Dictionary<string, object> meta, string format, params object[] args)
        {
            if (!logger.IsWarnEnabled)
                return;

            var entry = new NLog.LogEventInfo(NLog.LogLevel.Warn, loggerName, null, format, args)
            {
                Exception = ex
            };

            SetLogEntryPropertiesForException(ex, meta, entry);
            logger.Log(entry);
        }

        public void Error(string message)
        {
            ErrorFormat(null, null, message, null);
        }

        public void Error(string message, Dictionary<string, object> meta)
        {
            ErrorFormat(null, meta, message, null);
        }

        public void Error(Exception ex, string message)
        {
            ErrorFormat(ex, null, message, null);
        }

        public void Error(Exception ex, Dictionary<string, object> meta, string message)
        {
            ErrorFormat(ex, meta, message, null);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            ErrorFormat(null, null, format, args);
        }

        public void ErrorFormat(Exception ex, string format, params object[] args)
        {
            ErrorFormat(ex, null, format, args);
        }

        public void ErrorFormat(Exception ex, Dictionary<string, object> meta, string format, params object[] args)
        {
            if (!logger.IsErrorEnabled)
                return;

            var entry = new NLog.LogEventInfo(NLog.LogLevel.Error, loggerName, null, format, args)
            {
                Exception = ex
            };

            SetLogEntryPropertiesForException(ex, meta, entry);
            logger.Log(entry);
        }

        public IDisposable NestLog(string name)
        {
            return NLog.NestedDiagnosticsContext.Push(name);
        }

        private void SetLogEntryProperties(Dictionary<string, object> meta, NLog.LogEventInfo entry)
        {
            if (meta == null)
                meta = new Dictionary<string, object>();

            if (!meta.ContainsKey("ClientId"))
                meta["ClientId"] = clientId;

            if (!string.IsNullOrWhiteSpace(loggerTaskId))
            {
                if (!meta.ContainsKey("TaskId"))
                    meta["TaskId"] = loggerTaskId;
            }
            
            foreach (var item in meta)
                entry.Properties[item.Key] = item.Value;
        }

        private void SetLogEntryPropertiesForException(Exception exception, Dictionary<string, object> meta, NLog.LogEventInfo entry)
        {
            if (meta == null)
                meta = new Dictionary<string, object>();

            if (!meta.ContainsKey("ClientId"))
                meta["ClientId"] = clientId;

            if (!string.IsNullOrWhiteSpace(loggerTaskId))
            {
                if (!meta.ContainsKey("TaskId"))
                    meta["TaskId"] = loggerTaskId;

                if (exception != null)
                    meta["MessageType"] = "error";
            }

            ExceptionFormatter.AddAdditionalInformation(meta);
            foreach (var item in meta)
                entry.Properties[item.Key] = item.Value;
        }
    }
}

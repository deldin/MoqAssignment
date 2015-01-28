using System;
using System.Collections.Generic;

namespace Ista.FileServices.Infrastructure.Interfaces
{
    /// <summary>
    /// Responsible for Logging concerns.
    /// </summary>
    public interface ILogger
    {
        void Trace(string message);
        void Trace(string message, Dictionary<string, object> meta);
        void TraceFormat(string format, params object[] args);
        void TraceFormat(Dictionary<string, object> meta, string format, params object[] args);

        void Debug(string message);
        void Debug(string message, Dictionary<string, object> meta);
        void DebugFormat(string format, params object[] args);
        void DebugFormat(Dictionary<string, object> meta, string format, params object[] args);

        void Info(string message);
        void Info(string message, Dictionary<string, object> meta);
        void InfoFormat(string format, params object[] args);
        void InfoFormat(Dictionary<string, object> meta, string format, params object[] args);

        void Warn(string message);
        void Warn(string message, Dictionary<string, object> meta);
        void WarnFormat(string format, params object[] args);
        void WarnFormat(Dictionary<string, object> meta, string format, params object[] args);
        void WarnFormat(Exception ex, string format, params object[] args);
        void WarnFormat(Exception ex, Dictionary<string, object> meta, string format, params object[] args);

        void Error(string message);
        void Error(string message, Dictionary<string, object> meta);
        void Error(Exception ex, string message);
        void Error(Exception ex, Dictionary<string, object> meta, string message);
        void ErrorFormat(string format, params object[] args);
        void ErrorFormat(Exception ex, string format, params object[] args);
        void ErrorFormat(Exception ex, Dictionary<string, object> meta, string format, params object[] args);

        /// <summary>
        /// Creates a nested context for logging.
        /// </summary>
        /// <param name="name">Name of context</param>
        /// <returns></returns>
        IDisposable NestLog(string name);
    }
}

using System;
using System.Linq;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.Infrastructure.Serializers;
using Ista.FileServices.Service.Interfaces;
using NLog;

namespace Ista.FileServices.Service.Handlers
{
    public class HandlerChangeLogging : IScheduleMessageHandler
    {
        public bool IsSatisfiedBy(IIstaMessage message)
        {
            if (message.Type.Equals("logging"))
                return true;

            return false;
        }

        public bool Handle(IMiramarTaskProvider taskProvider, IMiramarScheduleProvider scheduleProvider, IMiramarContextProvider contextProvider, IIstaMessage message)
        {
            var request = JsonMessageSerializer.DeserializeType(message.Body, new
            {
                requestId = 0,
                requestedBy = string.Empty,
                requestedOn = new DateTime?(),
                logName = string.Empty,
                logLevel = string.Empty,
            });

            if (request == null)
                return false;

            var logName = request.logName;
            if (string.IsNullOrWhiteSpace(logName))
                return false;

            var minLogLevel = LogLevel.Trace.GetHashCode();
            var maxLogLevel = LogLevel.Off.GetHashCode();
            var logLevel = LogLevel.FromString(request.logLevel);

            var configuration = LogManager.Configuration;
            var configuredRules = configuration.LoggingRules;

            var rules = configuredRules
                .SelectMany(x => x.Targets, (r, t) => new
                {
                    LoggingRule = r,
                    Target = t
                })
                .Where(
                    x =>
                    !string.IsNullOrWhiteSpace(x.Target.Name) &&
                    x.Target.Name.Equals(logName, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.LoggingRule)
                .ToArray();

            if (!rules.Any())
                return false;

            foreach (var rule in rules)
            {
                for (var ordinal = minLogLevel; ordinal < maxLogLevel; ordinal++)
                {
                    var ordinalLogLevel = LogLevel.FromOrdinal(ordinal);
                    if (ordinalLogLevel < logLevel)
                    {
                        rule.DisableLoggingForLevel(ordinalLogLevel);
                        continue;
                    }

                    if (ordinalLogLevel >= logLevel)
                        rule.EnableLoggingForLevel(ordinalLogLevel);
                }
            }

            LogManager.ReconfigExistingLoggers();
            return true;
        }
    }
}

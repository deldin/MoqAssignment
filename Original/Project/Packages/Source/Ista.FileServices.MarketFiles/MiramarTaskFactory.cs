using System;
using System.Linq;
using Ista.FileServices.Infrastructure;
using Ista.FileServices.MarketFiles.DataAccess;
using Ista.Miramar.Interfaces;

namespace Ista.FileServices.MarketFiles
{
    public class MiramarTaskFactory : IMiramarTaskFactory
    {
        private static readonly string[] taskIdentifiers = new[]
        {
            "market.file.decrypt",
            "market.file.import",
            "market.file.export",
            "market.file.encrypt",
            "market.file.transmit",
            "ercot.cbf.encrypt",
            "ercot.cbf.transmit"
        };

        public bool IsSatisfiedBy(string taskId)
        {
            return taskIdentifiers.Contains(taskId, StringComparer.OrdinalIgnoreCase);
        }

        public IMiramarTask GetTask(IMiramarClientInfo clientInfo, string taskId)
        {
            var adminDataAccess = new AdminDataAccess(clientInfo.AdminConnection);

            var clientId = clientInfo.ClientId;
            var logName = string.Format("Client-{0}", clientId);
            var logTaskId = string.Format("client.{0}.{1}", clientId, taskId);
            var logger = InfrastructureFactory.CreateLogger(clientId, logName, logTaskId);

            if (taskId.StartsWith("market.file", StringComparison.OrdinalIgnoreCase))
            {
                var clientDataAccess = new ClientDataAccess(clientInfo.ClientConnection);

                if (taskId.Equals("market.file.decrypt", StringComparison.OrdinalIgnoreCase))
                    return new DecryptFileTask(adminDataAccess, clientDataAccess, logger, clientId);

                var marketDataAccess = new MarketDataAccess(clientInfo.MarketConnection);

                if (taskId.Equals("market.file.import", StringComparison.OrdinalIgnoreCase))
                    return new ImportMarketFileTask(adminDataAccess, marketDataAccess, logger, clientId);

                if (taskId.Equals("market.file.export", StringComparison.OrdinalIgnoreCase))
                    return new ExportMarketFileTask(adminDataAccess, marketDataAccess, logger, clientId);

                if (taskId.Equals("market.file.encrypt", StringComparison.OrdinalIgnoreCase))
                    return new EncryptFileTask(adminDataAccess, clientDataAccess, marketDataAccess, logger, clientId);

                if (taskId.Equals("market.file.transmit", StringComparison.OrdinalIgnoreCase))
                    return new TransmitFileTask(adminDataAccess, clientDataAccess, marketDataAccess, logger, clientId);
            }

            if (taskId.StartsWith("ercot.cbf", StringComparison.OrdinalIgnoreCase))
            {
                var clientDataAccess = new ClientDataAccess(clientInfo.ClientConnection);
                var marketDataAccess = new MarketDataAccess(clientInfo.MarketConnection);

                if (taskId.Equals("ercot.cbf.encrypt", StringComparison.OrdinalIgnoreCase))
                    return new ErcotEncryptFileTask(adminDataAccess, clientDataAccess, marketDataAccess, logger,
                        clientId);

                if (taskId.Equals("ercot.cbf.transmit", StringComparison.OrdinalIgnoreCase))
                    return new ErcotTransmitFileTask(adminDataAccess, clientDataAccess, marketDataAccess, logger,
                        clientId);
            }

            var message = string.Format("No Miramar Task could be identified for task \"{0}\".", taskId);
            throw new ArgumentOutOfRangeException("taskId", message);
        }
    }
}

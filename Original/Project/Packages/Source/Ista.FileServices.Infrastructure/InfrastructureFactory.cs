using Ista.FileServices.Infrastructure.Encryption;
using Ista.FileServices.Infrastructure.FtpHandlers;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.Infrastructure.Logging;

namespace Ista.FileServices.Infrastructure
{
    public class InfrastructureFactory
    {
        public static IPgpEncryption CreatePgpEncryptor(string encryptionKey, string signatureKey, string passphrase)
        {
            return PgpEncryption.Create(encryptionKey, signatureKey, passphrase);
        }

        public static IPgpEncryption CreatePgpEncryptor(string encryptionKey, string signatureKey, string passphrase, bool verify)
        {
            return PgpEncryption.Create(encryptionKey, signatureKey, passphrase, verify);
        }

        public static ILogger CreateLogger(string logName)
        {
            return new Logger(logName);
        }

        public static ILogger CreateLogger(int clientId, string logName)
        {
            return new Logger(clientId, logName);
        }

        public static ILogger CreateLogger(int clientId, string logName, string logTaskId)
        {
            return new Logger(clientId, logName, logTaskId);
        }

        public static IFtpHandler CreateFtpHandler(string server, string username, string password)
        {
            return DartFtpHandler.Create(server, username, password);
        }

        public static void ForceLogFlush()
        {
            NLog.LogManager.Flush();
        }

        public static void CloseLogTargets()
        {
            var configuration = NLog.LogManager.Configuration;
            foreach (var target in configuration.AllTargets)
                target.Dispose();
        }
    }
}

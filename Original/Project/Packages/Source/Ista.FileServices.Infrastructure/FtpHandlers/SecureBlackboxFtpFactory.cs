using SBSimpleFTPS;
using SBWinCertStorage;

namespace Ista.FileServices.Infrastructure.FtpHandlers
{
    public class SecureBlackboxFtpFactory
    {
        public static TElSimpleFTPSClient Create(string server, string username, string password, int port, bool useSsl)
        {
            var client = new TElSimpleFTPSClient
            {
                CertStorage = new TElWinCertStorage(),
                Address = server,
                Username = username,
                Password = password,
                Port = port,
                UseSSL = useSsl,
                PassiveMode = true,
                EncryptDataChannel = true,
                SSLMode = SBSimpleFTPS.Unit.smExplicit,
                AuthCmd = SBSimpleFTPS.Unit.acAuto,
                Versions = SBConstants.Unit.sbTLS1
            };

            return client;
        }
    }
}
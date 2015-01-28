using Renci.SshNet;

namespace Ista.FileServices.Infrastructure.FtpHandlers
{
    public class SshFtpFactory
    {
        public static SftpClient CreateConnection(string server, string username, string privateKeyPath)
        {
            var privateKey = new PrivateKeyFile(privateKeyPath);
            var privateKeyConnection = new PrivateKeyConnectionInfo(server, username, privateKey);

            return new SftpClient(privateKeyConnection);
        }
    }
}
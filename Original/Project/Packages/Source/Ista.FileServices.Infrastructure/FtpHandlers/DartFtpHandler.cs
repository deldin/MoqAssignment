using System;
using Ista.FileServices.Infrastructure.Interfaces;
using DartFtp = Dart.PowerTCP.Ftp;

namespace Ista.FileServices.Infrastructure.FtpHandlers
{
    public class DartFtpHandler : IFtpHandler
    {
        private readonly string server;
        private readonly string username;
        private readonly string password;

        public DartFtpHandler(string server, string username, string password)
        {
            this.server = server;
            this.username = username;
            this.password = password;
        }

        public void SendFile(string remoteDirectory, string localFileName, string remoteFileName)
        {
            if (string.IsNullOrEmpty(localFileName))
                throw new ArgumentNullException("localFileName");

            if (string.IsNullOrEmpty(remoteFileName))
                throw new ArgumentNullException("remoteFileName");

            var instance = DartFtpFactory.CreateConnection(server, username, password);
            using (instance)
            {
                if (!string.IsNullOrWhiteSpace(remoteDirectory))
                    instance.Invoke(DartFtp.FtpCommand.ChangeDir, remoteDirectory);

                var ftpFileResult = instance.Put(localFileName, remoteFileName);
                if (ftpFileResult.Status == DartFtp.FtpFileStatus.TransferCompleted)
                    return;

                if (ftpFileResult.Exception != null)
                    throw ftpFileResult.Exception;

                var status = ftpFileResult.Status.ToString();
                var message =
                    string.Format(
                        "FTP of file \"{0}\" to Remote Server \"{1}\" was not successful. Status returned \"{2}\".",
                        localFileName, server, status);

                throw new Exception(message);
            }
        }

        public static IFtpHandler Create(string server, string username, string password)
        {
            return new DartFtpHandler(server, username, password);
        }
    }
}

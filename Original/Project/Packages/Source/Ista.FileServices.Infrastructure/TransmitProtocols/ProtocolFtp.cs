using System;
using System.IO;
using Ista.FileServices.Infrastructure.FtpHandlers;

namespace Ista.FileServices.Infrastructure.TransmitProtocols
{
    public class ProtocolFtp
    {
        public TransmitResult TransmitFile(string localFile, string remoteServer, string remoteDirectory, string userId, string password)
        {
            var file = new FileInfo(localFile);
            if (!file.Exists)
            {
                return new TransmitResult
                {
                    Message = string.Format("Invalid filepath: \"{0}\".", localFile),
                    Transmitted = false,
                };
            }

            var configuration = new SecureBlackboxFtpConfiguration
            {
                FtpRemoteServer = remoteServer,
                FtpRemoteDirectory = remoteDirectory,
                FtpUsername = userId,
                FtpPassword = password,
                FtpPort = 21,
                FtpSsl = false,
            };

            try
            {
                var localDirectory = file.DirectoryName;

                var handler = new SecureBlackboxFtpHandler(configuration);
                handler.SendFiles(localDirectory, remoteDirectory,
                    x => x.Equals(localFile, StringComparison.OrdinalIgnoreCase));

                return new TransmitResult
                {
                    Message = "Transfer successful",
                    Transmitted = true,
                };
            }
            catch (Exception ex)
            {
                return new TransmitResult
                {
                    Message = ex.Message,
                    Transmitted = false,
                };
            }
        }
    }
}

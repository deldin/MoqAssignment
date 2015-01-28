using System;
using System.IO;
using System.Linq;

namespace Ista.FileServices.Infrastructure.FtpHandlers
{
    public class SshFtpHandler
    {
        private readonly string server;
        private readonly string username;
        private readonly string privateKeyPath;

        public SshFtpHandler(string server, string username, string privateKeyPath)
        {
            this.server = server;
            this.username = username;
            this.privateKeyPath = privateKeyPath;
        }

        public void GetFiles(string localDirectory)
        {
            GetFiles(string.Empty, localDirectory, string.Empty, x => true);
        }

        public void GetFiles(string localDirectory, Func<string, bool> remoteFilePredicate)
        {
            GetFiles(string.Empty, localDirectory, string.Empty, remoteFilePredicate);
        }

        public void GetFiles(string remoteDirectory, string localDirectory)
        {
            GetFiles(remoteDirectory, localDirectory, string.Empty, x => true);
        }

        public void GetFiles(string remoteDirectory, string localDirectory, Func<string, bool> remoteFilePredicate)
        {
            GetFiles(remoteDirectory, localDirectory, string.Empty, remoteFilePredicate);
        }

        public void GetFiles(string remoteDirectory, string localDirectory, string extension, Func<string, bool> remoteFilePredicate)
        {
            var directoryInfo = new DirectoryInfo(localDirectory);
            if (!directoryInfo.Exists)
                throw new DirectoryNotFoundException();

            using (var client = SshFtpFactory.CreateConnection(server, username, privateKeyPath))
            {
                client.Connect();
                
                if (!string.IsNullOrWhiteSpace(remoteDirectory))
                    client.ChangeDirectory(remoteDirectory);

                var collection = client.ListDirectory(string.Empty)
                    .Select(x => x.Name)
                    .ToArray();

                foreach (var item in collection)
                {
                    if (string.IsNullOrWhiteSpace(item))
                        continue;

                    if (!remoteFilePredicate(item))
                        continue;

                    var localPath = Path.Combine(directoryInfo.FullName, item);
                    if (!string.IsNullOrWhiteSpace(extension))
                        localPath = string.Concat(localPath, extension);

                    using (var localStream = File.Open(localPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                    {
                        client.DownloadFile(item, localStream);
                    }
                }
            }
        }

        public void SendFiles(string localDirectory)
        {
            SendFiles(string.Empty, localDirectory, string.Empty, x => true, null);
        }

        public void SendFiles(string localDirectory, Func<string, bool> localFilePredicate)
        {
            SendFiles(string.Empty, localDirectory, string.Empty, localFilePredicate, null);
        }

        public void SendFiles(string remoteDirectory, string localDirectory)
        {
            SendFiles(remoteDirectory, localDirectory, string.Empty, x => true, null);
        }

        public void SendFiles(string remoteDirectory, string localDirectory, Func<string, bool> localFilePredicate)
        {
            SendFiles(remoteDirectory, localDirectory, string.Empty, localFilePredicate, null);
        }

        public void SendFiles(string remoteDirectory, string localDirectory, string searchPattern, Func<string, bool> localFilePredicate)
        {
            SendFiles(remoteDirectory, localDirectory, searchPattern, localFilePredicate, null);
        }

        public void SendFiles(string remoteDirectory, string localDirectory, string searchPattern, Func<string, bool> localFilePredicate, Action<FileInfo> command)
        {
            var directoryInfo = new DirectoryInfo(localDirectory);
            if (!directoryInfo.Exists)
                throw new DirectoryNotFoundException();

            if (string.IsNullOrWhiteSpace(searchPattern))
                searchPattern = "*";

            var localFiles = directoryInfo.GetFiles(searchPattern);
            if (localFiles.Length == 0)
                return;

            using (var client = SshFtpFactory.CreateConnection(server, username, privateKeyPath))
            {
                client.Connect();

                if (!string.IsNullOrWhiteSpace(remoteDirectory))
                    client.ChangeDirectory(remoteDirectory);

                foreach (var localFile in localFiles)
                {
                    var fileName = localFile.Name;
                    if (!localFilePredicate(fileName))
                        continue;

                    using (var localStream = localFile.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        client.UploadFile(localStream, fileName);
                    }

                    if (command != null)
                        command(localFile);
                }
            }
        }
    }
}

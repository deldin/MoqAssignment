using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using SBCustomCertStorage;
using SBSimpleFTPS;
using SBX509;

namespace Ista.FileServices.Infrastructure.FtpHandlers
{
    public class SecureBlackboxFtpHandler
    {
        private const string License = "0311142292E7EBD0AFB5D8534E19E547E8A79B44DCB15B910DCC4C10CD1749A13E2DF9292F3325C41E98B1068C19181A7082C661DE0A62545419023547D79BEAC7EAF365914C544EEA6254AFE11F3081AA950C7E5B8F0CBB930D8C40C17AFBB42FD929D681F4AF287E4E8D7AE3DECE41BB33EADCB75627574B2A7CDFEE55C355B60EE6919E3F1C730D53720644A38DD5527EBD64CB45C44487A869F23BEA14BA48FC4D4E4264CBDD033EC4EAE7F98D82E7FE7805569B077EA7BDB1648E57C05DFC1BD0806597EB6612AEF2793D8B437726D1E97794E57511A3906FCA496DCAB3E64084063515C3925362E70ED720A773C0CD307735F560DE3747C8A213282FC8";

        private readonly bool serverSsl;
        private readonly int serverPort;
        private readonly string server;
        private readonly string username;
        private readonly string password;
        private readonly string certificateKey;
        private readonly string certificatePath;
        private readonly string certificatePassphrase;
        private readonly TElMemoryCertStorage memoryCertificateStorage;
        private readonly TElX509Certificate certificate;

        private TElSimpleFTPSClient client;
        private bool useCertificate;
        private int certificateIndex;
        private List<string> collection;

        public SecureBlackboxFtpHandler(string server, string username, string password)
            : this(server, username, password, string.Empty, string.Empty)
        {
        }

        public SecureBlackboxFtpHandler(string server, string username, string password, string certificatePath, string certificatePassphrase)
        {
            this.server = server;
            this.username = username;
            this.password = password;
            this.certificatePath = certificatePath;
            this.certificatePassphrase = certificatePassphrase;

            serverSsl = true;
            serverPort = 21;
            certificateKey = string.Empty;
            memoryCertificateStorage = new TElMemoryCertStorage();
            certificate = new TElX509Certificate();
        }

        public SecureBlackboxFtpHandler(SecureBlackboxFtpConfiguration configuration)
        {
            serverSsl = configuration.FtpSsl;
            serverPort = configuration.FtpPort;
            server = configuration.FtpRemoteServer;
            username = configuration.FtpUsername;
            password = configuration.FtpPassword;
            certificateKey = configuration.PfxKeyIdentifier;
            certificatePath = configuration.PfxFileName;
            certificatePassphrase = configuration.PfxPassphrase;
            
            memoryCertificateStorage = new TElMemoryCertStorage();
            certificate = new TElX509Certificate();
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

            SBUtils.Unit.SetLicenseKey(License);
            LoadCertificate();

            client = SecureBlackboxFtpFactory.Create(server, username, password, serverPort, serverSsl);
            client.OnCertificateNeededEx += OnCertificateNeededEvent;
            client.OnCertificateValidate += OnCertificateValidateEvent;
            client.OnSSLError += OnSslErrorEvent;
            client.OnTextDataLine += OnTextDataLineEvent;

            memoryCertificateStorage.Clear();

            certificateIndex = 0;
            collection = new List<string>();
            using (client)
            {
                client.Open();
                client.Login();

                if (!string.IsNullOrWhiteSpace(remoteDirectory))
                    client.Cwd(remoteDirectory);

                client.GetNameList();

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
                        client.Receive(item, localStream, 0, localStream.Length - 1);
                    }
                }

                if (client.Active)
                    client.Close(true);
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

            SBUtils.Unit.SetLicenseKey(License);
            LoadCertificate();

            client = SecureBlackboxFtpFactory.Create(server, username, password, serverPort, serverSsl);
            client.OnCertificateNeededEx += OnCertificateNeededEvent;
            client.OnCertificateValidate += OnCertificateValidateEvent;
            client.OnSSLError += OnSslErrorEvent;

            memoryCertificateStorage.Clear();

            certificateIndex = 0;
            collection = new List<string>();
            using (client)
            {
                client.Open();
                client.Login();

                if (!string.IsNullOrWhiteSpace(remoteDirectory))
                    client.Cwd(remoteDirectory);

                foreach (var localFile in localFiles)
                {
                    var fileName = localFile.Name;
                    if (!localFilePredicate(fileName))
                        continue;

                    using (var localStream = localFile.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        client.Send(localStream, fileName, 0, localStream.Length - 1, false, 0);
                    }

                    if (command != null)
                        command(localFile);
                }

                if (client.Active)
                    client.Close(true);
            }
        }

        public void LoadCertificate()
        {
            if (!string.IsNullOrWhiteSpace(certificateKey)) { 
                LoadCertificateFromStore();
                return;
            }

            LoadCertificateFromFile();
        }

        public void LoadCertificateFromStore()
        {
            var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                var certificates = store.Certificates
                    .Find(X509FindType.FindBySubjectKeyIdentifier, certificateKey, false);

                if (certificates.Count == 0)
                {
                    useCertificate = false;
                    return;
                }

                var item = certificates[0];
                using (var stream = new MemoryStream(item.Export(X509ContentType.Pfx, certificatePassphrase), false))
                {
                    stream.Position = 0;
                    var result = certificate.LoadFromStreamPFX(stream, certificatePassphrase, (int)stream.Length);
                    if (result != 0)
                    {
                        var message = string.Format("Unable to load certificate \"{0}\". Result: \"{1}\".",
                            certificatePath, result);

                        throw new ApplicationException(message);
                    }

                    useCertificate = true;
                }

            }
            catch
            {
                LoadCertificateFromFile();
            }
            finally
            {
                store.Close();
            }
        }

        public void LoadCertificateFromFile()
        {
            if (string.IsNullOrWhiteSpace(certificatePath))
            {
                useCertificate = false;
                return;
            }

            var certificateInfo = new FileInfo(certificatePath);
            if (!certificateInfo.Exists)
            {
                useCertificate = false;
                return;
            }

            using (var stream = certificateInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var result = certificate.LoadFromStreamPFX(stream, certificatePassphrase, (int)stream.Length);
                if (result != 0)
                {
                    var message = string.Format("Unable to load certificate \"{0}\". Result: \"{1}\".",
                        certificatePath, result);

                    throw new ApplicationException(message);
                }

                useCertificate = true;
            }
        }

        public void OnCertificateNeededEvent(object sender, ref TElX509Certificate concern)
        {
            if (!useCertificate || certificateIndex != 0)
            {
                concern = null;
                return;
            }

            concern = certificate;
            certificateIndex++;
        }

        public void OnCertificateValidateEvent(object sender, TElX509Certificate concern, ref bool validate)
        {
            var reason = 0;
            var validity = TSBCertificateValidity.cvInvalid;
            client.InternalValidate(ref validity, ref reason);

            if ((validity | (TSBCertificateValidity.cvOk | TSBCertificateValidity.cvSelfSigned)) == 0)
            {
                validity = memoryCertificateStorage.Validate(certificate, ref reason, DateTime.Now);

                if ((validity | (TSBCertificateValidity.cvOk | TSBCertificateValidity.cvSelfSigned)) == 0)
                    throw new ApplicationException("The server certificate is not valid.");
            }

            memoryCertificateStorage.Add(concern, true);
            validate = true;
        }

        public void OnTextDataLineEvent(object sender, byte[] data)
        {
            if (collection == null)
                collection = new List<string>();

            var content = Encoding.ASCII.GetString(data);
            collection.Add(content);
        }

        public void OnSslErrorEvent(object sender, int errorCode, bool isFatal, bool isRemote)
        {
            var source = (isRemote) ? "Remote" : "Local";
            var type = (isFatal) ? "Error (Fatal)" : "Error";

            var message = string.Format("{0} {1}: {2}", source, type, errorCode);
            throw new ApplicationException(message);
        }
    }
}
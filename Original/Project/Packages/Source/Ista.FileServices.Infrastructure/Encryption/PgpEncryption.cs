using System;
using System.IO;
using Ista.FileServices.Infrastructure.Interfaces;
using SBPGP;
using SBPGPConstants;

namespace Ista.FileServices.Infrastructure.Encryption
{
    public class PgpEncryption : IPgpEncryption
    {
        private const string License = "0282E88DFB137FA032D2E2A48675093815BE4C9868AD230B3732DA61932D86AC6E2A82EF64C16FAC79462DABFAF4F04ED24DA377682445D10E86375377C8E4C920675860A28B945EE601135CD5BA8992ABEE2BF4BFC4551FC9ADD86A8B49E9ACC01796F40D3A47B5C14FC103A98F9997B1D098FEED836791E709FC5C617A87661D3F8E370EED48202CC11E2BC60E5C189C3992968D67169FE06F1BDB9E71F6817E0841F56F96136116494BF772DC076275C9E752BB28DF5E6528E78F6EA81D44C6E1C35A2121E5324E32A00410B0BBF34D33D65FE43FE3EB603EA8E416ECD49B35CFAFB101CA4183E214B53E9FA752197481187D34867A53E85C9BA7BE065285";

        private readonly string encryptionKey;
        private readonly string signatureKey;
        private readonly string passphrase;

        public PgpEncryption(string encryptionKey, string signatureKey, string passphrase)
        {
            this.encryptionKey = encryptionKey;
            this.signatureKey = signatureKey;
            this.passphrase = passphrase;
        }

        public void DecryptFile(string sourceName, string targetName)
        {
            var sourceInfo = new FileInfo(sourceName);
            if (!sourceInfo.Exists)
                throw new FileNotFoundException("Source File was not found or has been deleted.", sourceName);

            var targetInfo = new FileInfo(targetName);
            if (targetInfo.Exists)
            {
                var message = string.Format("Target File \"{0}\" already exists.", targetName);
                throw new InvalidOperationException(message);
            }

            SBUtils.Unit.SetLicenseKey(SBUtils.Unit.BytesOfString(License));

            var privateRing = new SBPGPKeys.TElPGPKeyring();
            lock (typeof(SBPGPKeys.TElPGPKeyring))
            {
                privateRing.Load(encryptionKey, signatureKey, true);
            }

            var reader = new TElPGPReader
            {
                DecryptingKeys = privateRing,
                VerifyingKeys = privateRing,
                KeyPassphrase = passphrase,
            };

            try
            {
                using (var sourceStream = sourceInfo.OpenRead())
                using (var targetStream = targetInfo.Create())
                using (reader)
                {
                    reader.OutputStream = targetStream;
                    reader.DecryptAndVerify(sourceStream, 0);
                }
            }
            catch (Exception)
            {
                targetInfo.Refresh();
                if (targetInfo.Exists)
                    targetInfo.Delete();

                throw;
            }
        }

        public void EncryptFile(string sourceName, string targetName)
        {
            var sourceInfo = new FileInfo(sourceName);
            if (!sourceInfo.Exists)
                throw new FileNotFoundException("Source File was not found or has been deleted.", sourceName);

            var targetInfo = new FileInfo(targetName);
            if (targetInfo.Exists)
            {
                var message = string.Format("Target File \"{0}\" already exists.", targetName);
                throw new InvalidOperationException(message);
            }

            SBUtils.Unit.SetLicenseKey(SBUtils.Unit.BytesOfString(License));

            var publicKey = new SBPGPKeys.TElPGPPublicKey();
            publicKey.LoadFromFile(encryptionKey);

            var privateKey = new SBPGPKeys.TElPGPSecretKey();
            privateKey.LoadFromFile(signatureKey);
            privateKey.Passphrase = passphrase;

            var publicRing = new SBPGPKeys.TElPGPKeyring();
            publicRing.AddPublicKey(publicKey);

            var privateRing = new SBPGPKeys.TElPGPKeyring();
            privateRing.AddSecretKey(privateKey);

            var writer = new TElPGPWriter
            {
                Armor = true,
                Compress = false,
                CompressionLevel = 9,
                Filename = Path.GetFileName(sourceName),
                EncryptionType = TSBPGPEncryptionType.etPublicKey,
                Protection = TSBPGPProtectionType.ptNormal,
                UseNewFeatures = false,
            };

            writer.ArmorHeaders.Add("Version: PGP 6.5.2");
            writer.Passphrases.Add(passphrase);
            writer.EncryptingKeys = publicRing;
            writer.SigningKeys = privateRing;

            try
            {
                using (var sourceStream = sourceInfo.OpenRead())
                using (var targetStream = targetInfo.Create())
                using (writer)
                {
                    writer.EncryptAndSign(sourceStream, targetStream, 0);
                }
            }
            catch (Exception)
            {
                targetInfo.Refresh();
                if (targetInfo.Exists)
                    targetInfo.Delete();

                throw;
            }
        }

        public static IPgpEncryption Create(string encryptionKey, string signatureKey, string passphrase)
        {
            return Create(encryptionKey, signatureKey, passphrase, true);
        }

        public static IPgpEncryption Create(string encryptionKey, string signatureKey, string passphrase, bool verify)
        {
            if (verify)
            {
                if (!string.IsNullOrWhiteSpace(encryptionKey))
                {
                    var encryptionInfo = new FileInfo(encryptionKey);
                    if (!encryptionInfo.Exists)
                        throw new FileNotFoundException("Encryption Key was not found or has been deleted.",
                            encryptionKey);
                }

                if (!string.IsNullOrWhiteSpace(signatureKey))
                {
                    var signatureInfo = new FileInfo(signatureKey);
                    if (!signatureInfo.Exists)
                        throw new FileNotFoundException("Signature Key was not found or has been deleted.", signatureKey);
                }

                if (string.IsNullOrWhiteSpace(passphrase))
                    throw new ArgumentOutOfRangeException("passphrase", "Passphrase is not valid.");
            }

            return new PgpEncryption(encryptionKey, signatureKey, passphrase);
        }
    }
}

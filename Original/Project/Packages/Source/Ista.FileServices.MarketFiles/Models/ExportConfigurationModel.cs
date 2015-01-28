
namespace Ista.FileServices.MarketFiles.Models
{
    public class ExportConfigurationModel
    {
        public int ClientId { get; set; }
        public string Client { get; set; }
        public string ClientConnectionString { get; set; }
        public string MarketConnectionString { get; set; }
        public string DirectoryEncrypted { get; set; }
        public string DirectoryDecrypted { get; set; }
        public string DirectoryArchive { get; set; }
        public string DirectoryException { get; set; }
        public string PgpPassPhrase { get; set; }
        public string PgpEncryptionKey { get; set; }
        public string PgpSignatureKey { get; set; }
        public string FtpDirectory { get; set; }
        public string FtpUsername { get; set; }
        public string FtpPassword { get; set; }
        public string FtpRemoteServer { get; set; }
        public string FtpRemoteDirectory { get; set; }
    }
}
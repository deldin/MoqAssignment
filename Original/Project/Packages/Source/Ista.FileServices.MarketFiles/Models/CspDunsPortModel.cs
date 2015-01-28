
namespace Ista.FileServices.MarketFiles.Models
{
    public class CspDunsPortModel
    {
        public int CspDunsPortId { get; set; }
        public int CspDunsId { get; set; }
        public int? LdcId { get; set; }
        public int ProviderId { get; set; }
        public string Duns { get; set; }
        public string LdcDuns { get; set; }
        public string LdcShortName { get; set; }
        public string TradingPartnerId { get; set; }
        public string DirectoryIn { get; set; }
        public string DirectoryOut { get; set; }
        public string PgpEncryptionKey { get; set; }
        public string PgpSignatureKey { get; set; }
        public string PgpPassphrase { get; set; }
        public string FileType { get; set; }
        public string FtpRemoteServer { get; set; }
        public string FtpUserId { get; set; }
        public string FtpPassword { get; set; }
        public bool DecryptionEnabledFlag { get; set; }
        public bool EncryptionEnabledFlag { get; set; }
        public bool TransportEnabledFlag { get; set; }
        public string Protocol { get; set; }
        public int PortId { get; set; }
        public string GisbCommonCode { get; set; }
    }
}
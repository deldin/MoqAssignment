
namespace Ista.FileServices.Infrastructure.FtpHandlers
{
    public class SecureBlackboxFtpConfiguration
    {
        public bool FtpSsl { get; set; }
        public int FtpPort { get; set; }
        public string FtpRemoteDirectory { get; set; }
        public string FtpRemoteServer { get; set; }
        public string FtpUsername { get; set; }
        public string FtpPassword { get; set; }
        public string PfxFileName { get; set; }
        public string PfxPassphrase { get; set; }
        public string PfxKeyIdentifier { get; set; }
    }
}

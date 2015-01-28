using System;
using DartFtp = Dart.PowerTCP.Ftp;

namespace Ista.FileServices.Infrastructure.FtpHandlers
{
    public class DartFtpFactory
    {
        public static DartFtp.Ftp CreateConnection(string serverName, string username, string password)
        {
            var instance = new DartFtp.Ftp();
            instance.Connection.DoEvents = false;
            instance.Server = serverName;
            instance.Username = username;
            instance.Password = password;
            instance.ServerPort = 21;
            instance.Passive = false;
            instance.FileType = DartFtp.FileType.Ascii;
            instance.StoreType = DartFtp.StoreType.Replace;
            instance.Timeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;

            return instance;
        }
    }
}

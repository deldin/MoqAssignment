using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Ista.FileServices.Infrastructure.TransmitProtocols.Proxies;
using Microsoft.Web.Services2.Security;
using Microsoft.Web.Services2.Security.Tokens;

namespace Ista.FileServices.Infrastructure.TransmitProtocols
{
    public class ProtocolSoap
    {
        private const string ClientIdentifier = "66D5C932-3E70-486E-A096-C246C76C195E";

        public TransmitResult TransmitFile(string localFile, string remoteServer, string userid, string password)
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

            var token = new UsernameToken(ClientIdentifier, ClientIdentifier, PasswordOption.SendNone);
            var gisbProxy = new GisbProxyService { Url = remoteServer };
            gisbProxy.RequestSoapContext.Security.Tokens.Add(token);
            gisbProxy.RequestSoapContext.Security.Elements.Add(new MessageSignature(token));
            gisbProxy.RequestSoapContext.Security.Timestamp.TtlInSeconds = 60;
            gisbProxy.RequestSoapContext.Security.Elements.Add(new EncryptedData(token));
            gisbProxy.Credentials = new CredentialCache
            {
                {new Uri(remoteServer), "Basic", new NetworkCredential(userid, password)}
            };

            try
            {
                using (var stream = file.OpenRead())
                using (var reader = new StreamReader(stream))
                {
                    var content = reader.ReadToEnd();
                    var bytes = Encoding.UTF8.GetBytes(content);

                    var hash = (new MD5CryptoServiceProvider()).ComputeHash(bytes);

                    using (gisbProxy)
                    {
                        var resp = gisbProxy.SendEDI(bytes, hash, file.Name.Substring(0, 3));
                        return new TransmitResult
                        {
                            Message = resp.ErrorMessage,
                            Transmitted = resp.SuccessFlag,
                        };
                    }
                }
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
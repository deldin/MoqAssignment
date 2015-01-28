using System;
using System.IO;
using Ista.FileServices.Infrastructure.TransmitProtocols.Handlers;

namespace Ista.FileServices.Infrastructure.TransmitProtocols
{
    public class ProtocolNaesbHttp
    {
        public TransmitResult TransmitFile(string localFile, string remoteServer, string userId, string password, string fromPartner, string toPartner)
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

            try
            {
                var handler = new NaesbHandler();
                handler.UploadFile(localFile, remoteServer, userId, password, fromPartner, toPartner);

                return new TransmitResult
                {
                    Message = handler.PostText,
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
using System;
using System.Collections;
using System.IO;

namespace Ista.FileServices.Infrastructure.TransmitProtocols.Handlers
{
    public class GisbHandler
    {
        public string PostText { get; private set; }

        public void UploadFile(string fileName, string uploadUrl, string userName, string password, string fromTradingPartner, string toTradingPartner)
        {
            var file = new FileInfo(fileName);
            var collection = new Hashtable
            {
                {"from", fromTradingPartner}, 
                {"to", toTradingPartner}, 
                {"input-format", "X12"}
            };

            PostText = HttpUtility.Post(file.FullName, uploadUrl, userName, password, collection);

            if (!EvaluatePostResponse(PostText))
                throw new Exception(PostText);
        }

        public bool EvaluatePostResponse(string text)
        {
            return text.ToUpper().Contains("request-status=ok*".ToUpper());
        }
    }
}
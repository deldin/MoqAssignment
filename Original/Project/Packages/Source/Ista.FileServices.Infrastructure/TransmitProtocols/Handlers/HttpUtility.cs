using System;
using System.Collections;
using System.IO;
using System.Net;

namespace Ista.FileServices.Infrastructure.TransmitProtocols.Handlers
{
    internal static class HttpUtility
    {
        private const string NewLine = "\r\n";
        
        public struct UploadSpec
        {
            public byte[] Contents;
            public string FileName;
            public string FieldName;

            public UploadSpec(string filePath, string fieldName)
            {
                FieldName = fieldName;
                FileName = Path.GetFileName(filePath);

                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    Contents = bytes;
                }
            }
        }

        public static string Post(string fileName, string url, string userId, string password, Hashtable postBody)
        {
            var credentialCache = new CredentialCache
            {
                { new Uri(url), "Basic", new NetworkCredential(userId, password) }
            };

            try
            {
                var response = UploadFile(fileName, url, "input-data", null, credentialCache, postBody);
                if (response == null)
                    return string.Empty;

                using (var stream = response.GetResponseStream())
                {
                    if (stream == null)
                        return string.Empty;

                    using (var reader = new StreamReader(stream))
                        return reader.ReadToEnd().Trim();
                }
            }
            catch (Exception ex)
            {
                var message =
                    string.Format(
                        "Http File Upload of file \"{0}\" to URL \"{1}\" was not successful. Message: \"{2}\".",
                        fileName, url, ex.Message);

                throw new Exception(message);
            }
        }

        public static HttpWebResponse Upload(string url, CookieContainer cookies, CredentialCache credentials, Hashtable collection, params UploadSpec[] objects)
        {
            const string boundary = "-----------------------8008732200----";
            const string naesbBoundary = "ista-na";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 120000;
            request.KeepAlive = true;
            request.PreAuthenticate = true;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Method = "POST";
            request.ContentType = string.Concat("multipart/form-data; boundary=", boundary);

            if (cookies != null)
                request.CookieContainer = cookies;

            if (credentials != null)
                request.Credentials = credentials;

            ServicePointManager.Expect100Continue = false;

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                foreach (var item in objects)
                {
                    string version;
                    WriteFormFields(writer, collection, boundary, out version);

                    writer.Write("--{0}{1}", boundary, NewLine);
                    writer.Write("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}",
                        item.FieldName, item.FileName, NewLine);

                    if (version == "1.6")
                    {
                        writer.Write(
                            "Content-Type: multipart/encrypted; boundary={0}; protocol=\"application/pgp-encrypted\"{1}{1}",
                            naesbBoundary, NewLine);
                        writer.Write("--{0}{1}", naesbBoundary, NewLine);
                        writer.Write("Content-Type: application/pgp-encrypted{0}{0}", NewLine);
                        writer.Write("Version: 1{0}{0}", NewLine);
                        writer.Write("--{0}{1}", naesbBoundary, NewLine);
                    }

                    writer.Write("Content-Type: application/octet-stream{0}{0}", NewLine);
                    writer.Flush();

                    stream.Write(item.Contents, 0, item.Contents.Length);

                    writer.Write(NewLine);

                    if (version == "1.6")
                        writer.Write("--{0}--{1}", naesbBoundary, NewLine);
                }

                writer.Write("--{0}--{1}", boundary, NewLine);
                writer.Flush();

                request.ContentLength = stream.Length;
                using (var requestStream = request.GetRequestStream())
                {
                    stream.WriteTo(requestStream);
                }

                return (request.GetResponse() as HttpWebResponse);
            }
        }

        public static HttpWebResponse UploadFile(string pathname, string url, string fieldName, CookieContainer cookies, CredentialCache credentials, Hashtable collection)
        {
            return Upload(url, cookies, credentials, collection, new UploadSpec(pathname, fieldName));
        }

        private static void WriteFormFields(TextWriter writer, Hashtable collection, string boundary, out string version)
        {
            version = "1.4";

            foreach (string key in collection.Keys)
            {
                writer.Write("--{0}{1}", boundary, NewLine);
                writer.Write("Content-Disposition: form-data; name=\"{0}\"", key);
                writer.Write("{0}{0}", NewLine);
                writer.Write((string)collection[key]);
                writer.Write(NewLine);

                if (key == "version")
                    version = (string)collection[key];
            }
        }
    }
}
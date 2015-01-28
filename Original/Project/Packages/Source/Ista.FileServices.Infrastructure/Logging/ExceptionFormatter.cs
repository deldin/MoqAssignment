using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace Ista.FileServices.Infrastructure.Logging
{
    public class ExceptionFormatter
    {
        private const string Separator = "*********************************************";

        private static readonly HashAlgorithm algorithm = MD5.Create();

        public static string ComputeHash(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var bytes = Encoding.UTF8.GetBytes(value);
            var hash = algorithm.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static void AddAdditionalInformation(Dictionary<string, object> meta)
        {
            if (!meta.ContainsKey("TimeStamp"))
                meta["TimeStamp"] = DateTime.Now;

            meta["MachineName"] = Environment.MachineName;
            meta["AppDomainName"] = AppDomain.CurrentDomain.FriendlyName;
            meta["ThreadIdentity"] = Thread.CurrentPrincipal.Identity.Name;

            try
            {
                var identity = WindowsIdentity.GetCurrent();
                if (identity != null)
                    meta["WindowsIdentity"] = identity.Name;
            }
            catch
            {
                meta["WindowsIdentity"] = "Information could not be accessed.";
            }

            try
            {
                var trace = new StackTrace(2, true);
                meta["ExceptionPublisherStack"] = Environment.NewLine + trace;
            }
            catch
            {
                meta["ExceptionPublisherStack"] = "Information could not be accessed.";
            }
        }

        public static string FormatText(Exception exception, IDictionary<object, object> meta)
        {
            var buffer = new StringBuilder();
            if (meta != null && meta.Count != 0)
            {
                buffer
                    .AppendLine()
                    .Append("General Information ")
                    .AppendLine()
                    .Append(Separator)
                    .AppendLine()
                    .Append("Additional Info:");

                foreach (var metaPair in meta)
                {
                    buffer
                        .AppendLine()
                        .AppendFormat("{0}: {1}", metaPair.Key, metaPair.Value);
                }
            }

            if (exception == null)
            {
                return buffer
                    .AppendLine()
                    .AppendLine()
                    .Append("No Exception object has been provided.")
                    .AppendLine()
                    .ToString();
            }

            var count = 1;
            do
            {
                var exceptionType = exception.GetType();

                buffer
                    .AppendLine()
                    .AppendLine()
                    .AppendFormat("{0}) Exception Information", count)
                    .AppendLine()
                    .Append(Separator)
                    .AppendLine()
                    .AppendFormat("Exception Type: {0}", exceptionType.FullName);

                var properties = exceptionType.GetProperties();
                foreach (var property in properties)
                {
                    var name = property.Name;
                    if (name.Equals("InnerException", StringComparison.Ordinal) ||
                        name.Equals("StackTrace", StringComparison.Ordinal))
                        continue;

                    var value = property.GetValue(exception, null);
                    if (value == null)
                    {
                        buffer
                            .AppendLine()
                            .AppendFormat("{0}: NULL", name);
                        continue;
                    }

                    if (name.Equals("Data", StringComparison.OrdinalIgnoreCase) && value is IDictionary)
                    {
                        var dictionary = value as IDictionary;
                        if (dictionary.Count == 0)
                        {
                            buffer
                                .AppendLine()
                                .AppendFormat("{0}: None", name);

                            continue;
                        }

                        buffer
                            .AppendLine()
                            .AppendFormat("{0}: ", name);

                        foreach (string key in dictionary.Keys)
                            buffer.AppendFormat("{0} = {1}, ", key, dictionary[key]);

                        buffer.Remove(buffer.Length - 2, 2);
                        continue;
                    }

                    buffer
                        .AppendLine()
                        .AppendFormat("{0}: {1}", name, value);
                }

                if (exception.StackTrace != null)
                {
                    buffer
                        .AppendLine()
                        .AppendLine()
                        .Append("StackTrace Information")
                        .AppendLine()
                        .Append(Separator)
                        .AppendLine()
                        .Append(exception.StackTrace)
                        .AppendLine();
                }

                exception = exception.InnerException;
                count++;
            } while (exception != null);

            return buffer
                .AppendLine()
                .AppendLine()
                .AppendLine()
                .ToString();
        }
    }
}

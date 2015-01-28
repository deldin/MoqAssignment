using System;
using System.Text;
using Ista.FileServices.Infrastructure.Encoders;

namespace Ista.FileServices.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        private static readonly Encoding encoder = Encoding.GetEncoding(20127,
            new EncoderBestAttemptFallback(Encoding.ASCII),
            new DecoderExceptionFallback());

        public static string AtPosition(this string source, int startIndex, int length)
        {
            if (source.Length < (startIndex + length))
                return string.Empty;

            return source.Substring(startIndex, length).Trim();
        }

        public static int AtPositionInt(this string source, int startIndex, int length)
        {
            if (source.Length < (startIndex + length))
                return 0;

            var value = source.Substring(startIndex, length).Trim();

            int parsedValue;
            if (int.TryParse(value, out parsedValue))
                return parsedValue;

            return 0;
        }

        public static decimal AtPositionDecimal(this string source, int startIndex, int length)
        {
            return AtPositionDecimal(source, startIndex, length, 0);
        }

        public static decimal AtPositionDecimal(this string source, int startIndex, int length, int scale)
        {
            if (source.Length < (startIndex + length))
                return 0m;

            var value = source.Substring(startIndex, length).Trim();

            decimal parsedValue;
            if (decimal.TryParse(value, out parsedValue))
            {
                var divisor = (decimal)Math.Pow(10, scale);
                return parsedValue / divisor;
            }

            return 0m;
        }

        public static string ToAscii(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return string.Empty;

            var byteCount = encoder.GetByteCount(source);
            var bytes = new byte[byteCount];

            encoder.GetBytes(source, 0, source.Length, bytes, 0);
            return encoder.GetString(bytes);
        }
        
        public static void TryAtPosition(this string source, int startIndex, int length, Action<string> command)
        {
            if (source.Length < (startIndex + length))
                return;

            var value = source.Substring(startIndex, length).Trim();
            if (string.IsNullOrWhiteSpace(value))
                return;

            command(value);
        }

        public static void TryAtPositionInt(this string source, int startIndex, int length, Action<int> command)
        {
            if (source.Length < (startIndex + length))
                return;

            var value = source.Substring(startIndex, length).Trim();

            int parsedValue;
            if (int.TryParse(value, out parsedValue))
                command(parsedValue);
        }

        public static void TryAtPositionDecimal(this string source, int startIndex, int length, Action<decimal> command)
        {
            if (source.Length < (startIndex + length))
                return;

            var value = source.Substring(startIndex, length).Trim();

            decimal parsedValue;
            if (decimal.TryParse(value, out parsedValue))
                command(parsedValue);
        }
    }
}

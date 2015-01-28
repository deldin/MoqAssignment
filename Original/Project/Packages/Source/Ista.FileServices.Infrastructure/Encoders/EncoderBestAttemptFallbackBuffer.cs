using System;
using System.Text;

namespace Ista.FileServices.Infrastructure.Encoders
{
    public class EncoderBestAttemptFallbackBuffer : EncoderFallbackBuffer
    {
        private readonly Encoding encoder;

        private int currentIndex;
        private string fallback;

        public override int Remaining
        {
            get
            {
                if (currentIndex < fallback.Length)
                    return fallback.Length - currentIndex;
                return 0;
            }
        }

        public EncoderBestAttemptFallbackBuffer(Encoding encoder)
        {
            this.encoder = encoder;

            currentIndex = 1;
            fallback = string.Empty;
        }

        public override bool Fallback(char charUnknown, int index)
        {
            var sample = new string(charUnknown, 1);
            return Fallback(sample);
        }

        public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
        {
            var sample = new string(new[] { charUnknownHigh, charUnknownLow });
            return Fallback(sample);
        }

        public override char GetNextChar()
        {
            if (currentIndex >= fallback.Length)
            {
                if (currentIndex == fallback.Length)
                    currentIndex++;
                return '\0';
            }

            return fallback[currentIndex];
        }

        public override bool MovePrevious()
        {
            if (currentIndex <= fallback.Length)
                currentIndex--;

            return (currentIndex >= 0 || currentIndex < fallback.Length);
        }

        private bool Fallback(string concern)
        {
            if (currentIndex <= fallback.Length)
            {
                currentIndex = 1;
                fallback = string.Empty;

                throw new ArgumentException("Unexpected recursive fallback sequence.", "concern");
            }

            var normal = string.Empty;
            try
            {
                normal = concern.Normalize(NormalizationForm.FormKC);
                if (normal.Equals(concern))
                    normal = string.Empty;
            }
            catch (ArgumentException) { }

            var normalBytes = encoder.GetBytes(normal);
            fallback = encoder.GetString(normalBytes);
            if (fallback.Length == 0 || fallback[0] != normal[0])
                fallback = string.Empty;

            currentIndex = 0;
            return true;
        }
    }
}

using System.Text;

namespace Ista.FileServices.Infrastructure.Encoders
{
    public class EncoderBestAttemptFallback : EncoderFallback
    {
        private readonly Encoding encoder;

        public override int MaxCharCount
        {
            get { return 18; }
        }

        public EncoderBestAttemptFallback(Encoding encoder)
        {
            this.encoder = encoder;
        }

        public override EncoderFallbackBuffer CreateFallbackBuffer()
        {
            return new EncoderBestAttemptFallbackBuffer(encoder);
        }
    }
}

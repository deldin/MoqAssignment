using Ista.FileServices.Infrastructure.Interfaces;

namespace Ista.FileServices.Infrastructure.Queuing
{
    public class IstaMessage : IIstaMessage
    {
        public ulong MessageId { get; set; }
        public string Action { get; set; }
        public string Type { get; set; }
        public string CorrelationId { get; set; }
        public string ConsumerId { get; set; }
        public string Exchange { get; set; }
        public string ExchangeRoute { get; set; }
        public byte[] Body { get; set; }
        public bool Redelivered { get; set; }
        public IIstaMessageProperties Properties { get; set; }
    }
}
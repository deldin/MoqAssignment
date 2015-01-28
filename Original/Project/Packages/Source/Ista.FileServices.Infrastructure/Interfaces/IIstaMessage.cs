namespace Ista.FileServices.Infrastructure.Interfaces
{
    public interface IIstaMessage
    {
        ulong MessageId { get; }
        string Action { get; }
        string Type { get; }
        string CorrelationId { get; }
        string ConsumerId { get; }
        string Exchange { get; }
        string ExchangeRoute { get; }
        byte[] Body { get; }
        bool Redelivered { get; }
        IIstaMessageProperties Properties { get; }
    }
}

namespace Ista.Miramar.Interfaces
{
    public interface IMiramarClientInfo
    {
        int ClientId { get; }
        string Client { get; }
        string AdminConnection { get; }
        string ClientConnection { get; }
        string MarketConnection { get; }
    }
}

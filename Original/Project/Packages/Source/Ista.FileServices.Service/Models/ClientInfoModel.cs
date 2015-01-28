using Ista.Miramar.Interfaces;

namespace Ista.FileServices.Service.Models
{
    public class ClientInfoModel : IMiramarClientInfo
    {
        public int ClientId { get; set; }
        public string Client { get; set; }
        public string AdminConnection { get; set; }
        public string ClientConnection { get; set; }
        public string MarketConnection { get; set; }
    }
}

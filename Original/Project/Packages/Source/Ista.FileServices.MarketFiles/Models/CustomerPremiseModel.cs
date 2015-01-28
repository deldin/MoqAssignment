
namespace Ista.FileServices.MarketFiles.Models
{
    public class CustomerPremiseModel
    {
        public int CustId { get; set; }
        public string MeterNo { get; set; }
        public string CustName { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string EdiInfo1 { get; set; }
        public string EdiInfo2 { get; set; }
    }
}
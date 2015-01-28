
namespace Ista.FileServices.MarketFiles.Models
{
    public class CustomerDetailModel
    {
        public int CustId { get; set; }
        public string CustName { get; set; }
        public string CustNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string RemitAddress1 { get; set; }
        public string RemitAddress2 { get; set; }
        public string RemitCity { get; set; }
        public string RemitState { get; set; }
        public string RemitZip { get; set; }
    }
}
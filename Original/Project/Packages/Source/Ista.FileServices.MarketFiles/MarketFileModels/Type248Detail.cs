using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type248Detail : IType248Model
    {
        public Type248Types ModelType
        {
            get { return Type248Types.Detail; }
        }

        public int? DetailKey { get; set; }
        public int HeaderKey { get; set; }
        public string BalanceAmount { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTelephone1 { get; set; }
        public string CustomerTelephone2 { get; set; }
        public string ESIID { get; set; }
        public string ESPAccountNbr { get; set; }
        public string HierarchicalID { get; set; }
        public string HierarchicalLevelCode { get; set; }
        public string InvoiceAmount { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceNbr { get; set; }
        public string MarketerCustomerAccountNumber { get; set; }
        public string OldLdcAccountNbr { get; set; }
        public string ReinstatementDate { get; set; }
        public string ServiceTypeCode { get; set; }
        public string WriteOffAccountNbr { get; set; }
        public string WriteOffDate { get; set; }
    }
}
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type568Detail : IType568Model
    {
        public Type568Types ModelType
        {
            get { return Type568Types.Detail; }
        }

        public int? DetailKey { get; set; }
        public int HeaderKey { get; set; }
        public string Amount { get; set; }
        public string AssignedNumber { get; set; }
        public string CustomerName { get; set; }
        public string ESIID { get; set; }
        public string ESPAccountNbr { get; set; }
        public string GasPoolID { get; set; }
        public string MarketerCustomerAccountNumber { get; set; }
        public string OldEspAccountNbr { get; set; }
        public string PlanQualifier { get; set; }
        public string PlanTypeCode { get; set; }
        public string ReferenceDate { get; set; }
        public string ReferenceDescription { get; set; }
        public string ReferenceQualifier { get; set; }
        public string ServiceTypeCode { get; set; }
        public string TotalAmount { get; set; }
        public string TransactionReferenceNbr { get; set; }
    }
}
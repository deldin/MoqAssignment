using System;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type820Detail : IType820Model
    {
        public Type820Types ModelType
        {
            get { return Type820Types.Detail; }
        }

        public int? DetailKey { get; set; }
        public int HeaderKey { get; set; }
        public string AdjustmentAmount { get; set; }
        public string AdjustmentReasonCode { get; set; }
        public string AssignedId { get; set; }
        public string CommodityCode { get; set; }
        public string CrossReferenceNbr { get; set; }
        public string CustomerName { get; set; }
        public string DatePosted { get; set; }
        public string DiscountAmount { get; set; }
        public string EsiId { get; set; }
        public string ESPAccountNumber { get; set; }
        public string InvoiceAmount { get; set; }
        public string PaymentActionCode { get; set; }
        public string PaymentAmount { get; set; }
        public string PrevUtilityAccountNumber { get; set; }
        public string ReferenceId { get; set; }
        public string ReferenceNbr { get; set; }
        public string UnmeteredServiceDesignator { get; set; }
        public int ProcessFlag { get; set; }
        public DateTime ProcessDate { get; set; }
    }
}

namespace Ista.FileServices.MarketFiles.Models
{
    public class CustomerInvoiceConfigModel
    {
        public int LdcId { get; set; }
        public string CustomerDuns { get; set; }
        public bool BtCustomAddressLine { get; set; }
        public string BtEntityName { get; set; }
        public string BtEntityId { get; set; }
        public string BtAttn { get; set; }
        public string BtEntityAddress1 { get; set; }
        public string BtEntityAddress2 { get; set; }
        public string BtEntityCity { get; set; }
        public string BtEntityState { get; set; }
        public string BtEntityZip { get; set; }
        public bool ReAddressLine { get; set; }
        public string ReEntityName { get; set; }
        public string ReEntityId { get; set; }
        public string ReEntityAddress1 { get; set; }
        public string ReEntityAddress2 { get; set; }
        public string ReEntityCity { get; set; }
        public string ReEntityState { get; set; }
        public string ReEntityZip { get; set; }
        public bool AggregateByChargeIndicator { get; set; }
        public bool AggregateByChargeCode { get; set; }
        public bool AggregateByUom { get; set; }
        public bool AggregateByDescription { get; set; }
        public bool TaxesAsCharge { get; set; }
        public bool IncludeTaxesInTotal { get; set; }
        public bool CalculateChargeIndicator { get; set; }
        public bool UseAccNumberForInvoice { get; set; }
    }
}
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class TypeCustomerInfoRecord : ITypeCustomerInfoModel
    {
        public TypeCustomerInfoTypes ModelType
        {
            get { return TypeCustomerInfoTypes.Record; }
        }

        public int FileId { get; set; }
        public int? RecordId { get; set; }
        public int PremId { get; set; }
        public int SequenceId { get; set; }
        public string CustNo { get; set; }
        public string PremNo { get; set; }
        public string CrDuns { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string BillingCareOfName { get; set; }
        public string BillingAddress1 { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingPostalCode { get; set; }
        public string BillingCountryCode { get; set; }
        public string PrimaryTelephone { get; set; }
        public string PrimaryTelephoneExt { get; set; }
        public string SecondaryTelephone { get; set; }
        public string SecondaryTelephoneExt { get; set; }
        public string Email { get; set; }
    }
}
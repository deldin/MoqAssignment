using System;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class TypeCustomerInfoErrorRecord : ITypeCustomerInfoModel
    {
        public TypeCustomerInfoTypes ModelType
        {
            get { return TypeCustomerInfoTypes.ErrorRecord; }
        }

        public int FileId { get; set; }
        public int? ErrorRecordId { get; set; }
        public int RecordTypeId { get; set; }
        public int RecordId { get; set; }
        public int PremId { get; set; }
        public int CustId { get; set; }
        public int UserId { get; set; }
        public string ErrorMessage { get; set; }
        public string FieldName { get; set; }
        public string PremNo { get; set; }
        public bool IsCleared { get; set; }
        public DateTime ClearedDate { get; set; }
    }
}
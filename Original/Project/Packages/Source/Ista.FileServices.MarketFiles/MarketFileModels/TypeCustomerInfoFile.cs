using System;
using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class TypeCustomerInfoFile : ITypeCustomerInfoModel, IMarketHeaderModel
    {
        private readonly List<TypeCustomerInfoErrorRecord> errorRecords;

        public TypeCustomerInfoTypes ModelType
        {
            get { return TypeCustomerInfoTypes.File; }
        }

        public int CspDunsId { get; set; }
        public int FileId { get; set; }
        public int FileTypeId { get; set; }
        public int? UserId { get; set; }
        public string CrDuns { get; set; }
        public string FileNumber { get; set; }
        public string FileName { get; set; }
        public string ReferenceNumber { get; set; }
        public CustomerInfoFileStatusOptions Status { get; set; }
        public DateTime? StatusDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool Direction { get; set; }
        public int ErrorCount { get; set; }
        public int RecordCount { get; set; }

        public TypeCustomerInfoErrorRecord[] ErrorRecords
        {
            get { return errorRecords.ToArray(); }
        }

        public TypeCustomerInfoFile()
        {
            errorRecords = new List<TypeCustomerInfoErrorRecord>();
        }

        public void AddError(TypeCustomerInfoErrorRecord item)
        {
            errorRecords.Add(item);
        }
    }
}

using System;
using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type248Header : IType248Model, IMarketHeaderModel
    {
        private readonly List<Type248Detail> details;

        public Type248Types ModelType
        {
            get { return Type248Types.Header; }
        }

        public int? HeaderKey { get; set; }
        public int MarketFileId { get; set; }
        public string ControlNbr { get; set; }
        public string CrDuns { get; set; }
        public string CrName { get; set; }
        public string LDCDuns { get; set; }
        public string LDCName { get; set; }
        public string SegmentCount { get; set; }
        public string StructureCode { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionReferenceNbr { get; set; }
        public string TransactionSetPurposeCode { get; set; }
        public string TransactionTypeCode { get; set; }
        public bool Direction { get; set; }
        public bool ProcessFlag { get; set; }
        public DateTime ProcessDate { get; set; }

        public Type248Detail[] Details
        {
            get { return details.ToArray(); }
        }

        public Type248Header()
        {
            details = new List<Type248Detail>();
        }

        public void AddDetail(Type248Detail detail)
        {
            details.Add(detail);
        }
    }
}
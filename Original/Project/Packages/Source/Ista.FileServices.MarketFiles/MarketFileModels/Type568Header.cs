using System;
using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type568Header : IType568Model, IMarketHeaderModel
    {
        private readonly List<Type568Detail> details;

        public Type568Types ModelType
        {
            get { return Type568Types.Header; }
        }

        public int? HeaderKey { get; set; }
        public int MarketFileId { get; set; }
        public string ControlNbr { get; set; }
        public string CrDuns { get; set; }
        public string CrName { get; set; }
        public string LDCDuns { get; set; }
        public string LDCName { get; set; }
        public string MonetaryAmount { get; set; }
        public string SegmentCount { get; set; }
        public string SystemDate { get; set; }
        public string TransactionReferenceNbr { get; set; }
        public string TransactionSetPurposeCode { get; set; }
        public string TransactionTypeCode { get; set; }
        public bool Direction { get; set; }
        public bool ProcessFlag { get; set; }
        public DateTime ProcessDate { get; set; }

        public Type568Detail[] Details
        {
            get { return details.ToArray(); }
        }

        public Type568Header()
        {
            details = new List<Type568Detail>();
        }

        public void AddDetail(Type568Detail detail)
        {
            details.Add(detail);
        }
    }
}
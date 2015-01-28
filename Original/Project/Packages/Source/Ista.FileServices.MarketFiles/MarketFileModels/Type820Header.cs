using System;
using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type820Header : IType820Model, IMarketHeaderModel
    {
        private readonly List<Type820Detail> details;

        public Type820Types ModelType
        {
            get { return Type820Types.Header; }
        }

        public int? HeaderKey { get; set; }
        public int MarketFileId { get; set; }
        public string CrDuns { get; set; }
        public string CreateDate { get; set; }
        public string CreditDebitFlag { get; set; }
        public string CrName { get; set; }
        public string ESPUtilityAccountNumber { get; set; }
        public string PaymentMethodCode { get; set; }
        public string TdspDuns { get; set; }
        public string TdspDunsStructureCode { get; set; }
        public string TdspName { get; set; }
        public string TotalAmount { get; set; }
        public string TraceReferenceNbr { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionNbr { get; set; }
        public string TransactionSetControlNbr { get; set; }
        public string TransactionSetId { get; set; }
        public string TransactionTypeCode { get; set; }
        public int TransactionTypeId { get; set; }
        public int MarketId { get; set; }
        public int ProviderId { get; set; }
        public bool ProcessFlag { get; set; }
        public DateTime ProcessDate { get; set; }
        public bool Direction { get; set; }

        public Type820Detail[] Details
        {
            get { return details.ToArray(); }
        }

        public Type820Header()
        {
            details = new List<Type820Detail>();
        }

        public void AddDetail(Type820Detail detail)
        {
            details.Add(detail);
        }
    }
}
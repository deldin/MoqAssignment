using System;
using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type824Header : IType824Model, IMarketHeaderModel
    {
        private readonly List<Type824Reason> reasons;
        private readonly List<Type824Reference> references; 

        public Type824Types ModelType
        {
            get { return Type824Types.Header; }
        }

        public int? HeaderKey { get; set; }
        public int MarketFileId { get; set; }
        public string TransactionSetId { get; set; }
        public string TransactionSetControlNbr { get; set; }
        public string TransactionSetPurposeCode { get; set; }
        public string TransactionNbr { get; set; }
        public string TransactionDate { get; set; }
        public string ReportTypeCode { get; set; }
        public string ActionCode { get; set; }
        public string TdspDuns { get; set; }
        public string TdspName { get; set; }
        public string CrDuns { get; set; }
        public string CrName { get; set; }
        public string AppAckCode { get; set; }
        public string ReferenceNbr { get; set; }
        public string TransactionSetNbr { get; set; }
        public string EsiId { get; set; }
        public int ProcessFlag { get; set; }
        public DateTime ProcessDate { get; set; }
        public bool Direction { get; set; }
        public int TransactionTypeId { get; set; }
        public int MarketId { get; set; }
        public int ProviderId { get; set; }
        public string CrQualifier { get; set; }
        public string TdspQualifier { get; set; }
        public string EspUtilityAccountNumber { get; set; }
        public string CustomerName { get; set; }
        public string EspCustomerAccountNumber { get; set; }
        public string PreviousUtilityAccountNumber { get; set; }

        public Type824Reason[] Reasons
        {
            get { return reasons.ToArray(); }
        }

        public Type824Reference[] References
        {
            get { return references.ToArray(); }
        }

        public Type824Header()
        {
            reasons = new List<Type824Reason>();
            references = new List<Type824Reference>();
        }

        public void AddReason(Type824Reason item)
        {
            reasons.Add(item);
        }

        public void AddReference(Type824Reference item)
        {
            references.Add(item);
        }
    }
}

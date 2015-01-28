using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type824Reference : IType824Model
    {
        private readonly List<Type824TechError> techErrors;

        public Type824Types ModelType
        {
            get { return Type824Types.Reference; }
        }

        public int HeaderKey { get; set; }
        public int? ReferenceKey { get; set; }
        public string AppAckCode { get; set; }
        public string ReferenceQualifier { get; set; }
        public string ReferenceNbr { get; set; }
        public string TransactionSetId { get; set; }
        public string CrossReferenceNbr { get; set; }
        public string PurchaseOrderNbr { get; set; }
        public string PaymentsAppliedThroughDate { get; set; }
        public string TotalPaymentsApplied { get; set; }
        public string PaymentDueDate { get; set; }
        public string TotalAmountDue { get; set; }

        public Type824TechError[] TechErrors
        {
            get { return techErrors.ToArray(); }
        }

        public Type824Reference()
        {
            techErrors = new List<Type824TechError>();
        }

        public void AddTechError(Type824TechError item)
        {
            techErrors.Add(item);
        }
    }
}
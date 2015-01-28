using System;
using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type810Header : IType810Model, IMarketHeaderModel
    {
        private readonly List<Type810Detail> details;
        private readonly List<Type810Balance> balances;
        private readonly List<Type810Message> messages;
        private readonly List<Type810Name> names;
        private readonly List<Type810Note> notes;
        private readonly List<Type810Payment> payments;
        private readonly List<Type810Summary> summaries; 

        public Type810Types ModelType
        {
            get { return Type810Types.Header; }
        }

        public int? HeaderKey { get; set; }
        public int MarketFileId { get; set; }
        public string TransactionSetId { get; set; }
        public string TransactionSetControlNbr { get; set; }
        public string TransactionDate { get; set; }
        public string InvoiceNbr { get; set; }
        public string ReleaseNbr { get; set; }
        public string TransactionTypeCode { get; set; }
        public string TransactionSetPurposeCode { get; set; }
        public string OriginalInvoiceNbr { get; set; }
        public string EsiId { get; set; }
        public string CrAccountNumber { get; set; }
        public string PaymentDueDate { get; set; }
        public string TdspDuns { get; set; }
        public string TdspName { get; set; }
        public string CrDuns { get; set; }
        public string CrName { get; set; }
        public string TotalAmount { get; set; }
        public int ProcessFlag { get; set; }
        public DateTime ProcessDate { get; set; }
        public bool ProcessFlag820 { get; set; }
        public string ProcessDate820 { get; set; }
        public bool Direction { get; set; }
        public int? TransasctionTypeId { get; set; }
        public int? MarketId { get; set; }
        public int? ProviderId { get; set; }
        public string CustNoForESCO { get; set; }
        public string PreviousUtilityAccountNumber { get; set; }
        public string BillPresenter { get; set; }
        public string BillCalculator { get; set; }
        public string GasPoolId { get; set; }
        public string CustomerDUNS { get; set; }
        public string ChangeCode { get; set; }
        public string ChangeCodeDesc { get; set; }
        public string BillingCycleNumber { get; set; }
        public string InvoicePeriodStart { get; set; }
        public string InvoicePeriodEnd { get; set; }
        public string AlternateEsiId { get; set; }
        public string ServiceDeliveryPoint { get; set; }
   
        public Type810Detail[] Details
        {
            get { return details.ToArray(); }
        }

        public Type810Balance[] Balances
        {
            get { return balances.ToArray(); }
        }

        public Type810Message[] Messages
        {
            get { return messages.ToArray(); }
        }

        public Type810Name[] Names
        {
            get { return names.ToArray(); }
        }

        public Type810Note[] Notes
        {
            get { return notes.ToArray(); }
        }

        public Type810Payment[] Payments
        {
            get { return payments.ToArray(); }
        }

        public Type810Summary[] Summaries
        {
            get { return summaries.ToArray(); }
        }

        public Type810Header()
        {
            details = new List<Type810Detail>();
            balances = new List<Type810Balance>();
            messages = new List<Type810Message>();
            names = new List<Type810Name>();
            notes = new List<Type810Note>();
            payments = new List<Type810Payment>();
            summaries = new List<Type810Summary>();
        }

        public void AddDetail(Type810Detail item)
        {
            details.Add(item);
        }

        public void AddBalance(Type810Balance item)
        {
            balances.Add(item);
        }

        public void AddMessage(Type810Message item)
        {
            messages.Add(item);
        }

        public void AddName(Type810Name item)
        {
            names.Add(item);
        }

        public void AddNote(Type810Note item)
        {
            notes.Add(item);
        }

        public void AddPayment(Type810Payment item)
        {
            payments.Add(item);
        }

        public void AddSummary(Type810Summary item)
        {
            summaries.Add(item);
        }
    }
}

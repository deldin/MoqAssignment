using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Parsers.ImportContexts
{
    public class Prism867Context
    {
        private readonly Import867Model model; 
        private readonly Stack<IType867Model> stack;

        private int marketId;
        private MarketOptions marketOption;

        private bool valid;
        private string message;

        public string Alias { get; set; }
        public string TransactionNumber { get; set; }
        public string RecordType { get; set; }
        public string Qualifier { get; set; }
        public string Quantity { get; set; }
        public string Uom { get; set; }
        public string ReportTypeCode { get; set; }
        public string EsiId { get; set; }
        public string R05ServicePeriodBeginDate { get; set; }
        public string R05ServicePeriodEndDate { get; set; }
        public string R07ServicePeriodBeginDate { get; set; }
        public string R07ServicePeriodEndDate { get; set; }
        
        public int MarketFileId { get; set; }

        public int MarketId
        {
            get { return marketId; }
        }

        public MarketOptions Market
        {
            get { return marketOption; }
        }

        public IType867Model Current
        {
            get { return (stack.Count == 0) ? null : stack.Peek(); }
        }

        public int TransactionActualCount 
        {
            get { return model.TransactionActualCount; }
            set { model.TransactionActualCount = value; }
        }
        
        public int TransactionAuditCount
        {
            get { return model.TransactionAuditCount; }
            set { model.TransactionAuditCount = value; }
        }

        public bool IsValid
        {
            get { return valid; }
        }

        public string Message
        {
            get { return message; }
        }

        public bool ShouldResolve
        {
            get { return (stack.Count != 0); }
        }

        public Import867Model Results
        {
            get { return model; }
        }

        public Prism867Context()
        {
            model = new Import867Model();
            stack = new Stack<IType867Model>();
            valid = true;
        }

        public void Initialize()
        {
            TransactionNumber = string.Empty;
            Qualifier = string.Empty;
            Quantity = string.Empty;
            R05ServicePeriodBeginDate = string.Empty;
            R05ServicePeriodEndDate = string.Empty;
            R07ServicePeriodBeginDate = string.Empty;
            R07ServicePeriodEndDate = string.Empty;
            Uom = string.Empty;
            ReportTypeCode = string.Empty;
            EsiId = string.Empty;
            RecordType = string.Empty;

            marketOption = MarketOptions.Unknown;
            marketId = (int)MarketOptions.Unknown;
        }

        public void InitializeAccountBillQuantity()
        {
            Qualifier = string.Empty;
            Quantity = string.Empty;
            Uom = string.Empty;
        }

        public void InitializeMeterServiceSummary()
        {
            RecordType = string.Empty;
            R07ServicePeriodBeginDate = string.Empty;
            R07ServicePeriodEndDate = string.Empty;
        }

        public void InitializeNetIntervalQuantity(string qualifier, string quantity, string uom)
        {
            Qualifier = qualifier;
            Quantity = quantity;
            Uom = uom;
        }

        public void PushModel(IType867Model item)
        {
            stack.Push(item);
        }

        public void ResolveToHeader()
        {
            if (stack.Count == 0)
                return;

            var item = stack.Pop();
            while (item.ModelType != Type867Types.Header)
            {
                if (stack.Count == 0)
                    return;

                item = stack.Pop();
            }

            var header = item as Type867Header;
            if (header == null)
                return;

            model.AddHeader(header);
        }

        public void RevertToHeader()
        {
            if (stack.Count == 0)
                return;

            var currentItem = stack.Peek();
            if (currentItem.ModelType == Type867Types.Header)
                return;

            while (currentItem.ModelType != Type867Types.Header)
            {
                stack.Pop();
                if (stack.Count == 0)
                    return;

                currentItem = stack.Peek();
            }
        }

        public bool LastModelAddedIsDetail()
        {
            return IsType867Detail(stack.Peek().ModelType);
        }

        public void SetMarket(int id)
        {
            marketId = id;
            marketOption = (MarketOptions)id;
        }

        public bool IsType867Detail(Type867Types type)
        {
            switch (type)
            {
                case Type867Types.AccountBillQty:
                case Type867Types.IntervalDetail:
                case Type867Types.IntervalSummary:
                case Type867Types.IntervalSummaryAcrossMeters:
                case Type867Types.NetIntervalSummary:
                case Type867Types.NonIntervalDetail:
                case Type867Types.NonIntervalSummary:
                case Type867Types.UnMeterDetail:
                case Type867Types.UnMeterSummary:
                case Type867Types.Switch:
                    return true;
            }

            return false;
        }

        public void MarkAsInvalid(string reason)
        {
            valid = false;
            message = reason;
        }
    }
}

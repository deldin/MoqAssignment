using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Parsers.ImportContexts
{
    public class Prism824Context
    {
        private readonly Import824Model model;
        private readonly Stack<IType824Model> stack;

        private int marketId;
        private MarketOptions marketOption;

        public string Alias { get; set; }
        public string TransactionTypeCode { get; set; }
        public string TransactionNumber { get; set; }
        public string TransactionDate { get; set; }
        public string TdspName { get; set; }
        public string TdspDuns { get; set; }
        public string CrName { get; set; }
        public string CrDuns { get; set; }
        public string PremiseNumber { get; set; }

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

        public IType824Model Current
        {
            get { return (stack.Count == 0) ? null : stack.Peek(); }
        }

        public MarketOptions Market
        {
            get { return marketOption; }
        }

        public int MarketId
        {
            get { return marketId; }
        }

        public bool ShouldResolve
        {
            get { return (stack.Count != 0); }
        }

        public Import824Model Results
        {
            get { return model; }
        }

        public Prism824Context()
        {
            model = new Import824Model();
            stack = new Stack<IType824Model>();
        }

        public void Initialize()
        {
            Alias = string.Empty;
            TransactionTypeCode = string.Empty;
            TransactionNumber = string.Empty;
            TransactionDate = string.Empty;
            TdspName = string.Empty;
            TdspDuns = string.Empty;
            CrName = string.Empty;
            CrDuns = string.Empty;
            PremiseNumber = string.Empty;

            marketOption = MarketOptions.Unknown;
            marketId = (int)MarketOptions.Unknown;
        }

        public void PushModel(IType824Model item)
        {
            stack.Push(item);
        }

        public void ResolveToHeader()
        {
            if (stack.Count == 0)
                return;

            var item = stack.Pop();
            while (item.ModelType != Type824Types.Header)
            {
                if (stack.Count == 0)
                    return;

                item = stack.Pop();
            }

            var header = item as Type824Header;
            if (header == null)
                return;

            model.AddHeader(header);
        }

        public void SetMarket(int id)
        {
            marketId = id;
            marketOption = (MarketOptions)id;
        }
    }
}

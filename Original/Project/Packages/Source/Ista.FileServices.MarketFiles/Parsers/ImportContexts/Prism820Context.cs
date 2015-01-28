using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Parsers.ImportContexts
{
    public class Prism820Context
    {
        private readonly Import820Model model;
        private readonly Stack<IType820Model> stack;
        
        private int marketId;
        private MarketOptions marketOption;

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

        public IType820Model Current
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

        public Import820Model Results
        {
            get { return model; }
        }

        public Prism820Context()
        {
            model = new Import820Model();
            stack = new Stack<IType820Model>();
        }

        public void Initialize()
        {
            marketOption = MarketOptions.Unknown;
            marketId = (int)MarketOptions.Unknown;
        }

        public void PushModel(IType820Model item)
        {
            stack.Push(item);
        }

        public void ResolveToHeader()
        {
            if (stack.Count == 0)
                return;

            var item = stack.Pop();
            while (item.ModelType != Type820Types.Header)
            {
                if (stack.Count == 0)
                    return;

                item = stack.Pop();
            }

            var header = item as Type820Header;
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
using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Parsers.ImportContexts
{
    public class Prism650Context
    {
        private readonly Import650Model model;
        private readonly Stack<IType650Model> stack;

        private int marketId;
        private MarketOptions marketOption;

        public string Alias { get; set; }

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

        public IType650Model Current
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

        public Import650Model Results
        {
            get { return model; }
        }

        public Prism650Context()
        {
            model = new Import650Model();
            stack = new Stack<IType650Model>();
        }

        public void Initialize()
        {
            Alias = string.Empty;

            marketOption = MarketOptions.Unknown;
            marketId = (int)MarketOptions.Unknown;
        }

        public void PushModel(IType650Model item)
        {
            stack.Push(item);
        }

        public void ResolveToHeader()
        {
            if (stack.Count == 0)
                return;

            var item = stack.Pop();
            while (item.ModelType != Type650Types.Header)
            {
                if (stack.Count == 0)
                    return;

                item = stack.Pop();
            }

            var header = item as Type650Header;
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

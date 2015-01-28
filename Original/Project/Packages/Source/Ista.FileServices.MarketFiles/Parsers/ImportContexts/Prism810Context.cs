using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Parsers.ImportContexts
{
    public class Prism810Context
    {
        private readonly Import810Model model;
        private readonly Stack<IType810Model> stack;
        
        private int marketId;
        private MarketOptions marketOption;

        public string Alias { get; set; }
        public string TransactionNumber { get; set; }
        public bool FirstCharge { get; set; }

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

        public IType810Model Current
        {
            get { return (stack.Count == 0) ? null : stack.Peek(); }
        }

        public int MarketId
        {
            get { return marketId; }
        }

        public MarketOptions Market
        {
            get { return marketOption; }
        }

        public bool ShouldResolve
        {
            get { return (stack.Count != 0); }
        }

        public Import810Model Results
        {
            get { return model; }
        }

        public Prism810Context()
        {
            model = new Import810Model();
            stack = new Stack<IType810Model>();
        }

        public void Initialize()
        {
            Alias = string.Empty;
            TransactionNumber = string.Empty;
            FirstCharge = true;

            marketOption = MarketOptions.Unknown;
            marketId = (int)MarketOptions.Unknown;
        }

        public void PushModel(IType810Model item)
        {
            stack.Push(item);
        }

        public void ResolveToHeader()
        {
            if (stack.Count == 0)
                return;

            var item = stack.Pop();
            while (item.ModelType != Type810Types.Header)
            {
                if (stack.Count == 0)
                    return;

                item = stack.Pop();
            }

            var header = item as Type810Header;
            if (header == null)
                return;

            model.AddHeader(header);
        }

        public void RevertToHeader()
        {
            if (stack.Count == 0)
                return;

            var item = stack.Peek();
            while (item.ModelType != Type810Types.Header)
            {
                if (stack.Count == 0)
                    return;

                stack.Pop();
                item = stack.Peek();
            }
        }

        public void SetMarket(int id)
        {
            marketId = id;
            marketOption = (MarketOptions)id;
        }
    }
}
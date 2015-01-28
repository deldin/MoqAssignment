using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Parsers.ImportContexts
{
    public class Prism814Context
    {
        private readonly Import814Model model;
        private readonly Stack<IType814Model> stack;

        private int marketId;
        private MarketOptions marketOption;
        
        public string Alias { get; set; }
        public string TransactionNumber { get; set; }
        public string ProductType { get; set; }
        public string UnmeterQuantity { get; set; }
        public string UnmeterDescription { get; set; }

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

        public IType814Model Current
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

        public Import814Model Results
        {
            get { return model; }
        }
        
        public Prism814Context()
        {
            model = new Import814Model();
            stack = new Stack<IType814Model>();
        }

        public void Initialize()
        {
            Alias = string.Empty;
            TransactionNumber = string.Empty;
            ProductType = string.Empty;
            UnmeterQuantity = string.Empty;
            UnmeterDescription = string.Empty;

            marketOption = MarketOptions.Unknown;
            marketId = (int)MarketOptions.Unknown;
        }

        public void InitializeMeter()
        {
            ProductType = string.Empty;
            UnmeterQuantity = string.Empty;
            UnmeterDescription = string.Empty;
        }

        public void PushModel(IType814Model item)
        {
            stack.Push(item);
        }

        public void ResolveToHeader()
        {
            if (stack.Count == 0)
                return;

            var item = stack.Pop();
            while (item.ModelType != Type814Types.Header)
            {
                if (stack.Count == 0)
                    return;

                item = stack.Pop();
            }

            var header = item as Type814Header;
            if (header == null)
                return;

            model.AddHeader(header);
        }

        public void RevertToHeader()
        {
            if (stack.Count == 0)
                return;

            var currentItem = stack.Peek();
            if (currentItem.ModelType == Type814Types.Header)
                return;

            while (currentItem.ModelType != Type814Types.Header)
            {
                stack.Pop();
                if (stack.Count == 0)
                    return;

                currentItem = stack.Peek();
            }
        }

        public void RevertToService()
        {
            if (stack.Count == 0)
                return;

            var currentItem = stack.Peek();
            if (currentItem.ModelType == Type814Types.Service)
                return;

            while (currentItem.ModelType != Type814Types.Service)
            {
                stack.Pop();
                if (stack.Count == 0)
                    return;

                currentItem = stack.Peek();
            }
        }

        public void SetMarket(int id)
        {
            marketId = id;
            marketOption = (MarketOptions)id;
        }
    }
}
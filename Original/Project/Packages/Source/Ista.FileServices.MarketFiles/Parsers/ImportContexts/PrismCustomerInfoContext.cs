using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Parsers.ImportContexts
{
    public class PrismCustomerInfoContext
    {
        private readonly ImportCustomerInfoModel model;
        private readonly Stack<ITypeCustomerInfoModel> stack;

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

        public ITypeCustomerInfoModel Current
        {
            get { return (stack.Count == 0) ? null : stack.Peek(); }
        }

        public ImportCustomerInfoModel Results
        {
            get { return model; }
        }

        public PrismCustomerInfoContext()
        {
            model = new ImportCustomerInfoModel();
            stack = new Stack<ITypeCustomerInfoModel>();
        }

        public void PushModel(ITypeCustomerInfoModel item)
        {
            stack.Push(item);
        }

        public void ResolveToFile()
        {
            if (stack.Count == 0)
                return;

            var item = stack.Pop();
            while (item.ModelType != TypeCustomerInfoTypes.File)
            {
                if (stack.Count == 0)
                    return;

                item = stack.Pop();
            }

            var header = item as TypeCustomerInfoFile;
            if (header == null)
                return;

            model.AddHeader(header);
        }
    }
}

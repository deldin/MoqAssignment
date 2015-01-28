using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867NetIntervalSummary: IType867Model
    {
        private readonly List<Type867NetIntervalSummaryQty> netIntervalSummaryQtys;

        public Type867Types ModelType
        {
            get { return Type867Types.NetIntervalSummary; }
        }

        public int NetIntervalSummaryKey { get; set; }
        public int HeaderKey { get; set; }
        public string TypeCode { get; set; }
        public string MeterUom { get; set; }
        public string MeterInterval { get; set; }

        public Type867NetIntervalSummaryQty[] NetIntervalSummaryQtys
        {
            get { return netIntervalSummaryQtys.ToArray(); }
        }

        public Type867NetIntervalSummary()
        {
            netIntervalSummaryQtys = new List<Type867NetIntervalSummaryQty>();
        }

        public void AddQuantity(Type867NetIntervalSummaryQty item)
        {
            netIntervalSummaryQtys.Add(item);
        }
    }
}
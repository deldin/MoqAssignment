using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867UnMeterDetail : IType867Model  
    {
        private readonly List<Type867UnMeterDetailQty> unMeterDetailQtys;

        public Type867Types ModelType
        {
            get { return Type867Types.UnMeterDetail; }
        }

        public int UnMeterDetailKey { get; set; }
        public int HeaderKey { get; set; }
        public string TypeCode { get; set; }
        public string ServicePeriodStart { get; set; }
        public string ServicePeriodEnd { get; set; }
        public string ServiceType { get; set; }
        public string Description { get; set; }
        public string CommodityCode { get; set; }
        public string UtilityRateServiceClass { get; set; }
        public string RateSubClass { get; set; }
        public string LoadProfile { get; set; }
        
        public Type867UnMeterDetailQty[] UnMeterDetailQtys
        {
            get { return unMeterDetailQtys.ToArray(); }
        }
        
        public Type867UnMeterDetail()
        {
            unMeterDetailQtys = new List<Type867UnMeterDetailQty>();
        }

        public void AddQuantity(Type867UnMeterDetailQty item)
        {
            unMeterDetailQtys.Add(item);
        }
    }
}
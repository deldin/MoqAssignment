using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867Switch : IType867Model
    {
        private readonly List<Type867SwitchQty> switchQtys;

        public Type867Types ModelType
        {
            get { return Type867Types.Switch; }
        }

        public int SwitchKey { get; set; }
        public int HeaderKey { get; set; }
        public string TypeCode { get; set; }
        public string MeterNumber { get; set; }
        public string SwitchDate { get; set; }
        
        public Type867SwitchQty[] SwitchQtys
        {
            get { return switchQtys.ToArray(); }
        }
        
        public Type867Switch()
        {
            switchQtys = new List<Type867SwitchQty>();
        }

        public void AddQuantity(Type867SwitchQty item)
        {
            switchQtys.Add(item);
        }
    }
}
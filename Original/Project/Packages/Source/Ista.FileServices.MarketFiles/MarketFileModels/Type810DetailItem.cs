using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type810DetailItem : IType810Model
    {
        private readonly List<Type810DetailItemCharge> charges;
        private readonly List<Type810DetailItemService> services;
        private readonly List<Type810DetailItemTax> taxes;

        public Type810Types ModelType
        {
            get { return Type810Types.DetailItem; }
        }

        public int? ItemKey { get; set; }
        public int DetailKey { get; set; }
        public string AssignedId { get; set; }
        public string RelationshipCode { get; set; }
        public string ServiceOrderCompleteDate { get; set; }
        public string UnmeteredServiceDateRange { get; set; }
        public string InvoiceNbr { get; set; }
        public string ServiceOrderNbr { get; set; }
        public string Consumption { get; set; }
        public string EffectiveDate { get; set; }
        public string SequenceId { get; set; }

        public Type810DetailItemCharge[] Charges
        {
            get { return charges.ToArray(); }
        }

        public Type810DetailItemService[] Services
        {
            get { return services.ToArray(); }
        }

        public Type810DetailItemTax[] Taxes
        {
            get { return taxes.ToArray(); }
        }

        public Type810DetailItem()
        {
            charges = new List<Type810DetailItemCharge>();
            services = new List<Type810DetailItemService>();
            taxes = new List<Type810DetailItemTax>();
        }

        public void AddCharge(Type810DetailItemCharge item)
        {
            charges.Add(item);
        }

        public void AddService(Type810DetailItemService item)
        {
            services.Add(item);
        }

        public void AddTax(Type810DetailItemTax item)
        {
            taxes.Add(item);
        }
    }
}
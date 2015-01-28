using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type810Detail : IType810Model
    {
        private readonly List<Type810DetailItem> items;
        private readonly List<Type810DetailService> services;
        private readonly List<Type810DetailTax> taxes;

        public Type810Types ModelType
        {
            get { return Type810Types.Detail; }
        }

        public int? DetailKey { get; set; }
        public int HeaderKey { get; set; }
        public string AssignedId { get; set; }
        public string ServiceTypeCode { get; set; }
        public string ServiceType { get; set; }
        public string ServiceClassCode { get; set; }
        public string ServiceClass { get; set; }
        public string RateClass { get; set; }
        public string RateSubClass { get; set; }
        public string ServicePeriodStartDate { get; set; }
        public string ServicePeriodEndDate { get; set; }
        public string MeterNumber { get; set; }
        public string BillCycle { get; set; }
        public string GasPoolId { get; set; }
        public string ServiceAgreement { get; set; }
        public string ServiceTypeCode2 { get; set; }
        public string ServiceType2 { get; set; }
        public string ServiceTypeCode3 { get; set; }
        public string ServiceType3 { get; set; }
        public string ServiceDeliveryPoint { get; set; }
        public string OldAccountNumber { get; set; }
        public string RateCode { get; set; }
        public string OldMeterNumber { get; set; }
        public string TransferredDate { get; set; }
        public string InvoicePeriodStartDate { get; set; }
        public string InvoicePeriodEndDate { get; set; }
        public string SupplierContractId { get; set; }
        public string UtilityContractId { get; set; }

        public Type810DetailItem[] Items
        {
            get { return items.ToArray(); }
        }

        public Type810DetailService[] Services
        {
            get { return services.ToArray(); }
        }

        public Type810DetailTax[] Taxes
        {
            get { return taxes.ToArray(); }
        }

        public Type810Detail()
        {
            items = new List<Type810DetailItem>();
            services = new List<Type810DetailService>();
            taxes = new List<Type810DetailTax>();
        }

        public void AddItem(Type810DetailItem item)
        {
            items.Add(item);
        }

        public void AddService(Type810DetailService item)
        {
            services.Add(item);
        }

        public void AddTax(Type810DetailTax item)
        {
            taxes.Add(item);
        }
    }
}

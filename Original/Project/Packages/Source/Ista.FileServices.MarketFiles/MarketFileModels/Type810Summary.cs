using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type810Summary : IType810Model
    {
        private readonly List<Type810SummaryService> services;
        private readonly List<Type810SummaryTax> taxes;

        public Type810Types ModelType
        {
            get { return Type810Types.Summary; }
        }

        public int? SummaryKey { get; set; }
        public int HeaderKey { get; set; }
        public string TotalAmount { get; set; }
        public string TotalLineItems { get; set; }
        public string TotalSegments { get; set; }
        public string TransactionSetControlNbr { get; set; }

        public Type810SummaryService[] Details
        {
            get { return services.ToArray(); }
        }

        public Type810SummaryTax[] Balances
        {
            get { return taxes.ToArray(); }
        }

        public Type810Summary()
        {
            services = new List<Type810SummaryService>();
            taxes = new List<Type810SummaryTax>();
        }

        public void AddService(Type810SummaryService item)
        {
            services.Add(item);
        }

        public void AddTax(Type810SummaryTax item)
        {
            taxes.Add(item);
        }
    }
}
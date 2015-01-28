
namespace Ista.FileServices.MarketFiles.Models
{
    public class MeterConsumptionModel
    {
        public int ConsId { get; set; }
        public int InvoiceId { get; set; }
        public int MeterId { get; set; }
        public string MeterType { get; set; }
        public string RateCodeValue { get; set; }
        public string MeterNumber { get; set; }
        public string UOM { get; set; }
        public string BegRead { get; set; }
        public string EndRead { get; set; }
        public string MeterFactor { get; set; }
        public string TotalConsumption { get; set; }
        public string ServicePeriodStartDate { get; set; }
        public string ServicePeriodEndDate { get; set; }
    }
}
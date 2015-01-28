
namespace Ista.FileServices.MarketFiles.Models
{
    public class CustomerArSummaryModel
    {
        public decimal PrevBal { get; set; }
        public decimal CurrPmts { get; set; }
        public decimal CurrAdjs { get; set; }
        public decimal BalDue { get; set; }
    }
}
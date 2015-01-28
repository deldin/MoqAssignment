using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type810Balance : IType810Model
    {
        public Type810Types ModelType
        {
            get { return Type810Types.Balance; }
        }

        public int? BalanceKey { get; set; }
        public int HeaderKey { get; set; }
        public string TotalOutstandingBalance { get; set; }
        public string BeginningBalance { get; set; }
        public string BudgetCumulativeDifference { get; set; }
        public string BudgetMonthDifference { get; set; }
        public string BudgetBilledToDate { get; set; }
        public string ActualBilledToDate { get; set; }     
    }
}

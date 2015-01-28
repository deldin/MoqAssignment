using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867GasProfileFactorEvaluation : IType867Model
    {
        public Type867Types ModelType
        {
            get { return Type867Types.GasProfileFactorEvaluation; }
        }

        public int HeaderKey { get; set; }
        public string ProfilePeriodStartDate { get; set; }
        public string CustomerServiceInitDate { get; set; }
        public string UtilityRateServiceClass { get; set; }
        public string RateSubClass { get; set; }
        public string NonHeatLoadFactorQty { get; set; }
        public string WeatherNormLoadFactorQty { get; set; }
        public string LoadFactorRatio { get; set; }
        public string UFGRatePct { get; set; }
        public string MaximumDeliveryQty { get; set; }
        public int GasProfileFactorEvaluationKey { get; set; }
    }
}
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867GasProfileFactorSample: IType867Model
    {
        public Type867Types ModelType
        {
            get { return Type867Types.GasProfileFactorSample; }
        }

        public int HeaderKey { get; set; }
        public string ReportMonth { get; set; }
        public string AnnualPeriod { get; set; }
        public string NormProjectedUsageQty { get; set; }
        public string WeatherNormUsageProjectedQty { get; set; }
        public string NormProjectedDeliveryQty { get; set; }
        public string WeatherNormProjectedDeliveryQty { get; set; }
        public string ProjectedDailyDeliveryQty { get; set; }
        public string DesignProjectedUsageQty { get; set; }
        public string DesignProjectedDeliveryQty { get; set; }
        public string ProjectedBalancingUseQty { get; set; }
        public string ProjectedSwingChargeAmt { get; set; }
        public int GasProfileFactorSampleKey { get; set; }
    }
}
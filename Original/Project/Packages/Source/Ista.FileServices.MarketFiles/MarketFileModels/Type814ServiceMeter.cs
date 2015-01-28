using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type814ServiceMeter : IType814Model
    {
        private readonly List<Type814ServiceMeterType> types;
        private readonly List<Type814ServiceMeterChange> changes;
        private readonly List<Type814ServiceMeterTou> tous;

        public Type814Types ModelType
        {
            get { return Type814Types.ServiceMeter; }
        }

        public int? MeterKey { get; set; }
        public int ServiceKey { get; set; }
        public string EntityIdCode { get; set; }
        public string MeterNumber { get; set; }
        public string MeterCode { get; set; }
        public string MeterType { get; set; }
        public string LoadProfile { get; set; }
        public string RateClass { get; set; }
        public string RateSubClass { get; set; }
        public string MeterCycle { get; set; }
        public string MeterCycleDayOfMonth { get; set; }
        public string SpecialNeedsIndicator { get; set; }
        public string OldMeterNumber { get; set; }
        public string MeterOwnerIndicator { get; set; }
        public string EntityType { get; set; }
        public string TimeOfUse { get; set; }
        public string EspRateCode { get; set; }
        public string OrganizationName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string NamePrefix { get; set; }
        public string NameSuffix { get; set; }
        public string IdentificationCode { get; set; }
        public string EntityName2 { get; set; }
        public string EntityName3 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string CountryCode { get; set; }
        public string County { get; set; }
        public string PlanNumber { get; set; }
        public string ServicesReferenceNumber { get; set; }
        public string AffiliationNumber { get; set; }
        public string CostElement { get; set; }
        public string CoverageCode { get; set; }
        public string LossReportNumber { get; set; }
        public string GeographicNumber { get; set; }
        public string ItemNumber { get; set; }
        public string LocationNumber { get; set; }
        public string PriceListNumber { get; set; }
        public string ProductType { get; set; }
        public string QualityInspectionArea { get; set; }
        public string ShipperCarOrderNumber { get; set; }
        public string StandardPointLocation { get; set; }
        public string ReportIdentification { get; set; }
        public string Supplier { get; set; }
        public string Area { get; set; }
        public string CollectorIdentification { get; set; }
        public string VendorAgentNumber { get; set; }
        public string VendorAbbreviation { get; set; }
        public string VendorIdNumber { get; set; }
        public string VendorOrderNumber { get; set; }
        public string PricingStructureCode { get; set; }
        public string MeterOwnerDuns { get; set; }
        public string MeterOwner { get; set; }
        public string MeterInstallerDuns { get; set; }
        public string MeterInstaller { get; set; }
        public string MeterReaderDuns { get; set; }
        public string MeterReader { get; set; }
        public string MeterMaintenanceProviderDuns { get; set; }
        public string MeterMaintenanceProvider { get; set; }
        public string MeterDataManagementAgentDuns { get; set; }
        public string MeterDataManagementAgent { get; set; }
        public string SchedulingCoordinatorDuns { get; set; }
        public string SchedulingCoordinator { get; set; }
        public string MeterInstallPending { get; set; }
        public string PackageOption { get; set; }
        public string UsageCode { get; set; }
        public string MeterServiceVoltage { get; set; }
        public string LossFactor { get; set; }
        public string AmsIndicator { get; set; }
        public string SummaryInterval { get; set; }

        public Type814ServiceMeterChange[] Changes
        {
            get { return changes.ToArray(); }
        }

        public Type814ServiceMeterTou[] Tous
        {
            get { return tous.ToArray(); }
        }

        public Type814ServiceMeterType[] Types
        {
            get { return types.ToArray(); }
        }

        public Type814ServiceMeter()
        {
            types = new List<Type814ServiceMeterType>();
            changes = new List<Type814ServiceMeterChange>();
            tous = new List<Type814ServiceMeterTou>();
        }

        public void AddType(Type814ServiceMeterType item)
        {
            types.Add(item);
        }

        public void AddChange(Type814ServiceMeterChange item)
        {
            changes.Add(item);
        }

        public void AddTou(Type814ServiceMeterTou item)
        {
            tous.Add(item);
        }
    }
}
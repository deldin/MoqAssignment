using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type814Service : IType814Model
    {
        private readonly List<Type814ServiceReject> rejects;
        private readonly List<Type814ServiceStatus> statuses;
        private readonly List<Type814ServiceAccountChange> changes;
        private readonly List<Type814ServiceMeter> meters;
        private readonly List<Type814ServiceDate> dates; 

        public Type814Types ModelType
        {
            get { return Type814Types.Service; }
        } 

        public int? ServiceKey { get; set; }
        public int HeaderKey { get; set; }
        public string AssignId { get; set; }
        public string ServiceTypeCode1 { get; set; }
        public string ServiceType1 { get; set; }
        public string ServiceTypeCode2 { get; set; }
        public string ServiceType2 { get; set; }
        public string ServiceTypeCode3 { get; set; }
        public string ServiceType3 { get; set; }
        public string ServiceTypeCode4 { get; set; }
        public string ServiceType4 { get; set; }
        public string ActionCode { get; set; }
        public string MaintenanceTypeCode { get; set; }
        public string DistributionLossFactorCode { get; set; }
        public string PremiseType { get; set; }
        public string BillType { get; set; }
        public string BillCalculator { get; set; }
        public string EsiId { get; set; }
        public string StationId { get; set; }
        public string SpecialNeedsIndicator { get; set; }
        public string PowerRegion { get; set; }
        public string EnergizedFlag { get; set; }
        public string EsiIdStartDate { get; set; }
        public string EsiIdEndDate { get; set; }
        public string EsiIdEligbilityDate { get; set; }
        public string NotificationWaiver { get; set; }
        public string SpecialReadSwitchDate { get; set; }
        public string PriorityCode { get; set; }
        public string PermitIndicator { get; set; }
        public string RtoDate { get; set; }
        public string RtoTime { get; set; }
        public string CsaFlag { get; set; }
        public string MembershipId { get; set; }
        public string EspAccountNumber { get; set; }
        public string LdcBillingCycle { get; set; }
        public string LdcBudgetBillingCycle { get; set; }
        public string WaterHeaters { get; set; }
        public string LdcBudgetBillingStatus { get; set; }
        public string PaymentArrangement { get; set; }
        public string NextMeterReadDate { get; set; }
        public string ParticipatingInterest { get; set; }
        public string EligibleLoadPercentage { get; set; }
        public string TaxExceptionPercent { get; set; }
        public string CapacityObligation { get; set; }
        public string TransmissionObligation { get; set; }
        public string TotalKwhHistory { get; set; }
        public string NumberOfMonthsHistory { get; set; }
        public string PeakDemandHistory { get; set; }
        public string AirConditioners { get; set; }
        public string PreviousEsiId { get; set; }
        public string GasPoolId { get; set; }
        public string LbmpZone { get; set; }
        public string ResidentialTaxPortion { get; set; }
        public string EspCommodityPrice { get; set; }
        public string EspFixedCharge { get; set; }
        public string EspChargesCommTaxRate { get; set; }
        public string EspChargesResTaxRate { get; set; }
        public string GasSupplyServiceOption { get; set; }
        public string FundsAuthorization { get; set; }
        public string BudgetBillingStatus { get; set; }
        public string FixedMonthlyCharge { get; set; }
        public string TaxRate { get; set; }
        public string CommodityPrice { get; set; }
        public string MeterCycleCodeDesc { get; set; }
        public string BillCycleCodeDesc { get; set; }
        public string FeeApprovedApplied { get; set; }
        public string MarketerCustomerAccountNumber { get; set; }
        public string GasSupplyServiceOptionCode { get; set; }
        public string HumanNeeds { get; set; }
        public string ReinstatementDate { get; set; }
        public string MeterCycleCode { get; set; }
        public string SystemNumber { get; set; }
        public string StateLicenseNumber { get; set; }
        public string SupplementalAccountNumber { get; set; }
        public string NewCustomerIndicator { get; set; }
        public string PaymentCategory { get; set; }
        public string PreviousEspAccountNumber { get; set; }
        public string RenewableEnergyIndicator { get; set; }
        public string SicCode { get; set; }
        public string ApprovalCodeIndicator { get; set; }
        public string RenewableEnergyCertification { get; set; }
        public string NewPremiseIndicator { get; set; }
        public string SalesResponsibility { get; set; }
        public string CustomerReferenceNumber { get; set; }
        public string TransactionReferenceNumber { get; set; }
        public string EspTransactionNumber { get; set; }
        public string OldEspAccountNumber { get; set; }
        public string DfiIdentificationNumber { get; set; }
        public string DfiAccountNumber { get; set; }
        public string DfiIndicator1 { get; set; }
        public string DfiIndicator2 { get; set; }
        public string DfiQualifier { get; set; }
        public string DfiRoutingNumber { get; set; }
        public string SpecialReadSwitchTime { get; set; }
        public string LdcAccountBalance { get; set; }
        public string DisputedAmount { get; set; }
        public string CurrentBalance { get; set; }
        public string ArrearsBalance { get; set; }
        public string LdcSupplierBalance { get; set; }
        public string BudgetPlan { get; set; }
        public string BudgetInstallment { get; set; }
        public string Deposit { get; set; }
        public string RemainingUtilBalanceBucket1 { get; set; }
        public string RemainingUtilBalanceBucket2 { get; set; }
        public string RemainingUtilBalanceBucket3 { get; set; }
        public string RemainingUtilBalanceBucket4 { get; set; }
        public string RemainingUtilBalanceBucket5 { get; set; }
        public string RemainingUtilBalanceBucket6 { get; set; }
        public string IntervalStatusType { get; set; }
        public string CustomerAuthorization { get; set; }
        public string UnmeteredAcct { get; set; }
        public string PaymentOption { get; set; }
        public string MaxDailyAmt { get; set; }
        public string MeterAccessNote { get; set; }
        public string SpecialNeedsExpirationDate { get; set; }
        public string SwitchHoldStatusIndicator { get; set; }
        public string IgnoreRescind { get; set; }
        public string SpecialMeterConfig { get; set; }
        public string MaximumGeneration { get; set; }
        public string ServiceDeliveryPoint { get; set; }
        public string GasCapacityAssignment { get; set; }
        public string GovAggregationType { get; set; }
        public string GovAggregationCode { get; set; }
        public string DaysInArrears { get; set; }

        /// <summary>
        /// this property is actually in the 814 Header the stored procedure to insert, however, 
        /// expects this to be at this location
        /// </summary>
        /// <remarks>
        /// this should be changed to not be a concern  of the stored procedure but rather the code
        /// </remarks>
        public string PolrClass { get; set; }

        public Type814ServiceAccountChange[] Changes
        {
            get { return changes.ToArray(); }
        }

        public Type814ServiceDate[] Dates
        {
            get { return dates.ToArray(); }
        }

        public Type814ServiceMeter[] Meters
        {
            get { return meters.ToArray(); }
        }

        public Type814ServiceReject[] Rejects
        {
            get { return rejects.ToArray(); }
        }

        public Type814ServiceStatus[] Statuses
        {
            get { return statuses.ToArray(); }
        }
        
        public Type814Service()
        {
            rejects = new List<Type814ServiceReject>();
            statuses = new List<Type814ServiceStatus>();
            changes = new List<Type814ServiceAccountChange>();
            meters = new List<Type814ServiceMeter>();
            dates = new List<Type814ServiceDate>();
        }

        public void AddReject(Type814ServiceReject item)
        {
            rejects.Add(item);
        }

        public void AddStatus(Type814ServiceStatus item)
        {
            statuses.Add(item);
        }

        public void AddChange(Type814ServiceAccountChange item)
        {
            changes.Add(item);
        }

        public void AddMeter(Type814ServiceMeter item)
        {
            meters.Add(item);
        }

        public void AddDate(Type814ServiceDate item)
        {
            dates.Add(item);
        }
    }
}
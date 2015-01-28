using System;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class Import814XmlDataAccess : IMarket814Import
    {
        private readonly string connectionString;

        public Import814XmlDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int IdentifyTransactionType(string actionCode, string serviceActionCode)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_TransactionTypeLoad"))
            {
                SqlParameter outputParameter;

                command.AddWithValue("@TransType", "814")
                    .AddWithValue("@ActionCode", actionCode)
                    .AddWithValue("@ServiceActionCode", serviceActionCode)
                    .AddOutParameter("@TransactionTypeID", SqlDbType.Int, out outputParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (outputParameter.Value == null)
                    return 0;

                return (int)outputParameter.Value;
            }
        }

        public int InsertHeader(Type814Header model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814HeaderInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@MarketFileId", model.MarketFileId)
                    .AddWithValue("@TransactionSetId", model.TransactionSetId)
                    .AddWithValue("@TransactionSetControlNbr", model.TransactionSetControlNbr)
                    .AddIfNotEmptyOrDbNull("@TransactionSetPurposeCode", model.TransactionSetPurposeCode)
                    .AddIfNotEmptyOrDbNull("@TransactionNbr", model.TransactionNbr)
                    .AddIfNotEmptyOrDbNull("@TransactionDate", model.TransactionDate)
                    .AddIfNotEmptyOrDbNull("@ReferenceNbr", model.ReferenceNbr)
                    .AddIfNotEmptyOrDbNull("@ActionCode", model.ActionCode)
                    .AddIfNotEmptyOrDbNull("@TdspDuns", model.TdspDuns)
                    .AddIfNotEmptyOrDbNull("@TdspName", model.TdspName)
                    .AddIfNotEmptyOrDbNull("@CrDuns", model.CrDuns)
                    .AddIfNotEmptyOrDbNull("@CrName", model.CrName)
                    .AddWithValue("@Direction", true)
                    .AddWithValue("@TransactionTypeID", model.TransactionTypeId)
                    .AddWithValue("@MarketID", model.MarketId)
                    .AddWithValue("@ProviderID", model.ProviderId)
                    .AddWithValue("@TransactionTime", model.TransactionTime)
                    .AddWithValue("@TransactionTimeCode", model.TransactionTimeCode)
                    .AddWithValue("@TransactionQualifier", model.TransactionQualifier)
                    .AddOutParameter("@Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var headerKey = (int)keyParameter.Value;
                model.HeaderKey = headerKey;

                return headerKey;
            }
        }

        public int InsertName(Type814Name model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814NameInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@814_Key", model.HeaderKey)
                    .AddWithValue("@EntityIdType", model.EntityIdType)
                    .AddIfNotEmptyOrDbNull("@EntityName", model.EntityName)
                    .AddIfNotEmptyOrDbNull("@EntityName2", model.EntityName2)
                    .AddIfNotEmptyOrDbNull("@EntityName3", model.EntityName3)
                    .AddIfNotEmptyOrDbNull("@EntityDuns", model.EntityDuns)
                    .AddIfNotEmptyOrDbNull("@EntityIdCode", model.EntityIdCode)
                    .AddIfNotEmptyOrDbNull("@Address1", model.Address1)
                    .AddIfNotEmptyOrDbNull("@Address2", model.Address2)
                    .AddIfNotEmptyOrDbNull("@City", model.City)
                    .AddIfNotEmptyOrDbNull("@State", model.State)
                    .AddIfNotEmptyOrDbNull("@PostalCode", model.PostalCode)
                    .AddIfNotEmptyOrDbNull("@CountryCode", model.CountryCode)
                    .AddWithValue("@County", model.County)
                    .AddWithValue("@ContactCode", model.ContactCode)
                    .AddIfNotEmptyOrDbNull("@ContactName", model.ContactName)
                    .AddIfNotEmptyOrDbNull("@ContactPhoneNbr1", model.ContactPhoneNbr1)
                    .AddWithValue("@ContactPhoneNbr2", model.ContactPhoneNbr2)
                    .AddWithValue("@ContactPhoneNbr3", model.ContactPhoneNbr3)
                    .AddWithValue("@EntityFirstName", model.EntityFirstName)
                    .AddWithValue("@EntityLastName", model.EntityLastName)
                    .AddWithValue("@EntityEmail", model.EntityEmail)
                    .AddWithValue("@CustType", model.CustType)
                    .AddWithValue("@TaxingDistrict", model.TaxingDistrict)
                    .AddOutParameter("@Name_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var nameKey = (int)keyParameter.Value;
                model.NameKey = nameKey;

                return nameKey;
            }
        }

        public int InsertService(Type814Service model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814ServiceInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@814_Key", model.HeaderKey)
                    .AddIfNotEmptyOrDbNull("@AssignId", model.AssignId)
                    .AddIfNotEmptyOrDbNull("@ServiceTypeCode1", model.ServiceTypeCode1)
                    .AddIfNotEmptyOrDbNull("@ServiceType1", model.ServiceType1)
                    .AddIfNotEmptyOrDbNull("@ServiceTypeCode2", model.ServiceTypeCode2)
                    .AddIfNotEmptyOrDbNull("@ServiceType2", model.ServiceType2)
                    .AddIfNotEmptyOrDbNull("@ServiceTypeCode3", model.ServiceTypeCode3)
                    .AddIfNotEmptyOrDbNull("@ServiceType3", model.ServiceType3)
                    .AddIfNotEmptyOrDbNull("@ServiceTypeCode4", model.ServiceTypeCode4)
                    .AddIfNotEmptyOrDbNull("@ServiceType4", model.ServiceType4)
                    .AddIfNotEmptyOrDbNull("@ActionCode", model.ActionCode)
                    .AddIfNotEmptyOrDbNull("@MaintenanceTypeCode", model.MaintenanceTypeCode)
                    .AddIfNotEmptyOrDbNull("@DistributionLossFactorCode", model.DistributionLossFactorCode)
                    .AddIfNotEmptyOrDbNull("@PremiseType", model.PremiseType)
                    .AddIfNotEmptyOrDbNull("@BillType", model.BillType)
                    .AddIfNotEmptyOrDbNull("@BillCalculator", model.BillCalculator)
                    .AddIfNotEmptyOrDbNull("@EsiId", model.EsiId)
                    .AddIfNotEmptyOrDbNull("@StationId", model.StationId)
                    .AddIfNotEmptyOrDbNull("@PowerRegion", model.PowerRegion)
                    .AddIfNotEmptyOrDbNull("@EnergizedFlag", model.EnergizedFlag)
                    .AddIfNotEmptyOrDbNull("@NotificationWaiver", model.NotificationWaiver)
                    .AddIfNotEmptyOrDbNull("@EsiIdStartDate", model.EsiIdStartDate)
                    .AddIfNotEmptyOrDbNull("@EsiIdEndDate", model.EsiIdEndDate)
                    .AddIfNotEmptyOrDbNull("@EsiIdEligibilityDate", model.EsiIdEligbilityDate)
                    .AddIfNotEmptyOrDbNull("@SpecialReadSwitchDate", model.SpecialReadSwitchDate)
                    .AddWithValue("@SpecialReadSwitchTime", model.SpecialReadSwitchTime)
                    .AddWithValue("@PriorityCode", model.PriorityCode)
                    .AddWithValue("@RTODate", model.RtoDate)
                    .AddWithValue("@RTOTime", model.RtoTime)
                    .AddWithValue("@PermitIndicator", model.PermitIndicator)
                    .AddWithValue("@CSAFlag", model.CsaFlag)
                    .AddWithValue("@MembershipID", model.MembershipId)
                    .AddWithValue("@ESPAccountNumber", model.EspAccountNumber)
                    .AddWithValue("@LDCBillingCycle", model.LdcBillingCycle)
                    .AddWithValue("@LDCBudgetBillingCycle", model.LdcBudgetBillingCycle)
                    .AddWithValue("@WaterHeaters", model.WaterHeaters)
                    .AddWithValue("@LDCBudgetBillingStatus", model.LdcBudgetBillingStatus)
                    .AddWithValue("@PaymentArrangement", model.PaymentArrangement)
                    .AddWithValue("@NextMeterReadDate", model.NextMeterReadDate)
                    .AddWithValue("@ParticipatingInterest", model.ParticipatingInterest)
                    .AddWithValue("@EligibleLoadPercentage", model.EligibleLoadPercentage)
                    .AddWithValue("@TaxExemptionPercent", model.TaxExceptionPercent)
                    .AddWithValue("@CapacityObligation", model.CapacityObligation)
                    .AddWithValue("@TransmissionObligation", model.TransmissionObligation)
                    .AddWithValue("@TotalKWHHistory", model.TotalKwhHistory)
                    .AddWithValue("@NumberOfMonthsHistory", model.NumberOfMonthsHistory)
                    .AddWithValue("@PeakDemandHistory", model.PeakDemandHistory)
                    .AddWithValue("@AirConditioners", model.AirConditioners)
                    .AddWithValue("@PreviousEsiId", model.PreviousEsiId)
                    .AddWithValue("@GasPoolId", model.GasPoolId)
                    .AddWithValue("@LBMPZone", model.LbmpZone)
                    .AddWithValue("@ResidentialTaxPortion", model.ResidentialTaxPortion)
                    .AddWithValue("@ESPCommodityPrice", model.EspCommodityPrice)
                    .AddWithValue("@ESPFixedCharge", model.EspFixedCharge)
                    .AddWithValue("@ESPChargesCommTaxRate", model.EspChargesCommTaxRate)
                    .AddWithValue("@GasSupplyServiceOption", model.GasSupplyServiceOption)
                    .AddWithValue("@GasSupplyServiceDesc", model.GasSupplyServiceOptionCode)
                    .AddWithValue("@BudgetBillingStatus", model.BudgetBillingStatus)
                    .AddWithValue("@FixedMonthlyCharge", model.FixedMonthlyCharge)
                    .AddWithValue("@TaxRate", model.TaxRate)
                    .AddWithValue("@MeterCycleCodeDesc", model.MeterCycleCodeDesc)
                    .AddWithValue("@MeterCycleCode", model.MeterCycleCode)
                    .AddWithValue("@BillCycleCodeDesc", model.BillCycleCodeDesc)
                    .AddWithValue("@FeeApprovedApplied", model.FeeApprovedApplied)
                    .AddWithValue("@MarketerCustomerAccountNumber", model.MarketerCustomerAccountNumber)
                    .AddWithValue("@HumanNeeds", model.HumanNeeds)
                    .AddWithValue("@CustomerAuthorization", model.CustomerAuthorization)
                    .AddWithValue("@LDCAccountBalance", model.LdcAccountBalance)
                    .AddWithValue("@DisputedAmount", model.DisputedAmount)
                    .AddWithValue("@CurrentBalance", model.CurrentBalance)
                    .AddWithValue("@ArrearsBalance", model.ArrearsBalance)
                    .AddWithValue("@LDCSupplierBalance", model.LdcSupplierBalance)
                    .AddWithValue("@BudgetPlan", model.BudgetPlan)
                    .AddWithValue("@BudgetInstallment", model.BudgetInstallment)
                    .AddWithValue("@Deposit", model.Deposit)
                    .AddWithValue("@RemainingUtilBalanceBucket1", model.RemainingUtilBalanceBucket1)
                    .AddWithValue("@RemainingUtilBalanceBucket2", model.RemainingUtilBalanceBucket2)
                    .AddWithValue("@RemainingUtilBalanceBucket3", model.RemainingUtilBalanceBucket3)
                    .AddWithValue("@RemainingUtilBalanceBucket4", model.RemainingUtilBalanceBucket4)
                    .AddWithValue("@RemainingUtilBalanceBucket5", model.RemainingUtilBalanceBucket5)
                    .AddWithValue("@RemainingUtilBalanceBucket6", model.RemainingUtilBalanceBucket6)
                    .AddWithValue("@IntervalStatusType", model.IntervalStatusType)
                    .AddWithValue("@PaymentOption", model.PaymentOption)
                    .AddWithValue("@SystemNumber", model.SystemNumber)
                    .AddWithValue("@MaxDailyAmt", model.MaxDailyAmt)
                    .AddWithValue("@FundsAuthorization", model.FundsAuthorization)
                    .AddWithValue("@SpecialMeterConfig", model.SpecialMeterConfig)
                    .AddWithValue("@MaximumGeneration", model.MaximumGeneration)
                    .AddWithValue("@DaysInArrears", model.DaysInArrears)
                    .AddWithValue("@ServiceDeliveryPoint", model.ServiceDeliveryPoint)
                    .AddWithValue("@GasCapacityAssignment", model.GasCapacityAssignment)
                    .AddOutParameter("@Service_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var serviceKey = (int)keyParameter.Value;
                model.ServiceKey = serviceKey;

                return serviceKey;
            }
        }

        public int InsertServiceMeter(Type814ServiceMeter model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814ServiceMeterInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@Service_Key", model.ServiceKey)
                    .AddIfNotEmptyOrDbNull("@EntityIdCode", model.EntityIdCode)
                    .AddIfNotEmptyOrDbNull("@MeterNumber", model.MeterNumber)
                    .AddIfNotEmptyOrDbNull("@MeterCode", model.MeterCode)
                    .AddIfNotEmptyOrDbNull("@MeterType", model.MeterType)
                    .AddIfNotEmptyOrDbNull("@LoadProfile", model.LoadProfile)
                    .AddIfNotEmptyOrDbNull("@RateClass", model.RateClass)
                    .AddIfNotEmptyOrDbNull("@RateSubClass", model.RateSubClass)
                    .AddIfNotEmptyOrDbNull("@MeterCycle", model.MeterCycle)
                    .AddIfNotEmptyOrDbNull("@MeterCycleDayOfMonth", model.MeterCycleDayOfMonth)
                    .AddIfNotEmptyOrDbNull("@SpecialNeedsIndicator", model.SpecialNeedsIndicator)
                    .AddIfNotEmptyOrDbNull("@OldMeterNumber", model.OldMeterNumber)
                    .AddWithValue("@MeterOwnerIndicator", model.MeterOwnerIndicator)
                    .AddWithValue("@EntityType", model.EntityType)
                    .AddWithValue("@TimeOFUse", model.TimeOfUse)
                    .AddWithValue("@ESPRateCode", model.EspRateCode)
                    .AddWithValue("@PricingStructureCode", model.PricingStructureCode)
                    .AddWithValue("@MeterOwner", model.MeterOwner)
                    .AddWithValue("@MeterOwnerDUNS", model.MeterOwnerDuns)
                    .AddWithValue("@MeterInstaller", model.MeterInstaller)
                    .AddWithValue("@MeterInstallerDUNS", model.MeterInstallerDuns)
                    .AddWithValue("@MeterReader", model.MeterReader)
                    .AddWithValue("@MeterReaderDUNS", model.MeterReaderDuns)
                    .AddWithValue("@MeterMaintenanceProvider", model.MeterMaintenanceProvider)
                    .AddWithValue("@MeterMaintenanceProviderDUNS", model.MeterMaintenanceProviderDuns)
                    .AddWithValue("@MeterDataManagementAgent", model.MeterDataManagementAgent)
                    .AddWithValue("@MeterDataManagementAgentDUNS", model.MeterDataManagementAgentDuns)
                    .AddWithValue("@SchedulingCoordinator", model.SchedulingCoordinator)
                    .AddWithValue("@SchedulingCoordinatorDUNS", model.SchedulingCoordinatorDuns)
                    .AddWithValue("@MeterInstallPending", model.MeterInstallPending)
                    .AddWithValue("@PackageOption", model.PackageOption)
                    .AddWithValue("@UsageCode", model.UsageCode)
                    .AddWithValue("@MeterServiceVoltage", model.MeterServiceVoltage)
                    .AddWithValue("@SummaryInterval", model.SummaryInterval)
                    .AddOutParameter("@Meter_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var meterKey = (int)keyParameter.Value;
                model.MeterKey = meterKey;

                return meterKey;
            }
        }

        public int InsertServiceMeterType(Type814ServiceMeterType model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814ServiceMeterTypeInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@Meter_Key", model.MeterKey)
                    .AddWithValue("@MeterMultiplier", model.MeterMultiplier)
                    .AddWithValue("@MeterType", model.MeterType)
                    .AddWithValue("@ProductType", model.ProductType)
                    .AddWithValue("@TimeOfUse", model.TimeOfUse)
                    .AddWithValue("@TimeOfUse2", model.TimeOfUse2)
                    .AddWithValue("@NumberOfDials", model.NumberOfDials)
                    .AddWithValue("@UnmeteredNumberOfDevices", model.UnmeteredNumberOfDevices)
                    .AddWithValue("@UnmeteredDescription", model.UnmeteredDescription)
                    .AddWithValue("@StartMeterRead", model.StartMeterRead)
                    .AddWithValue("@EndMeterRead", model.EndMeterRead)
                    .AddWithValue("@ChangeReason", model.ChangeReason)
                    .AddOutParameter("@Type_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var typeKey = (int)keyParameter.Value;
                model.TypeKey = typeKey;

                return typeKey;
            }
        }

        public int InsertServiceMeterChange(Type814ServiceMeterChange model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814ServiceMeterChangeInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@Meter_Key", model.MeterKey)
                    .AddWithValue("@ChangeReason", model.ChangeReason)
                    .AddWithValue("@ChangeDescription", model.ChangeDescription)
                    .AddOutParameter("@Change_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var changeKey = (int)keyParameter.Value;
                model.ChangeKey = changeKey;

                return changeKey;
            }
        }

        public int InsertServiceMeterTou(Type814ServiceMeterTou model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814ServiceMeterTOUInsert"))
            {
                command.AddWithValue("@Meter_Key", model.MeterKey)
                    .AddWithValue("@TOUCode", model.TouCode)
                    .AddWithValue("@MeasurementType", model.MeasurementType);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                model.TouKey = 1;
                return 1;
            }
        }

        public int InsertServiceDate(Type814ServiceDate model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814ServiceDateInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@Service_Key", model.ServiceKey)
                    .AddIfNotEmptyOrDbNull("@Qualifier", model.Qualifier)
                    .AddIfNotEmptyOrDbNull("@Date", model.Date)
                    .AddIfNotEmptyOrDbNull("@Time", model.Time)
                    .AddIfNotEmptyOrDbNull("@TimeCode", model.TimeCode)
                    .AddIfNotEmptyOrDbNull("@PeriodFormat", model.PeriodFormat)
                    .AddIfNotEmptyOrDbNull("@Period", model.Period)
                    .AddIfNotEmptyOrDbNull("@NotesDate", model.NotesDate)
                    .AddOutParameter("@Date_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var dateKey = (int)keyParameter.Value;
                model.DateKey = dateKey;

                return dateKey;
            }
        }

        public int InsertServiceStatus(Type814ServiceStatus model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814ServiceStatusInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@Service_Key", model.ServiceKey)
                    .AddIfNotEmptyOrDbNull("@StatusCode", model.StatusCode)
                    .AddIfNotEmptyOrDbNull("@StatusReason", model.StatusReason)
                    .AddWithValue("@StatusType", model.StatusType)
                    .AddOutParameter("@Status_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var statusKey = (int)keyParameter.Value;
                model.StatusKey = statusKey;

                return statusKey;
            }
        }

        public int InsertServiceReject(Type814ServiceReject model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814ServiceRejectInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@Service_Key", model.ServiceKey)
                    .AddIfNotEmptyOrDbNull("@RejectCode", model.RejectCode)
                    .AddIfNotEmptyOrDbNull("@RejectReason", model.RejectReason)
                    .AddOutParameter("@Reject_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var rejectKey = (int)keyParameter.Value;
                model.RejectKey = rejectKey;

                return rejectKey;
            }
        }

        public int InsertServiceAccountChange(Type814ServiceAccountChange model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814ServiceAccountChangeInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@Service_Key", model.ServiceKey)
                    .AddWithValue("@ChangeReason", model.ChangeReason)
                    .AddWithValue("@ChangeDescription", model.ChangeDescription)
                    .AddOutParameter("@Change_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var changeKey = (int)keyParameter.Value;
                model.ChangeKey = changeKey;

                return changeKey;
            }
        }
    }
}

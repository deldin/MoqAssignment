using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class Export814PrismDataAccess : IMarket814Export
    {
        private readonly string connectionString;

        public Export814PrismDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Type814Header[] ListUnprocessed(string ldcDuns, string duns, int providerId)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814ExportList"))
            {
                command.AddIfNotEmptyOrDbNull("@TDSPDuns", ldcDuns)
                    .AddIfNotEmptyOrDbNull("@CrDuns", duns)
                    .AddWithValue("@ProviderID", providerId);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type814Header>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type814Header
                        {
                            HeaderKey = reader.GetInt32("814_Key"),
                            Direction = reader.GetBoolean("DirectionFlag"),
                            ProcessFlag = reader.GetInt16("ProcessFlag"),
                        };

                        reader.TryGetString("TransactionSetPurposeCode", x => item.TransactionSetPurposeCode = x);
                        reader.TryGetString("TransactionSetControlNbr", x => item.TransactionSetControlNbr = x);
                        reader.TryGetString("TransactionNbr", x => item.TransactionNbr = x);
                        reader.TryGetString("TransactionDate", x => item.TransactionDate = x);
                        reader.TryGetString("ReferenceNbr", x => item.ReferenceNbr = x);
                        reader.TryGetString("ActionCode", x => item.ActionCode = x);
                        reader.TryGetString("TdspDuns", x => item.TdspDuns = x);
                        reader.TryGetString("TdspName", x => item.TdspName = x);
                        reader.TryGetString("CrDuns", x => item.CrDuns = x);
                        reader.TryGetString("CrName", x => item.CrName = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type814Name[] ListNames(int headerKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814ExportListNameRecords"))
            {
                command.AddWithValue("@814Key", headerKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type814Name>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type814Name
                        {
                            HeaderKey = headerKey,
                        };

                        reader.TryGetString("EntityIdType", x => item.EntityIdType = x);
                        reader.TryGetString("EntityName", x => item.EntityName = x);
                        reader.TryGetString("EntityName2", x => item.EntityName2 = x);
                        reader.TryGetString("EntityName3", x => item.EntityName3 = x);
                        reader.TryGetString("Address1", x => item.Address1 = x);
                        reader.TryGetString("Address2", x => item.Address2 = x);
                        reader.TryGetString("City", x => item.City = x);
                        reader.TryGetString("State", x => item.State = x);
                        reader.TryGetString("PostalCode", x => item.PostalCode = x);
                        reader.TryGetString("CountryCode", x => item.CountryCode = x);
                        reader.TryGetString("ContactName", x => item.ContactName = x);
                        reader.TryGetString("ContactPhoneNbr1", x => item.ContactPhoneNbr1 = x);
                        reader.TryGetString("ContactPhoneNbr2", x => item.ContactPhoneNbr2 = x);
                        reader.TryGetString("EntityIdCode", x => item.EntityIdCode = x);
                        reader.TryGetString("EntityEmail", x => item.EntityEmail = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type814Service[] ListServices(int headerKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814ExportListServiceRecords"))
            {
                command.AddWithValue("@814Key", headerKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type814Service>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type814Service
                        {
                            HeaderKey = headerKey,
                            ServiceKey = reader.GetInt32("Service_Key"),
                            PaymentOption = reader.GetString("PaymentOption"),
                        };

                        reader.TryGetString("AssignId", x => item.AssignId = x);
                        reader.TryGetString("ServiceTypeCode1", x => item.ServiceTypeCode1 = x);
                        reader.TryGetString("ServiceType1", x => item.ServiceType1 = x);
                        reader.TryGetString("ServiceTypeCode2", x => item.ServiceTypeCode2 = x);
                        reader.TryGetString("ServiceType2", x => item.ServiceType2 = x);
                        reader.TryGetString("ServiceTypeCode3", x => item.ServiceTypeCode3 = x);
                        reader.TryGetString("ServiceType3", x => item.ServiceType3 = x);
                        reader.TryGetString("ServiceTypeCode4", x => item.ServiceTypeCode4 = x);
                        reader.TryGetString("ServiceType4", x => item.ServiceType4 = x);
                        reader.TryGetString("ActionCode", x => item.ActionCode = x);
                        reader.TryGetString("MaintenanceTypeCode", x => item.MaintenanceTypeCode = x);
                        reader.TryGetString("DistributionLossFactorCode", x => item.DistributionLossFactorCode = x);
                        reader.TryGetString("PremiseType", x => item.PremiseType = x);
                        reader.TryGetString("BillType", x => item.BillType = x);
                        reader.TryGetString("BillCalculator", x => item.BillCalculator = x);
                        reader.TryGetString("EsiId", x => item.EsiId = x);
                        reader.TryGetString("StationId", x => item.StationId = x);
                        reader.TryGetString("SpecialneedsIndicator", x => item.SpecialNeedsIndicator = x);
                        reader.TryGetString("PowerRegion", x => item.PowerRegion = x);
                        reader.TryGetString("EnergizedFlag", x => item.EnergizedFlag = x);
                        reader.TryGetString("EsiIdStartDate", x => item.EsiIdStartDate = x);
                        reader.TryGetString("EsiIdEndDate", x => item.EsiIdEndDate = x);
                        reader.TryGetString("EsiIdEligibilityDate", x => item.EsiIdEligbilityDate = x);
                        reader.TryGetString("NotificationWaiver", x => item.NotificationWaiver = x);
                        reader.TryGetString("SpecialReadSwitchDate", x => item.SpecialReadSwitchDate = x);
                        reader.TryGetString("PriorityCode", x => item.PriorityCode = x);
                        reader.TryGetString("PermitIndicator", x => item.PermitIndicator = x);
                        reader.TryGetString("RTODate", x => item.RtoDate = x);
                        reader.TryGetString("RTOTime", x => item.RtoTime = x);
                        reader.TryGetString("CSAFlag", x => item.CsaFlag = x);
                        reader.TryGetString("MembershipID", x => item.MembershipId = x);
                        reader.TryGetString("ESPAccountNumber", x => item.EspAccountNumber = x);
                        reader.TryGetString("LDCBillingCycle", x => item.LdcBillingCycle = x);
                        reader.TryGetString("LDCBudgetBillingCycle", x => item.LdcBudgetBillingCycle = x);
                        reader.TryGetString("WaterHeaters", x => item.WaterHeaters = x);
                        reader.TryGetString("LDCBudgetBillingStatus", x => item.LdcBudgetBillingStatus = x);
                        reader.TryGetString("PaymentArrangement", x => item.PaymentArrangement = x);
                        reader.TryGetString("NextMeterReadDate", x => item.NextMeterReadDate = x);
                        reader.TryGetString("ParticipatingInterest", x => item.ParticipatingInterest = x);
                        reader.TryGetString("EligibleLoadPercentage", x => item.EligibleLoadPercentage = x);
                        reader.TryGetString("TaxExemptionPercent", x => item.TaxExceptionPercent = x);
                        reader.TryGetString("CapacityObligation", x => item.CapacityObligation = x);
                        reader.TryGetString("TransmissionObligation", x => item.TransmissionObligation = x);
                        reader.TryGetString("TotalKWHHistory", x => item.TotalKwhHistory = x);
                        reader.TryGetString("NumberOfMonthsHistory", x => item.NumberOfMonthsHistory = x);
                        reader.TryGetString("PeakDemandHistory", x => item.PeakDemandHistory = x);
                        reader.TryGetString("AirConditioners", x => item.AirConditioners = x);
                        reader.TryGetString("PreviousEsiId", x => item.PreviousEsiId = x);
                        reader.TryGetString("GasPoolId", x => item.GasPoolId = x);
                        reader.TryGetString("LBMPZone", x => item.LbmpZone = x);
                        reader.TryGetString("ResidentialTaxPortion", x => item.ResidentialTaxPortion = x);
                        reader.TryGetString("ESPCommodityPrice", x => item.EspCommodityPrice = x);
                        reader.TryGetString("ESPFixedCharge", x => item.EspFixedCharge = x);
                        reader.TryGetString("ESPChargesCommTaxRate", x => item.EspChargesCommTaxRate = x);
                        reader.TryGetString("ESPChargesResTaxRate", x => item.EspChargesResTaxRate = x);
                        reader.TryGetString("GasSupplyServiceOption", x => item.GasSupplyServiceOption = x);
                        reader.TryGetString("FundsAuthorization", x => item.FundsAuthorization = x);
                        reader.TryGetString("BudgetBillingStatus", x => item.BudgetBillingStatus = x);
                        reader.TryGetString("FixedMonthlyCharge", x => item.FixedMonthlyCharge = x);
                        reader.TryGetString("TaxRate", x => item.TaxRate = x);
                        reader.TryGetString("CommodityPrice", x => item.CommodityPrice = x);
                        reader.TryGetString("MeterCycleCodeDesc", x => item.MeterCycleCodeDesc = x);
                        reader.TryGetString("BillCycleCodeDesc", x => item.BillCycleCodeDesc = x);
                        reader.TryGetString("FeeApprovedApplied", x => item.FeeApprovedApplied = x);
                        reader.TryGetString("MarketerCustomerAccountNumber", x => item.MarketerCustomerAccountNumber = x);
                        reader.TryGetString("GasSupplyServiceOptionCode", x => item.GasSupplyServiceOptionCode = x);
                        reader.TryGetString("HumanNeeds", x => item.HumanNeeds = x);
                        reader.TryGetString("ReinstatementDate", x => item.ReinstatementDate = x);
                        reader.TryGetString("MeterCycleCode", x => item.MeterCycleCode = x);
                        reader.TryGetString("SystemNumber", x => item.SystemNumber = x);
                        reader.TryGetString("StateLicenseNumber", x => item.StateLicenseNumber = x);
                        reader.TryGetString("SupplementalAccountNumber", x => item.SupplementalAccountNumber = x);
                        reader.TryGetString("NewCustomerIndicator", x => item.NewCustomerIndicator = x);
                        reader.TryGetString("PaymentCategory", x => item.PaymentCategory = x);
                        reader.TryGetString("PreviousESPAccountNumber", x => item.PreviousEspAccountNumber = x);
                        reader.TryGetString("RenewableEnergyIndicator", x => item.RenewableEnergyIndicator = x);
                        reader.TryGetString("SICCode", x => item.SicCode = x);
                        reader.TryGetString("ApprovalCodeIndicator", x => item.ApprovalCodeIndicator = x);
                        reader.TryGetString("RenewableEnergyCertification", x => item.RenewableEnergyCertification = x);
                        reader.TryGetString("NewPremiseIndicator", x => item.NewPremiseIndicator = x);
                        reader.TryGetString("SalesResponsibility", x => item.SalesResponsibility = x);
                        reader.TryGetString("CustomerReferenceNumber", x => item.CustomerReferenceNumber = x);
                        reader.TryGetString("TransactionReferenceNumber", x => item.TransactionReferenceNumber = x);
                        reader.TryGetString("ESPTransactionNumber", x => item.EspTransactionNumber = x);
                        reader.TryGetString("OldESPAccountNumber", x => item.OldEspAccountNumber = x);
                        reader.TryGetString("DFIIdentificationNumber", x => item.DfiIdentificationNumber = x);
                        reader.TryGetString("DFIAccountNumber", x => item.DfiAccountNumber = x);
                        reader.TryGetString("DFIIndicator1", x => item.DfiIndicator1 = x);
                        reader.TryGetString("DFIIndicator2", x => item.DfiIndicator2 = x);
                        reader.TryGetString("DFIQualifier", x => item.DfiQualifier = x);
                        reader.TryGetString("DFIRoutingNumber", x => item.DfiRoutingNumber = x);
                        reader.TryGetString("SpecialReadSwitchTime", x => item.SpecialReadSwitchTime = x);
                        reader.TryGetString("LDCAccountBalance", x => item.LdcAccountBalance = x);
                        reader.TryGetString("DisputedAmount", x => item.DisputedAmount = x);
                        reader.TryGetString("CurrentBalance", x => item.CurrentBalance = x);
                        reader.TryGetString("ArrearsBalance", x => item.ArrearsBalance = x);
                        reader.TryGetString("LDCSupplierBalance", x => item.LdcSupplierBalance = x);
                        reader.TryGetString("BudgetPlan", x => item.BudgetPlan = x);
                        reader.TryGetString("BudgetInstallment", x => item.BudgetInstallment = x);
                        reader.TryGetString("Deposit", x => item.Deposit = x);
                        reader.TryGetString("RemainingUtilBalanceBucket1", x => item.RemainingUtilBalanceBucket1 = x);
                        reader.TryGetString("RemainingUtilBalanceBucket2", x => item.RemainingUtilBalanceBucket2 = x);
                        reader.TryGetString("RemainingUtilBalanceBucket3", x => item.RemainingUtilBalanceBucket3 = x);
                        reader.TryGetString("RemainingUtilBalanceBucket4", x => item.RemainingUtilBalanceBucket4 = x);
                        reader.TryGetString("RemainingUtilBalanceBucket5", x => item.RemainingUtilBalanceBucket5 = x);
                        reader.TryGetString("RemainingUtilBalanceBucket6", x => item.RemainingUtilBalanceBucket6 = x);
                        reader.TryGetString("IntervalStatusType", x => item.IntervalStatusType = x);
                        reader.TryGetString("CustomerAuthorization", x => item.CustomerAuthorization = x);
                        reader.TryGetString("UnmeteredAcct", x => item.UnmeteredAcct = x);
                        reader.TryGetString("MaxDailyAmt", x => item.MaxDailyAmt = x);
                        reader.TryGetString("MeterAccessNote", x => item.MeterAccessNote = x);
                        reader.TryGetString("SpecialNeedsExpirationDate", x => item.SpecialNeedsExpirationDate = x);
                        reader.TryGetString("SwitchHoldStatusIndicator", x => item.SwitchHoldStatusIndicator = x);
                        reader.TryGetString("IgnoreRescind", x => item.IgnoreRescind = x);
                        reader.TryGetString("SpecialMeterConfig", x => item.SpecialMeterConfig = x);
                        reader.TryGetString("MaximumGeneration", x => item.MaximumGeneration = x);
                        reader.TryGetString("DaysInArrears", x => item.DaysInArrears = x);
                        reader.TryGetString("ServiceDeliveryPoint", x => item.ServiceDeliveryPoint = x);
                        reader.TryGetString("GasCapacityAssignment", x => item.GasCapacityAssignment = x);
                        
                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type814ServiceAccountChange[] ListServiceAccountChanges(int serviceKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814ExportListServiceAccountChangeRecords"))
            {
                command.AddWithValue("@Service_Key", serviceKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type814ServiceAccountChange>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type814ServiceAccountChange
                        {
                            ChangeKey = reader.GetInt32("ChangeKey"),
                            ServiceKey = serviceKey,
                        };

                        reader.TryGetString("ChangeReason", x => item.ChangeReason = x);
                        reader.TryGetString("ChangeDescription", x => item.ChangeDescription = x);
                        
                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type814ServiceDate[] ListServiceDates(int serviceKey)
        {
            return new Type814ServiceDate[0];
        }

        public Type814ServiceStatus[] ListServiceStatuses(int serviceKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814ExportListServiceStatusRecords"))
            {
                command.AddWithValue("@Service_Key", serviceKey)
                    .AddWithValue("@StatusType", "S");

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type814ServiceStatus>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type814ServiceStatus
                        {
                            ServiceKey = serviceKey,
                        };

                        reader.TryGetString("StatusCode", x => item.StatusCode = x);
                        reader.TryGetString("StatusReason", x => item.StatusReason = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type814ServiceReject[] ListServiceRejects(int serviceKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814ExportListServiceRejectRecords"))
            {
                command.AddWithValue("@Service_Key", serviceKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type814ServiceReject>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type814ServiceReject
                        {
                            ServiceKey = serviceKey,
                        };

                        reader.TryGetString("RejectCode", x => item.RejectCode = x);
                        reader.TryGetString("RejectReason", x => item.RejectReason = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type814ServiceMeter[] ListServiceMeters(int serviceKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814ExportListServiceMeterRecords"))
            {
                command.AddWithValue("@Service_Key", serviceKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type814ServiceMeter>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type814ServiceMeter
                        {
                            MeterKey = reader.GetInt32("Meter_Key"),
                            ServiceKey = serviceKey,
                        };

                        reader.TryGetString("EntityIdCode", x => item.EntityIdCode = x);
                        reader.TryGetString("MeterNumber", x => item.MeterNumber = x);
                        reader.TryGetString("MeterCode", x => item.MeterCode = x);
                        reader.TryGetString("MeterType", x => item.MeterType = x);
                        reader.TryGetString("LoadProfile", x => item.LoadProfile = x);
                        reader.TryGetString("RateClass", x => item.RateClass = x);
                        reader.TryGetString("RateSubClass", x => item.RateSubClass = x);
                        reader.TryGetString("MeterCycle", x => item.MeterCycle = x);
                        reader.TryGetString("MeterCycleDayOfMonth", x => item.MeterCycleDayOfMonth = x);
                        reader.TryGetString("SpecialneedsIndicator", x => item.SpecialNeedsIndicator = x);
                        reader.TryGetString("OldMeterNumber", x => item.OldMeterNumber = x);
                        reader.TryGetString("MeterOwnerIndicator", x => item.MeterOwnerIndicator = x);
                        reader.TryGetString("EntityType", x => item.EntityType = x);
                        reader.TryGetString("TimeOFUse", x => item.TimeOfUse = x);
                        reader.TryGetString("ESPRateCode", x => item.EspRateCode = x);
                        reader.TryGetString("OrganizationName", x => item.OrganizationName = x);
                        reader.TryGetString("FirstName", x => item.FirstName = x);
                        reader.TryGetString("MiddleName", x => item.MiddleName = x);
                        reader.TryGetString("NamePrefix", x => item.NamePrefix = x);
                        reader.TryGetString("NameSuffix", x => item.NameSuffix = x);
                        reader.TryGetString("IdentificationCode", x => item.IdentificationCode = x);
                        reader.TryGetString("EntityName2", x => item.EntityName2 = x);
                        reader.TryGetString("EntityName3", x => item.EntityName3 = x);
                        reader.TryGetString("Address1", x => item.Address1 = x);
                        reader.TryGetString("Address2", x => item.Address2 = x);
                        reader.TryGetString("City", x => item.City = x);
                        reader.TryGetString("State", x => item.State = x);
                        reader.TryGetString("Zip", x => item.Zip = x);
                        reader.TryGetString("CountryCode", x => item.CountryCode = x);
                        reader.TryGetString("County", x => item.County = x);
                        reader.TryGetString("PlanNumber", x => item.PlanNumber = x);
                        reader.TryGetString("ServicesReferenceNumber", x => item.ServicesReferenceNumber = x);
                        reader.TryGetString("AffiliationNumber", x => item.AffiliationNumber = x);
                        reader.TryGetString("CostElement", x => item.CostElement = x);
                        reader.TryGetString("CoverageCode", x => item.CoverageCode = x);
                        reader.TryGetString("LossReportNumber", x => item.LossReportNumber = x);
                        reader.TryGetString("GeographicNumber", x => item.GeographicNumber = x);
                        reader.TryGetString("ItemNumber", x => item.ItemNumber = x);
                        reader.TryGetString("LocationNumber", x => item.LocationNumber = x);
                        reader.TryGetString("PriceListNumber", x => item.PriceListNumber = x);
                        reader.TryGetString("ProductType", x => item.ProductType = x);
                        reader.TryGetString("QualityInspectionArea", x => item.QualityInspectionArea = x);
                        reader.TryGetString("ShipperCarOrderNumber", x => item.ShipperCarOrderNumber = x);
                        reader.TryGetString("StandardPointLocation", x => item.StandardPointLocation = x);
                        reader.TryGetString("ReportIdentification", x => item.ReportIdentification = x);
                        reader.TryGetString("Supplier", x => item.Supplier = x);
                        reader.TryGetString("Area", x => item.Area = x);
                        reader.TryGetString("CollectorIdentification", x => item.CollectorIdentification = x);
                        reader.TryGetString("VendorAgentNumber", x => item.VendorAgentNumber = x);
                        reader.TryGetString("VendorAbbreviation", x => item.VendorAbbreviation = x);
                        reader.TryGetString("VendorIdNumber", x => item.VendorIdNumber = x);
                        reader.TryGetString("VendorOrderNumber", x => item.VendorOrderNumber = x);
                        reader.TryGetString("PricingStructureCode", x => item.PricingStructureCode = x);
                        reader.TryGetString("MeterOwnerDUNS", x => item.MeterOwnerDuns = x);
                        reader.TryGetString("MeterOwner", x => item.MeterOwner = x);
                        reader.TryGetString("MeterInstallerDUNS", x => item.MeterInstallerDuns = x);
                        reader.TryGetString("MeterInstaller", x => item.MeterInstaller = x);
                        reader.TryGetString("MeterReaderDUNS", x => item.MeterReaderDuns = x);
                        reader.TryGetString("MeterReader", x => item.MeterReader = x);
                        reader.TryGetString("MeterMaintenanceProviderDUNS", x => item.MeterMaintenanceProviderDuns = x);
                        reader.TryGetString("MeterMaintenanceProvider", x => item.MeterMaintenanceProvider = x);
                        reader.TryGetString("MeterDataManagementAgentDUNS", x => item.MeterDataManagementAgentDuns = x);
                        reader.TryGetString("MeterDataManagementAgent", x => item.MeterDataManagementAgent = x);
                        reader.TryGetString("SchedulingCoordinatorDUNS", x => item.SchedulingCoordinatorDuns = x);
                        reader.TryGetString("SchedulingCoordinator", x => item.SchedulingCoordinator = x);
                        reader.TryGetString("MeterInstallPending", x => item.MeterInstallPending = x);
                        reader.TryGetString("PackageOption", x => item.PackageOption = x);
                        reader.TryGetString("UsageCode", x => item.UsageCode = x);
                        reader.TryGetString("MeterServiceVoltage", x => item.MeterServiceVoltage = x);
                        reader.TryGetString("LossFactor", x => item.LossFactor = x);
                        reader.TryGetString("AMSIndicator", x => item.AmsIndicator = x);
                        reader.TryGetString("SummaryInterval", x => item.SummaryInterval = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type814ServiceMeterChange[] ListServiceMeterChangesByService(int serviceKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814ExportListServiceMeterChangeRecords"))
            {
                command.AddWithValue("@Service_Key", serviceKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type814ServiceMeterChange>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type814ServiceMeterChange
                        {
                            ChangeKey = reader.GetInt32("Change_Key"),
                        };

                        reader.TryGetInt32("Meter_Key", x => item.MeterKey = x);
                        reader.TryGetString("ChangeReason", x => item.ChangeReason = x);
                        reader.TryGetString("ChangeDescription", x => item.ChangeDescription = x);
                        
                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type814ServiceMeterChange[] ListServiceMeterChanges(int meterKey)
        {
            return new Type814ServiceMeterChange[0];
        }

        public Type814ServiceMeterTou[] ListServiceMeterTous(int meterKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814ExportListServiceMeterChangeRecords"))
            {
                command.AddWithValue("@Meter_Key", meterKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type814ServiceMeterTou>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type814ServiceMeterTou();
                        reader.TryGetString("TOUCode", x => item.TouCode = x);
                        reader.TryGetString("MeasurementType", x => item.MeasurementType = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type814ServiceMeterType[] ListServiceMeterTypes(int meterKey)
        {
            return new Type814ServiceMeterType[0];
        }

        public void UpdateHeader(int headerKey, int marketFileId, string fileName)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp814ExportHeaderUpdate"))
            {
                command.AddWithValue("@814Key", headerKey)
                    .AddWithValue("@FileName", fileName);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }
    }
}

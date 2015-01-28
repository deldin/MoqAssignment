using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Parsers
{
    public class Import814Xml : IMarketFileParser
    {
        private readonly IMarketDataAccess marketDataAccess;
        private readonly ILogger logger;

        public Import814Xml(IMarketDataAccess marketDataAccess, ILogger logger)
        {
            this.marketDataAccess = marketDataAccess;
            this.logger = logger;
        }

        public IMarketFileParseResult Parse(string fileName)
        {
            logger.DebugFormat("Importing File \"{0}\"", fileName);

            var marketFile = new FileInfo(fileName);
            if (!marketFile.Exists)
            {
                logger.DebugFormat("File \"{0}\" does not exist or has been deleted.", fileName);
                return new Import814Model();
            }

            using (var stream = marketFile.OpenRead())
            {
                return Parse(stream);
            }
        }

        public IMarketFileParseResult Parse(Stream stream)
        {
            var context = new Import814Model();
            var document = XDocument.Load(stream);

            var documentElement = document.Root;
            if (documentElement == null)
                throw new InvalidOperationException();

            var namespaces = documentElement.Attributes()
                .Where(x => x.IsNamespaceDeclaration)
                .GroupBy(x => (x.Name.Namespace == XNamespace.None) ? string.Empty : x.Name.LocalName,
                    x => XNamespace.Get(x.Value))
                .ToDictionary(x => x.Key, x => x.First());

            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var countElement = documentElement.Element(empty + "TransactionCount");
            if (countElement != null)
                context.TransactionAuditCount = (int)countElement;

            context.InterchangeControlNbr = documentElement.GetChildText(empty + "InterchangeControlNbr");

            var headerElements = documentElement.Descendants(empty + "Header");
            foreach (var headerElement in headerElements)
            {
                var header = ParseHeader(headerElement, namespaces);
                context.AddHeader(header);
                context.TransactionActualCount++;
            }

            return context;
        }

        public Type814Header ParseHeader(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type814Header
            {
                TransactionSetId = element.GetChildText(empty + "TransactionSetId"),
                TransactionSetControlNbr = element.GetChildText(empty + "TransactionSetControlNbr"),
                TransactionSetPurposeCode = element.GetChildText(empty + "TransactionSetPurposeCode"),
                TransactionNbr = element.GetChildText(empty + "TransactionNbr"),
                TransactionDate = element.GetChildText(empty + "TransactionDate"),
                TransactionTime = element.GetChildText(empty + "TransactionTime"),
                TransactionTimeCode = element.GetChildText(empty + "TransactionTimeCode"),
                ReferenceNbr = element.GetChildText(empty + "ReferenceNbr"),
                ActionCode = element.GetChildText(empty + "ActionCode"),
                TdspDuns = element.GetChildText(empty + "TdspDuns"),
                TdspName = element.GetChildText(empty + "TdspName"),
                CrDuns = element.GetChildText(empty + "CrDuns"),
                CrName = element.GetChildText(empty + "CrName"),
                TransactionQualifier = element.GetChildText(empty + "TransactionQualifier"),
            };

            var nameLoopElement = element.Element(empty + "NameLoop");
            if (nameLoopElement != null)
            {
                var nameElements = nameLoopElement.Elements(empty + "Name");
                foreach(var nameElement in nameElements)
                {
                    var nameModel = ParseName(nameElement, namespaces);
                    model.AddName(nameModel);
                }
            }

            var serviceLoopElement = element.Element(empty + "ServiceLoop");
            if (serviceLoopElement != null)
            {
                var serviceElements = serviceLoopElement.Elements(empty + "Service");
                foreach (var serviceElement in serviceElements)
                {
                    var serviceModel = ParseService(serviceElement, namespaces);
                    model.AddService(serviceModel);
                }
            }

            if (model.Services.Any())
            {
                var firstServiceModel = model.Services[0];
                var transactionTypeId = marketDataAccess
                    .IdentifyTransactionType(model.ActionCode, firstServiceModel.ActionCode);

                model.TransactionTypeId = transactionTypeId;
            }

            return model;
        }

        public Type814Name ParseName(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type814Name
            {
                EntityIdType = element.GetChildText(empty + "EntityIdType"),
                EntityName = element.GetChildText(empty + "EntityName"),
                EntityName2 = element.GetChildText(empty + "EntityName2"),
                EntityName3 = element.GetChildText(empty + "EntityName3"),
                EntityDuns = element.GetChildText(empty + "EntityDuns"),
                EntityIdCode = element.GetChildText(empty + "EntityIdCode"),
                Address1 = element.GetChildText(empty + "Address1"),
                Address2 = element.GetChildText(empty + "Address2"),
                City = element.GetChildText(empty + "City"),
                State = element.GetChildText(empty + "State"),
                PostalCode = element.GetChildText(empty + "PostalCode"),
                CountryCode = element.GetChildText(empty + "CountryCode"),
                County = element.GetChildText(empty + "County"),
                ContactCode = element.GetChildText(empty + "ContactCode"),
                ContactName = element.GetChildText(empty + "ContactName"),
                ContactPhoneNbr1 = element.GetChildText(empty + "ContactPhoneNbr1"),
                ContactPhoneNbr2 = element.GetChildText(empty + "ContactPhoneNbr2"),
                ContactPhoneNbr3 = element.GetChildText(empty + "ContactPhoneNbr3"),
                EntityFirstName = element.GetChildText(empty + "EntityFirstName"),
                EntityLastName = element.GetChildText(empty + "EntityLastName"),
                CustType = element.GetChildText(empty + "CustType"),
                TaxingDistrict = element.GetChildText(empty + "TaxingDistrict"),
                EntityMiddleName = element.GetChildText(empty + "EntityMiddleName"),
            };

            return model;
        }

        public Type814Service ParseService(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type814Service
            {
                AssignId = element.GetChildText(empty + "AssignId"),
                ServiceTypeCode1 = element.GetChildText(empty + "ServiceTypeCode1"),
                ServiceType1 = element.GetChildText(empty + "ServiceType1"),
                ServiceTypeCode2 = element.GetChildText(empty + "ServiceTypeCode2"),
                ServiceType2 = element.GetChildText(empty + "ServiceType2"),
                ServiceTypeCode3 = element.GetChildText(empty + "ServiceTypeCode3"),
                ServiceType3 = element.GetChildText(empty + "ServiceType3"),
                ServiceTypeCode4 = element.GetChildText(empty + "ServiceTypeCode4"),
                ServiceType4 = element.GetChildText(empty + "ServiceType4"),
                ActionCode = element.GetChildText(empty + "ActionCode"),
                MaintenanceTypeCode = element.GetChildText(empty + "MaintenanceTypeCode"),
                DistributionLossFactorCode = element.GetChildText(empty + "DistributionLossFactorCode"),
                PremiseType = element.GetChildText(empty + "PremiseType"),
                BillType = element.GetChildText(empty + "BillType"),
                BillCalculator = element.GetChildText(empty + "BillCalculator"),
                EsiId = element.GetChildText(empty + "EsiId"),
                SpecialNeedsIndicator = element.GetChildText(empty + "SpecialNeedsIndicator"),
                StationId = element.GetChildText(empty + "StationId"),
                PowerRegion = element.GetChildText(empty + "PowerRegion"),
                EnergizedFlag = element.GetChildText(empty + "EnergizedFlag"),
                NotificationWaiver = element.GetChildText(empty + "NotificationWaiver"),
                EsiIdStartDate = element.GetChildText(empty + "EsiIdStartDate"),
                EsiIdEndDate = element.GetChildText(empty + "EsiIdEndDate"),
                EsiIdEligbilityDate = element.GetChildText(empty + "EsiIdEligibilityDate"),
                SpecialReadSwitchDate = element.GetChildText(empty + "SpecialReadSwitchDate"),
                SpecialReadSwitchTime = element.GetChildText(empty + "SpecialReadSwitchTime"),
                PriorityCode = element.GetChildText(empty + "PriorityCode"),
                RtoDate = element.GetChildText(empty + "RTODate"),
                RtoTime = element.GetChildText(empty + "RTOTime"),
                PermitIndicator = element.GetChildText(empty + "PermitIndicator"),
                CsaFlag = element.GetChildText(empty + "CSAFlag"),
                MembershipId = element.GetChildText(empty + "MembershipID"),
                EspAccountNumber = element.GetChildText(empty + "ESPAccountNumber"),
                LdcBillingCycle = element.GetChildText(empty + "LDCBillingCycle"),
                LdcBudgetBillingCycle = element.GetChildText(empty + "LDCBudgetBillingCycle"),
                WaterHeaters = element.GetChildText(empty + "WaterHeaters"),
                LdcBudgetBillingStatus = element.GetChildText(empty + "LDCBudgetBillingStatus"),
                PaymentArrangement = element.GetChildText(empty + "PaymentArrangement"),
                NextMeterReadDate = element.GetChildText(empty + "NextMeterReadDate"),
                ParticipatingInterest = element.GetChildText(empty + "ParticipatingInterest"),
                EligibleLoadPercentage = element.GetChildText(empty + "EligibleLoadPercentage"),
                TaxExceptionPercent = element.GetChildText(empty + "TaxExemptionPercent"),
                CapacityObligation = element.GetChildText(empty + "CapacityObligation"),
                TransmissionObligation = element.GetChildText(empty + "TransmissionObligation"),
                TotalKwhHistory = element.GetChildText(empty + "TotalKWHHistory"),
                NumberOfMonthsHistory = element.GetChildText(empty + "NumberOfMonthsHistory"),
                PeakDemandHistory = element.GetChildText(empty + "PeakDemandHistory"),
                AirConditioners = element.GetChildText(empty + "AirConditioners"),
                PreviousEsiId = element.GetChildText(empty + "PreviousEsiId"),
                GasPoolId = element.GetChildText(empty + "GasPoolId"),
                LbmpZone = element.GetChildText(empty + "LBMPZone"),
                ResidentialTaxPortion = element.GetChildText(empty + "ResidentialTaxPortion"),
                EspCommodityPrice = element.GetChildText(empty + "ESPCommodityPrice"),
                EspFixedCharge = element.GetChildText(empty + "ESPFixedCharge"),
                EspChargesCommTaxRate = element.GetChildText(empty + "ESPChargesCommTaxRate"),
                EspChargesResTaxRate = element.GetChildText(empty + "ESPChargesResTaxRate"),
                GasSupplyServiceOption = element.GetChildText(empty + "GasSupplyServiceOption"),
                GasSupplyServiceOptionCode = element.GetChildText(empty + "GasSupplyServiceOptionCode"),
                BudgetBillingStatus = element.GetChildText(empty + "BudgetBillingStatus"),
                FixedMonthlyCharge = element.GetChildText(empty + "FixedMonthlyCharge"),
                TaxRate = element.GetChildText(empty + "TaxRate"),
                MeterCycleCodeDesc = element.GetChildText(empty + "MeterCycleCodeDesc"),
                MeterCycleCode = element.GetChildText(empty + "MeterCycleCode"),
                BillCycleCodeDesc = element.GetChildText(empty + "BillCycleCodeDesc"),
                FeeApprovedApplied = element.GetChildText(empty + "FeeApprovedApplied"),
                MarketerCustomerAccountNumber = element.GetChildText(empty + "MarketerCustomerAccountNumber"),
                HumanNeeds = element.GetChildText(empty + "HumanNeeds"),
                ReinstatementDate = element.GetChildText(empty + "ReinstatementDate"),
                NewCustomerIndicator = element.GetChildText(empty + "NewCustomerIndicator"),
                NewPremiseIndicator = element.GetChildText(empty + "NewPremiseIndicator"),
                CustomerAuthorization = element.GetChildText(empty + "CustomerAuthorization"),
                LdcAccountBalance = element.GetChildText(empty + "LDCAccountBalance"),
                DisputedAmount = element.GetChildText(empty + "DisputedAmount"),
                CurrentBalance = element.GetChildText(empty + "CurrentBalance"),
                ArrearsBalance = element.GetChildText(empty + "ArrearsBalance"),
                LdcSupplierBalance = element.GetChildText(empty + "LDCSupplierBalance"),
                BudgetPlan = element.GetChildText(empty + "BudgetPlan"),
                BudgetInstallment = element.GetChildText(empty + "BudgetInstallment"),
                Deposit = element.GetChildText(empty + "Deposit"),
                RemainingUtilBalanceBucket1 = element.GetChildText(empty + "RemainingUtilBalanceBucket1"),
                RemainingUtilBalanceBucket2 = element.GetChildText(empty + "RemainingUtilBalanceBucket2"),
                RemainingUtilBalanceBucket3 = element.GetChildText(empty + "RemainingUtilBalanceBucket3"),
                RemainingUtilBalanceBucket4 = element.GetChildText(empty + "RemainingUtilBalanceBucket4"),
                RemainingUtilBalanceBucket5 = element.GetChildText(empty + "RemainingUtilBalanceBucket5"),
                RemainingUtilBalanceBucket6 = element.GetChildText(empty + "RemainingUtilBalanceBucket6"),
                IntervalStatusType = element.GetChildText(empty + "IntervalStatusType"),
                PaymentOption = element.GetChildText(empty + "PaymentOption"),
                MaxDailyAmt = element.GetChildText(empty + "MaxDailyAmt"),
                FundsAuthorization = element.GetChildText(empty + "FundsAuthorization"),
                SystemNumber = element.GetChildText(empty + "SystemNumber"),
                SpecialMeterConfig = element.GetChildText(empty + "SpecialMeterConfig"),
                MaximumGeneration = element.GetChildText(empty + "MaximumGeneration"),
                DaysInArrears = element.GetChildText(empty + "DaysInArrears"),
                ServiceDeliveryPoint = element.GetChildText(empty + "ServiceDeliveryPoint"),
            };

            var meterLoopElement = element.Element(empty + "ServiceMeterLoop");
            if (meterLoopElement != null)
            {
                var meterElements = meterLoopElement.Elements(empty + "ServiceMeter");
                foreach (var meterElement in meterElements)
                {
                    var meterModel = ParseServiceMeter(meterElement, namespaces);
                    model.AddMeter(meterModel);
                }
            }

            var rejectLoopElement = element.Element(empty + "ServiceRejectLoop");
            if (rejectLoopElement != null)
            {
                var rejectElements = rejectLoopElement.Elements(empty + "ServiceReject");
                foreach (var rejectElement in rejectElements)
                {
                    var rejectModel = ParseServiceReject(rejectElement, namespaces);
                    model.AddReject(rejectModel);
                }
            }

            var statusLoopElement = element.Element(empty + "ServiceStatusLoop");
            if (statusLoopElement != null)
            {
                var statusElements = statusLoopElement.Elements(empty + "ServiceStatus");
                foreach (var statusElement in statusElements)
                {
                    var statusModel = ParseServiceStatus(statusElement, namespaces);
                    model.AddStatus(statusModel);
                }
            }

            var changeLoopElement = element.Element(empty + "ServiceAccountChangeLoop");
            if (changeLoopElement != null)
            {
                var changeElements = changeLoopElement.Elements(empty + "ServiceAccountChange");
                foreach (var changeElement in changeElements)
                {
                    var changeModel = ParseServiceAccountChange(changeElement, namespaces);
                    model.AddChange(changeModel);
                }
            }

            var dateLoopElement = element.Element(empty + "ServiceDateLoop");
            if (dateLoopElement != null)
            {
                var dateElements = dateLoopElement.Elements(empty + "ServiceDate");
                foreach (var dateElement in dateElements)
                {
                    var dateModel = ParseServiceDate(dateElement, namespaces);
                    model.AddDate(dateModel);
                }
            }

            return model;
        }

        public Type814ServiceMeter ParseServiceMeter(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type814ServiceMeter
            {
                EntityIdCode = element.GetChildText(empty + "EntityIdCode"),
                MeterNumber = element.GetChildText(empty + "MeterNumber"),
                MeterCode = element.GetChildText(empty + "MeterCode"),
                MeterType = element.GetChildText(empty + "MeterType"),
                LoadProfile = element.GetChildText(empty + "LoadProfile"),
                RateClass = element.GetChildText(empty + "RateClass"),
                RateSubClass = element.GetChildText(empty + "RateSubClass"),
                MeterCycle = element.GetChildText(empty + "MeterCycle"),
                MeterCycleDayOfMonth = element.GetChildText(empty + "MeterCycleDayOfMonth"),
                SpecialNeedsIndicator = element.GetChildText(empty + "SpecialNeedsIndicator"),
                OldMeterNumber = element.GetChildText(empty + "OldMeterNumber"),
                MeterOwnerIndicator = element.GetChildText(empty + "MeterOwnerIndicator"),
                TimeOfUse = element.GetChildText(empty + "TimeOFUse"),
                EspRateCode = element.GetChildText(empty + "ESPRateCode"),
                PricingStructureCode = element.GetChildText(empty + "PricingStructureCode"),
                MeterServiceVoltage = element.GetChildText(empty + "MeterServiceVoltage"),
                SummaryInterval = element.GetChildText(empty + "SummaryInterval"),
            };

            var changeLoopElement = element.Element(empty + "ServiceMeterChangeLoop");
            if (changeLoopElement != null)
            {
                var changeElements = changeLoopElement.Elements(empty + "ServiceMeterChange");
                foreach (var changeElement in changeElements)
                {
                    var changeModel = ParseServiceMeterChange(changeElement, namespaces);
                    model.AddChange(changeModel);
                }
            }

            var touLoopElement = element.Element(empty + "ServiceMeterTOULoop");
            if (touLoopElement != null)
            {
                var touElements = touLoopElement.Elements(empty + "ServiceMeterTOU");
                foreach (var touElement in touElements)
                {
                    var touModel = ParseServiceMeterTou(touElement, namespaces);
                    model.AddTou(touModel);
                }
            }

            var typeLoopElement = element.Element(empty + "ServiceMeterTypeLoop");
            if (typeLoopElement != null)
            {
                var typeElements = typeLoopElement.Elements(empty + "ServiceMeterType");
                foreach (var typeElement in typeElements)
                {
                    var typeModel = ParseServiceMeterType(typeElement, namespaces);
                    model.AddType(typeModel);
                }
            }
            
            return model;
        }

        public Type814ServiceMeterChange ParseServiceMeterChange(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type814ServiceMeterChange
            {
                ChangeReason = element.GetChildText(empty + "ChangeReason"),
                ChangeDescription = element.GetChildText(empty + "ChangeDescription"),
            };

            return model;
        }

        public Type814ServiceMeterTou ParseServiceMeterTou(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type814ServiceMeterTou
            {
                TouCode = element.GetChildText(empty + "TOUCode"),
                MeasurementType = element.GetChildText(empty + "MeasurementType"),
            };

            return model;
        }

        public Type814ServiceMeterType ParseServiceMeterType(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type814ServiceMeterType
            {
                MeterMultiplier = element.GetChildText(empty + "MeterMultiplier"),
                MeterType = element.GetChildText(empty + "MeterType"),
                ProductType = element.GetChildText(empty + "ProductType"),
                TimeOfUse = element.GetChildText(empty + "TimeOfUse"),
                TimeOfUse2 = element.GetChildText(empty + "TimeOfUse2"),
                NumberOfDials = element.GetChildText(empty + "NumberOfDials"),
                UnmeteredNumberOfDevices = element.GetChildText(empty + "UnmeteredNumberOfDevices"),
                UnmeteredDescription = element.GetChildText(empty + "UnmeteredDescription"),
                StartMeterRead = element.GetChildText(empty + "StartMeterRead"),
                ChangeReason = element.GetChildText(empty + "ChangeReason"),
            };
            
            return model;
        }

        public Type814ServiceReject ParseServiceReject(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type814ServiceReject
            {
                RejectCode = element.GetChildText(empty + "RejectCode"),
                RejectReason = element.GetChildText(empty + "RejectReason"),
            };

            return model;
        }

        public Type814ServiceStatus ParseServiceStatus(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type814ServiceStatus
            {
                StatusCode = element.GetChildText(empty + "StatusCode"),
                StatusReason = element.GetChildText(empty + "StatusReason"),
                StatusType = element.GetChildText(empty + "StatusType"),
            };

            return model;
        }

        public Type814ServiceAccountChange ParseServiceAccountChange(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type814ServiceAccountChange
            {
                ChangeReason = element.GetChildText(empty + "ChangeReason"),
                ChangeDescription = element.GetChildText(empty + "ChangeDescription"),
            };

            return model;
        }

        public Type814ServiceDate ParseServiceDate(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type814ServiceDate
            {
                Qualifier = element.GetChildText(empty + "Qualifier"),
                Date = element.GetChildText(empty + "Date"),
                Time = element.GetChildText(empty + "Time"),
                TimeCode = element.GetChildText(empty + "TimeCode"),
                PeriodFormat = element.GetChildText(empty + "PeriodFormat"),
                Period = element.GetChildText(empty + "Period"),
                NotesDate = element.GetChildText(empty + "NotesDate"),
            };

            return model;
        }
    }
}

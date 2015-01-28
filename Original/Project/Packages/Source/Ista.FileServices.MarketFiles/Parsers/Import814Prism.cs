using System;
using System.IO;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Models;
using Ista.FileServices.MarketFiles.Parsers.ImportContexts;

namespace Ista.FileServices.MarketFiles.Parsers
{
    public class Import814Prism : IMarketFileParser
    {
        private readonly IClientDataAccess clientDataAccess;
        private readonly ILogger logger;

        public Import814Prism(IClientDataAccess clientDataAccess, ILogger logger)
        {
            this.clientDataAccess = clientDataAccess;
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
            var context = new Prism814Context();
            using (var reader = new StreamReader(stream))
            {
                string marketFileLine;
                while ((marketFileLine = reader.ReadLine()) != null)
                    ParseLine(context, marketFileLine);
            }

            if (context.ShouldResolve)
            {
                logger.Warn("Unresolved data identified after parsing 814. Transactions may not be completed.");
                context.ResolveToHeader();
            }

            return context.Results;
        }

        public void ParseLine(Prism814Context context, string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return;

            if (line.Length < 2)
                return;

            var indicator = line.Substring(0, 2);
            var marketFields = line.Split('|');

            switch (indicator)
            {
                case "SH":
                    context.ResolveToHeader();
                    context.Initialize();
                    context.TransactionActualCount++;
                    marketFields.TryAtIndex(3, x => context.Alias = x);
                    break;
                case "01":
                    ParseHeader(context, marketFields);
                    break;
                case "05":
                    ParseName(context, marketFields);
                    break;
                case "10":
                    context.RevertToHeader();
                    ParseService(context, marketFields);
                    break;
                case "11":
                    ParseServiceDate(context, marketFields);
                    break;
                case "12":
                    ParseServiceRate(context, marketFields);
                    break;
                case "15":
                    ParseServiceReject(context, marketFields);
                    break;
                case "16":
                    ParseServiceStatus(context, marketFields);
                    break;
                case "30":
                    context.RevertToService();
                    context.InitializeMeter();
                    ParseServiceMeter(context, marketFields);
                    break;
                case "35":
                    ParseServiceMeterType(context, marketFields);
                    break;
                case "38":
                    ParseServiceMeterChange(context, marketFields);
                    break;
                case "40":
                    context.RevertToService();
                    ParseServiceStatus(context, marketFields);
                    break;
                case "TL":
                    context.ResolveToHeader();
                    marketFields.TryAtIndexInt(1, x => context.TransactionAuditCount = x);
                    break;
            }
        }

        public void ParseHeader(Prism814Context context, string[] marketFields)
        {
            var model = new Type814Header
            {
                TransactionSetPurposeCode = "11",
                TransactionSetId = marketFields.AtIndex(16),
                TransactionNbr = marketFields.AtIndex(4),
                TransactionDate = marketFields.AtIndex(11),
                ReferenceNbr = marketFields.AtIndex(5),
                TdspDuns = marketFields.AtIndex(6),
                TdspName = marketFields.AtIndex(7),
                CrDuns = marketFields.AtIndex(8),
                CrName = marketFields.AtIndex(9),
            };

            marketFields.TryAtIndex(3, x => model.TransactionSetPurposeCode = x);

            var identifiedMarket = clientDataAccess.IdentifyMarket(model.TdspDuns);
            if (identifiedMarket.HasValue)
                context.SetMarket(identifiedMarket.Value);
            else
                context.SetMarket((int)MarketOptions.Texas);

            model.MarketId = context.MarketId;
            model.ProviderId = 1;

            model.ActionCode = context.Alias;
            if (context.Market == MarketOptions.Texas)
                model.ActionCode = marketFields.AtIndex(16);

            context.PushModel(model);
        }

        public void ParseName(Prism814Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type814Types.Header)
                throw new InvalidOperationException();

            var header = current as Type814Header;
            if (header == null)
                throw new InvalidOperationException();

            var model = new Type814Name
            {
                EntityIdType = marketFields.AtIndex(2),
                EntityName2 = marketFields.AtIndex(5),
                EntityName3 = marketFields.AtIndex(6),
                Address1 = marketFields.AtIndex(7),
                Address2 = marketFields.AtIndex(8),
                City = marketFields.AtIndex(9),
                State = marketFields.AtIndex(10),
                PostalCode = marketFields.AtIndex(11),
                ContactName = marketFields.AtIndex(16),
                ContactPhoneNbr1 = marketFields.AtIndex(14),
                CountryCode = marketFields.AtIndex(12),
                ContactPhoneNbr2 = marketFields.AtIndex(15),
            };

            marketFields.TryAtIndex(3, x => model.EntityName = x);

            header.AddName(model);
        }

        public void ParseService(Prism814Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type814Types.Header)
                throw new InvalidOperationException();

            var header = current as Type814Header;
            if (header == null)
                throw new InvalidOperationException();

            var model = new Type814Service
            {
                AssignId = marketFields.AtIndex(5),
                ServiceType1 = marketFields.AtIndex(2),
                ServiceTypeCode1 = string.Empty,
                ServiceType2 = marketFields.AtIndex(3),
                ServiceTypeCode2 = string.Empty,
                ServiceType3 = marketFields.AtIndex(4),
                ServiceTypeCode3 = string.Empty,
                ServiceType4 = marketFields.AtIndex(42),
                ServiceTypeCode4 = string.Empty,
                ActionCode = marketFields.AtIndex(7),
                DistributionLossFactorCode = marketFields.AtIndex(44),
                PremiseType = marketFields.AtIndex(47),
                BillType = marketFields.AtIndex(30),
                StationId = marketFields.AtIndex(33),
                EnergizedFlag = marketFields.AtIndex(46),
                PriorityCode = marketFields.AtIndex(45),
                PermitIndicator = marketFields.AtIndex(23),
                MembershipId = marketFields.AtIndex(9),
                PolrClass = marketFields.AtIndex(14),
                LdcBillingCycle = marketFields.AtIndex(28),
                LdcBudgetBillingStatus = marketFields.AtIndex(32),
                SpecialNeedsIndicator = marketFields.AtIndex(66),
                HumanNeeds = marketFields.AtIndex(54),
                SwitchHoldStatusIndicator = marketFields.AtIndex(60),
                SpecialMeterConfig = marketFields.AtIndex(69),
                MaximumGeneration = marketFields.AtIndex(70),
                DaysInArrears = marketFields.AtIndex(71),
            };

            marketFields.TryAtIndex(2, x => model.ServiceTypeCode1 = "SH");
            marketFields.TryAtIndex(3, x => model.ServiceTypeCode2 = "SH");
            marketFields.TryAtIndex(4, x => model.ServiceTypeCode3 = "SH");
            marketFields.TryAtIndex(42, x => model.ServiceTypeCode4 = "SH");
            
            if (context.Market == MarketOptions.Texas)
            {
                model.BillCalculator = string.Empty;
                model.EsiId = marketFields.AtIndex(43);

                var alias = context.Alias.TrimStart('0');
                switch(alias)
                {
                    case "19":
                        model.MaintenanceTypeCode = marketFields.AtIndex(6);
                        break;
                    case "8":
                    case "9":
                    case "11":
                    case "14":
                    case "28":
                        model.MaintenanceTypeCode = marketFields.AtIndex(8);
                        break;
                    case "20":
                        model.MaintenanceTypeCode = marketFields.AtIndex(24);
                        break;
                }
            }

            if (context.Market == MarketOptions.Maryland)
            {
                model.BillCalculator = marketFields.AtIndex(31);
                model.EsiId = marketFields.AtIndex(9);
                model.MaintenanceTypeCode = marketFields.AtIndex(6);
            }
            
            if (context.Market == MarketOptions.Maryland)
            {
                if (header.ActionCode.Equals("E", StringComparison.Ordinal) &&
                    model.ServiceType2.Equals("HU", StringComparison.Ordinal))
                {
                    header.ActionCode = "HU";
                    header.ReferenceNbr = model.AssignId;
                }
            }

            header.AddService(model);
            context.PushModel(model);
        }

        public void ParseServiceDate(Prism814Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type814Types.Service)
                throw new InvalidOperationException();

            var service = current as Type814Service;
            if (service == null)
                throw new InvalidOperationException();

            var effectiveDate = IdentifyServiceEffectiveDate(context, marketFields);

            service.SpecialReadSwitchDate = effectiveDate;
            service.SpecialNeedsExpirationDate = marketFields.AtIndex(7);
            service.RtoDate = marketFields.AtIndex(2);
            service.RtoTime = marketFields.AtIndex(3);
            service.EsiIdEndDate = marketFields.AtIndex(18);
            service.EsiIdEligbilityDate = marketFields.AtIndex(19);

            if (context.Market == MarketOptions.Texas)
            {
                var alias = context.Alias.TrimStart('0');
                var fieldIndex = alias.Equals("14") ? 5 : 17;
                service.EsiIdStartDate = marketFields.AtIndex(fieldIndex);
            }

            if (context.Market == MarketOptions.Maryland ||
                context.Market == MarketOptions.NewYork ||
                context.Market == MarketOptions.Pennsylvania)
            {
                service.EsiIdStartDate = marketFields.AtIndex(5);
            }
        }

        public void ParseServiceRate(Prism814Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type814Types.Service)
                throw new InvalidOperationException();

            var service = current as Type814Service;
            if (service == null)
                throw new InvalidOperationException();

            if (context.Market == MarketOptions.Texas)
                return;

            service.ParticipatingInterest = marketFields.AtIndex(2);
            service.EligibleLoadPercentage = marketFields.AtIndex(3);
            service.TaxExceptionPercent = marketFields.AtIndex(4);
            service.CapacityObligation = marketFields.AtIndex(6);
            service.TransmissionObligation = marketFields.AtIndex(7);
            service.TotalKwhHistory = marketFields.AtIndex(8);
            service.NumberOfMonthsHistory = marketFields.AtIndex(9);
            service.PeakDemandHistory = marketFields.AtIndex(10);
            service.AirConditioners = marketFields.AtIndex(13);
            service.WaterHeaters = marketFields.AtIndex(14);
        }

        public void ParseServiceReject(Prism814Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type814Types.Service)
                throw new InvalidOperationException();

            var service = current as Type814Service;
            if (service == null)
                throw new InvalidOperationException();

            var model = new Type814ServiceReject
            {
                RejectCode = marketFields.AtIndex(2), 
                RejectReason = marketFields.AtIndex(3),
            };

            service.AddReject(model);
        }

        public void ParseServiceStatus(Prism814Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type814Types.Service)
                throw new InvalidOperationException();

            var service = current as Type814Service;
            if (service == null)
                throw new InvalidOperationException();

            if (context.Market == MarketOptions.Texas)
            {
                var statusModel = new Type814ServiceStatus
                {
                    StatusCode = marketFields.AtIndex(2),
                    StatusReason = marketFields.AtIndex(3),
                };

                service.AddStatus(statusModel);
                return;
            }

            var changeModel = new Type814ServiceAccountChange
            {
                ChangeReason = marketFields.AtIndex(2),
                ChangeDescription = marketFields.AtIndex(3),
            };

            service.AddChange(changeModel);
        }

        public void ParseServiceMeter(Prism814Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type814Types.Service)
                throw new InvalidOperationException();

            var service = current as Type814Service;
            if (service == null)
                throw new InvalidOperationException();

            var model = new Type814ServiceMeter
            {
                EntityIdCode = marketFields.AtIndex(4),
                MeterNumber = marketFields.AtIndex(2),
                MeterType = marketFields.AtIndex(5),
                LoadProfile = marketFields.AtIndex(8),
                RateClass = marketFields.AtIndex(6),
                RateSubClass = marketFields.AtIndex(7),
                MeterCycle = marketFields.AtIndex(10),
                MeterCycleDayOfMonth = marketFields.AtIndex(36),
                SpecialNeedsIndicator = marketFields.AtIndex(21),
                OldMeterNumber = marketFields.AtIndex(3),
                MeterOwnerIndicator = marketFields.AtIndex(23),
                AmsIndicator = marketFields.AtIndex(51),
                SummaryInterval = marketFields.AtIndex(51),
            };

            service.AddMeter(model);
            context.PushModel(model);

            marketFields.TryAtIndex(11, x => context.ProductType = x);
            marketFields.TryAtIndex(13, x => context.UnmeterQuantity = x);
            marketFields.TryAtIndex(14, x => context.UnmeterDescription = x);

            var hasIntervalType = false;
            var hasProductType = false;
            marketFields.TryAtIndex(5, x => hasIntervalType = true);
            marketFields.TryAtIndex(11, x => hasProductType = true);

            if (hasIntervalType || !hasProductType) 
                return;

            var meterTypeModel = new Type814ServiceMeterType
            {
                ProductType = marketFields.AtIndex(11),
                UnmeteredNumberOfDevices = marketFields.AtIndex(13),
                UnmeteredDescription = marketFields.AtIndex(37),
            };

            model.AddType(meterTypeModel);
        }

        public void ParseServiceMeterType(Prism814Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type814Types.ServiceMeter)
                throw new InvalidOperationException();

            var meter = current as Type814ServiceMeter;
            if (meter == null)
                throw new InvalidOperationException();

            var model = new Type814ServiceMeterType
            {
                ProductType = context.ProductType,
                UnmeteredNumberOfDevices = context.UnmeterQuantity,
                UnmeteredDescription = context.UnmeterDescription,
                MeterMultiplier = marketFields.AtIndex(3),
                MeterType = marketFields.AtIndex(2),
                TimeOfUse = marketFields.AtIndex(5),
                TimeOfUse2 = marketFields.AtIndex(6),
                NumberOfDials = marketFields.AtIndex(4),
                StartMeterRead = marketFields.AtIndex(10),
                EndMeterRead = marketFields.AtIndex(11),
            };

            meter.AddType(model);
        }

        public void ParseServiceMeterChange(Prism814Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type814Types.ServiceMeter)
                throw new InvalidOperationException();

            var meter = current as Type814ServiceMeter;
            if (meter == null)
                throw new InvalidOperationException();

            var model = new Type814ServiceMeterChange
            {
                ChangeReason = marketFields.AtIndex(2),
                ChangeDescription = marketFields.AtIndex(3),
            };

            meter.AddChange(model);
        }

        public string IdentifyServiceEffectiveDate(Prism814Context context, string[] marketFields)
        {
            var alias = context.Alias.Trim('0');

            var effectiveDateIndex = 0;
            switch(alias)
            {
                case "1":
                case "10":
                    marketFields.TryAtIndex(11, x => effectiveDateIndex = 11);
                    break;
                case "6":
                case "11":
                case "D":
                    marketFields.TryAtIndex(7, x => effectiveDateIndex = 7);
                    break;
                case "12":
                case "13":
                    marketFields.TryAtIndex(15, x => effectiveDateIndex = 15);
                    if (effectiveDateIndex == 0)
                        marketFields.TryAtIndex(16, x => effectiveDateIndex = 16);
                    break;
                case "14":
                case "25":
                    marketFields.TryAtIndex(16, x => effectiveDateIndex = 16);
                    break;
                case "20":
                case "C":
                    marketFields.TryAtIndex(2, x => effectiveDateIndex = 2);
                    break;
                default:
                    marketFields.TryAtIndex(5, x => effectiveDateIndex = 5);
                    break;
            }

            return (effectiveDateIndex != 0)
                       ? marketFields.AtIndex(effectiveDateIndex)
                       : string.Empty;
        }
    }
}

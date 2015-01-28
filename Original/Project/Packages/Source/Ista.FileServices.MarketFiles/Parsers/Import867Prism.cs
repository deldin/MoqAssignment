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
    public class Import867Prism : IMarketFileParser
    {
        private readonly IClientDataAccess clientDataAccess;
        private readonly ILogger logger;
        
        public Import867Prism(IClientDataAccess clientDataAccess, ILogger logger)
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
                return new Import867Model();
            }

            using (var stream = marketFile.OpenRead())
            {
                return Parse(stream);
            }
        }
        
        public IMarketFileParseResult Parse(Stream stream)
        {
            var context = new Prism867Context();
            using (var reader = new StreamReader(stream))
            {
                string marketFileLine;
                while ((marketFileLine = reader.ReadLine()) != null)
                {
                    ParseLine(context, marketFileLine);
                    if (!context.IsValid)
                        throw new ApplicationException(context.Message);
                }
            }

            if (context.ShouldResolve)
            {
                logger.Warn("Unresolved data identified after parsing 867. Transactions may not be completed.");
                context.ResolveToHeader();
            }

            return context.Results;
        }

        public void ParseLine(Prism867Context context, string line)
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
                    ParseAccountBilledDates(context, marketFields);
                    break;
                case "06":
                    context.InitializeAccountBillQuantity();
                    ParseAccountBillQuantity(context, marketFields);
                    break;
                case "07":
                    context.RevertToHeader();
                    context.InitializeMeterServiceSummary();
                    ParseMeterServiceSummary(context, marketFields);
                    break;
                case "08":
                    context.InitializeNetIntervalQuantity(string.Empty, string.Empty, string.Empty);

                    if (context.LastModelAddedIsDetail() || context.Market == MarketOptions.Maryland)
                        context.InitializeNetIntervalQuantity(
                            marketFields.AtIndex(2), 
                            marketFields.AtIndex(3),
                            marketFields.AtIndex(4));

                    if (context.Market == MarketOptions.Maryland)
                        ParseMeterServiceSummaryQty(context, marketFields);

                    break;
                case "09":
                    if (context.LastModelAddedIsDetail())
                        ParseMeterServiceSummaryQty(context, marketFields);
                    break;
                case "10":
                    context.RevertToHeader();
                    context.RecordType = string.Empty;
                    ParseIntervalSummaryAcrossMeters(context, marketFields);
                    break;
                case "11":
                    if (context.LastModelAddedIsDetail())
                    {
                        context.RecordType = string.Empty;
                        ParseIntervalSummaryAcrossMetersQty(context, marketFields);
                    }
                    break;
                case "15":
                    context.RevertToHeader();
                    context.RecordType = string.Empty;
                    ParseServiceInfo(context, marketFields);
                    break;
                case "16":
                    if (context.LastModelAddedIsDetail())
                        ParseServiceInfoQty(context, marketFields);
                    break;
                case "17":
                    context.RevertToHeader();
                    context.RecordType = string.Empty;
                    ParseIntervalDetail(context, marketFields);
                    break;
                case "18":
                    if (context.LastModelAddedIsDetail())
                        ParseIntervalDetailQty(context, marketFields);
                    break;
                case "20":
                    context.RevertToHeader();
                    ParseScheduleDeterminants(context, marketFields);
                    break;
                case "30":
                    context.RevertToHeader();
                    context.RecordType = string.Empty;
                    ParseSwitch(context, marketFields);
                    break;
                case "35":
                    if (context.LastModelAddedIsDetail())
                        ParseSwitchQty(context, marketFields);
                    break;
                case "TL":
                    context.ResolveToHeader();
                    marketFields.TryAtIndexInt(1, x => context.TransactionAuditCount = x);
                    break;
                default:
                    logger.DebugFormat("Record Usage Type not found: {0}.", context.RecordType);
                    break;
            }
        }

        private void ParseSwitchQty(Prism867Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type867Types.Switch)
                throw new InvalidOperationException();

            var header = current as Type867Switch;
            if (header == null)
                throw new InvalidOperationException();
            
            var model = new Type867SwitchQty
            {
                Qualifier = marketFields.AtIndex(2),
                Uom = marketFields.AtIndex(3),
                SwitchRead = marketFields.AtIndex(4),
                MeasurementSignificanceCode = marketFields.AtIndex(5),
                Message = null
            };
            
            header.AddQuantity(model);
        }

        private void ParseSwitch(Prism867Context context, string[] marketFields)
        {
            context.RecordType = GetRecordType(context,marketFields, 0);

            var current = context.Current;
            if (current == null || current.ModelType != Type867Types.Header)
                throw new InvalidOperationException();

            var header = current as Type867Header;
            if (header == null)
                throw new InvalidOperationException();

            var model = new Type867Switch
            {
                TypeCode = context.RecordType,
                MeterNumber = marketFields.AtIndex(2),
                SwitchDate = marketFields.AtIndex(3)
            };

            context.PushModel(model);
            header.AddSwitch(model);
        }

        private void ParseScheduleDeterminants(Prism867Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type867Types.Header)
                throw new InvalidOperationException();

            var header = current as Type867Header;
            if (header == null)
                throw new InvalidOperationException();
            
            var model = new Type867ScheduleDeterminants
            {
                CapacityObligation = marketFields.AtIndex(2),
                TransmissionObligation = marketFields.AtIndex(3),
                LoadProfile = marketFields.AtIndex(4),
                LDCRateClass = marketFields.AtIndex(5),
                SpecialMeterConfig = marketFields.AtIndex(12),
                MaximumGeneration = marketFields.AtIndex(13),
                LDCRateSubClass = marketFields.AtIndex(14)
            };
            
            header.AddScheduledDeterminant(model);
        }

        private void ParseIntervalDetailQty(Prism867Context context, string[] marketFields)
        {
            switch (context.RecordType)
            {
                case "PM":
                    break;
                default:
                    logger.DebugFormat("Record Usage Type not found: {0}. Transaction Number \"{1}\".",
                        context.RecordType, context.TransactionNumber);
                    return;
            }

            var current = context.Current;
            if (current == null || current.ModelType != Type867Types.IntervalDetail)
                throw new InvalidOperationException();

            var detail = current as Type867IntervalDetail;
            if (detail == null)
                throw new InvalidOperationException();
            
            var model = new Type867IntervalDetailQty
            {
                Qualifier = marketFields.AtIndex(2),
                Quantity = marketFields.AtIndex(3),
                IntervalEndDate = marketFields.AtIndex(5),
                IntervalEndTime = marketFields.AtIndex(6)
            };
            
            detail.AddQuantity(model);
        }

        private void ParseIntervalDetail(Prism867Context context, string[] marketFields)
        {
            context.RecordType = GetRecordType(context, marketFields, 0);
            switch (context.RecordType)
            {
                case "PM":
                    break;
                default:
                    logger.DebugFormat("Record Usage Type not found: {0}. Transaction Number \"{1}\".",
                        context.RecordType, context.TransactionNumber);
                    return;
            }

            var current = context.Current;
            if (current == null || current.ModelType != Type867Types.Header)
                throw new InvalidOperationException();

            var header = current as Type867Header;
            if (header == null)
                throw new InvalidOperationException();

            var meterType = marketFields.AtIndex(6) + marketFields.AtIndex(7);

            var model = new Type867IntervalDetail
            {
                TypeCode = context.RecordType,
                MeterNumber = marketFields.AtIndex(4),
                ServicePeriodStart = marketFields.AtIndex(2),
                ServicePeriodEnd = marketFields.AtIndex(3),
                ExchangeDate = marketFields.AtIndex(5),
                ChannelNumber = marketFields.AtIndex(8),
                MeterRole = marketFields.AtIndex(9),
                MeterUOM = meterType.Length > 1 ? meterType.Substring(0, 2) : string.Empty,
                MeterInterval = meterType.Length > 2 ? meterType.Substring(2, 3) : string.Empty,
            };

            context.PushModel(model);
            header.AddIntervalDetail(model);
        }

        private void ParseServiceInfoQty(Prism867Context context, string[] marketFields)
        {
            switch (context.RecordType)
            {
                case "PL":
                    ParseNonIntervalDetailQty(context, marketFields);
                    break;
                case "BD":
                    ParseUnmeterDetailQty(context, marketFields);
                    break;
                case "BO":
                    ParseIntervalSummaryQty(context, marketFields);
                    break;
                default:
                    logger.DebugFormat("Record Usage Type not found: {0}. Transaction Number \"{1}\".",
                        context.RecordType, context.TransactionNumber);
                    break;
            }
        }

        private void ParseNonIntervalDetailQty(Prism867Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type867Types.NonIntervalDetail)
                throw new InvalidOperationException();

            var detail = current as Type867NonIntervalDetail;
            if (detail == null)
                throw new InvalidOperationException();

            // MeasurementSigificanceCode needs to be mapped to either 41, 42, 43 or 51
            var measurementCode = MapMeasurementSignificanceCode(marketFields.AtIndex(7));
            
            var model = new Type867NonIntervalDetailQty
            {
                Qualifier = marketFields.AtIndex(2),
                Quantity = marketFields.AtIndex(3),
                MeasurementCode = marketFields.AtIndex(6),
                Uom = marketFields.AtIndex(4),
                BeginRead = marketFields.AtIndex(8),
                EndRead = marketFields.AtIndex(9),
                TransformerLossFactor = marketFields.AtIndex(12),
                MeterMultiplier = marketFields.AtIndex(10),
                PowerFactor = marketFields.AtIndex(11)
            };

            if (!string.IsNullOrEmpty(measurementCode))
                model.MeasurementSignificanceCode = measurementCode;

            detail.AddQuantity(model);
        }

        private void ParseServiceInfo(Prism867Context context, string[] marketFields)
        {
            context.RecordType = GetRecordType(context, marketFields, 0);
            switch (context.RecordType)
            {
                case "PL":
                    ParseNonIntervalDetail(context, marketFields);
                    break;
                case "BD":
                    ParseUnmeterDetail(context, marketFields);
                    break;
                case "BO":
                    ParseIntervalSummary(context, marketFields);
                    break;
                default:
                    logger.DebugFormat("Record Usage Type not found: {0}. Transaction Number \"{1}\".",
                        context.RecordType, context.TransactionNumber);
                    break;
            }
        }

        private void ParseNonIntervalDetail(Prism867Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type867Types.Header)
                throw new InvalidOperationException();

            var header = current as Type867Header;
            if (header == null)
                throw new InvalidOperationException();

            var meterType = string.Empty;
            string ratchetDateTime = null;

            switch (context.Market)
            {
                case MarketOptions.Texas:
                    meterType = marketFields.AtIndex(6) + marketFields.AtIndex(7);
                    break;
                case MarketOptions.Maryland:
                    meterType = context.Uom;
                    break;
                case MarketOptions.Georgia:
                    ratchetDateTime = marketFields.AtIndex(31);
                    break;
            }

            var model = new Type867NonIntervalDetail
            {
                TypeCode = context.RecordType,
                MeterNumber = marketFields.AtIndex(4),
                MovementTypeCode = marketFields.AtIndex(18),
                ServicePeriodStart = marketFields.AtIndex(2),
                ServicePeriodEnd = marketFields.AtIndex(3),
                ExchangeDate = marketFields.AtIndex(5),
                MeterRole = marketFields.AtIndex(10),
                MeterUom = string.Empty,
                MeterInterval = string.Empty,
                RatchetDateTime = ratchetDateTime
            };

            if (!string.IsNullOrEmpty(meterType))
            {
                if (meterType.Length > 1)
                    model.MeterUom = meterType.Substring(0, 2);

                if (meterType.Length > 2)
                    model.MeterInterval = meterType.Substring(2);
            }

            context.PushModel(model);
            header.AddNonIntervalDetail(model);
        }

        private void ParseIntervalSummaryAcrossMetersQty(Prism867Context context, string[] marketFields)
        {
            context.RecordType = GetRecordType(context, marketFields, 0);
            switch (context.RecordType)
            {
                case "PP":
                    break;
                default:
                    logger.DebugFormat("Record Usage Type not found: {0}. Transaction Number \"{1}\".",
                        context.RecordType, context.TransactionNumber);
                    return;
            }

            var current = context.Current;
            if (current == null || current.ModelType != Type867Types.IntervalSummaryAcrossMeters)
                throw new InvalidOperationException();

            var detail = current as Type867IntervalSummaryAcrossMeters;
            if (detail == null)
                throw new InvalidOperationException();

            var model = new Type867IntervalSummaryAcrossMetersQty
            {
                Qualifier = marketFields.AtIndex(2),
                Quantity = marketFields.AtIndex(3),
                IntervalEndDate = marketFields.AtIndex(5),
                IntervalEndTime = marketFields.AtIndex(6)
            };
            
            detail.AddQuantity(model);
        }

        private void ParseIntervalSummaryAcrossMeters(Prism867Context context, string[] marketFields)
        {
            context.RecordType = GetRecordType(context, marketFields, 0);
            switch (context.RecordType)
            {
                case "PP":
                    break;
                default:
                    logger.DebugFormat("Record Usage Type not found: {0}. Transaction Number \"{1}\".",
                        context.RecordType, context.TransactionNumber);
                    return;
            }

            var current = context.Current;
            if (current == null || current.ModelType != Type867Types.Header)
                throw new InvalidOperationException();

            var header = current as Type867Header;
            if (header == null)
                throw new InvalidOperationException();

            var model = new Type867IntervalSummaryAcrossMeters
            {
                TypeCode = context.RecordType,
                MeterRole = marketFields.AtIndex(6),
                MeterUOM = string.Empty,
                MeterInterval = string.Empty,
            };

            var servicePeriodStart = marketFields.AtIndex(2);
            if (servicePeriodStart.Length < 9)
                model.ServicePeriodStart = servicePeriodStart;
            else
            {
                model.ServicePeriodStart = servicePeriodStart.Substring(0, 8);
                model.ServicePeriodStartTime = servicePeriodStart.Substring(8);
            }

            var servicePeriodEnd = marketFields.AtIndex(3);
            if (servicePeriodEnd.Length < 9)
                model.ServicePeriodEnd = servicePeriodEnd;
            else
            {
                model.ServicePeriodEnd = servicePeriodEnd.Substring(0, 8);
                model.ServicePeriodEndTime = servicePeriodEnd.Substring(8);
            }

            var meterType = marketFields.AtIndex(4) + marketFields.AtIndex(5);
            if (meterType.Length > 1)
                model.MeterUOM = meterType.Substring(0, 2);

            if (meterType.Length > 2)
                model.MeterInterval = meterType.Substring(2, 3);
            
            context.PushModel(model);
            header.AddIntervalSummaryAcrossMeters(model);
        }

        private void ParseMeterServiceSummaryQty(Prism867Context context, string[] marketFields)
        {
            switch (context.RecordType)
            {
                case "SU":
                    ParseNonIntervalSummaryQty(context, marketFields);
                    break;
                case "IA":
                    ParseNetIntervalSummaryQty(context, marketFields);
                    break;
                case "BO":
                    ParseIntervalSummaryQty(context, marketFields);
                    break;
                case "BD":
                    ParseUnmeterDetailQty(context, marketFields);
                    break;
                default:
                    logger.DebugFormat("Record Usage Type not found: {0}. Transaction Number \"{1}\".",
                        context.RecordType, context.TransactionNumber);
                    break;
            }
        }

        private void ParseUnmeterDetailQty(Prism867Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type867Types.UnMeterDetail)
                throw new InvalidOperationException();

            var detail = current as Type867UnMeterDetail;
            if (detail == null)
                throw new InvalidOperationException();

            var model = new Type867UnMeterDetailQty
            {
                Qualifier = marketFields.AtIndex(2),
                Quantity = marketFields.AtIndex(3),
                CompositeUom = marketFields.AtIndex(15),
                NumberOfDevices = marketFields.AtIndex(5),
                ConsumptionPerDevice = marketFields.AtIndex(16)
            };

            detail.AddQuantity(model);
        }

        private void ParseIntervalSummaryQty(Prism867Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type867Types.IntervalSummary)
                throw new InvalidOperationException();

            var detail = current as Type867IntervalSummary;
            if (detail == null)
                throw new InvalidOperationException();

            var model = new Type867IntervalSummaryQty
            {
                Qualifier = marketFields.AtIndex(2),
                Quantity = marketFields.AtIndex(3),
                Uom = marketFields.AtIndex(4),
            };

            if (marketFields.AtIndex(0) == "08")
            {
                model.MeasurementSignificanceCode = "51";
                model.MeterMultiplier = "1";
            }
            else
            {
                model.MeasurementCode = marketFields.AtIndex(6);
                model.MeasurementSignificanceCode = marketFields.AtIndex(7);
                model.BeginRead = marketFields.AtIndex(8);
                model.EndRead = marketFields.AtIndex(9);
                model.MeterMultiplier = marketFields.AtIndex(10);
                model.PowerFactor = marketFields.AtIndex(11);
                model.TransformerLossFactor = marketFields.AtIndex(12);
            }

            detail.AddQuantity(model);
        }

        private void ParseNetIntervalSummaryQty(Prism867Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type867Types.NetIntervalSummary)
                throw new InvalidOperationException();

            var detail = current as Type867NetIntervalSummary;
            if (detail == null)
                throw new InvalidOperationException();

            var model = new Type867NetIntervalSummaryQty
            {
                 Qualifier = context.Qualifier,
                 Quantity = context.Quantity,
            };

            if (context.Market == MarketOptions.Texas)
            {
                model.ServicePeriodStart = marketFields.AtIndex(4);
                model.ServicePeriodEnd = marketFields.AtIndex(5);
            }
            else
            {
                model.ServicePeriodStart = context.R05ServicePeriodBeginDate;
                model.ServicePeriodEnd = context.R05ServicePeriodEndDate;
            }

            detail.AddQuantity(model);
        }

        private void ParseNonIntervalSummaryQty(Prism867Context context, string[] marketFields)
        {
            // If Market is Maryland then we need to add the detail model first 
            // as it was not done with ParseNonIntervalSummary call earlier
            if (context.Market == MarketOptions.Maryland)
            {
                //This will also put the correct detail model in context Stack
                ParseNonIntervalSummaryForMaryland(context, marketFields);
            }
            
            var current = context.Current;
            if (current == null || current.ModelType != Type867Types.NonIntervalSummary)
                throw new InvalidOperationException();

            var detail = current as Type867NonIntervalSummary;
            if (detail == null)
                throw new InvalidOperationException();

            var model = new Type867NonIntervalSummaryQty();

            if (context.Market == MarketOptions.Maryland)
            {
                // MD does not provide 09 records in 867 transactions so we are parsing these as 08 records
                // 08 records do not contain begin/end date so we are using dates from the preceeding 07 record
                // We aren't getting any TOU usage in MD
                model.Qualifier = context.Qualifier;
                model.Quantity = context.Quantity;
                model.MeasurementSignificanceCode = "51"; 
                model.ServicePeriodStart = context.R07ServicePeriodBeginDate;
                model.ServicePeriodEnd = context.R07ServicePeriodEndDate;
            }
            else
            {
                model.Qualifier = context.Qualifier;
                model.Quantity = marketFields.AtIndex(2);
                model.MeasurementSignificanceCode = marketFields.AtIndex(3);
                model.ServicePeriodStart = marketFields.AtIndex(4);
                model.ServicePeriodEnd = marketFields.AtIndex(5);
            }
            
            detail.AddQuantity(model);
        }
        
        private void ParseHeader(Prism867Context context, string[] marketFields)
        {
            context.TransactionNumber = marketFields.AtIndex(6);
            context.ReportTypeCode = marketFields.AtIndex(4);

            // since we are going to end up with the value at 16
            // if the value in 35 is null or empty... 
            context.EsiId = marketFields.AtIndex(16);

            // let's try to the value at 35
            // if there it will overwrite what we already have
            marketFields.TryAtIndex(35, x => context.EsiId = x);

            var model = new Type867Header
            {
                DirectionFlag = true,
                TransactionSetId = context.Alias,
                TransactionNbr = context.TransactionNumber,
                TransactionDate = marketFields.AtIndex(12),
                ReportTypeCode = context.ReportTypeCode,
                EsiId = context.EsiId,
                ActionCode = marketFields.AtIndex(5),
                PowerRegion = marketFields.AtIndex(38),
                OriginalTransactionNbr = marketFields.AtIndex(7),
                ReferenceNbr = marketFields.AtIndex(39),
                TdspDuns = marketFields.AtIndex(8),
                TdspName = marketFields.AtIndex(9),
                CrDuns = marketFields.AtIndex(10),
                CrName = marketFields.AtIndex(11),
                UtilityAccountNumber = marketFields.AtIndex(16),
                EstimationReason = marketFields.AtIndex(48),
                EstimationDescription = marketFields.AtIndex(49),
                DoorHangerFlag = marketFields.AtIndex(50),
                EsnCount = marketFields.AtIndex(51),
                QoCount = marketFields.AtIndex(52)
            };

            marketFields.TryAtIndex(3, x => model.TransactionSetPurposeCode = x);

            if (!string.IsNullOrEmpty(model.TransactionDate) && model.TransactionDate.Length > 8)
                model.TransactionDate = model.TransactionDate.Substring(0, 8);
            
            var identifiedMarket = clientDataAccess.IdentifyMarket(model.TdspDuns);
            if (identifiedMarket.HasValue)
                context.SetMarket(identifiedMarket.Value);
            else
            {
                context.MarkAsInvalid(string.Format("Failed to load LDC Record for DUNS \"{0}\".", model.TdspDuns ?? "(null)"));
                return;
            }

            model.MarketID = context.MarketId;
            model.MarketFileId = context.MarketFileId;

            context.PushModel(model);
        }

        private void ParseAccountBilledDates(Prism867Context context, string[] marketFields)
        {
            marketFields.TryAtIndex(2, x => context.R05ServicePeriodBeginDate = x);
            marketFields.TryAtIndex(3, x => context.R05ServicePeriodEndDate = x);
        }

        private void ParseAccountBillQuantity(Prism867Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type867Types.Header)
                throw new InvalidOperationException();

            var header = current as Type867Header;
            if (header == null)
                throw new InvalidOperationException();
            
            context.Qualifier = marketFields.AtIndex(2);
            context.Quantity = marketFields.AtIndex(3);
            context.Uom = marketFields.AtIndex(4);

            var model = new Type867AccountBillQty
            {
                Qualifier = context.Qualifier,
                Quantity = context.Quantity,
                UOM = context.Uom,
                BeginDate = context.R05ServicePeriodBeginDate,
                EndDate = context.R05ServicePeriodEndDate
            };

            header.AddAccountBillQuantity(model);
        }

        private void ParseMeterServiceSummary(Prism867Context context, string[] marketFields)
        {
            context.RecordType = GetRecordType(context, marketFields, 0);
            switch (context.RecordType)
            {
                case "SU":
                    ParseNonIntervalSummary(context, marketFields);
                    break;
                case "IA":
                    ParseNetIntervalSummary(context, marketFields);
                    break;
                case "BO":
                    ParseIntervalSummary(context, marketFields);
                    break;
                case "BD":
                    ParseUnmeterDetail(context, marketFields.AtIndex(2), marketFields.AtIndex(3), "BD", string.Empty);
                    break;
                default:
                    logger.DebugFormat("Record Usage Type not found: {0}. Transaction Number \"{1}\".",
                        context.RecordType, context.TransactionNumber);
                    break;
            }
        }

        private void ParseUnmeterDetail(Prism867Context context, string[] marketFields)
        {
            ParseUnmeterDetail(context, marketFields.AtIndex(2), marketFields.AtIndex(3), marketFields.AtIndex(13),
                marketFields.AtIndex(17));
        }

        private void ParseUnmeterDetail(Prism867Context context, string servicePeriodStart, string servicePeriodEnd, string serviceType, string description)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type867Types.Header)
                throw new InvalidOperationException();

            var header = current as Type867Header;
            if (header == null)
                throw new InvalidOperationException();

            var model = new Type867UnMeterDetail
            {
                TypeCode = context.RecordType,
                ServicePeriodStart = servicePeriodStart,
                ServicePeriodEnd = servicePeriodEnd,
                ServiceType = serviceType,
                Description = description
            };

            context.PushModel(model);
            header.AddUnMeterDetail(model);
        }

        private void ParseIntervalSummary(Prism867Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type867Types.Header)
                throw new InvalidOperationException();

            var header = current as Type867Header;
            if (header == null)
                throw new InvalidOperationException();

            var model = new Type867IntervalSummary
            {
                TypeCode = context.RecordType,
                ServicePeriodStart = marketFields.AtIndex(2),
                ServicePeriodEnd = marketFields.AtIndex(3),
                MeterUOM = string.Empty,
                MeterInterval = string.Empty,
            };

            if (marketFields.AtIndex(0) == "07")
            {
                model.MeterNumber = context.EsiId;
                model.MeterRole = "A";
            }
            else
            {
                model.MeterNumber = marketFields.AtIndex(4);
                model.MovementTypeCode = marketFields.AtIndex(18); 
                model.ExchangeDate = marketFields.AtIndex(5); 
                model.ChannelNumber = marketFields.AtIndex(16); 
                model.MeterRole = marketFields.AtIndex(10); 
            }

            var meterType = marketFields.AtIndex(6) + marketFields.AtIndex(7);
            if (meterType.Length > 1)
                model.MeterUOM = meterType.Substring(0, 2);

            if (meterType.Length > 2)
                model.MeterInterval = meterType.Substring(2, 3);

            context.PushModel(model);
            header.AddIntervalSummary(model);
        }

        private void ParseNetIntervalSummary(Prism867Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type867Types.Header)
                throw new InvalidOperationException();

            var header = current as Type867Header;
            if (header == null)
                throw new InvalidOperationException();

            var model = new Type867NetIntervalSummary
            {
                MeterUom = marketFields.AtIndex(6),
                MeterInterval = marketFields.AtIndex(7),
                TypeCode = context.RecordType
            };

            context.PushModel(model);
            header.AddNetInervalSummary(model);
        }

        private void ParseNonIntervalSummary(Prism867Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type867Types.Header)
                throw new InvalidOperationException();

            var header = current as Type867Header;
            if (header == null)
                throw new InvalidOperationException();
            
            var meterType = string.Empty;

            switch (context.Market)
            {
                case MarketOptions.Texas:
                    meterType = marketFields.AtIndex(6) + marketFields.AtIndex(7);
                    break;
                case MarketOptions.Maryland:
                    // MD 07 records will be imported along with with the 08 records
                    // because the UOM is a part of the 08 record and must be read from there first
                    context.R07ServicePeriodBeginDate = marketFields.AtIndex(2);
                    context.R07ServicePeriodEndDate = marketFields.AtIndex(3);
                    return;
            }

            var model = new Type867NonIntervalSummary
            {
                TypeCode = context.RecordType,
                MeterUOM = string.Empty,
                MeterInterval = string.Empty,
            };

            if (meterType.Length > 1)
                model.MeterUOM = meterType.Substring(0, 2);

            if (meterType.Length > 2)
                model.MeterInterval = meterType.Substring(2);

            context.PushModel(model);
            header.AddNonIntervalSummary(model);
        }

        public void ParseNonIntervalSummaryForMaryland(Prism867Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type867Types.Header)
                throw new InvalidOperationException();

            var header = current as Type867Header;
            if (header == null)
                throw new InvalidOperationException();

            var model = new Type867NonIntervalSummary
            {
                TypeCode = context.RecordType,
                MeterUOM = string.Empty,
                MeterInterval = string.Empty,
            };

            if (!string.IsNullOrEmpty(context.Uom))
            {
                var meterType = context.Uom;
                if (meterType.Length > 1)
                    model.MeterUOM = meterType.Substring(0, 2);

                if (meterType.Length > 2)
                    model.MeterInterval = meterType.Substring(2);
            }

            context.PushModel(model);
            header.AddNonIntervalSummary(model);
        }
        
        private string GetRecordType(Prism867Context context, string[] marketFields, int indexToMatch)
        {
            var indicator = marketFields.AtIndex(indexToMatch);
            if (indicator.Equals("07"))
                return GetRecordTypeFor07(context, marketFields);

            if (indicator.Equals("15"))
                return GetRecordTypeFor15(marketFields);

            switch (indicator)
            {
                case "10":
                case "11":
                    return "PP";
                case "17":
                    return "PM";
                case "30":
                    return "BJ";
                default:
                    return string.Empty;
            }
        }

        private string GetRecordTypeFor07(Prism867Context context, string[] marketFields)
        {
            var meterFlag = marketFields.AtIndex(4);
            if (context.ReportTypeCode.Equals("DD"))
                return (context.Market == MarketOptions.Maryland && meterFlag.Equals("U"))
                           ? "BD"
                           : "SU";

            if (context.ReportTypeCode.Equals("C1"))
                return (context.Market == MarketOptions.Maryland)
                           ? "BO"
                           : "IA";

            switch (meterFlag)
            {
                case "M":
                case "U":
                    return "SU";
                case "I":
                    return "IA";
            }

            return string.Empty;
        }

        private string GetRecordTypeFor15(string[] marketFields)
        {
            var meterNumber = string.Empty;
            var meterInterval = string.Empty;
            marketFields.TryAtIndex(4, x => meterNumber = x);
            marketFields.TryAtIndexInt(7, x => meterInterval = x.ToString());

            var meterRole = marketFields.AtIndex(10);
            var masterAdditiveUsage = marketFields.AtIndex(18);

            if (meterRole.Equals("S") && masterAdditiveUsage.Equals("AO") && string.IsNullOrEmpty(meterNumber))
            {
                return (string.IsNullOrEmpty(meterInterval))
                           ? "PL"
                           : "BO";
            }

            if (string.IsNullOrEmpty(meterNumber))
                return "BD";

            return (string.IsNullOrEmpty(meterInterval))
                       ? "PL"
                       : "BO";
        }

        private string MapMeasurementSignificanceCode(string measurementSignificanceCode)
        {
            if (measurementSignificanceCode.Equals("71"))
                return "42";

            return measurementSignificanceCode;
        }
    }
}

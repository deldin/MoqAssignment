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
    public class Import867Xml : IMarketFileParser
    {
        private readonly ILogger _logger;

        public Import867Xml(ILogger logger)
        {
            _logger = logger;
        }

        public IMarketFileParseResult Parse(string fileName)
        {
            _logger.DebugFormat("Importing File \"{0}\"", fileName);

            var marketFile = new FileInfo(fileName);
            if (!marketFile.Exists)
            {
                _logger.DebugFormat("File \"{0}\" does not exist or has been deleted.", fileName);
                return Import867Model.Empty;
            }

            using (var stream = marketFile.OpenRead())
            {
                return Parse(stream);
            }
        }

        public IMarketFileParseResult Parse(Stream stream)
        {
            var context = new Import867Model();
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

        public Type867Header ParseHeader(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type867Header
            {
                DirectionFlag = true,
                TransactionSetId = element.GetChildText(empty + "TransactionSetId"),
                TransactionSetControlNbr = element.GetChildText(empty + "TransactionSetControlNbr"),
                TransactionSetPurposeCode = element.GetChildText(empty + "TransactionSetPurposeCode"),
                TransactionNbr = element.GetChildText(empty + "TransactionNbr"),
                TransactionDate = element.GetChildText(empty + "TransactionDate"),
                ReportTypeCode = element.GetChildText(empty + "ReportTypeCode"),
                ActionCode = element.GetChildText(empty +  "ActionCode"),
                ReferenceNbr = element.GetChildText(empty +  "ReferenceNbr"),
                DocumentDueDate = element.GetChildText(empty +  "DocumentDueDate"),
                EsiId = element.GetChildText(empty +  "EsiId"),
                PowerRegion = element.GetChildText(empty +  "PowerRegion"),
                OriginalTransactionNbr = element.GetChildText(empty +  "OriginalTransactionNbr"),
                TdspDuns = element.GetChildText(empty +  "TdspDuns"),
                TdspName = element.GetChildText(empty +  "TdspName"),
                CrDuns = element.GetChildText(empty +  "CrDuns"),
                CrName = element.GetChildText(empty +  "CrName"),
                UtilityAccountNumber = element.GetChildText(empty +  "UtilityAccountNumber"),
                PreviousUtilityAccountNumber = element.GetChildText(empty +  "PreviousUtilityAccountNumber"),
                EstimationReason = element.GetChildText(empty +  "EstimationReason"),
                EstimationDescription = element.GetChildText(empty +  "EstimationDescription"),
                DoorHangerFlag = element.GetChildText(empty +  "DoorHangerFlag"),
                EsnCount = element.GetChildText(empty +  "ESNCount"),
                QoCount = element.GetChildText(empty +  "QOCount"),
                NextMeterReadDate = element.GetChildText(empty +  "NextMeterReadDate"),
                InvoiceNbr = element.GetChildText(empty +  "InvoiceNbr"),
                UtilityContractID = element.GetChildText(empty +  "UtilityContractID"),
            };

            var nameLoopElement = element.Element(empty + "NameLoop");
            if (nameLoopElement != null)
            {
                var nameElements = nameLoopElement.Elements(empty + "Name");
                foreach (var nameElement in nameElements)
                {
                    var nameModel = ParseName(nameElement, namespaces);
                    model.AddName(nameModel);
                }
            }

            var nonIntervalDetailLoopElement = element.Element(empty + "NonIntervalDetailLoop");
            if (nonIntervalDetailLoopElement != null)
            {
                var nonIntervalDetailElements = nonIntervalDetailLoopElement.Elements(empty + "NonIntervalDetail");
                foreach (var nonIntervalDetailElement in nonIntervalDetailElements)
                {
                    var nonIntervalDetailModel = ParseNonIntervalDetail(nonIntervalDetailElement, namespaces);

                    if (String.IsNullOrEmpty(nonIntervalDetailModel.MeterNumber))//Logic that was in CSLA NonIntervalDetail.Insert method
                        nonIntervalDetailModel.MeterNumber = model.UtilityAccountNumber;

                    model.AddNonIntervalDetail(nonIntervalDetailModel);
                }
            }

            var nonIntervalSummaryLoopElement = element.Element(empty + "NonIntervalSummaryLoop");
            if (nonIntervalSummaryLoopElement != null)
            {
                var nonIntervalSummaryElements = nonIntervalSummaryLoopElement.Elements(empty + "NonIntervalSummary");
                foreach (var nonIntervalSummaryElement in nonIntervalSummaryElements)
                {
                    var nonIntervalSummaryModel = ParseNonIntervalSummary(nonIntervalSummaryElement, namespaces);
                    model.AddNonIntervalSummary(nonIntervalSummaryModel);
                }
            }

            var intervalDetailLoopElement = element.Element(empty + "IntervalDetailLoop");
            if (intervalDetailLoopElement != null)
            {
                var intervalDetailElements = intervalDetailLoopElement.Elements(empty + "IntervalDetail");
                foreach (var intervalDetailElement in intervalDetailElements)
                {
                    var intervalDetailModel = ParseIntervalDetail(intervalDetailElement, namespaces);
                    model.AddIntervalDetail(intervalDetailModel);
                }
            }

            var intervalSummaryLoopElement = element.Element(empty + "IntervalSummaryLoop");
            if (intervalSummaryLoopElement != null)
            {
                var intervalSummaryElements = intervalSummaryLoopElement.Elements(empty + "IntervalSummary");
                foreach (var intervalSummaryElement in intervalSummaryElements)
                {
                    var intervalSummaryModel = ParseIntervalSummary(intervalSummaryElement, namespaces);
                    model.AddIntervalSummary(intervalSummaryModel);
                }
            }

            var unMeterDetailLoopElement = element.Element(empty + "UnmeterDetailLoop");
            if (unMeterDetailLoopElement != null)
            {
                var unMeterDetailElements = unMeterDetailLoopElement.Elements(empty + "UnmeterDetail");
                foreach (var unMeterDetailElement in unMeterDetailElements)
                {
                    var unMeterDetailModel = ParseUnMeterDetail(unMeterDetailElement, namespaces);
                    model.AddUnMeterDetail(unMeterDetailModel);
                }
            }

            var unMeterSummaryLoopElement = element.Element(empty + "UnmeterSummaryLoop");
            if (unMeterSummaryLoopElement != null)
            {
                var unMeterSummaryElements = unMeterSummaryLoopElement.Elements(empty + "UnmeterSummary");
                foreach (var unMeterSummaryElement in unMeterSummaryElements)
                {
                    var unMeterSummaryModel = ParseUnMeterSummary(unMeterSummaryElement, namespaces);
                    model.AddUnMeterSummary(unMeterSummaryModel);
                }
            }

            var scheduledDeterminantsLoopElement = element.Element(empty + "ScheduleDeterminantsLoop");
            if (scheduledDeterminantsLoopElement != null)
            {
                var scheduledDeterminantsElements = scheduledDeterminantsLoopElement.Elements(empty + "ScheduleDeterminants");
                foreach (var scheduledDeterminantsElement in scheduledDeterminantsElements)
                {
                    var scheduledDeterminantModel = ParseScheduledDeterminant(scheduledDeterminantsElement, namespaces);
                    model.AddScheduledDeterminant(scheduledDeterminantModel);
                }
            }

            var intervalSummaryAcrossMetersLoopElement = element.Element(empty + "IntervalSummaryAcrossMetersLoop");
            if (intervalSummaryAcrossMetersLoopElement != null)
            {
                var intervalSummaryAcrossMetersElements = intervalSummaryAcrossMetersLoopElement.Elements(empty + "IntervalSummaryAcrossMeters");
                foreach (var intervalSummaryAcrossMetersElement in intervalSummaryAcrossMetersElements)
                {
                    var intervalSummaryAcrossMetersModel = ParseIntervalSummaryAcrossMeters(intervalSummaryAcrossMetersElement, namespaces);
                    model.AddIntervalSummaryAcrossMeters(intervalSummaryAcrossMetersModel);
                }
            }

            var gasProfileFactorEvaluationLoopElement = element.Element(empty + "GasProfileFactorEvaluationLoop");
            if (gasProfileFactorEvaluationLoopElement != null)
            {
                var gasProfileFactorEvaluationElements = gasProfileFactorEvaluationLoopElement.Elements(empty + "GasProfileFactorEvaluation");
                foreach (var gasProfileFactorEvaluationElement in gasProfileFactorEvaluationElements)
                {
                    var gasProfileFactorEvaluationModel = ParseGasProfileFactorEvaluation(gasProfileFactorEvaluationElement, namespaces);
                    model.AddGasProfileFactorEvaluation(gasProfileFactorEvaluationModel);
                }
            }

            var gasProfileFactorSampleLoopElement = element.Element(empty + "GasProfileFactorSampleLoop");
            if (gasProfileFactorSampleLoopElement != null)
            {
                var gasProfileFactorSampleElements = gasProfileFactorSampleLoopElement.Elements(empty + "GasProfileFactorSample");
                foreach (var gasProfileFactorSampleElement in gasProfileFactorSampleElements)
                {
                    var gasProfileFactorSampleModel = ParseGasProfileFactorSample(gasProfileFactorSampleElement, namespaces);
                    model.AddGasProfileFactorSample(gasProfileFactorSampleModel);
                }
            }
            
            return model;
        }
        
        public Type867Name ParseName(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type867Name
            {
                EntityIdType = element.GetChildText(empty +  "EntityIdType"),
                EntityName = element.GetChildText(empty +  "EntityName"),
                EntityDuns = element.GetChildText(empty +  "EntityDuns"),
                EntityIdCode = element.GetChildText(empty +  "EntityIdCode"),
                ServiceAddress1 = element.GetChildText(empty +  "ServiceAddress1"),
                ServiceAddress2 = element.GetChildText(empty +  "ServiceAddress2"),
                ServiceCity = element.GetChildText(empty +  "ServiceCity"),
                ServiceState = element.GetChildText(empty +  "ServiceState"),
                ServiceZipCode = element.GetChildText(empty +  "ServiceZipCode")
            };

            return model;
        }

        public Type867NonIntervalDetail ParseNonIntervalDetail(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type867NonIntervalDetail
            {
                TypeCode = element.GetChildText(empty +  "TypeCode"),
                MeterNumber = element.GetChildText(empty +  "MeterNumber"),
                MovementTypeCode = element.GetChildText(empty +  "MovementTypeCode"),
                ServicePeriodStart = element.GetChildText(empty +  "ServicePeriodStart"),
                ServicePeriodEnd = element.GetChildText(empty +  "ServicePeriodEnd"),
                ExchangeDate = element.GetChildText(empty +  "ExchangeDate"),
                MeterRole = element.GetChildText(empty +  "MeterRole"),
                MeterInterval = element.GetChildText(empty +  "MeterInterval"),
                CommodityCode = element.GetChildText(empty +  "CommodityCode"),
                NumberOfDials = element.GetChildText(empty +  "NumberOfDials"),
                ServicePointId = element.GetChildText(empty +  "ServicePointId"),
                UtilityRateServiceClass = element.GetChildText(empty +  "UtilityRateServiceClass"),
                RateSubClass = element.GetChildText(empty +  "RateSubClass"),
                MeterUom = element.GetChildText(empty +  "MeterUOM"),
                RatchetDateTime =element.GetChildText(empty +  "Ratchet_DateTime")
            };

            var quantitiesLoopElement = element.Element(empty + "NonIntervalDetailQtyLoop");
            if (quantitiesLoopElement != null)
            {
                var quantityElements = quantitiesLoopElement.Elements(empty + "NonIntervalDetailQty");
                foreach (var quantityElement in quantityElements)
                {
                    var quantityModel = ParseNonIntervalDetailQty(quantityElement, namespaces);
                    model.AddQuantity(quantityModel);
                }
            }
            
            return model;
        }

        public Type867NonIntervalDetailQty ParseNonIntervalDetailQty(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type867NonIntervalDetailQty
            {
                Qualifier = element.GetChildText(empty + "Qualifier"),
                Quantity = element.GetChildText(empty + "Quantity"),
                MeasurementCode = element.GetChildText(empty + "MeasurementCode"),
                CompositeUom = element.GetChildText(empty + "CompositeUOM"),
                Uom = element.GetChildText(empty + "UOM"),
                BeginRead = element.GetChildText(empty + "BeginRead"),
                EndRead = element.GetChildText(empty + "EndRead"),
                MeasurementSignificanceCode = element.GetChildText(empty + "MeasurementSignificanceCode"),
                TransformerLossFactor = element.GetChildText(empty + "TransformerLossFactor"),
                MeterMultiplier = element.GetChildText(empty + "MeterMultiplier"),
                PowerFactor = element.GetChildText(empty + "PowerFactor"),
                RangeMin = element.GetChildText(empty + "RangeMin"),
                RangeMax = element.GetChildText(empty + "RangeMax"),
                ThermFactor = element.GetChildText(empty + "ThermFactor"),
                DegreeDayFactor = element.GetChildText(empty + "DegreeDayFactor"),
                BackoutCredit = element.GetChildText(empty + "BackoutCredit")
            };

            return model;
        }
        
        public Type867NonIntervalSummary ParseNonIntervalSummary(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type867NonIntervalSummary
            {
                TypeCode = element.GetChildText(empty + "TypeCode"),
                MeterUOM = element.GetChildText(empty + "MeterUOM"),
                MeterInterval = element.GetChildText(empty + "MeterInterval"),
                CommodityCode = element.GetChildText(empty + "CommodityCode"),
                NumberOfDials = element.GetChildText(empty + "NumberOfDials"),
                ServicePointId = element.GetChildText(empty + "ServicePointId"),
                UtilityRateServiceClass = element.GetChildText(empty + "UtilityRateServiceClass"),
                RateSubClass = element.GetChildText(empty + "RateSubClass")
                
            };

            var quantitiesLoopElement = element.Element(empty + "NonIntervalSummaryQtyLoop");
            if (quantitiesLoopElement != null)
            {
                var quantityElements = quantitiesLoopElement.Elements(empty + "NonIntervalSummaryQty");
                foreach (var quantityElement in quantityElements)
                {
                    var quantityModel = ParseNonIntervalSummaryQty(quantityElement, namespaces);
                    model.AddQuantity(quantityModel);
                }
            }

            return model;
        }

        public Type867NonIntervalSummaryQty ParseNonIntervalSummaryQty(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type867NonIntervalSummaryQty
            {
                Qualifier = element.GetChildText(empty + "Qualifier"),
                Quantity = element.GetChildText(empty + "Quantity"),
                MeasurementSignificanceCode = element.GetChildText(empty + "MeasurementSignificanceCode"),
                ServicePeriodStart = element.GetChildText(empty + "ServicePeriodStart"),
                ServicePeriodEnd = element.GetChildText(empty + "ServicePeriodEnd"),
                RangeMin = element.GetChildText(empty + "RangeMin"),
                RangeMax = element.GetChildText(empty + "RangeMax"),
                ThermFactor = element.GetChildText(empty + "ThermFactor"),
                DegreeDayFactor = element.GetChildText(empty + "DegreeDayFactor"),
                CompositeUom = element.GetChildText(empty + "CompositeUOM")
            };
            
            return model;
        }
        
        public Type867UnMeterDetail ParseUnMeterDetail(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type867UnMeterDetail
            {
                TypeCode = element.GetChildText(empty +  "TypeCode"),
                CommodityCode = element.GetChildText(empty +  "CommodityCode"),
                ServicePeriodStart = element.GetChildText(empty +  "ServicePeriodStart"),
                ServicePeriodEnd = element.GetChildText(empty +  "ServicePeriodEnd"),
                ServiceType = element.GetChildText(empty +  "ServiceType"),
                Description = element.GetChildText(empty +  "Description"),
                UtilityRateServiceClass = element.GetChildText(empty +  "UtilityRateServiceClass"),
                RateSubClass = element.GetChildText(empty +  "RateSubClass")
            };

            var quantitiesLoopElement = element.Element(empty + "UnmeterDetailQtyLoop");
            if (quantitiesLoopElement != null)
            {
                var quantityElements = quantitiesLoopElement.Elements(empty + "UnmeterDetailQty");
                foreach (var quantityElement in quantityElements)
                {
                    var quantityModel = ParseUnMeterDetailQty(quantityElement, namespaces);
                    model.AddQuantity(quantityModel);
                }
            }

            return model;
        }

        public Type867UnMeterDetailQty ParseUnMeterDetailQty(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type867UnMeterDetailQty
            {
                Qualifier = element.GetChildText(empty + "Qualifier"),
                Quantity = element.GetChildText(empty + "Quantity"),
                CompositeUom = element.GetChildText(empty + "CompositeUOM"),
                Uom = element.GetChildText(empty + "UOM"),
                NumberOfDevices = element.GetChildText(empty + "NumberOfDevices"),
                ConsumptionPerDevice = element.GetChildText(empty + "ConsumptionPerDevice"),
                BackoutCredit = element.GetChildText(empty + "BackoutCredit"),
            };
            
            return model;
        }
        
        public Type867UnMeterSummary ParseUnMeterSummary(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type867UnMeterSummary
            {
                TypeCode = element.GetChildText(empty +  "TypeCode"),
                MeterUom = element.GetChildText(empty +  "MeterUOM"),
                CommodityCode = element.GetChildText(empty +  "CommodityCode"),
                UtilityRateServiceClass = element.GetChildText(empty +  "UtilityRateServiceClass"),
                RateSubClass = element.GetChildText(empty +  "RateSubClass")
            };

            var quantitiesLoopElement = element.Element(empty + "UnmeterSummaryQtyLoop");
            if (quantitiesLoopElement != null)
            {
                var quantityElements = quantitiesLoopElement.Elements(empty + "UnmeterSummaryQty");
                foreach (var quantityElement in quantityElements)
                {
                    var quantityModel = ParseUnMeterSummaryQty(quantityElement, namespaces);
                    model.AddQuantity(quantityModel);
                }
            }

            return model;
        }

        public Type867UnMeterSummaryQty ParseUnMeterSummaryQty(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type867UnMeterSummaryQty
            {
                Qualifier = element.GetChildText(empty +  "Qualifier"),
                Quantity = element.GetChildText(empty +  "Quantity"),
                MeasurementSignificanceCode = element.GetChildText(empty +  "MeasurementSignificanceCode"),
                ServicePeriodStart = element.GetChildText(empty +  "ServicePeriodStart"),
                ServicePeriodEnd = element.GetChildText(empty +  "ServicePeriodEnd")

            };

            return model;
        }

        public Type867IntervalDetail ParseIntervalDetail(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            int intervalDetailKey;
            int intervalSummaryKey;
            int headerKey;

            int.TryParse(element.GetChildText(empty + "IntervalDetail_Key"), out intervalDetailKey);
            int.TryParse(element.GetChildText(empty + "IntervalSummary_Key"), out intervalSummaryKey);
            int.TryParse(element.GetChildText(empty + "Header_Key"), out headerKey);
            
            var model = new Type867IntervalDetail
            {
                IntervalDetailKey = intervalDetailKey,
                IntervalSummaryKey = intervalSummaryKey,
                HeaderKey = headerKey, 
                TypeCode = element.GetChildText(empty +  "TypeCode"),
                MeterNumber = element.GetChildText(empty +  "MeterNumber"),
                ServicePeriodStart = element.GetChildText(empty +  "ServicePeriodStart"),
                ServicePeriodEnd = element.GetChildText(empty +  "ServicePeriodEnd"),
                ExchangeDate = element.GetChildText(empty +  "ExchangeDate"),
                ChannelNumber = element.GetChildText(empty +  "ChannelNumber"),
                MeterUOM = element.GetChildText(empty +  "MeterUOM"),
                MeterInterval = element.GetChildText(empty +  "MeterInterval"),
                MeterRole = element.GetChildText(empty +  "MeterRole"),
                CommodityCode = element.GetChildText(empty +  "CommodityCode"),
                NumberOfDials = element.GetChildText(empty +  "NumberOfDials"),
                ServicePointId = element.GetChildText(empty +  "ServicePointId"),
                UtilityRateServiceClass = element.GetChildText(empty +  "UtilityRateServiceClass"),
                RateSubClass = element.GetChildText(empty +  "RateSubClass")
            };

            var quantitiesLoopElement = element.Element(empty + "IntervalDetailQtyLoop");
            if (quantitiesLoopElement != null)
            {
                var quantityElements = quantitiesLoopElement.Elements(empty + "IntervalDetailQty");
                foreach (var quantityElement in quantityElements)
                {
                    var quantityModel = ParseIntervalDetailQty(quantityElement, namespaces);
                    model.AddQuantity(quantityModel);
                }
            }

            return model;
        }

        public Type867IntervalDetailQty ParseIntervalDetailQty(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            int intervalDetailKey;
            int intervalDetailQtyKey;
            
            int.TryParse(element.GetChildText(empty + "IntervalDetail_Key"), out intervalDetailKey);
            int.TryParse(element.GetChildText(empty + "IntervalDetailQty_Key"), out intervalDetailQtyKey);
            
            var model = new Type867IntervalDetailQty
            {
                IntervalDetailKey = intervalDetailKey,
                IntervalDetailQtyKey = intervalDetailQtyKey,
                DegreeDayFactor = element.GetChildText(empty +  "DegreeDayFactor"),
                IntervalEndDate = element.GetChildText(empty +  "IntervalEndDate"),
                IntervalEndTime = element.GetChildText(empty +  "IntervalEndTime"),
                ProcessDate = DateTime.Now,
                ProcessFlag = 0,
                Qualifier = element.GetChildText(empty +  "Qualifier"),
                Quantity = element.GetChildText(empty +  "Quantity"),
                RangeMax = element.GetChildText(empty +  "RangeMax"),
                RangeMin = element.GetChildText(empty +  "RangeMin"),
                ThermFactor = element.GetChildText(empty +  "ThermFactor")
            };

            return model;
        }
        
        public Type867IntervalSummary ParseIntervalSummary(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type867IntervalSummary
            {
                TypeCode = element.GetChildText(empty +  "TypeCode"),
                MeterNumber = element.GetChildText(empty +  "MeterNumber"),
                MovementTypeCode = element.GetChildText(empty +  "MovementTypeCode"),
                ServicePeriodStart = element.GetChildText(empty +  "ServicePeriodStart"),
                ServicePeriodEnd = element.GetChildText(empty +  "ServicePeriodEnd"),
                ExchangeDate = element.GetChildText(empty +  "ExchangeDate"),
                ChannelNumber = element.GetChildText(empty +  "ChannelNumber"),
                MeterRole = element.GetChildText(empty +  "MeterRole"),
                MeterUOM = element.GetChildText(empty +  "MeterUOM"),
                MeterInterval = element.GetChildText(empty +  "MeterInterval"),
                CommodityCode = element.GetChildText(empty +  "CommodityCode"),
                NumberOfDials = element.GetChildText(empty +  "NumberOfDials"),
                ServicePointId = element.GetChildText(empty +  "ServicePointId"),
                UtilityRateServiceClass = element.GetChildText(empty +  "UtilityRateServiceClass"),
                RateSubClass = element.GetChildText(empty +  "RateSubClass")
            };

            var quantitiesLoopElement = element.Element(empty + "IntervalSummaryQtyLoop");
            if (quantitiesLoopElement != null)
            {
                var quantityElements = quantitiesLoopElement.Elements(empty + "IntervalSummaryQty");
                foreach (var quantityElement in quantityElements)
                {
                    var quantityModel = ParseIntervalSummaryQty(quantityElement, namespaces);
                    model.AddQuantity(quantityModel);
                }
            }

            return model;
        }

        public Type867IntervalSummaryQty ParseIntervalSummaryQty(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type867IntervalSummaryQty
            {
                Qualifier = element.GetChildText(empty + "Qualifier"),
                Quantity = element.GetChildText(empty + "Quantity"),
                MeasurementCode = element.GetChildText(empty + "MeasurementCode"),
                CompositeUom = element.GetChildText(empty + "CompositeUOM"),
                Uom = element.GetChildText(empty + "UOM"),
                BeginRead = element.GetChildText(empty + "BeginRead"),
                EndRead = element.GetChildText(empty + "EndRead"),
                MeasurementSignificanceCode = element.GetChildText(empty + "MeasurementSignificanceCode"),
                TransformerLossFactor = element.GetChildText(empty + "TransformerLossFactor"),
                MeterMultiplier = element.GetChildText(empty + "MeterMultiplier"),
                PowerFactor = element.GetChildText(empty + "PowerFactor"),
                RangeMin = element.GetChildText(empty + "RangeMin"),
                RangeMax = element.GetChildText(empty + "RangeMax"),
                ThermFactor = element.GetChildText(empty + "ThermFactor"),
                DegreeDayFactor = element.GetChildText(empty + "DegreeDayFactor")
            };

            return model;
        }

        public Type867IntervalSummaryAcrossMeters ParseIntervalSummaryAcrossMeters(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;
            
            var model = new Type867IntervalSummaryAcrossMeters
            {
                TypeCode = element.GetChildText(empty + "TypeCode"),
                ServicePeriodStart = element.GetChildText(empty + "ServicePeriodStart"),
                ServicePeriodStartTime = element.GetChildText(empty + "ServicePeriodStartTime"),
                ServicePeriodEnd = element.GetChildText(empty + "ServicePeriodEnd"),
                ServicePeriodEndTime = element.GetChildText(empty + "ServicePeriodEndTime"),
                MeterRole = element.GetChildText(empty + "MeterRole"),
                MeterUOM = element.GetChildText(empty + "MeterUOM"),
                MeterInterval = element.GetChildText(empty + "MeterInterval")
            };

            var quantitiesLoopElement = element.Element(empty + "IntervalSummaryAcrossMetersQtyLoop");
            if (quantitiesLoopElement != null)
            {
                var quantityElements = quantitiesLoopElement.Elements(empty + "IntervalSummaryAcrossMetersQty");
                foreach (var quantityElement in quantityElements)
                {
                    var quantityModel = ParseIntervalSummaryAcrossMetersQty(quantityElement, namespaces);
                    model.AddQuantity(quantityModel);
                }
            }

            return model;
        }

        public Type867IntervalSummaryAcrossMetersQty ParseIntervalSummaryAcrossMetersQty(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;
            
            var model = new Type867IntervalSummaryAcrossMetersQty
            {
                Qualifier = element.GetChildText(empty + "Qualifier"),
                Quantity = element.GetChildText(empty + "Quantity"),
                IntervalEndDate =element.GetChildText(empty + "IntervalEndDate"),
                IntervalEndTime = element.GetChildText(empty + "IntervalEndTime")
            };

            return model;
        }

        public Type867ScheduleDeterminants ParseScheduledDeterminant(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;
            
            var model = new Type867ScheduleDeterminants
            {
                CapacityObligation = element.GetChildText(empty + "CapacityObligation"),
                TransmissionObligation = element.GetChildText(empty + "TransmissionObligation"),
                LoadProfile = element.GetChildText(empty + "LoadProfile"),
                LDCRateClass = element.GetChildText(empty + "LDCRateClass"),
                Zone = element.GetChildText(empty + "Zone"),
                BillCycle = element.GetChildText(empty + "BillCycle"),
                MeterNumber = element.GetChildText(empty + "MeterNumber"),
                EffectiveDate = element.GetChildText(empty + "EffectiveDate"),
                LossFactor = element.GetChildText(empty + "LossFactor"),
                ServiceVoltage = element.GetChildText(empty + "ServiceVoltage"),
                SpecialMeterConfig = element.GetChildText(empty + "SpecialMeterConfig"),
                MaximumGeneration = element.GetChildText(empty + "MaximumGeneration"),
                LDCRateSubClass = element.GetChildText(empty + "LDCRateSubClass")
            };

            return model;
        }

        public Type867GasProfileFactorEvaluation ParseGasProfileFactorEvaluation(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type867GasProfileFactorEvaluation
            {
                ProfilePeriodStartDate = element.GetChildText(empty + "ProfilePeriodStartDate"),
                CustomerServiceInitDate = element.GetChildText(empty + "CustomerServiceInitdate"),
                UtilityRateServiceClass = element.GetChildText(empty + "UtilityRateServiceClass"),
                RateSubClass = element.GetChildText(empty + "RateSubClass"),
                NonHeatLoadFactorQty = element.GetChildText(empty + "NonHeatLoadFactorQty"),
                WeatherNormLoadFactorQty = element.GetChildText(empty + "WeatherNormLoadFactorQty"),
                LoadFactorRatio = element.GetChildText(empty + "LoadFactorRatio"),
                UFGRatePct = element.GetChildText(empty + "UFGRatePct"),
                MaximumDeliveryQty = element.GetChildText(empty + "MaximumDeliveryQty")
            };

            return model;
        }

        public Type867GasProfileFactorSample ParseGasProfileFactorSample(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;
            
            var model = new Type867GasProfileFactorSample
            {
                ReportMonth = element.GetChildText(empty + "ReportMonth"),
                AnnualPeriod = element.GetChildText(empty + "AnnualPeriod"),
                NormProjectedUsageQty = element.GetChildText(empty + "NormProjectedUsageQty"),
                WeatherNormUsageProjectedQty = element.GetChildText(empty + "WeatherNormUsageProjectedQty"),
                NormProjectedDeliveryQty = element.GetChildText(empty + "NormProjectedDeliveryQty"),
                WeatherNormProjectedDeliveryQty = element.GetChildText(empty + "WeatherNormProjectedDeliveryQty"),
                ProjectedDailyDeliveryQty = element.GetChildText(empty + "ProjectedDailyDeliveryQty"),
                DesignProjectedUsageQty = element.GetChildText(empty + "DesignProjectedUsageQty"),
                DesignProjectedDeliveryQty = element.GetChildText(empty + "DesignProjectedDeliveryQty"),
                ProjectedBalancingUseQty = element.GetChildText(empty + "ProjectedBalancingUseQty"),
                ProjectedSwingChargeAmt = element.GetChildText(empty + "ProjectedSwingChargeAmt")
            };

            return model;
        }  
    }
}

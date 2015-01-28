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
    public class Import650Xml : IMarketFileParser
    {
        private readonly ILogger logger;

        public Import650Xml(ILogger logger)
        {
            this.logger = logger;
        }

        public IMarketFileParseResult Parse(string fileName)
        {
            logger.DebugFormat("Importing File \"{0}\"", fileName);

            var marketFile = new FileInfo(fileName);
            if (!marketFile.Exists)
            {
                logger.DebugFormat("File \"{0}\" does not exist or has been deleted.", fileName);
                return Import650Model.Empty;
            }

            using (var stream = marketFile.OpenRead())
            {
                return Parse(stream);
            }
        }

        public IMarketFileParseResult Parse(Stream stream)
        {
            var context = new Import650Model();
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

        public Type650Header ParseHeader(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type650Header
            {
                TransactionSetId = element.GetChildText(empty + "TransactionSetId"),
                TransactionSetControlNbr = element.GetChildText(empty + "TransactionSetControlNbr"),
                TransactionSetPurposeCode = element.GetChildText(empty + "TransactionSetPurposeCode"),
                TransactionDate = element.GetChildText(empty + "TransactionDate"),
                TransactionNbr = element.GetChildText(empty + "TransactionNbr"),
                ReferenceNbr = element.GetChildText(empty + "ReferenceNbr"),
                TransactionType = element.GetChildText(empty + "TransactionType"),
                ActionCode = element.GetChildText(empty + "ActionCode"),
                TdspName = element.GetChildText(empty + "TdspName"),
                TdspDuns = element.GetChildText(empty + "TdspDuns"),
                CrName = element.GetChildText(empty + "CrName"),
                CrDuns = element.GetChildText(empty + "CrDuns"),
                ProcessedReceivedDateTime = element.GetChildText(empty + "ProcessedReceivedDateTime"),
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

            return model;
        }

        public Type650Name ParseName(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type650Name
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
                ContactName = element.GetChildText(empty + "ContactName"),
                ContactPhoneNbr1 = element.GetChildText(empty + "ContactPhoneNbr1"),
                ContactPhoneNbr2 = element.GetChildText(empty + "CotnactPhoneNbr2"),
            };

            return model;
        }

        public Type650Service ParseService(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type650Service
            {
                PurposeCode = element.GetChildText(empty + "PurposeCode"),
                PriorityCode = element.GetChildText(empty + "PriorityCode"),
                EsiId = element.GetChildText(empty + "ESIID"),
                SpecialProcessCode = element.GetChildText(empty + "SpecialProcessCode"),
                ServiceReqDate = element.GetChildText(empty + "ServiceReqDate"),
                NotBeforeDate = element.GetChildText(empty + "NotBeforeDate"),
                CallAhead = element.GetChildText(empty + "CallAhead"),
                PremLocation = element.GetChildText(empty + "PremLocation"),
                AccStatusCode = element.GetChildText(empty + "AccStatusCode"),
                AccStatusDesc = element.GetChildText(empty + "AccStatusDesc"),
                EquipLocation = element.GetChildText(empty + "EquipLocation"),
                ServiceOrderNbr = element.GetChildText(empty + "ServiceOrderNbr"),
                CompletionDate = element.GetChildText(empty + "CompletionDate"),
                CompletionTime = element.GetChildText(empty + "CompletionTime"),
                ReportRemarks = element.GetChildText(empty + "ReportRemarks"),
                Directions = element.GetChildText(empty + "Directions"),
                MeterNbr = element.GetChildText(empty + "MeterNbr"),
                MeterReadDate = element.GetChildText(empty + "MeterReadDate"),
                MeterTestDate = element.GetChildText(empty + "MeterTestDate"),
                MeterTestResults = element.GetChildText(empty + "MeterTestResults"),
                IncidentCode = element.GetChildText(empty + "IncidentCode"),
                EstRestoreDate = element.GetChildText(empty + "EstRestoreDate"),
                EstRestoreTime = element.GetChildText(empty + "EstRestoreTime"),
                IntStartDate = element.GetChildText(empty + "IntStartDate"),
                IntStartTime = element.GetChildText(empty + "IntStartTime"),
                RepairRecommended = element.GetChildText(empty + "RepairRecommended"),
                Rescheduled = element.GetChildText(empty + "Rescheduled"),
                InterDurationPeriod = element.GetChildText(empty + "InterDurationPeriod"),
                AreaOutage = element.GetChildText(empty + "AreaOutage"),
                CustRepairRemarks = element.GetChildText(empty + "CustRepairRemarks"),
                MeterReadUom = element.GetChildText(empty + "MeterReadUOM"),
                MeterRead = element.GetChildText(empty + "MeterRead"),
                MeterReadCode = element.GetChildText(empty + "MeterReadCode"),
                Membership = element.GetChildText(empty + "Membership"),
                RemarksPermanentSuspend = element.GetChildText(empty + "RemarksPermanentSuspend"),
                DisconnectAuthorization = element.GetChildText(empty + "DisconnectAuthorization"),
            };

            var changeLoopElement = element.Element(empty + "ServiceChangeLoop");
            if (changeLoopElement != null)
            {
                var changeElements = changeLoopElement.Elements(empty + "ServiceChange");
                foreach (var changeElement in changeElements)
                {
                    var changeModel = ParseServiceChange(changeElement, namespaces);
                    model.AddChange(changeModel);
                }
            }

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

            var poleLoopElement = element.Element(empty + "ServicePoleLoop");
            if (poleLoopElement != null)
            {
                var poleElements = poleLoopElement.Elements(empty + "ServicePole");
                foreach (var poleElement in poleElements)
                {
                    var poleModel = ParseServicePole(poleElement, namespaces);
                    model.AddPole(poleModel);
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

            return model;
        }

        public Type650ServiceChange ParseServiceChange(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type650ServiceChange
            {
                ChangeReason = element.GetChildText(empty + "ChangeReason"),
            };

            return model;
        }

        public Type650ServiceMeter ParseServiceMeter(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type650ServiceMeter
            {
                MeterNumber = element.GetChildText(empty + "MeterNumber"),
            };

            return model;
        }

        public Type650ServicePole ParseServicePole(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type650ServicePole
            {
                PoleNbr = element.GetChildText(empty + "PoleNbr"),
            };

            return model;
        }

        public Type650ServiceReject ParseServiceReject(XElement element, IDictionary<string, XNamespace> namespaces)
        {
            XNamespace empty;
            if (!namespaces.TryGetValue(string.Empty, out empty))
                empty = XNamespace.None;

            var model = new Type650ServiceReject
            {
                RejectCode = element.GetChildText(empty + "RejectCode"),
                RejectReason = element.GetChildText(empty + "RejectReason"),
                UnexCode = element.GetChildText(empty + "UnexCode"),
                UnexReason = element.GetChildText(empty + "UnexReason"),
            };

            return model;
        }
    }
}
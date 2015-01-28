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
    public class Import650Prism : IMarketFileParser
    {
        private readonly IClientDataAccess clientDataAccess;
        private readonly ILogger logger;

        public Import650Prism(IClientDataAccess clientDataAccess, ILogger logger)
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
                return Import650Model.Empty;
            }

            using (var stream = marketFile.OpenRead())
            {
                return Parse(stream);
            }
        }

        public IMarketFileParseResult Parse(Stream stream)
        {
            var context = new Prism650Context();
            using (var reader = new StreamReader(stream))
            {
                string marketFileLine;
                while ((marketFileLine = reader.ReadLine()) != null)
                    ParseLine(context, marketFileLine);
            }

            if (context.ShouldResolve)
            {
                logger.Warn("Unresolved data identified after parsing 650. Transactions may not be completed.");
                context.ResolveToHeader();
            }

            return context.Results;
        }

        public void ParseLine(Prism650Context context, string line)
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
                case "10":
                    ParseService(context, marketFields);
                    break;
                case "11":
                    ParseServicePole(context, marketFields);
                    break;
                case "12":
                    ParseServiceChange(context, marketFields);
                    break;
                case "13":
                    ParseServiceReject(context, marketFields);
                    break;
                case "14":
                    ParseServiceMeter(context, marketFields);
                    break;
                case "TL":
                    context.ResolveToHeader();
                    marketFields.TryAtIndexInt(1, x => context.TransactionAuditCount = x);
                    break;
            }
        }

        public void ParseHeader(Prism650Context context, string[] marketFields)
        {
            var headerModel = new Type650Header
            {
                TransactionSetPurposeCode = marketFields.AtIndex(3),
                TransactionDate = marketFields.AtIndex(4),
                TransactionNbr = marketFields.AtIndex(5),
                ReferenceNbr = marketFields.AtIndex(6),
                TransactionType = marketFields.AtIndex(7),
                ActionCode = marketFields.AtIndex(8),
                TdspName = marketFields.AtIndex(20),
                TdspDuns = marketFields.AtIndex(21),
                CrName = marketFields.AtIndex(22),
                CrDuns = marketFields.AtIndex(23),
                ProcessedReceivedDateTime = marketFields.AtIndex(24),
            };

            var identifiedMarket = clientDataAccess.IdentifyMarket(headerModel.TdspDuns);
            if (identifiedMarket.HasValue)
                context.SetMarket(identifiedMarket.Value);

            headerModel.MarketId = context.MarketId;
            headerModel.ProviderId = 1;

            context.PushModel(headerModel);

            var hasEntityName = false;
            var hasContactName = false;
            marketFields.TryAtIndex(9, x => hasEntityName = true);
            marketFields.TryAtIndex(17, x => hasContactName = true);

            if (!hasEntityName || !hasContactName)
                return;

            var nameModel = new Type650Name
            {
                EntityName = marketFields.AtIndex(9),
                EntityName2 = marketFields.AtIndex(10),
                EntityName3 = marketFields.AtIndex(11),
                Address1 = marketFields.AtIndex(12),
                Address2 = marketFields.AtIndex(13),
                City = marketFields.AtIndex(14),
                State = marketFields.AtIndex(15),
                PostalCode = marketFields.AtIndex(16),
                ContactName = marketFields.AtIndex(17),
                ContactPhoneNbr1 = marketFields.AtIndex(18),
                ContactPhoneNbr2 = marketFields.AtIndex(19),
            };

            headerModel.AddName(nameModel);
        }

        public void ParseService(Prism650Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type650Types.Header)
                throw new InvalidOperationException();

            var header = current as Type650Header;
            if (header == null)
                throw new InvalidOperationException();

            var model = new Type650Service
            {
                PurposeCode = marketFields.AtIndex(2),
                PriorityCode = marketFields.AtIndex(3),
                EsiId = marketFields.AtIndex(4),
                SpecialProcessCode = marketFields.AtIndex(5),
                ServiceReqDate = marketFields.AtIndex(6),
                NotBeforeDate = marketFields.AtIndex(7),
                CallAhead = marketFields.AtIndex(8),
                PremLocation = marketFields.AtIndex(9),
                AccStatusCode = marketFields.AtIndex(10),
                AccStatusDesc = marketFields.AtIndex(11),
                EquipLocation = marketFields.AtIndex(12),
                ServiceOrderNbr = marketFields.AtIndex(13),
                CompletionDate = marketFields.AtIndex(14),
                CompletionTime = marketFields.AtIndex(15),
                ReportRemarks = marketFields.AtIndex(16),
                Directions = marketFields.AtIndex(17),
                MeterNbr = marketFields.AtIndex(18),
                MeterReadDate = marketFields.AtIndex(19),
                MeterTestDate = marketFields.AtIndex(20),
                MeterTestResults = marketFields.AtIndex(21),
                IncidentCode = marketFields.AtIndex(22),
                EstRestoreDate = marketFields.AtIndex(23),
                EstRestoreTime = marketFields.AtIndex(24),
                IntStartDate = marketFields.AtIndex(25),
                IntStartTime = marketFields.AtIndex(26),
                RepairRecommended = marketFields.AtIndex(27),
                Rescheduled = marketFields.AtIndex(28),
                InterDurationPeriod = marketFields.AtIndex(29),
                AreaOutage = marketFields.AtIndex(30),
                CustRepairRemarks = marketFields.AtIndex(31),
                MeterReadUom = marketFields.AtIndex(32),
                MeterRead = marketFields.AtIndex(33),
                MeterReadCode = marketFields.AtIndex(34),
                Membership = marketFields.AtIndex(35),
                RemarksPermanentSuspend = marketFields.AtIndex(36),
                DisconnectAuthorization = marketFields.AtIndex(37),
                PremiseTypeVerification = marketFields.AtIndex(38),
                PremiseTypeDesc = marketFields.AtIndex(39),
                SwitchHoldIndicator = marketFields.AtIndex(40),
                SwitchHoldDesc = marketFields.AtIndex(41),
            };

            header.AddService(model);
            context.PushModel(model);
        }

        public void ParseServicePole(Prism650Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type650Types.Service)
                throw new InvalidOperationException();

            var service = current as Type650Service;
            if (service == null)
                throw new InvalidOperationException();

            var model = new Type650ServicePole
            {
                PoleNbr = marketFields.AtIndex(2),
            };

            service.AddPole(model);
        }

        public void ParseServiceChange(Prism650Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type650Types.Service)
                throw new InvalidOperationException();

            var service = current as Type650Service;
            if (service == null)
                throw new InvalidOperationException();

            var model = new Type650ServiceChange
            {
                ChangeReason = marketFields.AtIndex(2),
            };

            service.AddChange(model);
        }

        public void ParseServiceReject(Prism650Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type650Types.Service)
                throw new InvalidOperationException();

            var service = current as Type650Service;
            if (service == null)
                throw new InvalidOperationException();

            var model = new Type650ServiceReject
            {
                RejectCode = marketFields.AtIndex(2),
                RejectReason = marketFields.AtIndex(3),
                UnexCode = marketFields.AtIndex(4),
                UnexReason = marketFields.AtIndex(5),
            };

            service.AddReject(model);
        }

        public void ParseServiceMeter(Prism650Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type650Types.Service)
                throw new InvalidOperationException();

            var service = current as Type650Service;
            if (service == null)
                throw new InvalidOperationException();

            var model = new Type650ServiceMeter
            {
                MeterNumber = marketFields.AtIndex(2),
            };

            service.AddMeter(model);
        }
    }
}

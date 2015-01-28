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
    public class Import810Prism : IMarketFileParser
    {
        private readonly IClientDataAccess clientDataAccess;
        private readonly ILogger logger;

        public Import810Prism(IClientDataAccess clientDataAccess, ILogger logger)
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
                return Import810Model.Empty;
            }

            using (var stream = marketFile.OpenRead())
            {
                return Parse(stream);
            }
        }

        public IMarketFileParseResult Parse(Stream stream)
        {
            var context = new Prism810Context();
            using (var reader = new StreamReader(stream))
            {
                string marketFileLine;
                while ((marketFileLine = reader.ReadLine()) != null)
                    ParseLine(context, marketFileLine);
            }

            if (context.ShouldResolve)
            {
                logger.Warn("Unresolved data identified after parsing 810. Transactions may not be completed.");
                context.ResolveToHeader();
            }

            return context.Results;
        }

        public void ParseLine(Prism810Context context, string line)
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
                    context.RevertToHeader();
                    context.FirstCharge = true;
                    ParseDetail(context, marketFields);
                    break;
                case "40":
                    if (context.FirstCharge)
                    {
                        ParseDetailItem(context, marketFields);
                        context.FirstCharge = false;
                    }
                    ParseCharge(context, marketFields);
                    break;
                case "45":
                    ParseTax(context, marketFields);
                    break;
                case "60":
                    ParseSummary(context, marketFields);
                    break;
                case "TL":
                    context.ResolveToHeader();
                    marketFields.TryAtIndexInt(1, x => context.TransactionAuditCount = x);
                    break;
            }
        }

        public void ParseHeader(Prism810Context context, string[] marketFields)
        {
            var model = new Type810Header
            {
                TransactionSetId = "810",
                TransactionSetPurposeCode = marketFields.AtIndex(7),
                InvoiceNbr = marketFields.AtIndex(4),
                TransactionDate = marketFields.AtIndex(3),
                ReleaseNbr = marketFields.AtIndex(5),
                TransactionTypeCode = marketFields.AtIndex(6),
                OriginalInvoiceNbr = marketFields.AtIndex(12),
                EsiId = marketFields.AtIndex(53),
                PaymentDueDate = marketFields.AtIndex(25),
                TdspDuns = marketFields.AtIndex(20),
                TdspName = marketFields.AtIndex(19),
                CrDuns = marketFields.AtIndex(22),
                CrName = marketFields.AtIndex(21),
                Direction = true,
            };

            var identifiedMarket = clientDataAccess.IdentifyMarket(model.TdspDuns);
            if (identifiedMarket.HasValue)
                context.SetMarket(identifiedMarket.Value);

            model.MarketId = context.MarketId;
            model.ProviderId = 1;

            context.PushModel(model);
        }

        public void ParseDetail(Prism810Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type810Types.Header)
                throw new InvalidOperationException();

            var header = current as Type810Header;
            if (header == null)
                throw new InvalidOperationException();

            var model = new Type810Detail
            {
                AssignedId = marketFields.AtIndex(2),
                ServiceTypeCode = "SV",
                ServiceType = marketFields.AtIndex(3),
                ServiceClassCode = "C3",
                ServiceClass = marketFields.AtIndex(4),
                RateClass = marketFields.AtIndex(11),
                RateSubClass = marketFields.AtIndex(13),
                ServicePeriodStartDate = marketFields.AtIndex(6),
                ServicePeriodEndDate = marketFields.AtIndex(7),
            };

            header.AddDetail(model);
            context.PushModel(model);
        }

        public void ParseDetailItem(Prism810Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type810Types.Detail)
                throw new InvalidOperationException();

            var detail = current as Type810Detail;
            if (detail == null)
                throw new InvalidOperationException();

            var model = new Type810DetailItem
            {
                AssignedId = marketFields.AtIndex(2),
                ServiceOrderCompleteDate = marketFields.AtIndex(13),
                UnmeteredServiceDateRange = marketFields.AtIndex(16),
                InvoiceNbr = marketFields.AtIndex(15),
                ServiceOrderNbr = marketFields.AtIndex(14),
                Consumption = marketFields.AtIndex(17),
                EffectiveDate = marketFields.AtIndex(18),
            };

            detail.AddItem(model);
            context.PushModel(model);
        }

        public void ParseCharge(Prism810Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type810Types.DetailItem)
                throw new InvalidOperationException();

            var detailItem = current as Type810DetailItem;
            if (detailItem == null)
                throw new InvalidOperationException();

            var amount = (marketFields.AtIndexDecimal(6) * 100);
            var model = new Type810DetailItemCharge
            {
                ChargeIndicator = marketFields.AtIndex(3),
                AgencyCode = "EU",
                ChargeCode = marketFields.AtIndex(5),
                Amount = amount.ToString("0"),
                Rate = marketFields.AtIndex(7),
                UOM = marketFields.AtIndex(8),
                Quantity = marketFields.AtIndex(9),
                Description = marketFields.AtIndex(11),
            };

            detailItem.AddCharge(model);
        }

        public void ParseTax(Prism810Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type810Types.DetailItem)
                throw new InvalidOperationException();

            var detailItem = current as Type810DetailItem;
            if (detailItem == null)
                throw new InvalidOperationException();

            var model = new Type810DetailItemTax
            {
                TaxTypeCode = marketFields.AtIndex(2),
                TaxAmount = marketFields.AtIndex(3),
                RelationshipCode = marketFields.AtIndex(6),
            };

            detailItem.AddTax(model);
        }

        public void ParseSummary(Prism810Context context, string[] marketFields)
        {
            context.RevertToHeader();
            var current = context.Current;
            if (current == null || current.ModelType != Type810Types.Header)
                throw new InvalidOperationException();

            var header = current as Type810Header;
            if (header == null)
                throw new InvalidOperationException();

            var model = new Type810Summary
            {
                TotalAmount = marketFields.AtIndex(2),
                TotalLineItems = marketFields.AtIndex(3),
            };

            header.AddSummary(model);
        }
    }
}

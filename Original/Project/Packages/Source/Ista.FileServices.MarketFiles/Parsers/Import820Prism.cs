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
    public class Import820Prism : IMarketFileParser
    {
        private readonly IClientDataAccess clientDataAccess;
        private readonly ILogger logger;

        public Import820Prism(IClientDataAccess clientDataAccess, ILogger logger)
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
                return Import820Model.Empty;
            }

            using (var stream = marketFile.OpenRead())
            {
                return Parse(stream);
            }
        }

        public IMarketFileParseResult Parse(Stream stream)
        {
            var context = new Prism820Context();
            using (var reader = new StreamReader(stream))
            {
                string marketFileLine;
                while ((marketFileLine = reader.ReadLine()) != null)
                    ParseLine(context, marketFileLine);
            }

            if (context.ShouldResolve)
            {
                logger.Warn("Unresolved data identified after parsing 820. Transactions may not be completed.");
                context.ResolveToHeader();
            }

            return context.Results;
        }

        public void ParseLine(Prism820Context context, string line)
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
                    break;
                case "01":
                    ParseHeader(context, marketFields);
                    break;
                case "20":
                    ParseDetail(context, marketFields);
                    break;
                case "TL":
                    context.ResolveToHeader();
                    marketFields.TryAtIndexInt(1, x => context.TransactionAuditCount = x);
                    break;
            }
        }

        public void ParseHeader(Prism820Context context, string[] marketFields)
        {
            var model = new Type820Header
            {
                TotalAmount = marketFields.AtIndex(4),
                PaymentMethodCode = marketFields.AtIndex(5),
                TransactionDate = marketFields.AtIndex(13),
                TraceReferenceNbr = marketFields.AtIndex(14),
                TdspName = marketFields.AtIndex(15),
                TdspDuns = marketFields.AtIndex(16),
                CrName = marketFields.AtIndex(17),
                CrDuns = marketFields.AtIndex(18),
                CreditDebitFlag = marketFields.AtIndex(19),
            };

            var identifiedMarket = clientDataAccess.IdentifyMarket(model.TdspDuns);
            if (identifiedMarket.HasValue)
                context.SetMarket(identifiedMarket.Value);

            model.MarketId = context.MarketId;
            model.ProviderId = 1;
            
            context.PushModel(model);
        }

        public void ParseDetail(Prism820Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type820Types.Header)
                throw new InvalidOperationException();

            var header = current as Type820Header;
            if (header == null)
                throw new InvalidOperationException();

            var model = new Type820Detail
            {
                ReferenceNbr = marketFields.AtIndex(2),
                PaymentActionCode = marketFields.AtIndex(3),
                PaymentAmount = marketFields.AtIndex(4),
                AdjustmentReasonCode = marketFields.AtIndex(5),
                AdjustmentAmount = marketFields.AtIndex(6),
            };

            if (context.Market == MarketOptions.Maryland)
            {
                marketFields.TryAtIndex(11, x => model.CrossReferenceNbr = x);
                marketFields.TryAtIndex(2, x => model.EsiId = x);
            }
            else
            {
                marketFields.TryAtIndex(9, x => model.CrossReferenceNbr = x);
                marketFields.TryAtIndex(16, x => model.EsiId = x);
            }

            header.AddDetail(model);
        }
    }
}

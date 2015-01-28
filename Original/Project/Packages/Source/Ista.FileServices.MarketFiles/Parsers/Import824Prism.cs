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
    public class Import824Prism : IMarketFileParser
    {
        private readonly IClientDataAccess clientDataAccess;
        private readonly ILogger logger;

        public Import824Prism(IClientDataAccess clientDataAccess, ILogger logger)
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
                return Import824Model.Empty;
            }

            using (var stream = marketFile.OpenRead())
            {
                return Parse(stream);
            }
        }

        public IMarketFileParseResult Parse(Stream stream)
        {
            var context = new Prism824Context();
            using (var reader = new StreamReader(stream))
            {
                string marketFileLine;
                while ((marketFileLine = reader.ReadLine()) != null)
                    ParseLine(context, marketFileLine);
            }

            if (context.ShouldResolve)
            {
                logger.Warn("Unresolved data identified after parsing 824. Transactions may not be completed.");
                context.ResolveToHeader();
            }

            return context.Results;
        }

        public void ParseLine(Prism824Context context, string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return;

            if (line.Length < 2)
                return;

            var indicator = line.Substring(0, 2);
            var marketFields = line.Split('|');

            switch(indicator)
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
                    ParseTransaction(context, marketFields);
                    break;
                case "20":
                    ParseReason(context, marketFields);
                    break;
                case "TL":
                    context.ResolveToHeader();
                    marketFields.TryAtIndexInt(1, x => context.TransactionAuditCount = x);
                    break;
            }
        }

        public void ParseHeader(Prism824Context context, string[] marketFields)
        {
            context.TransactionTypeCode = marketFields.AtIndex(5);
            context.TransactionDate = marketFields.AtIndex(3);
            context.TransactionNumber = marketFields.AtIndex(4);
            context.TdspName = marketFields.AtIndex(6);
            context.TdspDuns = marketFields.AtIndex(7);
            context.CrName = marketFields.AtIndex(12);
            context.CrDuns = marketFields.AtIndex(13);
            context.PremiseNumber = marketFields.AtIndex(20);
        }

        public void ParseTransaction(Prism824Context context, string[] marketFields)
        {
            var model = new Type824Header
            {
                TransactionDate = context.TransactionDate,
                TransactionNbr = context.TransactionNumber,
                ActionCode = context.TransactionTypeCode,
                CrDuns = context.CrDuns,
                CrName = context.CrName,
                TdspDuns = context.TdspDuns,
                TdspName = context.TdspName,
                AppAckCode = marketFields.AtIndex(2),
                TransactionSetNbr = marketFields.AtIndex(4),
                ReferenceNbr = marketFields.AtIndex(3),
            };

            var identifiedMarket = clientDataAccess.IdentifyMarket(model.TdspDuns);
            if (identifiedMarket.HasValue)
                context.SetMarket(identifiedMarket.Value);

            model.MarketId = context.MarketId;
            model.ProviderId = 1;

            model.EsiId = context.PremiseNumber;
            if (context.Market == MarketOptions.Texas)
                model.EsiId = marketFields.AtIndex(9);

            context.PushModel(model);
        }

        public void ParseReason(Prism824Context context, string[] marketFields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != Type824Types.Header)
                throw new InvalidOperationException();

            var header = current as Type824Header;
            if (header == null)
                throw new InvalidOperationException();

            var model = new Type824Reason
            {
                ReasonCode = marketFields.AtIndex(2),
                ReasonText = marketFields.AtIndex(3),
            };

            header.AddReason(model);
        }
    }
}

using System;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Contexts;
using Ista.FileServices.MarketFiles.DataAccess;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.Parsers;

namespace Ista.FileServices.MarketFiles.Factories
{
    public class ImportParserFactory
    {
        public static IMarketFileParser GetParser(ImportFileContext context, ILogger logger)
        {
            return (context.ProviderId == 1)
                       ? GetPrismParser(context, logger)
                       : GetXmlParser(context, logger);
        }

        public static IMarketFileParser GetPrismParser(ImportFileContext context, ILogger logger)
        {
            var clientDataAccess = new ClientDataAccess(context.ClientConnectionString);

            var fileType = context.FileType;

            switch (fileType)
            {
                case "650":
                    return new Import650Prism(clientDataAccess, logger);
                case "810":
                    return new Import810Prism(clientDataAccess, logger);
                case "814":
                    return new Import814Prism(clientDataAccess, logger);
                case "820":
                    return new Import820Prism(clientDataAccess, logger);
                case "824":
                    return new Import824Prism(clientDataAccess, logger);
                case "867":
                    return new Import867Prism(clientDataAccess, logger);
                case "CBF":
                    return new ImportCustomerInfo(clientDataAccess, logger);
            }

            throw new ArgumentOutOfRangeException(fileType);
        }

        public static IMarketFileParser GetXmlParser(ImportFileContext context, ILogger logger)
        {
            var fileType = context.FileType;
            switch (fileType)
            {
                case "248":
                    return new Import248Xml(logger);
                case "650":
                    return new Import650Xml(logger);
                case "810":
                    return new Import810Xml(logger);
                case "814":
                    var marketDataAccess = new MarketDataAccess(context.MarketConnectionString);
                    return new Import814Xml(marketDataAccess, logger);
                case "820":
                    return new Import820Xml(logger);
                case "824":
                    return new Import824Xml(logger);
                case "867":
                    return new Import867Xml(logger);
                case "997":
                    return new Import997Xml(logger);
            }

            throw new ArgumentOutOfRangeException(fileType);
        }
    }
}

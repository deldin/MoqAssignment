using System;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Contexts;
using Ista.FileServices.MarketFiles.DataAccess;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.Parsers;

namespace Ista.FileServices.MarketFiles.Factories
{
    public class ExportParserFactory
    {
        public static IMarketFileExporter GetParser(ExportFileContext context, ILogger logger)
        {
            switch (context.ProviderId)
            {
                case 1:
                    return GetPrismExporter(context, logger);

                case 2:
                    return GetXmlExporter(context, logger);

                case 3:
                    return GetSsxExporter(context, logger);

        
            }

            return null;
        }

        private static IMarketFileExporter GetSsxExporter(ExportFileContext context, ILogger logger)
        {
            var clientDataAccess = new ClientDataAccess(context.ClientConnectionString);
            var marketDataAccess = new MarketDataAccess(context.MarketConnectionString);


            var fileType = context.FileType;

            switch (fileType)
            {
                  case "814":
                    {
                        var dataAccess = new Export814SsxDataAccess(context.MarketConnectionString);
                        return new Export814Ssx(clientDataAccess, marketDataAccess, dataAccess, logger);
                    }

            }

            throw new ArgumentOutOfRangeException(fileType);
        }

        public static IMarketFileExporter GetPrismExporter(ExportFileContext context, ILogger logger)
        {
            var clientDataAccess = new ClientDataAccess(context.ClientConnectionString);

            var fileType = context.FileType;
            switch (fileType)
            {
                case "650":
                    {
                        var dataAccess = new Export650PrismDataAccess(context.MarketConnectionString);
                        return new Export650Prism(clientDataAccess, dataAccess, logger);
                    }
                case "810":
                    {
                        var dataAccess = new Export810PrismDataAccess(context.MarketConnectionString);
                        return new Export810Prism(clientDataAccess, dataAccess, logger);
                    }
                case "814":
                    {
                        var dataAccess = new Export814PrismDataAccess(context.MarketConnectionString);
                        return new Export814Prism(clientDataAccess, dataAccess, logger);
                    }
                case "820":
                    {
                        var dataAccess = new Export820PrismDataAccess(context.MarketConnectionString);
                        return new Export820Prism(clientDataAccess, dataAccess, logger);
                    }
                case "824":
                    {
                        var dataAccess = new Export824PrismDataAccess(context.MarketConnectionString);
                        return new Export824Prism(clientDataAccess, dataAccess, logger);
                    }
                case "CBF":
                    {
                        var dataAccess = new ExportCustomerInfoDataAccess(context.ClientConnectionString);
                        return new ExportCustomerInfoPrism(dataAccess, logger);
                    }
            }

            throw new ArgumentOutOfRangeException(fileType);
        }

        public static IMarketFileExporter GetXmlExporter(ExportFileContext context, ILogger logger)
        {
            var clientDataAccess = new ClientDataAccess(context.ClientConnectionString);
            var marketDataAccess = new MarketDataAccess(context.MarketConnectionString);

            var fileType = context.FileType;
            switch (fileType)
            {
                case "650":
                    {
                        var dataAccess = new Export650XmlDataAccess(context.MarketConnectionString);
                        return new Export650Xml(clientDataAccess, marketDataAccess, dataAccess, logger);
                    }
                case "810":
                    {
                        var dataAccess = new Export810XmlDataAccess(context.MarketConnectionString);
                        return new Export810Xml(clientDataAccess, marketDataAccess, dataAccess, logger);
                    }
                case "814":
                    {
                        var dataAccess = new Export814XmlDataAccess(context.MarketConnectionString);
                        return new Export814Xml(clientDataAccess, marketDataAccess, dataAccess, logger);
                    }
                case "820":
                    {
                        var dataAccess = new Export820XmlDataAccess(context.MarketConnectionString);
                        return new Export820Xml(clientDataAccess, marketDataAccess, dataAccess, logger);
                    }
                case "824":
                    {
                        var dataAccess = new Export824XmlDataAccess(context.MarketConnectionString);
                        return new Export824Xml(clientDataAccess, marketDataAccess, dataAccess, logger);
                    }
            }

            throw new ArgumentOutOfRangeException(fileType);
        }
    }
}

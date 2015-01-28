using System;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Contexts;
using Ista.FileServices.MarketFiles.DataAccess;
using Ista.FileServices.MarketFiles.Handlers;
using Ista.FileServices.MarketFiles.Interfaces;

namespace Ista.FileServices.MarketFiles.Factories
{
    public class ExportHandlerFactory
    {
        public static IExportTransactionHandler GetHandler(ExportFileContext context, ILogger logger)
        {
            switch (context.ProviderId)
            {
                case 1:
                    return GetPrismHandler(context, logger);

                case 2:
                    return GetXmlHandler(context, logger);

                case 3:
                    return GetSsxHandler(context, logger);


            }

            return null;

        }

        private static IExportTransactionHandler GetSsxHandler(ExportFileContext context, ILogger logger)
        {
            var fileType = context.FileType;
            switch (fileType)
            {
                case "814":
                    {
                        var dataAccess = new Export814SsxDataAccess(context.MarketConnectionString);
                        return new Export814Handler(dataAccess, logger);
                    }
                
            }

            throw new ArgumentOutOfRangeException(fileType);
            

        }

        public static IExportTransactionHandler GetPrismHandler(ExportFileContext context, ILogger logger)
        {
            var fileType = context.FileType;
            switch (fileType)
            {
                case "650":
                    {
                        var dataAccess = new Export650PrismDataAccess(context.MarketConnectionString);
                        return new Export650Handler(dataAccess, logger);
                    }
                case "810":
                    {
                        var dataAccess = new Export810PrismDataAccess(context.MarketConnectionString);
                        return new Export810Handler(dataAccess, logger);
                    }
                case "814":
                    {
                        var dataAccess = new Export814PrismDataAccess(context.MarketConnectionString);
                        return new Export814Handler(dataAccess, logger);
                    }
                case "820":
                    {
                        var dataAccess = new Export820PrismDataAccess(context.MarketConnectionString);
                        return new Export820Handler(dataAccess, logger);
                    }
                case "824":
                    {
                        var dataAccess = new Export824PrismDataAccess(context.MarketConnectionString);
                        return new Export824Handler(dataAccess, logger);
                    }
                case "CBF":
                    {
                        var dataAccess = new ExportCustomerInfoDataAccess(context.ClientConnectionString);
                        return new ExportCustomerInfoHandler(dataAccess, logger);
                    }
            }

            throw new ArgumentOutOfRangeException(fileType);
        }

        public static IExportTransactionHandler GetXmlHandler(ExportFileContext context, ILogger logger)
        {
            var fileType = context.FileType;
            switch (fileType)
            {
                case "650":
                    {
                        var dataAccess = new Export650XmlDataAccess(context.MarketConnectionString);
                        return new Export650Handler(dataAccess, logger);
                    }
                case "810":
                    {
                        var dataAccess = new Export810XmlDataAccess(context.MarketConnectionString);
                        return new Export810Handler(dataAccess, logger);
                    }
                case "814":
                    {
                        var dataAccess = new Export814XmlDataAccess(context.MarketConnectionString);
                        return new Export814Handler(dataAccess, logger);
                    }
                case "820":
                    {
                        var dataAccess = new Export820XmlDataAccess(context.MarketConnectionString);
                        return new Export820Handler(dataAccess, logger);
                    }
                case "824":
                    {
                        var dataAccess = new Export824XmlDataAccess(context.MarketConnectionString);
                        return new Export824Handler(dataAccess, logger);
                    }
            }

            throw new ArgumentOutOfRangeException(fileType);
        }
    }
}

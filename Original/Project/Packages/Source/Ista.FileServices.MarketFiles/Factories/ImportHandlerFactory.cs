using System;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Contexts;
using Ista.FileServices.MarketFiles.DataAccess;
using Ista.FileServices.MarketFiles.Handlers;
using Ista.FileServices.MarketFiles.Interfaces;

namespace Ista.FileServices.MarketFiles.Factories
{
    public class ImportHandlerFactory
    {
        public static IImportTransactionHandler GetHandler(ImportFileContext context, ILogger logger)
        {
            return (context.ProviderId == 1)
                       ? GetPrismHandler(context, logger)
                       : GetXmlHandler(context, logger);
        }

        public static IImportTransactionHandler GetPrismHandler(ImportFileContext context, ILogger logger)
        {
            var fileType = context.FileType;

            var connectionString = context.MarketConnectionString;
            switch (fileType)
            {
                case "650":
                    {
                        var dataAccess = new Import650PrismDataAccess(connectionString);
                        return new Import650Handler(dataAccess, logger);
                    }
                case "810":
                    {
                        var dataAccess = new Import810PrismDataAccess(connectionString);
                        return new Import810Handler(dataAccess, logger);
                    }
                case "814":
                    {
                        var dataAccess = new Import814PrismDataAccess(connectionString);
                        return new Import814Handler(dataAccess, logger);
                    }
                case "820":
                    {
                        var dataAccess = new Import820PrismDataAccess(connectionString);
                        return new Import820Handler(dataAccess, logger);
                    }
                case "824":
                    {
                        var dataAccess = new Import824PrismDataAccess(connectionString);
                        return new Import824Handler(dataAccess, logger);
                    }
                case "867":
                    {
                        var dataAccess = new Import867PrismDataAccess(connectionString);
                        return new Import867Handler(dataAccess, logger);
                    }
                case "CBF":
                    {
                        var dataAccess = new ImportCustomerInfoDataAccess(connectionString);
                        return new ImportClientInfoHandler(dataAccess, logger);
                    }
            }

            throw new ArgumentOutOfRangeException(fileType);
        }

        public static IImportTransactionHandler GetXmlHandler(ImportFileContext context, ILogger logger)
        {
            var fileType = context.FileType;

            var connectionString = context.MarketConnectionString;
            switch (fileType)
            {
                case "248":
                    {
                        var dataAccess = new Import248XmlDataAccess(connectionString);
                        return new Import248Handler(dataAccess, logger);
                    }
                case "650":
                    {
                        var dataAccess = new Import650XmlDataAccess(connectionString);
                        return new Import650Handler(dataAccess, logger);
                    }
                case "810":
                    {
                        var dataAccess = new Import810XmlDataAccess(connectionString);
                        return new Import810Handler(dataAccess, logger);
                    }
                case "814":
                    {
                        var dataAccess = new Import814XmlDataAccess(connectionString);
                        return new Import814Handler(dataAccess, logger);
                    }
                case "820":
                    {
                        var dataAccess = new Import820XmlDataAccess(connectionString);
                        return new Import820Handler(dataAccess, logger);
                    }
                case "824":
                    {
                        var dataAccess = new Import824XmlDataAccess(connectionString);
                        return new Import824Handler(dataAccess, logger);
                    }
                case "867":
                    {
                        var dataAccess = new Import867XmlDataAccess(connectionString);
                        return new Import867Handler(dataAccess, logger);
                    }
                case "997":
                    {
                        var dataAccess = new Import997XmlDataAccess(connectionString);
                        return new Import997Handler(dataAccess, logger);
                    }
            }

            throw new ArgumentOutOfRangeException(fileType);
        }
    }
}

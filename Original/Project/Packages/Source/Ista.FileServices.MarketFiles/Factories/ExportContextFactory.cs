using System.Collections.Generic;
using System.Linq;
using Ista.FileServices.MarketFiles.Contexts;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Factories
{
    public class ExportContextFactory
    {
        public static ExportFileContext[] CreateContextList(ProviderModel[] provider, ExportConfigurationModel configuration)
        {
            var collection = new List<ExportFileContext>();
            if (provider.Any(x => x.ProviderId == 1))
                collection.AddRange(CreatePrismContext(configuration));
            if (provider.Any(x => x.ProviderId == 2))
                collection.AddRange(CreateXmlContext(configuration));
            if (provider.Any(x => x.ProviderId == 3))
                collection.AddRange(CreateSsxContext(configuration));

            return collection.ToArray();
        }

        public static ExportFileContext[] CreatePrismContext(ExportConfigurationModel configuration)
        {
            return new[]
            {
                ExportFileContext.CreatePrism("650", configuration),
                ExportFileContext.CreatePrism("810", configuration),
                ExportFileContext.CreatePrism("814", configuration),
                ExportFileContext.CreatePrism("820", configuration),
                ExportFileContext.CreatePrism("824", configuration),
                ExportFileContext.CreatePrism("CBF", configuration)
            };
        }

        public static ExportFileContext[] CreateXmlContext(ExportConfigurationModel configuration)
        {
            return new[]
            {
                ExportFileContext.CreateXml("810", configuration),
                ExportFileContext.CreateXml("814", configuration),
                ExportFileContext.CreateXml("820", configuration),
                ExportFileContext.CreateXml("824", configuration),
                ExportFileContext.CreateXml("650", configuration)
            };
        }

        public static ExportFileContext[] CreateSsxContext(ExportConfigurationModel configuration)
        {
            return new[]
            {
                ExportFileContext.CreateXml("814", configuration)
            };
        }
    }
}

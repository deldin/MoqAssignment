using System.Collections.Generic;
using System.Linq;
using Ista.FileServices.MarketFiles.Contexts;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Factories
{
    public class ImportContextFactory
    {
        public static ImportFileContext[] CreateContextList(ProviderModel[] provider, ImportConfigurationModel configuration)
        {
            var collection = new List<ImportFileContext>();
            if (provider.Any(x => x.ProviderId == 1))
                collection.AddRange(CreatePrismContext(configuration));
            if (provider.Any(x => x.ProviderId == 2))
                collection.AddRange(CreateXmlContext(configuration));

            return collection.ToArray();
        }

        public static ImportFileContext[] CreatePrismContext(ImportConfigurationModel configuration)
        {
            return new[]
            {
                ImportFileContext.CreatePrism("*_810.*", "810", configuration),
                ImportFileContext.CreatePrism("*_814.*", "814", configuration),
                ImportFileContext.CreatePrism("*_867.*", "867", configuration),
                ImportFileContext.CreatePrism("*_LSE.*", "867", configuration),
                ImportFileContext.CreatePrism("*_650.*", "650", configuration),
                ImportFileContext.CreatePrism("*_820.*", "820", configuration),
                ImportFileContext.CreatePrism("*_824.*", "824", configuration),
                ImportFileContext.CreatePrism("*_MTCRCustomerInformation.*", "CBF", configuration),
                ImportFileContext.CreatePrism("*_MTERCOT2CR.*", "CBF", configuration)
            };
        }

        public static ImportFileContext[] CreateXmlContext(ImportConfigurationModel configuration)
        {
            return new[]
            {
                ImportFileContext.CreateXml("810*.xml", "810", configuration),
                ImportFileContext.CreateXml("814*.xml", "814", configuration),
                ImportFileContext.CreateXml("867*.xml", "867", configuration),
                ImportFileContext.CreateXml("LSE*.xml", "867", configuration),
                ImportFileContext.CreateXml("650*.xml", "650", configuration),
                ImportFileContext.CreateXml("820*.xml", "820", configuration),
                ImportFileContext.CreateXml("824*.xml", "824", configuration),
                ImportFileContext.CreateXml("997*.xml", "997", configuration),
                ImportFileContext.CreateXml("248*.xml", "248", configuration)
            };
        }
    }
}

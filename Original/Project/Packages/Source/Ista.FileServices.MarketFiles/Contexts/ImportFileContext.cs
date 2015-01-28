using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Contexts
{
    public class ImportFileContext
    {
        public string FileType { get; set; }
        public string FilePattern { get; set; }
        public string DirectoryIn { get; set; }
        public string DirectoryInArchive { get; set; }
        public string DirectoryInException { get; set; }
        public string ClientConnectionString { get; set; }
        public string MarketConnectionString { get; set; }
        public int ProviderId { get; set; }

        public static ImportFileContext CreatePrism(string filePattern, string fileType, ImportConfigurationModel model)
        {
            var item = CreateFromConfiguration(model);
            item.ProviderId = 1;
            item.FileType = fileType;
            item.FilePattern = filePattern;

            return item;
        }

        public static ImportFileContext CreateXml(string filePattern, string fileType, ImportConfigurationModel model)
        {
            var item = CreateFromConfiguration(model);
            item.ProviderId = 2;
            item.FileType = fileType;
            item.FilePattern = filePattern;

            return item;
        }

        private static ImportFileContext CreateFromConfiguration(ImportConfigurationModel model)
        {
            return new ImportFileContext
            {
                ClientConnectionString = model.ClientConnectionString,
                MarketConnectionString = model.MarketConnectionString,
                DirectoryIn = model.DirectoryDecrypted,
                DirectoryInArchive = model.DirectoryArchive,
                DirectoryInException = model.DirectoryException,
            };
        }
    }
}

using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Contexts
{
    public class ExportFileContext
    {
        public string DirectoryOut { get; set; }
        public string Extension { get; set; }
        public string FileType { get; set; }
        public string ClientConnectionString { get; set; }
        public string MarketConnectionString { get; set; }
        public int ProviderId { get; set; }

        public static ExportFileContext CreatePrism(string fileType, ExportConfigurationModel model)
        {
            var item = CreateFromConfiguration(model);
            item.ProviderId = 1;
            item.Extension = "txt";
            item.FileType = fileType;

            return item;
        }

        public static ExportFileContext CreateXml(string fileType, ExportConfigurationModel model)
        {
            var item = CreateFromConfiguration(model);
            item.ProviderId = 2;
            item.Extension = "xml";
            item.FileType = fileType;

            return item;
        }

        public static ExportFileContext CreateSsx(string fileType, ExportConfigurationModel model)
        {
            var item = CreateFromConfiguration(model);
            item.ProviderId = 3;
            item.Extension = "csv";
            item.FileType = fileType;

            return item;
        }

        private static ExportFileContext CreateFromConfiguration(ExportConfigurationModel model)
        {
            return new ExportFileContext
            {
                ClientConnectionString = model.ClientConnectionString,
                MarketConnectionString = model.MarketConnectionString,
                DirectoryOut = model.DirectoryDecrypted,
            };
        }
    }
}

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
    public class ImportCustomerInfo : IMarketFileParser
    {
        private readonly IClientDataAccess clientDataAccess;
        private readonly ILogger logger;

        public ImportCustomerInfo(IClientDataAccess clientDataAccess, ILogger logger)
        {
            this.clientDataAccess = clientDataAccess;
            this.logger = logger;
        }

        public IMarketFileParseResult Parse(string fileName)
        {
            logger.DebugFormat("Importing File \"{0}\"", fileName);

            var customerInfoFile = new FileInfo(fileName);
            if (!customerInfoFile.Exists)
            {
                logger.DebugFormat("File \"{0}\" does not exist or has been deleted.", fileName);
                return ImportCustomerInfoModel.Empty;
            }

            using (var stream = customerInfoFile.OpenRead())
            {
                return Parse(stream);
            }
        }

        public IMarketFileParseResult Parse(Stream stream)
        {
            var context = new PrismCustomerInfoContext();
            using (var reader = new StreamReader(stream))
            {
                string customerInfoFileLine;
                while ((customerInfoFileLine = reader.ReadLine()) != null)
                    ParseLine(context, customerInfoFileLine);

                context.ResolveToFile();
            }

            return context.Results;
        }

        public void ParseLine(PrismCustomerInfoContext context, string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return;

            var fields = line.Split('|');
            var indicator = fields.AtIndex(0);

            switch(indicator)
            {
                case "HDR":
                    context.ResolveToFile();
                    ParseHeader(context, fields);
                    break;
                case "ER1":
                case "ER2":
                    ParseError(context, fields);
                    break;
                case "SUM":
                    context.ResolveToFile();
                    break;
            }
        }

        public void ParseHeader(PrismCustomerInfoContext context, string[] fields)
        {
            var typeId = 3;
            var typeIndicator = fields.AtIndex(1);
            if (typeIndicator.Equals("MTCRCUSTOMERINFORMATIONERCOTRESPONSE", StringComparison.OrdinalIgnoreCase))
                typeId = 2;

            var model = new TypeCustomerInfoFile
            {
                FileTypeId = typeId,
                ReferenceNumber = fields.AtIndex(2),
                CrDuns = fields.AtIndex(3),
                Status = CustomerInfoFileStatusOptions.Imported,
            };

            var cspDunsId = clientDataAccess.IdentifyCspDunsId(model.CrDuns);
            model.CspDunsId = cspDunsId;

            context.PushModel(model);
        }

        public void ParseError(PrismCustomerInfoContext context, string[] fields)
        {
            var current = context.Current;
            if (current == null || current.ModelType != TypeCustomerInfoTypes.File)
                throw new InvalidOperationException();

            var file = current as TypeCustomerInfoFile;
            if (file == null)
                throw new InvalidOperationException();

            var recordTypeId = 2;
            var recordTypeIndicator = fields[0];
            if (recordTypeIndicator.Equals("ER1", StringComparison.OrdinalIgnoreCase))
                recordTypeId = 1;

            var model = new TypeCustomerInfoErrorRecord
            {
                RecordTypeId = recordTypeId,
                RecordId = fields.AtIndexInt(4),
                ErrorMessage = fields.AtIndex(6),
                FieldName = fields.AtIndex(5),
                PremNo = fields.AtIndex(2),
            };

            file.AddError(model);
        }
    }
}

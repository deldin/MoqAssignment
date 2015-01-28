using System;
using System.Collections.Generic;
using System.Linq;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Parsers.ExportContexts
{
    public class PrismCustomerInfoContext
    {
        private readonly List<ExportCustomerInfoModel> models;
        private ExportCustomerInfoModel currentModel;

        public IMarketFileExportResult[] Models
        {
            get
            {
                return models
                    .Cast<IMarketFileExportResult>()
                    .ToArray();
            }
        }

        public PrismCustomerInfoContext()
        {
            models = new List<ExportCustomerInfoModel>();
        }

        public void AppendLine(string line)
        {
            if (currentModel == null)
                throw new InvalidOperationException();

            var buffer = currentModel.Buffer;
            buffer.Append(line).AppendLine();
        }

        public void PushFile(int cspDunsId, string fileName)
        {
            currentModel = new ExportCustomerInfoModel(fileName)
            {
                CspDunsId = cspDunsId,
                LdcId = 0,
                CspDuns = string.Empty,
                LdcDuns = string.Empty,
                LdcShortName = string.Empty,
                TradingPartnerId = string.Empty,
            };

            models.Add(currentModel);
        }
    }
}

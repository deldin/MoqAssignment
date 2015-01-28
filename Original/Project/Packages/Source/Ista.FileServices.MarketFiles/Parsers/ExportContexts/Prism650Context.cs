using System;
using System.Collections.Generic;
using System.Linq;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Parsers.ExportContexts
{
    public class Prism650Context
    {
        private readonly List<Export650Model> models;
        private Export650Model currentModel;

        public string TransactionSetPurposeCode { get; set; }
        public string LdcCode { get; private set; }
        public string TradingPartnerId { get; private set; }

        public int MarketId { get; private set; }
        public MarketOptions Market { get; private set; }

        public IMarketFileExportResult[] Models
        {
            get
            {
                return models
                    .Cast<IMarketFileExportResult>()
                    .ToArray();
            }
        }

        public Prism650Context()
        {
            models = new List<Export650Model>();

            Market = MarketOptions.Unknown;
            MarketId = (int)MarketOptions.Unknown;
        }

        public void AppendLine(string line)
        {
            if (currentModel == null)
                throw new InvalidOperationException();

            var buffer = currentModel.Buffer;
            buffer.Append(line).AppendLine();
        }

        public void Initialize()
        {
            TransactionSetPurposeCode = string.Empty;
            LdcCode = string.Empty;
            TradingPartnerId = string.Empty;

            Market = MarketOptions.Unknown;
            MarketId = (int)MarketOptions.Unknown;
        }

        public void SetMarket(int id)
        {
            Market = (MarketOptions)id;
            MarketId = id;
        }

        public void SetFileProperties(CspDunsPortModel model, string tdspDuns, string suffix)
        {
            var partnerId = model.TradingPartnerId
                .Replace("{DUNS}", tdspDuns);

            LdcCode = model.LdcShortName;
            TradingPartnerId = string.Concat(partnerId, suffix);

            var existingModel = models.FirstOrDefault(x =>
                x.LdcShortName.Equals(LdcCode) &&
                x.TradingPartnerId.Equals(TradingPartnerId));

            if (existingModel != null)
            {
                currentModel = existingModel;
                return;
            }

            currentModel = new Export650Model(false)
            {
                LdcId = model.LdcId ?? 0,
                CspDunsId = model.CspDunsId,
                CspDuns = string.Empty,
                LdcDuns = string.Empty,
                LdcShortName = LdcCode,
                TradingPartnerId = TradingPartnerId,
            };

            models.Add(currentModel);
        }

        public void SetHeaderId(int headerKey)
        {
            currentModel.AddHeaderKey(headerKey);
        }
    }
}

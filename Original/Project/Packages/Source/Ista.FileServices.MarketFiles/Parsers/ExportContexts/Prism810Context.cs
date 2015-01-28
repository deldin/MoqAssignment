using System;
using System.Collections.Generic;
using System.Linq;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Parsers.ExportContexts
{
    public class Prism810Context
    {
        private readonly List<Export810Model> models;
        private Export810Model currentModel;

        public bool IsCustomerInvoice { get; set; }
        public string BillFromName { get; set; }
        public string BillFromDuns { get; set; }
        public decimal RunningTotal { get; private set; }
        
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

        public Prism810Context()
        {
            models = new List<Export810Model>();

            Market = MarketOptions.Unknown;
            MarketId = (int)MarketOptions.Unknown;
        }

        public void AddToRunningTotal(string amount)
        {
            if (string.IsNullOrWhiteSpace(amount))
                return;

            decimal value;
            if (!decimal.TryParse(amount, out value))
                return;

            RunningTotal += value;
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
            IsCustomerInvoice = false;
            BillFromName = string.Empty;
            BillFromDuns = string.Empty;
            RunningTotal = 0m;

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
            SetFileProperties(model, model.LdcShortName, model.TradingPartnerId, tdspDuns, suffix);
        }

        public void SetFileProperties(CspDunsPortModel model, string ldcCode, string tradingPartnerId, string tdspDuns, string suffix)
        {
            var partnerId = tradingPartnerId
                .Replace("{DUNS}", tdspDuns);

            LdcCode = ldcCode;
            TradingPartnerId = string.Concat(partnerId, suffix);

            var existingModel = models.FirstOrDefault(x =>
                x.LdcShortName.Equals(LdcCode) &&
                x.TradingPartnerId.Equals(TradingPartnerId));

            if (existingModel != null)
            {
                currentModel = existingModel;
                return;
            }

            currentModel = new Export810Model(false)
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

        public void SetFileNamePrefix(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                return;

            currentModel.FileNamePrefix = prefix;
        }

        public void SetTradingPartnerId(string tradingPartnerId)
        {
            TradingPartnerId = tradingPartnerId;
        }

        public void SetHeaderId(int headerKey)
        {
            currentModel.AddHeaderKey(headerKey);
        }
    }
}
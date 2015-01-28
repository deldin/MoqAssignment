using System;
using System.Linq;
using System.Threading;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Models;
using Ista.FileServices.MarketFiles.Parsers.ExportContexts;

namespace Ista.FileServices.MarketFiles.Parsers
{
    public class Export810Prism : IMarketFileExporter
    {
        private readonly IClientDataAccess clientDataAccess;
        private readonly IMarket810Export marketDataAccess;
        private readonly ILogger logger;

        public Export810Prism(IClientDataAccess clientDataAccess, IMarket810Export marketDataAccess, ILogger logger)
        {
            this.clientDataAccess = clientDataAccess;
            this.marketDataAccess = marketDataAccess;
            this.logger = logger;
        }

        public IMarketFileExportResult[] Export(CancellationToken token)
        {
            var cspDunsPorts = clientDataAccess.ListCspDunsPort();
            var prismPorts = cspDunsPorts
                .Where(x => x.ProviderId == 1)
                .ToArray();

            var context = new Prism810Context();
            foreach (var prismPort in prismPorts)
            {
                if (token.IsCancellationRequested)
                    break;

                var headers = marketDataAccess.ListUnprocessed(prismPort.LdcDuns, prismPort.Duns, 1);
                if (headers.Length == 0)
                {
                    logger.TraceFormat("Zero 810 Prism records found to export for TDSP Duns \"{0}\" and CR Duns \"{1}\".",
                        prismPort.LdcDuns, prismPort.Duns);
                    continue;
                }

                logger.DebugFormat("Exporting {0} unprocessed 810 record(s) for TDSP Duns \"{1}\" and CR Duns \"{2}\".",
                    headers.Length, prismPort.LdcDuns, prismPort.Duns);

                var cspDuns = clientDataAccess.LoadDunsByCspDunsId(prismPort.CspDunsId);

                foreach (var header in headers)
                {
                    if (!header.HeaderKey.HasValue)
                        continue;

                    if (token.IsCancellationRequested)
                        break;

                    var headerKey = header.HeaderKey.Value;
                    context.Initialize();

                    var ldcModel = clientDataAccess.LoadLdcByTdspDuns(header.TdspDuns);
                    if (ldcModel == null)
                    {
                        logger.ErrorFormat("Failed to load LDC for DUNS \"{0}\".", header.TdspDuns);
                        return context.Models;
                    }

                    context.SetMarket(ldcModel.MarketId);
                    context.SetFileProperties(prismPort, header.TdspDuns, "INV");
                    context.IsCustomerInvoice = false;
                    context.BillFromName = header.TdspName;
                    context.BillFromDuns = header.TdspDuns;

                    if (!string.IsNullOrWhiteSpace(cspDuns) && cspDuns.Equals("RYDER0000", StringComparison.Ordinal))
                        context.SetFileNamePrefix("ECPCUSTINVRYDER");

                    if (!string.IsNullOrEmpty(header.CustomerDUNS))
                    {
                        var seekDuns = header.TdspDuns;
                        if (seekDuns.Length > 9)
                            seekDuns = seekDuns.Substring(0, 9);

                        var partnerId = context.TradingPartnerId.Replace(seekDuns, header.CustomerDUNS);
                        context.SetTradingPartnerId(partnerId);
                        
                        var cspDunsList = clientDataAccess.ListCspDuns();
                        if (cspDunsList == null || cspDunsList.Length == 0)
                        {
                            logger.Error("No CSP DUNS exist or none could be found.");
                            return context.Models;
                        }

                        var cspDunsItem = cspDunsList.First();
                        context.IsCustomerInvoice = true;
                        context.BillFromName = cspDunsItem.Description;
                        context.BillFromDuns = cspDunsItem.Duns;

                        if (header.CustomerDUNS.Equals("055458350"))
                        {
                            var crdPartnerId = context.TradingPartnerId;
                            var fixedCrdPartnerId = string.Concat("CRD",
                                crdPartnerId.Substring(3, crdPartnerId.Length - 3));

                            context.SetTradingPartnerId(fixedCrdPartnerId);
                        }

                        if (header.CustomerDUNS.Equals("PROKARMAI", StringComparison.Ordinal))
                        {
                            var lptPartnerId = context.TradingPartnerId;
                            var fixedLptPartnerId = string.Concat("LPT",
                                lptPartnerId.Substring(3, lptPartnerId.Length - 3));

                            var zzDuns = string.Concat("ZZ", header.CustomerDUNS);
                            context.SetFileProperties(prismPort, zzDuns, fixedLptPartnerId, header.CustomerDUNS, string.Empty);
                        }
                    }

                    context.SetHeaderId(headerKey);

                    var invoiceConfig = clientDataAccess
                        .LoadCustomerInvoiceConfig(header.CustomerDUNS, ldcModel.LdcId);

                    if (invoiceConfig != null)
                    {
                        var zzPartnerId = context.TradingPartnerId;
                        var fixedZzPartnerId = string.Concat(zzPartnerId.Substring(0, 3), "ZZ",
                            zzPartnerId.Substring(5, zzPartnerId.Length - 5));

                        context.SetTradingPartnerId(fixedZzPartnerId);

                        var shouldSkipMeterReads = false;
                        var configuredCustomerDuns = clientDataAccess
                            .LoadParameterConfigurationValue("CustomerDunsConfiguredForSkip30Record");
                        if (!string.IsNullOrWhiteSpace(configuredCustomerDuns))
                            shouldSkipMeterReads = configuredCustomerDuns.Equals(invoiceConfig.CustomerDuns);
                        
                        WriteCustInvHeader(context, invoiceConfig, header);
                        WriteCustInvAccount(context, invoiceConfig, header);
                        WriteCustInvAddress(context, invoiceConfig, header);
                        WriteCustInvRemitanceAddress(context, invoiceConfig, header);
                        WriteCustInvDetail(context, invoiceConfig, header);
                        WriteCustInvAccountTaxCharges(context, invoiceConfig, header);
                        if (!shouldSkipMeterReads)
                            WriteCustInvMeterReads(context, invoiceConfig, header);
                        WriteCustInvCharges(context, invoiceConfig, header);
                        WriteCustInvTax(context, invoiceConfig, header);
                        WriteCustInvSummary(context, invoiceConfig, header);
                    }
                    else
                    {
                        if (context.TradingPartnerId.Equals("CRDTX055458350INV", StringComparison.Ordinal))
                        {
                            var txPartnerId = context.TradingPartnerId;
                            var fixedTxPartnerId = txPartnerId.Replace("CRDTX", "CRDZZ");
                            
                            context.SetTradingPartnerId(fixedTxPartnerId);

                            WriteNiscHeader(context, header);
                            WriteNiscAccount(context, header);
                            WriteNiscRemitanceAddress(context, header);
                            WriteNiscDetail(context, header);
                            WriteNiscAccountTaxCharges(context, header);
                            WriteNiscMeterReads(context, header);
                            WriteNiscCharges(context, header);
                            WriteNiscTax(context, header);
                            WriteNiscSummary(context, header);
                        }
                        else
                        {
                            WriteHeader(context, header);
                            WriteAccount(context, header);
                            WriteRemitanceAddress(context, header);
                            WriteDetail(context, header);
                            WriteMeterReads(context, header);
                            WriteCharges(context, header);
                            WriteTax(context, header);
                            WriteSummary(context, header);
                        }
                    }
                }
            }

            return context.Models;
        }

        public void WriteCustInvHeader(Prism810Context context, CustomerInvoiceConfigModel configModel, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            var referenceNbr = header.InvoiceNbr;
            if (configModel.UseAccNumberForInvoice)
                referenceNbr = header.EsiId;

            var line = string.Format("SH|{0}|{1}|O|", context.TradingPartnerId, referenceNbr);
            context.AppendLine(line);
            logger.TraceFormat("Wrote 810 \"SH\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteCustInvAccount(Prism810Context context, CustomerInvoiceConfigModel configModel, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            var partnerId = context.TradingPartnerId;
            var stateId = partnerId.Substring(3, 2);

            var invoiceNbr = header.InvoiceNbr;
            if (configModel.UseAccNumberForInvoice)
                invoiceNbr = header.EsiId;

            var arSummary = clientDataAccess.LoadArSummaryByInvoice(header.InvoiceNbr);
            if (arSummary == null)
            {
                logger.ErrorFormat("Could not load Customer AR Summary from Invoice {0}", header.InvoiceNbr);
                return;
            }

            var previousBalance = arSummary.PrevBal;
            var currentBalance = arSummary.BalDue;
            var billingBalance = (arSummary.PrevBal - arSummary.CurrPmts + arSummary.CurrAdjs);

            var billActionCode = "BD";
            var ldcName = header.TdspName;

            if (header.CustomerDUNS.Equals("007928344"))
            {
                billActionCode = "PR";
            }

            if (header.CustomerDUNS.Equals("055458350"))
            {
                billActionCode = "ME";
                ldcName = "NISC";
            }

            if (header.CustomerDUNS.Equals("PROKARMAI", StringComparison.Ordinal))
            {
                billActionCode = "ME";
                ldcName = "PROKARMA";
            }
            
            var line =
                string.Format(
                    "01|{0}|{1}|{2}|{3}|{4}||||||||{5}|{6}|||||{7}|{8}|{9}|{10}|||||{11}|{12}|{13}|{14}|||||||||||||||||||||||||||00||||",
                    context.TradingPartnerId, stateId, header.TransactionDate, invoiceNbr, billActionCode,
                    header.CrAccountNumber, header.EsiId, ldcName, header.CustomerDUNS, context.BillFromName,
                    context.BillFromDuns, header.PaymentDueDate, previousBalance, billingBalance, currentBalance);

            context.AppendLine(line);
            logger.TraceFormat("Wrote 810 \"01\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteCustInvAddress(Prism810Context context, CustomerInvoiceConfigModel configModel, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            string entityName;
            string entityId;
            string address1;
            string address2;
            string city;
            string state;
            string postalCode;
            var attn = string.Empty;

            if (configModel.BtCustomAddressLine)
            {
                entityName = configModel.BtEntityName;
                entityId = configModel.BtEntityId;
                attn = configModel.BtAttn;
                address1 = configModel.BtEntityAddress1;
                address2 = configModel.BtEntityAddress2;
                city = configModel.BtEntityCity;
                state = configModel.BtEntityState;
                postalCode = configModel.BtEntityZip;
            }
            else
            {
                var customer = clientDataAccess.LoadCustomerDetailByEsiId(header.EsiId);
                if (customer == null)
                {
                    logger.ErrorFormat("Could not load Customer from EsiId {0}.", header.EsiId);
                    return;
                }

                entityName = customer.CustName.ToAscii();
                entityId = customer.CustNo.ToAscii();
                address1 = customer.Address1.ToAscii();
                address2 = customer.Address2.ToAscii();
                city = customer.City.ToAscii();
                state = customer.State.ToAscii();
                postalCode = customer.Zip.ToAscii();
            }

            var line = string.Format("05|{0}|BT|{1}|{2}|{3}||{4}|{5}|{6}|{7}|{8}||||||||", context.TradingPartnerId,
                entityName, entityId, attn, address1, address2, city, state, postalCode);

            context.AppendLine(line);
            logger.TraceFormat("Wrote 810 \"05\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteCustInvRemitanceAddress(Prism810Context context, CustomerInvoiceConfigModel configModel, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            if (!configModel.ReAddressLine)
                return;

            var entityName = configModel.ReEntityName.ToAscii();
            var entityId = configModel.ReEntityId.ToAscii();
            var address1 = configModel.ReEntityAddress1.ToAscii();
            var address2 = configModel.ReEntityAddress2.ToAscii();
            var city = configModel.ReEntityCity.ToAscii();
            var state = configModel.ReEntityState;
            var postalCode = configModel.ReEntityZip;

            var line = string.Format("05|{0}|RE|{1}|{2}|||{3}|{4}|{5}|{6}|{7}||||||||", context.TradingPartnerId,
                entityName, entityId, address1, address2, city, state, postalCode);

            context.AppendLine(line);
            logger.TraceFormat("Wrote 810 \"05\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteCustInvDetail(Prism810Context context, CustomerInvoiceConfigModel configModel, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            if (!header.HeaderKey.HasValue)
                return;

            var headerKey = header.HeaderKey.Value;
            var details = marketDataAccess.ListDetails(headerKey);
            if (details == null || details.Length == 0)
            {
                logger.ErrorFormat("No detail record found for 810 Key {0}.", headerKey);
                return;
            }

            var detail = details.First();
            var premiseInfo = clientDataAccess.LoadPremiseByEsiId(header.EsiId);
            if (premiseInfo == null)
            {
                logger.ErrorFormat("Could not load Premise information for EsiId {0}", header.EsiId);
                return;
            }

            var line =
                string.Format("10|{0}|1|ELECTRIC|{1}||||{2}|{3}|{4}|{5}|{6}||{7}||{8}|{9}|{10}|{11}|{12}|||||||{13}|{14}|",
                    context.TradingPartnerId, detail.ServiceClass, detail.ServicePeriodStartDate,
                    detail.ServicePeriodEndDate, header.EsiId, premiseInfo.MeterNo, detail.RateClass,
                    premiseInfo.CustName.ToAscii(), premiseInfo.Addr1.ToAscii(), premiseInfo.Addr2.ToAscii(),
                    premiseInfo.City.ToAscii(), premiseInfo.State, premiseInfo.Zip, premiseInfo.EdiInfo1, premiseInfo.EdiInfo2);

            context.AppendLine(line);
            logger.TraceFormat("Wrote 810 \"10\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteCustInvAccountTaxCharges(Prism810Context context, CustomerInvoiceConfigModel configModel, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            if (configModel.TaxesAsCharge)
                return;

            if (!header.HeaderKey.HasValue)
                return;

            var headerKey = header.HeaderKey.Value;
            var charges = marketDataAccess.ListDetailItemChargesByHeader(headerKey);
            if (charges == null || charges.Length == 0)
                return;

            foreach (var charge in charges)
            {
                if (!charge.ChargeCode.Equals("D140", StringComparison.Ordinal))
                    continue;

                var amount = charge.Amount;
                if (amount.Contains("."))
                    amount = amount.Substring(0, amount.IndexOf('.') + 3);

                var line = string.Format("20|{0}|LS|{1}||||", context.TradingPartnerId, amount);
                context.AppendLine(line);
                logger.TraceFormat("Wrote 810 \"20\" PRISM line for Header {0}", header.HeaderKey);

                if (configModel.IncludeTaxesInTotal)
                    context.AddToRunningTotal(amount);
            }
        }

        public void WriteCustInvMeterReads(Prism810Context context, CustomerInvoiceConfigModel configModel, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            var meterCons = clientDataAccess.ListMeterConsumptionByInvoice(header.InvoiceNbr);
            if (meterCons == null || meterCons.Length == 0)
                return;

            foreach (var meterCon in meterCons)
            {
                var line = string.Format("30|{0}|AA|{1}|KH|51|{2}|{3}||||", context.TradingPartnerId,
                    meterCon.MeterFactor, meterCon.BegRead, meterCon.EndRead);

                context.AppendLine(line);
                logger.TraceFormat("Wrote 810 \"30\" PRISM line for Header {0}", header.HeaderKey);
            }
        }

        public void WriteCustInvCharges(Prism810Context context, CustomerInvoiceConfigModel configModel, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            if (!header.HeaderKey.HasValue)
                return;

            var headerKey = header.HeaderKey.Value;
            var charges = marketDataAccess.ListDetailItemChargesByHeader(headerKey);
            if (charges == null || charges.Length == 0)
                return;

            var lineNumber = 1;
            foreach (var charge in charges)
            {
                if (charge.ChargeCode.Equals("D140", StringComparison.Ordinal) && !configModel.TaxesAsCharge)
                    continue;

                var rate = charge.Rate;
                if (rate.Length > 0)
                    rate = decimal.Round(decimal.Parse(rate), 5).ToString();

                var uom = charge.UOM;
                if (string.IsNullOrEmpty(uom))
                    uom = "EA";

                var amount = charge.Amount;
                if (amount.Contains("."))
                    amount = amount.Substring(0, amount.IndexOf('.') + 3);

                var line = string.Format("40|{0}|{1}|{2}||{3}|{4}|{5}|{6}|{7}||{8}||", context.TradingPartnerId,
                    lineNumber, charge.ChargeIndicator, charge.ChargeCode, amount, rate, uom, charge.Quantity,
                    charge.Description);

                lineNumber++;
                context.AddToRunningTotal(amount);

                context.AppendLine(line);
                logger.TraceFormat("Wrote 810 \"40\" PRISM line for Header {0}", header.HeaderKey);
            }
        }

        public void WriteCustInvTax(Prism810Context context, CustomerInvoiceConfigModel configModel, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            if (!header.HeaderKey.HasValue)
                return;

            var headerKey = header.HeaderKey.Value;
            var taxes = marketDataAccess.ListDetailItemTaxesByHeader(headerKey);
            if (taxes == null || taxes.Length == 0)
                return;

            foreach (var tax in taxes)
            {
                var amount = tax.TaxAmount;
                if (amount.Contains("."))
                    amount = amount.Substring(0, amount.IndexOf('.') + 3);

                var line = string.Format("45|{0}|{1}|{2}|||||", context.TradingPartnerId, tax.TaxTypeCode, amount);
                context.AppendLine(line);
                logger.TraceFormat("Wrote 810 \"45\" PRISM line for Header {0}", header.HeaderKey);
            }
        }

        public void WriteCustInvSummary(Prism810Context context, CustomerInvoiceConfigModel configModel, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            var runningTotal = (Math.Truncate(context.RunningTotal * 100) / 100).ToString("#.#0");

            var line = string.Format("60|{0}|{1}|1|", context.TradingPartnerId, runningTotal);
            context.AppendLine(line);
            logger.TraceFormat("Wrote 810 \"60\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteNiscHeader(Prism810Context context, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            var line = string.Format("SH|{0}|{1}|O|", context.TradingPartnerId, header.InvoiceNbr);
            context.AppendLine(line);
            logger.TraceFormat("Wrote 810 \"SH\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteNiscAccount(Prism810Context context, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            var partnerId = context.TradingPartnerId;
            var stateId = partnerId.Substring(3, 2);

            var arSummary = clientDataAccess.LoadArSummaryByInvoice(header.InvoiceNbr);
            var previousBalance = arSummary.PrevBal;
            var currentBalance = arSummary.BalDue;
            var billingBalance = (arSummary.PrevBal - arSummary.CurrPmts + arSummary.CurrAdjs);

            var line =
                string.Format(
                    "01|{0}|{1}|{2}|{3}|ME||||||||{4}|{5}|||||NISC|{6}|{7}|{8}|||||{9}|{10}|{11}|{12}|||||||||||||||||||||||||||00||||",
                    context.TradingPartnerId, stateId, header.TransactionDate, header.InvoiceNbr,
                    header.CrAccountNumber, header.EsiId, header.CustomerDUNS, context.BillFromName,
                    context.BillFromDuns, header.PaymentDueDate, previousBalance, billingBalance, currentBalance);

            context.AppendLine(line);
            logger.TraceFormat("Wrote 810 \"01\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteNiscRemitanceAddress(Prism810Context context, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            var customer = clientDataAccess.LoadCustomerDetailByEsiId(header.EsiId);
            if (customer == null)
            {
                logger.ErrorFormat("Could not load Customer from EsiId {0}.", header.EsiId);
                return;
            }

            var entityName = customer.CustName.ToAscii();
            var entityId = customer.CustNo.ToAscii();
            var address1 = customer.Address1.ToAscii();
            var address2 = customer.Address2.ToAscii();
            var city = customer.City.ToAscii();
            var state = customer.State;
            var postalCode = customer.Zip;

            var line = string.Format("05|{0}|BT|{1}|{2}|||{3}|{4}|{5}|{6}|{7}||||||||", context.TradingPartnerId,
                entityName, entityId, address1, address2, city, state, postalCode);

            context.AppendLine(line);
            logger.TraceFormat("Wrote 810 \"05\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteNiscDetail(Prism810Context context, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            if (!header.HeaderKey.HasValue)
                return;

            var headerKey = header.HeaderKey.Value;
            var details = marketDataAccess.ListDetails(headerKey);
            if (details == null || details.Length == 0)
            {
                logger.ErrorFormat("No 810 Detail records found for 810 Key {0}.", headerKey);
                return;
            }

            var detail = details.First();
            var premiseInfo = clientDataAccess.LoadPremiseByEsiId(header.EsiId);
            if (premiseInfo == null)
            {
                logger.ErrorFormat("Could not load Premise information for EsiId {0}", header.EsiId);
                return;
            }

            var line =
                string.Format("10|{0}|BTC002|ELECTRIC|{1}||||{2}|{3}|{4}|{5}|{6}||{7}||{8}|{9}|{10}|{11}|{12}|||||||{13}|{14}|",
                    context.TradingPartnerId, detail.ServiceClass, detail.ServicePeriodStartDate,
                    detail.ServicePeriodEndDate, header.EsiId, premiseInfo.MeterNo, detail.RateClass,
                    premiseInfo.CustName.ToAscii(), premiseInfo.Addr1.ToAscii(), premiseInfo.Addr2.ToAscii(),
                    premiseInfo.City.ToAscii(), premiseInfo.State, premiseInfo.Zip, premiseInfo.EdiInfo1, premiseInfo.EdiInfo2);

            context.AppendLine(line);
            logger.TraceFormat("Wrote 810 \"10\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteNiscAccountTaxCharges(Prism810Context context, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            if (!header.HeaderKey.HasValue)
                return;

            var headerKey = header.HeaderKey.Value;
            var charges = marketDataAccess.ListDetailItemChargesByHeader(headerKey);
            if (charges == null || charges.Length == 0)
                return;

            foreach (var charge in charges)
            {
                if (!charge.ChargeCode.Equals("D140", StringComparison.Ordinal))
                    continue;

                var amount = charge.Amount;
                if (amount.Contains("."))
                    amount = amount.Substring(0, amount.IndexOf('.') + 3);

                var line = string.Format("20|{0}|LS|{1}||||", context.TradingPartnerId, amount);
                context.AppendLine(line);
                logger.TraceFormat("Wrote 810 \"20\" PRISM line for Header {0}", header.HeaderKey);
            }
        }

        public void WriteNiscMeterReads(Prism810Context context, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            var meterCons = clientDataAccess.ListMeterConsumptionByInvoice(header.InvoiceNbr);
            if (meterCons == null || meterCons.Length == 0)
                return;

            foreach (var meterCon in meterCons)
            {
                var line = string.Format("30|{0}|AA|{1}|KH|51|{2}|{3}||||", context.TradingPartnerId,
                    meterCon.MeterFactor, meterCon.BegRead, meterCon.EndRead);

                context.AppendLine(line);
                logger.TraceFormat("Wrote 810 \"30\" PRISM line for Header {0}", header.HeaderKey);
            }
        }

        public void WriteNiscCharges(Prism810Context context, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            if (!header.HeaderKey.HasValue)
                return;

            var headerKey = header.HeaderKey.Value;
            var charges = marketDataAccess.ListDetailItemChargesByHeader(headerKey);
            if (charges == null || charges.Length == 0)
                return;

            var lineNumber = 1;
            foreach (var charge in charges)
            {
                if (charge.ChargeCode.Equals("D140", StringComparison.Ordinal))
                    continue;

                var rate = charge.Rate;
                if (rate.Length > 0)
                    rate = decimal.Round(decimal.Parse(rate), 5).ToString();

                var uom = charge.UOM;
                if (string.IsNullOrEmpty(uom))
                    uom = "EA";

                var amount = charge.Amount;
                if (amount.Contains("."))
                    amount = amount.Substring(0, amount.IndexOf('.') + 3);

                var chargeIndicator = "A";
                double amountValue;
                if (double.TryParse(charge.Amount, out amountValue))
                    if (amountValue > 0d)
                        chargeIndicator = "C";

                var line = string.Format("40|{0}|{1}|{2}||{3}|{4}|{5}|{6}|{7}||{8}||", context.TradingPartnerId,
                    lineNumber, chargeIndicator, charge.ChargeCode, amount, rate, uom, charge.Quantity,
                    charge.Description);

                lineNumber++;
                context.AppendLine(line);
                logger.TraceFormat("Wrote 810 \"40\" PRISM line for Header {0}", header.HeaderKey);
            }
        }

        public void WriteNiscTax(Prism810Context context, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            if (!header.HeaderKey.HasValue)
                return;

            var headerKey = header.HeaderKey.Value;
            var taxes = marketDataAccess.ListDetailItemTaxesByHeader(headerKey);
            if (taxes == null || taxes.Length == 0)
                return;

            foreach (var tax in taxes)
            {
                var amount = tax.TaxAmount;
                if (amount.Contains("."))
                    amount = amount.Substring(0, amount.IndexOf('.') + 3);

                var line = string.Format("45|{0}|{1}|{2}|||||", context.TradingPartnerId, tax.TaxTypeCode, amount);
                context.AppendLine(line);
                logger.TraceFormat("Wrote 810 \"45\" PRISM line for Header {0}", header.HeaderKey);
            }
        }

        public void WriteNiscSummary(Prism810Context context, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            var amount = header.TotalAmount;
            if (amount.Contains("."))
                amount = amount.Substring(0, amount.IndexOf('.') + 3);

            var line = string.Format("60|{0}|{1}|1|", context.TradingPartnerId, amount);
            context.AppendLine(line);
            logger.TraceFormat("Wrote 810 \"60\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteHeader(Prism810Context context, Type810Header header)
        {
            string line;
            if (context.IsCustomerInvoice)
            {
                line = string.Format("SH|{0}|{1}|", context.TradingPartnerId, header.InvoiceNbr);
                context.AppendLine(line);
                logger.TraceFormat("Wrote 810 \"SH\" PRISM line for Header {0}", header.HeaderKey);
                return;
            }
            
            line = string.Format("SH|{0}|{1}|O|", context.TradingPartnerId, header.InvoiceNbr);
            context.AppendLine(line);
            logger.TraceFormat("Wrote 810 \"SH\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteAccount(Prism810Context context, Type810Header header)
        {
            var partnerId = context.TradingPartnerId;
            var stateId = partnerId.Substring(3, 2);

            if (!header.HeaderKey.HasValue)
                return;

            var headerKey = header.HeaderKey.Value;

            string line;
            if (context.IsCustomerInvoice)
            {
                
                var details = marketDataAccess.ListDetails(headerKey);
                if (details == null || details.Length == 0)
                {
                    logger.ErrorFormat("No 810 Detail records found for 810 Key {0}", headerKey);
                    return;
                }

                var customer = clientDataAccess.LoadCustomerDetailByEsiId(header.EsiId);
                if (customer == null)
                {
                    logger.ErrorFormat("Could not load Customer from EsiId {0}.", header.EsiId);
                    return;
                }

                var arSummary = clientDataAccess.LoadArSummaryByInvoice(header.InvoiceNbr);
                if (arSummary == null)
                {
                    logger.ErrorFormat("Could not load Customer AR Summary from Invoice {0}", header.InvoiceNbr);
                    return;
                }

                var detail = details.First();
                var previousBalance = arSummary.PrevBal;
                var currentBalance = arSummary.BalDue;
                var billingBalance = (arSummary.PrevBal - arSummary.CurrPmts + arSummary.CurrAdjs);

                line =
                    string.Format(
                        "01|{0}|{1}|{2}|{3}|{4}|||||||{5}|{6}|||||||{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|||||{15}|{16}|||{17}|{18}|{19}|{20}|{21}|||||||||00||||",
                        context.TradingPartnerId, stateId, header.TransactionDate, header.InvoiceNbr,
                        header.TransactionTypeCode, customer.CustNo, header.EsiId, context.BillFromName,
                        context.BillFromDuns, customer.CustName.ToAscii(), header.CustNoForESCO, header.PaymentDueDate,
                        previousBalance, billingBalance, currentBalance, detail.ServicePeriodStartDate,
                        detail.ServicePeriodEndDate, customer.Address1.ToAscii(), customer.Address2.ToAscii(),
                        customer.City.ToAscii(), customer.State, customer.Zip);

                context.AppendLine(line);
                logger.TraceFormat("Wrote 810 \"01\" PRISM line for Header {0}", header.HeaderKey);
                return;
            }

            var customerName = string.Empty;
            if (context.Market == MarketOptions.Texas)
            {
                var firstName = marketDataAccess.LoadFirstName(headerKey);
                if (firstName != null)
                {
                    customerName = firstName.EntityName;
                    if (customerName.Length > 60)
                        customerName = customerName.Substring(0, 60);
                }
            }

            var esiId = IdentifyEsiId(context, header);
            var membershipId = IdentifyLdcAccountNumber(context, header);

            line =
                string.Format(
                    "01|{0}|{1}|{2}|{3}|{4}|{5}|{6}|||||{7}|{8}|{14}|||{15}|{16}|{9}|{10}|{11}|{12}|{17}||||||||||||||||||||||||||||||{13}|||||",
                    context.TradingPartnerId, stateId, header.TransactionDate, header.InvoiceNbr, header.ReleaseNbr,
                    header.TransactionTypeCode, header.TransactionSetPurposeCode, header.OriginalInvoiceNbr,
                    header.CrAccountNumber, context.BillFromName, context.BillFromDuns, header.CrName.ToAscii(),
                    header.CrDuns, esiId, membershipId, header.BillPresenter, header.BillCalculator,
                    customerName.ToAscii());

            context.AppendLine(line);
            logger.TraceFormat("Wrote 810 \"01\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteRemitanceAddress(Prism810Context context, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            var customer = clientDataAccess.LoadCustomerDetailByEsiId(header.EsiId);
            if (customer == null)
            {
                logger.ErrorFormat("Could not load Customer from EsiId {0}.", header.EsiId);
                return;
            }

            var address1 = customer.RemitAddress1.ToAscii();
            var address2 = customer.RemitAddress2.ToAscii();
            var city = customer.RemitCity.ToAscii();
            var state = customer.RemitState;
            var postalCode = customer.RemitZip;

            var line = string.Format("05|{0}|RE|||||{1}|{2}|{3}|{4}|{5}||||||||", context.TradingPartnerId,
                address1, address2, city, state, postalCode);

            context.AppendLine(line);
            logger.TraceFormat("Wrote 810 \"05\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteDetail(Prism810Context context, Type810Header header)
        {
            if (!header.HeaderKey.HasValue)
                return;

            var headerKey = header.HeaderKey.Value;
            var details = marketDataAccess.ListDetails(headerKey);
            if (details == null || details.Length == 0)
            {
                logger.ErrorFormat("No 810 Detail records found for 810 Key {0}.", headerKey);
                return;
            }

            var detail = details.First();

            string line;
            if (context.IsCustomerInvoice)
            {
                var premiseInfo = clientDataAccess.LoadPremiseByEsiId(header.EsiId);
                if (premiseInfo == null)
                {
                    logger.ErrorFormat("Could not load Premise information for EsiId {0}", header.EsiId);
                    return;
                }

                line = string.Format("10|{0}|BTC002|ELECTRIC|{1}||||{2}|{3}|{4}|{5}||{6}||{7}|{8}|{9}|{10}|{11}|||||",
                    context.TradingPartnerId, detail.ServiceClass, detail.ServicePeriodStartDate,
                    detail.ServicePeriodEndDate, premiseInfo.MeterNo, detail.RateClass, premiseInfo.CustName.ToAscii(),
                    premiseInfo.Addr1.ToAscii(), premiseInfo.Addr2.ToAscii(), premiseInfo.City.ToAscii(),
                    premiseInfo.State, premiseInfo.Zip);

                context.AppendLine(line);
                logger.TraceFormat("Wrote 810 \"10\" PRISM line for Header {0}", header.HeaderKey);
                return;
            }

            line = string.Format("10|{0}|1|ELECTRIC|{1}||{2}|{3}|||||||||", context.TradingPartnerId,
                detail.ServiceClass, detail.ServicePeriodStartDate, detail.ServicePeriodEndDate);

            context.AppendLine(line);
            logger.TraceFormat("Wrote 810 \"10\" PRISM line for Header {0}", header.HeaderKey);
        }

        public void WriteMeterReads(Prism810Context context, Type810Header header)
        {
            if (!context.IsCustomerInvoice)
                return;

            var meterCons = clientDataAccess.ListMeterConsumptionByInvoice(header.InvoiceNbr);
            if (meterCons == null || meterCons.Length == 0)
                return;

            foreach (var meterCon in meterCons)
            {
                var line = string.Format("30|{0}|AA|{1}|KH|51|{2}|{3}||||", context.TradingPartnerId,
                    meterCon.MeterFactor, meterCon.BegRead, meterCon.EndRead);

                context.AppendLine(line);
                logger.TraceFormat("Wrote 810 \"30\" PRISM line for Header {0}", header.HeaderKey);
            }
        }

        public void WriteCharges(Prism810Context context, Type810Header header)
        {
            if (!header.HeaderKey.HasValue)
                return;

            var headerKey = header.HeaderKey.Value;
            var charges = marketDataAccess.ListDetailItemChargesByHeader(headerKey);
            if (charges == null || charges.Length == 0)
                return;

            var lineNumber = 1;
            foreach (var charge in charges)
            {
                var rate = charge.Rate;
                if (rate.Length > 0)
                    rate = decimal.Round(decimal.Parse(rate), 5).ToString();

                var amount = charge.Amount;
                if (amount.Contains("."))
                    amount = amount.Substring(0, amount.IndexOf('.') + 3);

                if (context.IsCustomerInvoice)
                {
                    var uom = charge.UOM;
                    if (string.IsNullOrEmpty(uom))
                        uom = "EA";

                    var chargeIndicator = "A";
                    double amountValue;
                    if (double.TryParse(charge.Amount, out amountValue))
                        if (amountValue > 0d)
                            chargeIndicator = "C";

                    var line = string.Format("40|{0}||{1}||{2}|{3}|{4}|{5}|{6}||{7}||", context.TradingPartnerId,
                        chargeIndicator, charge.ChargeCode, amount, rate, uom, charge.Quantity, charge.Description);

                    context.AppendLine(line);
                    logger.TraceFormat("Wrote 810 \"40\" PRISM line for Header {0}", header.HeaderKey);
                }
                else
                {
                    var description = "---";
                    if (!string.IsNullOrEmpty(charge.Description))
                        description = charge.Description;
                    else if (!string.IsNullOrEmpty(charge.ChargeIndicator))
                        description = charge.ChargeIndicator;

                    var line = string.Format("40|{0}|{1}|{2}|{3}||{4}|{5}|{6}|{7}|{1}|{8}||||||||",
                        context.TradingPartnerId, lineNumber, charge.ChargeIndicator, charge.ChargeCode, amount, rate,
                        charge.UOM, charge.Quantity, description);

                    lineNumber++;
                    context.AppendLine(line);
                    logger.TraceFormat("Wrote 810 \"40\" PRISM line for Header {0}", header.HeaderKey);
                }
            }
        }

        public void WriteTax(Prism810Context context, Type810Header header)
        {
            if (!header.HeaderKey.HasValue)
                return;

            var headerKey = header.HeaderKey.Value;
            var taxes = marketDataAccess.ListDetailItemTaxesByHeader(headerKey);
            if (taxes == null || taxes.Length == 0)
                return;

            foreach (var tax in taxes)
            {
                var amount = tax.TaxAmount;
                if (amount.Contains("."))
                    amount = amount.Substring(0, amount.IndexOf('.') + 3);

                if (context.IsCustomerInvoice)
                {
                    var line = string.Format("45|{0}|{1}|{2}|||||", context.TradingPartnerId, tax.TaxTypeCode, amount);
                    context.AppendLine(line);
                    logger.TraceFormat("Wrote 810 \"45\" PRISM line for Header {0}", header.HeaderKey);
                }
                else
                {
                    var line = string.Format("45|{0}|{1}|{2}|||{3}|||", context.TradingPartnerId, tax.TaxTypeCode,
                        amount, tax.RelationshipCode);
                    context.AppendLine(line);
                    logger.TraceFormat("Wrote 810 \"45\" PRISM line for Header {0}", header.HeaderKey);
                }
            }
        }

        public void WriteSummary(Prism810Context context, Type810Header header)
        {
            var amount = header.TotalAmount;
            if (amount.Contains("."))
                amount = amount.Substring(0, amount.IndexOf('.') + 3);

            var line = string.Format("60|{0}|{1}|1|", context.TradingPartnerId, amount);
            context.AppendLine(line);
            logger.TraceFormat("Wrote 810 \"60\" PRISM line for Header {0}", header.HeaderKey);
        }

        public string IdentifyEsiId(Prism810Context context, Type810Header header)
        {
            if (context.Market == MarketOptions.Maryland)
                return string.Empty;

            return header.EsiId;
        }

        public string IdentifyLdcAccountNumber(Prism810Context context, Type810Header header)
        {
            if (context.Market == MarketOptions.Maryland)
                return header.EsiId;

            return string.Empty;
        }
    }
}

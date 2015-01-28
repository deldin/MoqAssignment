using System.Text.RegularExpressions;
using System.Threading;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Parsers.ExportContexts;

namespace Ista.FileServices.MarketFiles.Parsers
{
    public class ExportCustomerInfoPrism : IMarketFileExporter
    {
        private static readonly Regex numericExp = new Regex("[^0-9]");
        private static readonly Regex alphaNumericExp = new Regex("[^0-9a-zA-Z]");
        private static readonly Regex emailMatch = new Regex(@"^(?:[a-zA-Z0-9_'^&amp;/+-])+(?:\.(?:[a-zA-Z0-9_'^&amp;/+-])+)*@(?:(?:\[?(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?))\.){3}(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\]?)|(?:[a-zA-Z0-9-]+\.)+(?:[a-zA-Z]){2,}\.?)$");

        private readonly IClientCustomerInfoExport exportDataAccess;
        private readonly ILogger logger;

        public ExportCustomerInfoPrism(IClientCustomerInfoExport exportDataAccess, ILogger logger)
        {
            this.exportDataAccess = exportDataAccess;
            this.logger = logger;
        }

        public IMarketFileExportResult[] Export(CancellationToken token)
        {
            var files = exportDataAccess.ListCustomerInfoReadyForTransmission();
            if (files.Length == 0)
            {
                logger.TraceFormat("Zero Customer Billing File records found to export.");
                return new IMarketFileExportResult[0];
            }

            logger.DebugFormat("Exporting {0} Customer Billing Files for transmission.", files.Length);
            
            var context = new PrismCustomerInfoContext();
            foreach (var file in files)
            {
                if (token.IsCancellationRequested)
                    break;

                context.PushFile(file.CspDunsId, file.FileName);
                WriteHeader(context, file);
                WriteRecord(context, file);
            }

            return context.Models;
        }

        public void WriteHeader(PrismCustomerInfoContext context, TypeCustomerInfoFile file)
        {
            var line = string.Format("HDR|MTCRCustomerInformation|{0}|{1}",
                file.FileNumber, file.CrDuns);

            context.AppendLine(line);
        }

        public void WriteRecord(PrismCustomerInfoContext context, TypeCustomerInfoFile file)
        {
            var records = exportDataAccess.ListRecords(file.FileId);
            foreach (var record in records)
            {
                var email = (emailMatch.IsMatch(record.Email))
                                ? record.Email
                                : string.Empty;

                var line =
                    string.Format(
                        "DET|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}",
                        record.SequenceId, record.CrDuns, record.PremNo, record.CustNo,
                        alphaNumericExp.Replace(record.FirstName, string.Empty),
                        alphaNumericExp.Replace(record.LastName, string.Empty),
                        alphaNumericExp.Replace(record.CompanyName, string.Empty),
                        alphaNumericExp.Replace(record.ContactName, string.Empty),
                        alphaNumericExp.Replace(record.BillingCareOfName, string.Empty),
                        alphaNumericExp.Replace(record.BillingAddress1, string.Empty),
                        alphaNumericExp.Replace(record.BillingAddress2, string.Empty),
                        alphaNumericExp.Replace(record.BillingCity, string.Empty),
                        record.BillingState,
                        alphaNumericExp.Replace(record.BillingPostalCode, string.Empty),
                        record.BillingCountryCode.Trim().Substring(0, 2),
                        numericExp.Replace(record.PrimaryTelephone, string.Empty),
                        alphaNumericExp.Replace(record.PrimaryTelephoneExt, string.Empty),
                        alphaNumericExp.Replace(record.SecondaryTelephone, string.Empty),
                        numericExp.Replace(record.SecondaryTelephoneExt, string.Empty),
                        email);

                context.AppendLine(line);
            }

            context.AppendLine(string.Format("SUM|{0}|0|0", records.Length));
        }
    }
}

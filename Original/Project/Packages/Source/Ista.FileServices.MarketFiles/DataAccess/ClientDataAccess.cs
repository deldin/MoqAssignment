using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class ClientDataAccess : IClientDataAccess
    {
        private readonly string connectionString;

        public ClientDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int? IdentifyMarket(string tdspDuns)
        {
            if (string.IsNullOrEmpty(tdspDuns))
                return null;

            var ldcModel = LoadLdcByTdspDuns(tdspDuns);
            if (ldcModel == null)
                return null;

            return ldcModel.MarketId;
        }

        public int IdentifyCspDunsId(string cspDuns)
        {
            if (string.IsNullOrWhiteSpace(cspDuns))
                throw new ArgumentNullException("cspDuns");

            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_CSPDUNSLoad"))
            {
                command.AddWithValue("@DUNS", cspDuns);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        return reader.GetInt32("CSPDUNSID");
                }
            }

            return 0;
        }

        public CspDunsModel[] ListCspDuns()
        {
            var collection = new List<CspDunsModel>();
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("cspCspDunsList"))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new CspDunsModel
                        {
                            CspDunsId = reader.GetInt32("CSPDUNSID"),
                            CspId = reader.GetInt32("CSPID"),
                            Duns = reader.GetString("DUNS"),
                            Description = reader.GetString("Description"),
                            TradingPartnerId = reader.GetString("TradingPartnerIdentifier"),
                        };

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public CspDunsPortModel[] ListCspDunsPort()
        {
            return ListCspDunsPort(string.Empty);
        }

        public CspDunsPortModel[] ListCspDunsPort(string fileType)
        {
            var dunsList = ListCspDuns();
            if (dunsList.Length == 0)
                return new CspDunsPortModel[0];

            var collection = new List<CspDunsPortModel>();
            using (var connection = new SqlConnection(connectionString))
            {
                for (int index = 0, count = dunsList.Length; index < count; index++)
                {
                    var model = dunsList[index];
                    using (var command = connection.CreateCommand("csp_CSPDUNSPortList"))
                    {
                        command.AddWithValue("@CSPDUNSID", model.CspDunsId);

                        if (connection.State != ConnectionState.Open)
                            connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new CspDunsPortModel
                                {
                                    CspDunsId = model.CspDunsId,
                                    Duns = model.Duns,
                                    CspDunsPortId = reader.GetInt32("CSPDUNSPortID"),
                                    ProviderId = reader.GetInt32("ProviderID"),
                                    TradingPartnerId = reader.GetString("TradingPartnerIdentifier"),
                                    DirectoryIn = reader.GetString("DirectoryIn"),
                                    DirectoryOut = reader.GetString("DirectoryOut"),
                                    PgpEncryptionKey = reader.GetString("PgpEncryptionKey"),
                                    PgpSignatureKey = reader.GetString("PgpSignatureKey"),
                                    PgpPassphrase = reader.GetString("PgpPassPhrase"),
                                    Protocol = reader.GetString("Protocol"),
                                    PortId = reader.GetInt32("PortID"),
                                    FtpRemoteServer = reader.GetString("RemoteServer"),
                                    FtpUserId = reader.GetString("UserId"),
                                    FtpPassword = reader.GetString("Password"),
                                };

                                reader.TryGetInt32("LDCID", x => item.LdcId = x);
                                reader.TryGetString("LDCDUNS", x => item.LdcDuns = x);
                                reader.TryGetString("LDCShortName", x => item.LdcShortName = x);
                                reader.TryGetBoolean("TransportEnabledFlag", x => item.TransportEnabledFlag = x);
                                reader.TryGetBoolean("DecryptionEnabledFlag", x => item.DecryptionEnabledFlag = x);
                                reader.TryGetBoolean("EncryptionEnabledFlag", x => item.EncryptionEnabledFlag = x);
                                reader.TryGetString("FileType", x => item.FileType = x);
                                reader.TryGetString("GISBCommonCode", x => item.GisbCommonCode = x);
                                collection.Add(item);
                            }
                        }
                    }
                }
            }

            if (collection.Count == 0)
                return new CspDunsPortModel[0];

            if (!string.IsNullOrWhiteSpace(fileType))
            {
                collection.RemoveAll(x => !string.IsNullOrEmpty(x.FileType) && !x.FileType.Equals(fileType));

                var matches = collection
                    .Where(x => !string.IsNullOrEmpty(x.FileType) && x.FileType.Equals(fileType))
                    .ToArray();

                if (matches.Any())
                {
                    foreach (var match in matches)
                        collection.RemoveAll(
                            x =>
                            x.CspDunsId.Equals(match.CspDunsId) && x.LdcId.Equals(match.LdcId) &&
                            string.IsNullOrWhiteSpace(x.FileType) && !x.CspDunsPortId.Equals(match.CspDunsPortId));
                }

                return collection.ToArray();
            }

            return collection
                .Where(x => string.IsNullOrEmpty(x.FileType))
                .ToArray();
        }

        public MeterConsumptionModel[] ListMeterConsumptionByInvoice(string invoiceNbr)
        {
            if (string.IsNullOrWhiteSpace(invoiceNbr))
                throw new ArgumentNullException("invoiceNbr");

            int invoiceId;
            if (!int.TryParse(invoiceNbr, out invoiceId))
                throw new ArgumentException("Invoice Number must be numeric.", "invoiceNbr");

            var collection = new List<MeterConsumptionModel>();

            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("cspConsumptionMeterByInvoiceID"))
            {
                command.AddWithValue("@InvoiceID", invoiceId);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new MeterConsumptionModel
                        {
                            ConsId = reader.GetInt32("ConsID"),
                            InvoiceId = reader.GetInt32("InvoiceId"),
                            MeterId = reader.GetInt32("MeterID"),
                            MeterType = reader.GetString("MeterType"),
                            RateCodeValue = reader.GetString("RateCode"),
                            MeterNumber = reader.GetString("MeterNo"),
                            UOM = reader.GetString("UOM"),
                            BegRead = reader.GetString("BegRead"),
                            EndRead = reader.GetString("EndRead"),
                            MeterFactor = reader.GetString("MeterFactor"),
                            TotalConsumption = reader.GetString("TotalConsumption"),
                        };

                        reader.TryGetDateTime("DateFrom", x => item.ServicePeriodStartDate = x.ToString("yyyyMMdd"));
                        reader.TryGetDateTime("DateTo", x => item.ServicePeriodEndDate = x.ToString("yyyyMMdd"));

                        collection.Add(item);
                    }
                    return collection.ToArray();
                }
            }
        }

        public bool ShouldExportMeterData(string tdspDuns)
        {
            bool exportMeterData = false;

            if (string.IsNullOrWhiteSpace(tdspDuns))
                exportMeterData = true;
            else
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = connection.CreateCommand("cspLDCDetailByDuns"))
                {
                    command.AddWithValue("@DUNS", tdspDuns);

                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            reader.TryGetBoolean("ExportMeterDataFor810s", x => exportMeterData = x);
                    }
                }   
            }
            return exportMeterData;
        }

        public CustomerInvoiceConfigModel LoadCustomerInvoiceConfig(string customerDuns, int ldcId)
        {
            if (string.IsNullOrWhiteSpace(customerDuns))
                throw new ArgumentNullException("customerDuns");

            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810GetCustInvConfig"))
            {
                command
                    .AddWithValue("@CustomerDUNS", customerDuns)
                    .AddWithValue("@LDCId", ldcId);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    var item = new CustomerInvoiceConfigModel
                    {
                        CustomerDuns = reader.GetString("CustomerDUNS"),
                        LdcId = reader.GetInt32("LDCID"),
                        BtCustomAddressLine = reader.GetBoolean("BTCustomAddressLine"),
                        ReAddressLine = reader.GetBoolean("REAddressLine"),
                        AggregateByChargeIndicator = reader.GetBoolean("AggregateByChargeIndicator"),
                        AggregateByChargeCode = reader.GetBoolean("AggregateByChargeCode"),
                        AggregateByUom = reader.GetBoolean("AggregateByUOM"),
                        AggregateByDescription = reader.GetBoolean("AggregateByDescription"),
                        TaxesAsCharge = reader.GetBoolean("TaxesAsCharge"),
                        CalculateChargeIndicator = reader.GetBoolean("CalculateChargeIndicator"),
                        UseAccNumberForInvoice = reader.GetBoolean("UseAccNumberForInvoice"),
                    };

                    reader.TryGetString("BTEntityName", x => item.BtEntityName = x);
                    reader.TryGetString("BTEntityId", x => item.BtEntityId = x);
                    reader.TryGetString("BTAttn", x => item.BtAttn = x);
                    reader.TryGetString("BTEntityAddress1", x => item.BtEntityAddress1 = x);
                    reader.TryGetString("BTEntityAddress2", x => item.BtEntityAddress2 = x);
                    reader.TryGetString("BTEntityCity", x => item.BtEntityCity = x);
                    reader.TryGetString("BTEntityState", x => item.BtEntityState = x);
                    reader.TryGetString("BTEntityZip", x => item.BtEntityZip = x);

                    reader.TryGetString("REEntityName", x => item.ReEntityName = x);
                    reader.TryGetString("REEntityId", x => item.ReEntityId = x);
                    reader.TryGetString("REEntityAddress1", x => item.ReEntityAddress1 = x);
                    reader.TryGetString("REEntityAddress2", x => item.ReEntityAddress2 = x);
                    reader.TryGetString("REEntityCity", x => item.ReEntityCity = x);
                    reader.TryGetString("REEntityState", x => item.ReEntityState = x);
                    reader.TryGetString("REEntityZip", x => item.ReEntityZip = x);

                    return item;
                }
            }
        }

        public CustomerPremiseModel LoadPremiseByEsiId(string esiId)
        {
            if (string.IsNullOrWhiteSpace(esiId))
                throw new ArgumentNullException("esiId");

            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("cspPremiseInfoByPremNo"))
            {
                command.AddWithValue("@PremNo", esiId);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    var item = new CustomerPremiseModel
                    {
                        CustId = reader.GetInt32("CustID"),
                    };

                    reader.TryGetString("MeterNo", x => item.MeterNo = x);
                    reader.TryGetString("CustName", x => item.CustName = x, false);
                    reader.TryGetString("Addr1", x => item.Addr1 = x, false);
                    reader.TryGetString("Addr2", x => item.Addr2 = x, false);
                    reader.TryGetString("City", x => item.City = x, false);
                    reader.TryGetString("State", x => item.State = x, false);
                    reader.TryGetString("Zip", x => item.Zip = x, false);
                    reader.TryGetString("EDI_Info1", x => item.EdiInfo1 = x, false);
                    reader.TryGetString("EDI_Info2", x => item.EdiInfo2 = x, false);

                    return item;
                }
            }
        }

        public CustomerDetailModel LoadCustomerDetail(int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentOutOfRangeException("customerId", "Customer Id must be greater than zero.");

            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("cspCustomerDetail"))
            {
                command.AddWithValue("@CustID", customerId);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    var item = new CustomerDetailModel
                    {
                        CustId = reader.GetInt32("CustID"),
                        CustNo = reader.GetString("CustNo"),
                        CustName = reader.GetString("CustName"),
                        Address1 = reader.GetString("Addr1"),
                        City = reader.GetString("City"),
                        State = reader.GetString("State"),
                        Zip = reader.GetString("Zip"),
                    };

                    reader.TryGetString("Addr2", x => item.Address2 = x);
                    reader.TryGetString("RemitAddr1", x => item.RemitAddress1 = x, false);
                    reader.TryGetString("RemitAddr2", x => item.RemitAddress2 = x, false);
                    reader.TryGetString("RemitCity", x => item.RemitCity = x, false);
                    reader.TryGetString("RemitState", x => item.RemitState = x, false);
                    reader.TryGetString("RemitZip", x => item.RemitZip = x, false);

                    return item;
                }
            }
        }

        public CustomerDetailModel LoadCustomerDetailByEsiId(string esiId)
        {
            if (string.IsNullOrWhiteSpace(esiId))
                throw new ArgumentNullException("esiId");

            var premise = LoadPremiseByEsiId(esiId);
            if (premise == null)
                return null;

            return LoadCustomerDetail(premise.CustId);
        }

        public CustomerArSummaryModel LoadArSummaryByInvoice(string invoiceNbr)
        {
            if (string.IsNullOrWhiteSpace(invoiceNbr))
                throw new ArgumentNullException("invoiceNbr");


            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("cspInvoiceAccountSummary"))
            {
                command
                    .AddWithValue("@InvoiceID", 0)
                    .AddWithValue("@TransactionNumber", invoiceNbr);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    var item = new CustomerArSummaryModel
                    {
                        PrevBal = reader.GetDecimal("PrevBal"),
                        BalDue = reader.GetDecimal("BalDue"),
                        CurrAdjs = reader.GetDecimal("CurrAdjs"),
                        CurrPmts = reader.GetDecimal("CurrPmts"),
                    };

                    return item;
                }
            }
        }

        public LdcModel LoadLdcByTdspDuns(string tdspDuns)
        {
            if (string.IsNullOrWhiteSpace(tdspDuns))
                throw new ArgumentNullException("tdspDuns");

            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("cspLDCDetailByDuns"))
            {
                command.AddWithValue("@DUNS", tdspDuns);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new LdcModel
                        {
                            LdcId = reader.GetInt32("LDCID"),
                            MarketId = reader.GetInt32("MarketID"),
                        };
                    }
                }
            }

            return null;
        }

        public LdcModel LoadLdcById(int ldcId)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp_LdcLoad"))
            {
                command.AddWithValue("@LDCID", ldcId);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new LdcModel
                        {
                            LdcId = reader.GetInt32("LDCID"),
                            MarketId = reader.GetInt32("MarketID"),
                            LdcName = reader.GetString("LdcName")
                        };
                    }
                }
            }

            return null;
        }

        public string LoadDunsByCspDunsId(int cspDunsId)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_CSPDUNSLoad"))
            {
                command.AddWithValue("@CSPDUNSID", cspDunsId);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        return reader.GetString("Duns");
                }
            }

            return string.Empty;
        }

        public string LoadParameterConfigurationValue(string parameterConfigurationName)
        {
            if (string.IsNullOrWhiteSpace(parameterConfigurationName))
                return string.Empty;

            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_ParameterConfigurationGlobalByName"))
            {
                command.AddWithValue("@ParameterConfigName", parameterConfigurationName);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    var value = string.Empty;

                    if (reader.Read())
                        reader.TryGetString("Value", x => value = x);

                    return value;
                }
            }
        }
    }
}

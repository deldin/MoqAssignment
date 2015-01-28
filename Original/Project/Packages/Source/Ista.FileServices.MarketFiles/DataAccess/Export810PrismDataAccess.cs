using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class Export810PrismDataAccess : IMarket810Export
    {
        private readonly string connectionString;

        public Export810PrismDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Type810Header[] ListUnprocessed(string ldcDuns, string duns, int providerId)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810ExportList"))
            {
                command.AddIfNotEmptyOrDbNull("@TDSPDuns", ldcDuns)
                    .AddIfNotEmptyOrDbNull("@CrDuns", duns)
                    .AddWithValue("@ProviderID", providerId);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type810Header>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type810Header
                        {
                            HeaderKey = reader.GetInt32("810_Key"),
                            Direction = reader.GetBoolean("Direction"),
                        };

                        reader.TryGetInt32("MarketFileId", x => item.MarketFileId = x);
                        reader.TryGetString("TransactionSetId", x => item.TransactionSetId = x);
                        reader.TryGetString("TransactionSetControlNbr", x => item.TransactionSetControlNbr = x);
                        reader.TryGetString("TransactionDate", x => item.TransactionDate = x);
                        reader.TryGetString("InvoiceNbr", x => item.InvoiceNbr = x);
                        reader.TryGetString("ReleaseNbr", x => item.ReleaseNbr = x);
                        reader.TryGetString("TransactionTypeCode", x => item.TransactionTypeCode = x);
                        reader.TryGetString("TransactionSetPurposeCode", x => item.TransactionSetPurposeCode = x);
                        reader.TryGetString("OriginalInvoiceNbr", x => item.OriginalInvoiceNbr = x);
                        reader.TryGetString("EsiId", x => item.EsiId = x);
                        reader.TryGetString("CRAccountNumber", x => item.CrAccountNumber = x);
                        reader.TryGetString("PaymentDueDate", x => item.PaymentDueDate = x);
                        reader.TryGetString("TdspDuns", x => item.TdspDuns = x);
                        reader.TryGetString("TdspName", x => item.TdspName = x);
                        reader.TryGetString("CrDuns", x => item.CrDuns = x);
                        reader.TryGetString("CrName", x => item.CrName = x);
                        reader.TryGetString("TotalAmount", x => item.TotalAmount = x);
                        reader.TryGetInt32("ProcessFlag", x => item.ProcessFlag = x);
                        reader.TryGetDateTime("ProcessDate", x => item.ProcessDate = x);
                        reader.TryGetInt32("TransactionTypeID", x => item.TransasctionTypeId = x);
                        reader.TryGetInt32("MarketID", x => item.MarketId = x);
                        reader.TryGetInt32("ProviderID", x => item.ProviderId = x);
                        reader.TryGetString("CustNoForESCO", x => item.CustNoForESCO = x);
                        reader.TryGetString("PreviousUtilityAccountNumber", x => item.PreviousUtilityAccountNumber = x);
                        reader.TryGetString("BillPresenter", x => item.BillPresenter = x);
                        reader.TryGetString("BillCalculator", x => item.BillCalculator = x);
                        reader.TryGetString("GasPoolId", x => item.GasPoolId = x);
                        reader.TryGetString("CustomerDUNS", x => item.CustomerDUNS = x);
                        reader.TryGetString("ChangeCode", x => item.ChangeCode = x);
                        reader.TryGetString("ChangeCodeDesc", x => item.ChangeCodeDesc = x);
                        reader.TryGetString("BillingCycleNumber", x => item.BillingCycleNumber = x);
                        reader.TryGetString("InvoicePeriodStart", x => item.InvoicePeriodStart = x);
                        reader.TryGetString("InvoicePeriodEnd", x => item.InvoicePeriodEnd = x);
                        reader.TryGetString("AlternateEsiId", x => item.AlternateEsiId = x);
                        reader.TryGetString("ServiceDeliveryPoint", x => item.ServiceDeliveryPoint = x);
                        
                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type810Detail[] ListDetails(int headerKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810ExportDetail"))
            {
                command.AddWithValue("@810Key", headerKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type810Detail>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type810Detail
                        {
                            HeaderKey = headerKey,
                            DetailKey = reader.GetInt32("Detail_Key"),
                        };

                        reader.TryGetString("AssignedId", x => item.AssignedId = x);
                        reader.TryGetString("ServiceTypeCode", x => item.ServiceTypeCode = x);
                        reader.TryGetString("ServiceType", x => item.ServiceType = x);
                        reader.TryGetString("ServiceClassCode", x => item.ServiceClassCode = x);
                        reader.TryGetString("ServiceClass", x => item.ServiceClass = x);
                        reader.TryGetString("RateClass", x => item.RateClass = x);
                        reader.TryGetString("RateSubClass", x => item.RateSubClass = x);
                        reader.TryGetString("ServicePeriodStartDate", x => item.ServicePeriodStartDate = x);
                        reader.TryGetString("ServicePeriodEndDate", x => item.ServicePeriodEndDate = x);
                        reader.TryGetString("SupplierContractId", x => item.SupplierContractId = x);
                        reader.TryGetString("UtilityContractId", x => item.UtilityContractId = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type810DetailItemCharge[] ListDetailItemChargesByHeader(int headerKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810ExportChargeList"))
            {
                command.AddWithValue("@810Key", headerKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type810DetailItemCharge>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type810DetailItemCharge
                        {
                            ChargeKey = reader.GetInt32("Charge_Key"),
                        };

                        reader.TryGetString("ChargeIndicator", x => item.ChargeIndicator = x);
                        reader.TryGetString("ChargeCode", x => item.ChargeCode = x);
                        reader.TryGetString("Amount", x => item.Amount = x);
                        reader.TryGetString("Rate", x => item.Rate = x);
                        reader.TryGetString("UOM", x => item.UOM = x);
                        reader.TryGetString("Quantity", x => item.Quantity = x);
                        reader.TryGetString("Descripton", x => item.Description = x);
                        
                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type810DetailItemTax[] ListDetailItemTaxesByHeader(int headerKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810ExportTaxList"))
            {
                command.AddWithValue("@810Key", headerKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type810DetailItemTax>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type810DetailItemTax
                        {
                            TaxKey = reader.GetInt32("Tax_Key"),
                        };

                        reader.TryGetString("ChargeIndicator", x => item.TaxTypeCode = x);
                        reader.TryGetString("ChargeCode", x => item.TaxAmount = x);
                        
                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type810Name LoadFirstName(int headerKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810ExportName"))
            {
                command.AddWithValue("@HeaderKey", headerKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    var item = new Type810Name
                    {
                        HeaderKey = headerKey,
                        NameKey = reader.GetInt32("Name_Key"),
                    };

                    reader.TryGetString("EntityIdType", x => item.EntityIdType = x);
                    reader.TryGetString("EntityName", x => item.EntityName = x);
                    reader.TryGetString("EntityDuns", x => item.EntityDuns = x);
                    reader.TryGetString("EntityIdCode", x => item.EntityIdCode = x);
                    reader.TryGetString("Address1", x => item.Address1 = x);
                    reader.TryGetString("Address2", x => item.Address2 = x);
                    reader.TryGetString("City", x => item.City = x);
                    reader.TryGetString("State", x => item.State = x);
                    reader.TryGetString("PostalCode", x => item.PostalCode = x);
                    reader.TryGetString("EntityName2", x => item.EntityName2 = x);
                    reader.TryGetString("EntityName3", x => item.EntityName3 = x);

                    return item;
                }
            }
        }

        public void UpdateHeader(int headerKey, int marketFileId, string fileName)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810ExportHeaderUpdate"))
            {
                command.AddWithValue("@810Key", headerKey)
                    .AddWithValue("@FileName", fileName);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public Type810Balance[] ListBalances(int headerKey)
        {
            return new Type810Balance[0];
        }

        public Type810DetailItem[] ListDetailItems(int detailKey)
        {
            return new Type810DetailItem[0];
        }

        public Type810DetailItemCharge[] ListDetailItemCharges(int detailItemKey)
        {
            return new Type810DetailItemCharge[0];
        }

        public Type810DetailItemTax[] ListDetailItemTaxes(int detailItemKey)
        {
            return new Type810DetailItemTax[0];
        }

        public Type810DetailTax[] ListDetailTaxes(int detailKey)
        {
            return new Type810DetailTax[0];
        }

        public Type810Message[] ListMessages(int headerKey)
        {
            return new Type810Message[0];
        }

        public Type810Name[] ListNames(int headerKey)
        {
            return new Type810Name[0];
        }

        public Type810Summary[] ListSummaries(int headerKey)
        {
            return new Type810Summary[0];
        }
    }
}

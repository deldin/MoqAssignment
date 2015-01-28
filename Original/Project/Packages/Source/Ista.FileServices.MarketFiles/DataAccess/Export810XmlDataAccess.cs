using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class Export810XmlDataAccess : IMarket810Export
    {
        private readonly string connectionString;

        public Export810XmlDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Type810Header[] ListUnprocessed(string ldcDuns, string duns, int providerId)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_810ExportList"))
            {
                command.AddIfNotEmptyOrDbNull("@CrDuns", duns)
                    .AddIfNotEmptyOrDbNull("@TDSPDuns", ldcDuns);

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
                        reader.TryGetString("AlternateEsiId", x => item.AlternateEsiId = x);
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
            using (var command = connection.CreateCommand("esp_810DetailList"))
            {
                command.AddWithValue("@HeaderKey", headerKey);

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
                        reader.TryGetString("MeterNumber", x => item.MeterNumber = x);
                        reader.TryGetString("BillCycle", x => item.BillCycle = x);
                        reader.TryGetString("GasPoolId", x => item.GasPoolId = x);
                        reader.TryGetString("ServiceAgreement", x => item.ServiceAgreement = x);
                        reader.TryGetString("ServiceTypeCode2", x => item.ServiceTypeCode2 = x);
                        reader.TryGetString("ServiceType2", x => item.ServiceType2 = x);
                        reader.TryGetString("ServiceTypeCode3", x => item.ServiceTypeCode3 = x);
                        reader.TryGetString("ServiceType3", x => item.ServiceType3 = x);
                        reader.TryGetString("ServiceDeliveryPoint", x => item.ServiceDeliveryPoint = x);
                        reader.TryGetString("OldAccountNumber", x => item.OldAccountNumber = x);
                        reader.TryGetString("RateCode", x => item.RateCode = x);
                        reader.TryGetString("OldMeterNumber", x => item.OldMeterNumber = x);
                        reader.TryGetString("TransferredDate", x => item.TransferredDate = x);
                        reader.TryGetString("InvoicePeriodStartDate", x => item.InvoicePeriodStartDate = x);
                        reader.TryGetString("InvoicePeriodEndDate", x => item.InvoicePeriodEndDate = x);
                        reader.TryGetString("SupplierContractId", x => item.SupplierContractId = x);
                        reader.TryGetString("UtilityContractId", x => item.UtilityContractId = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type810DetailItem[] ListDetailItems(int detailKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_810DetailItemList"))
            {
                command.AddWithValue("@DetailKey", detailKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type810DetailItem>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type810DetailItem
                        {
                            DetailKey = detailKey,
                            ItemKey = reader.GetInt32("Item_Key"),
                        };

                        reader.TryGetString("AssignedId", x => item.AssignedId = x);
                        reader.TryGetString("RelationshipCode", x => item.RelationshipCode = x);
                        reader.TryGetString("ServiceOrderCompleteDate", x => item.ServiceOrderCompleteDate = x);
                        reader.TryGetString("UnmeteredServiceDateRange", x => item.UnmeteredServiceDateRange = x);
                        reader.TryGetString("InvoiceNbr", x => item.InvoiceNbr = x);
                        reader.TryGetString("ServiceOrderNbr", x => item.ServiceOrderNbr = x);
                        reader.TryGetString("Consumption", x => item.Consumption = x);
                        reader.TryGetString("EffectiveDate", x => item.EffectiveDate = x);
                        reader.TryGetString("SequenceId", x => item.SequenceId = x);
                        
                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type810DetailItemCharge[] ListDetailItemCharges(int detailItemKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_810DetailItemChargeList"))
            {
                command.AddWithValue("@ItemKey", detailItemKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type810DetailItemCharge>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type810DetailItemCharge
                        {
                            ItemKey = detailItemKey,
                            ChargeKey = reader.GetInt32("Charge_Key"),
                        };

                        reader.TryGetString("ChargeIndicator", x => item.ChargeIndicator = x);
                        reader.TryGetString("AgencyCode", x => item.AgencyCode = x);
                        reader.TryGetString("ChargeCode", x => item.ChargeCode = x);
                        reader.TryGetString("Amount", x => item.Amount = x);
                        reader.TryGetString("Rate", x => item.Rate = x);
                        reader.TryGetString("UOM", x => item.UOM = x);
                        reader.TryGetString("Quantity", x => item.Quantity = x);
                        reader.TryGetString("Description", x => item.Description = x);
                        reader.TryGetString("PrintSeqId", x => item.PrintSeqId = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type810DetailItemTax[] ListDetailItemTaxes(int detailItemKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_810DetailItemTaxList"))
            {
                command.AddWithValue("@ItemKey", detailItemKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type810DetailItemTax>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type810DetailItemTax
                        {
                            ItemKey = detailItemKey,
                            TaxKey = reader.GetInt32("Tax_Key"),
                        };

                        reader.TryGetString("TaxTypeCode", x => item.TaxTypeCode = x);
                        reader.TryGetString("TaxAmount", x => item.TaxAmount = x);
                        reader.TryGetString("RelationshipCode", x => item.RelationshipCode = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type810DetailTax[] ListDetailTaxes(int detailKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp810ExportDetailTax"))
            {
                command.AddWithValue("@DetailKey", detailKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type810DetailTax>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type810DetailTax
                        {
                            DetailKey = detailKey,
                            TaxKey = reader.GetInt32("Tax_Key"),
                        };

                        reader.TryGetString("AssignedId", x => item.AssignedId = x);
                        reader.TryGetString("MonetaryAmount", x => item.MonetaryAmount = x);
                        reader.TryGetString("Percent", x => item.Percent = x);
                        reader.TryGetString("RelationshipCode", x => item.RelationshipCode = x);
                        reader.TryGetString("DollarBasis", x => item.DollarBasis = x);
                        reader.TryGetString("TaxTypeCode", x => item.TaxTypeCode = x);
                        reader.TryGetString("JurisdictionCode", x => item.JurisdictionCode = x);
                        reader.TryGetString("JurisdictionCodeQualifier", x => item.JurisdictionCodeQualifier = x);
                        reader.TryGetString("ExemptCode", x => item.ExemptCode = x);
                        reader.TryGetString("Description", x => item.Description = x);
                        
                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type810Name[] ListNames(int headerKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_810NameList"))
            {
                command.AddWithValue("@HeaderKey", headerKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type810Name>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type810Name
                        {
                            HeaderKey = headerKey,
                            NameKey = reader.GetInt32("Name_Key"),
                        };

                        reader.TryGetString("EntityIdType", x => item.EntityIdType = x);
                        reader.TryGetString("EntityName", x => item.EntityName = x);
                        reader.TryGetString("EntityDuns", x => item.EntityDuns = x);
                        reader.TryGetString("EntityIdCode", x => item.EntityIdCode = x);
                        reader.TryGetString("EntityName2", x => item.EntityName2 = x);
                        reader.TryGetString("EntityName3", x => item.EntityName3 = x);
                        reader.TryGetString("Address1", x => item.Address1 = x);
                        reader.TryGetString("Address2", x => item.Address2 = x);
                        reader.TryGetString("City", x => item.City = x);
                        reader.TryGetString("State", x => item.State = x);
                        reader.TryGetString("PostalCode", x => item.PostalCode = x);
                        
                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type810Balance[] ListBalances(int headerKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_810BalanceList"))
            {
                command.AddWithValue("@HeaderKey", headerKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type810Balance>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type810Balance
                        {
                            HeaderKey = headerKey,
                            BalanceKey = reader.GetInt32("Balance_Key"),
                        };

                        reader.TryGetString("TotalOutstandingBalance", x => item.TotalOutstandingBalance = x);
                        reader.TryGetString("BeginningBalance", x => item.BeginningBalance = x);
                        reader.TryGetString("BudgetCumulativeDifference", x => item.BudgetCumulativeDifference = x);
                        reader.TryGetString("BudgetMonthDifference", x => item.BudgetMonthDifference = x);
                        reader.TryGetString("BudgetBilledToDate", x => item.BudgetBilledToDate = x);
                        reader.TryGetString("ActualBilledToDate", x => item.ActualBilledToDate = x);
                        
                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type810Summary[] ListSummaries(int headerKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_810SummaryList"))
            {
                command.AddWithValue("@HeaderKey", headerKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type810Summary>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type810Summary
                        {
                            HeaderKey = headerKey,
                            SummaryKey = reader.GetInt32("Summary_Key"),
                        };

                        reader.TryGetString("TotalAmount", x => item.TotalAmount = x);
                        reader.TryGetString("TotalLineItems", x => item.TotalLineItems = x);
                        reader.TryGetString("TotalSegments", x => item.TotalSegments = x);
                        reader.TryGetString("TransactionSetControlNbr", x => item.TransactionSetControlNbr = x);
                        
                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public Type810Message[] ListMessages(int headerKey)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_810MessageList"))
            {
                command.AddWithValue("@HeaderKey", headerKey);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<Type810Message>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Type810Message
                        {
                            HeaderKey = headerKey,
                            MessageKey = reader.GetInt32("Message_Key"),
                        };

                        reader.TryGetString("ItemDescType", x => item.ItemDescType = x);
                        reader.TryGetString("ProductCode", x => item.ProductCode = x);
                        reader.TryGetString("Description", x => item.Description = x);
                        reader.TryGetString("PositionCode", x => item.PositionCode = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public void UpdateHeader(int headerKey, int marketFileId, string fileName)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_810HeaderStatusUpdate"))
            {
                command.AddWithValue("@HeaderKey", headerKey)
                    .AddWithValue("@Status", 1)
                    .AddWithValue("@MarketFileID", marketFileId);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public Type810DetailItemCharge[] ListDetailItemChargesByHeader(int headerKey)
        {
            return new Type810DetailItemCharge[0];
        }

        public Type810DetailItemTax[] ListDetailItemTaxesByHeader(int headerKey)
        {
            return new Type810DetailItemTax[0];
        }

        public Type810Name LoadFirstName(int headerKey)
        {
            return null;
        }
    }
}
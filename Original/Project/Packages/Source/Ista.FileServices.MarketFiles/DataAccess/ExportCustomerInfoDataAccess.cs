using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class ExportCustomerInfoDataAccess : IClientCustomerInfoExport
    {
        private readonly string connectionString;

        public ExportCustomerInfoDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public TypeCustomerInfoFile[] ListCustomerInfoReadyForTransmission()
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_CustomerBillingFileListByStatus"))
            {
                // Status Id 3 => Ready For Transmission
                command.AddWithValue("@StatusID", 3);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<TypeCustomerInfoFile>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new TypeCustomerInfoFile
                        {
                            Direction = false,
                            CrDuns = reader.GetString("CRDuns"),
                            CspDunsId = reader.GetInt32("CSPDUNSID"),
                            FileId = reader.GetInt32("CustomerBillingFileID"),
                            FileTypeId = reader.GetInt32("TypeID"),
                            Status = (CustomerInfoFileStatusOptions)reader.GetInt32("StatusID"),
                            ErrorCount = reader.GetInt32("ErrorCount"),
                            RecordCount = reader.GetInt32("RecordCount"),
                        };

                        reader.TryGetString("CustomerBillingFileNumber", x => item.FileNumber = x);
                        reader.TryGetString("CustomerBillingFileReferenceNumber", x => item.ReferenceNumber = x);
                        reader.TryGetString("FileName", x => item.FileName = x);
                        reader.TryGetDateTime("StatusDate", x => item.StatusDate = x);
                        reader.TryGetDateTime("CreateDate", x => item.CreateDate = x);
                        reader.TryGetInt32("UserID", x => item.UserId = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public TypeCustomerInfoRecord[] ListRecords(int fileId)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_CustomerBillingFileRecordList"))
            {
                command.AddWithValue("@CustomerBillingFileID", fileId);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var collection = new List<TypeCustomerInfoRecord>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new TypeCustomerInfoRecord
                        {
                            FileId = fileId,
                            RecordId = reader.GetInt32("CustomerBillingFileRecordID"),
                            SequenceId = reader.GetInt32("SequenceID"),
                            PremId = reader.GetInt32("PremID"),
                            CrDuns = reader.GetString("CRDuns"),
                            CompanyName = reader.GetString("CompanyName"),
                            BillingCountryCode = reader.GetString("BillingCountryCode"),
                            PrimaryTelephone = reader.GetString("PrimaryTelephone"),
                            PrimaryTelephoneExt = reader.GetString("PrimaryTelephoneExt"),
                            SecondaryTelephoneExt = reader.GetString("SecondaryTelephoneExt"),
                        };

                        reader.TryGetString("PremNo", x => item.PremNo = x);
                        reader.TryGetString("CustNo", x => item.CustNo = x);
                        reader.TryGetString("LastName", x => item.LastName = x);
                        reader.TryGetString("FirstName", x => item.FirstName = x);

                        // dba field from customer
                        reader.TryGetString("ContactName", x => item.ContactName = x);

                        // mail address information
                        // from address table
                        reader.TryGetString("BillingCareOfName", x => item.BillingCareOfName = x);
                        reader.TryGetString("BillingAddress1", x => item.BillingAddress1 = x);
                        reader.TryGetString("BillingAddress2", x => item.BillingAddress2 = x);
                        reader.TryGetString("BillingCity", x => item.BillingCity = x);
                        reader.TryGetString("BillingState", x => item.BillingState = x);
                        reader.TryGetString("BillingPostalCode", x => item.BillingPostalCode = x);
                        reader.TryGetString("SecondaryTelephone", x => item.SecondaryTelephone = x);
                        reader.TryGetString("Email", x => item.Email = x);

                        collection.Add(item);
                    }

                    return collection.ToArray();
                }
            }
        }

        public TypeCustomerInfoFile LoadCustomerInfoFile(int fileId)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_CustomerBillingFileLoad"))
            {
                command.AddWithValue("@CustomerBillingFileID", fileId);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    var item = new TypeCustomerInfoFile
                    {
                        Direction = false,
                        CrDuns = reader.GetString("CRDuns"),
                        CspDunsId = reader.GetInt32("CSPDUNSID"),
                        FileId = reader.GetInt32("CustomerBillingFileID"),
                        FileTypeId = reader.GetInt32("TypeID"),
                        Status = (CustomerInfoFileStatusOptions)reader.GetInt32("StatusID"),
                        ErrorCount = reader.GetInt32("ErrorCount"),
                        RecordCount = reader.GetInt32("RecordCount"),
                    };

                    reader.TryGetString("CustomerBillingFileNumber", x => item.FileNumber = x);
                    reader.TryGetString("CustomerBillingFileReferenceNumber", x => item.ReferenceNumber = x);
                    reader.TryGetString("FileName", x => item.FileName = x);
                    reader.TryGetDateTime("StatusDate", x => item.StatusDate = x);
                    reader.TryGetDateTime("CreateDate", x => item.CreateDate = x);
                    reader.TryGetInt32("UserID", x => item.UserId = x);

                    return item;
                }
            }
        }

        public void UpdateCustomerInfoFile(int fileId, CustomerInfoFileStatusOptions status)
        {
            var model = LoadCustomerInfoFile(fileId);
            if (model == null)
                return;

            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("esp_CustomerBillingFileUpdate"))
            {
                command.AddWithValue("@CustomerBillingFileID", fileId)
                    .AddWithValue("@FileName", model.FileName)
                    .AddIfNotEmptyOrDbNull("@CustomerBillingFileNumber", model.FileNumber)
                    .AddIfNotEmptyOrDbNull("@CustomerBillingFileReferenceNumber", model.ReferenceNumber)
                    .AddWithValue("@CSPDUNSID", model.CspDunsId)
                    .AddWithValue("@StatusID", (int)status)
                    .AddWithValue("@TypeID", model.FileTypeId)
                    .AddWithValue("@DirectionFlag", model.Direction)
                    .AddWithValueOrDbNull("@CreateDate", model.CreateDate)
                    .AddWithValue("@StatusDate", DateTime.Now)
                    .AddWithValueOrDbNull("@UserID", model.UserId);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }
    }
}
using System;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class Import650XmlDataAccess : IMarket650Import
    {
        private readonly string connectionString;

        public Import650XmlDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int InsertHeader(Type650Header model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp650HeaderInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@MarketFileId", model.MarketFileId)
                    .AddIfNotEmptyOrDbNull("@TransactionSetPurposeCode", model.TransactionSetPurposeCode)
                    .AddIfNotEmptyOrDbNull("@TransactionDate", model.TransactionDate)
                    .AddIfNotEmptyOrDbNull("@TransactionNbr", model.TransactionNbr)
                    .AddIfNotEmptyOrDbNull("@ReferenceNbr", model.ReferenceNbr)
                    .AddIfNotEmptyOrDbNull("@TransactionType", model.TransactionType)
                    .AddIfNotEmptyOrDbNull("@ActionCode", model.ActionCode)
                    .AddIfNotEmptyOrDbNull("@TdspName", model.TdspName)
                    .AddIfNotEmptyOrDbNull("@TdspDuns", model.TdspDuns)
                    .AddWithValue("@CrName", model.CrName)
                    .AddWithValue("@CrDuns", model.CrDuns)
                    .AddIfNotEmptyOrDbNull("@ProcessedReceivedDateTime", model.ProcessedReceivedDateTime)
                    .AddWithValue("@Direction", true)
                    .AddWithValue("@MarketID", model.MarketId)
                    .AddWithValue("@ProviderID", model.ProviderId)
                    .AddWithValue("@TransactionTypeID", model.TransactionTypeId)
                    .AddOutParameter("@Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var headerKey = (int)keyParameter.Value;
                model.HeaderKey = headerKey;

                return headerKey;
            }
        }

        public int InsertName(Type650Name model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp650NameInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@650_Key", model.HeaderKey)
                    .AddIfNotEmptyOrDbNull("@EntityIdType", model.EntityIdType)
                    .AddIfNotEmptyOrDbNull("@EntityName", model.EntityName)
                    .AddIfNotEmptyOrDbNull("@EntityName2", model.EntityName2)
                    .AddIfNotEmptyOrDbNull("@EntityName3", model.EntityName3)
                    .AddIfNotEmptyOrDbNull("@EntityDuns", model.EntityDuns)
                    .AddIfNotEmptyOrDbNull("@EntityIdCode", model.EntityIdCode)
                    .AddIfNotEmptyOrDbNull("@Address1", model.Address1)
                    .AddIfNotEmptyOrDbNull("@Address2", model.Address2)
                    .AddIfNotEmptyOrDbNull("@City", model.City)
                    .AddIfNotEmptyOrDbNull("@State", model.State)
                    .AddIfNotEmptyOrDbNull("@PostalCode", model.PostalCode)
                    .AddIfNotEmptyOrDbNull("@CountryCode", model.CountryCode)
                    .AddIfNotEmptyOrDbNull("@ContactName", model.ContactName)
                    .AddIfNotEmptyOrDbNull("@ContactPhoneNbr1", model.ContactPhoneNbr1)
                    .AddIfNotEmptyOrDbNull("@ContactPhoneNbr2", model.ContactPhoneNbr2)
                    .AddOutParameter("@Name_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var nameKey = (int)keyParameter.Value;
                model.NameKey = nameKey;

                return nameKey;
            }
        }

        public int InsertService(Type650Service model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp650ServiceInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@650_Key", model.HeaderKey)
                    .AddIfNotEmptyOrDbNull("@PurposeCode", model.PurposeCode)
                    .AddIfNotEmptyOrDbNull("@PriorityCode", model.PriorityCode)
                    .AddIfNotEmptyOrDbNull("@ESIID", model.EsiId)
                    .AddIfNotEmptyOrDbNull("@SpecialProcessCode", model.SpecialProcessCode)
                    .AddIfNotEmptyOrDbNull("@ServiceReqDate", model.ServiceReqDate)
                    .AddIfNotEmptyOrDbNull("@NotBeforeDate", model.NotBeforeDate)
                    .AddIfNotEmptyOrDbNull("@CallAhead", model.CallAhead)
                    .AddIfNotEmptyOrDbNull("@PremLocation", model.PremLocation)
                    .AddIfNotEmptyOrDbNull("@AccStatusCode", model.AccStatusCode)
                    .AddIfNotEmptyOrDbNull("@AccStatusDesc", model.AccStatusDesc)
                    .AddIfNotEmptyOrDbNull("@EquipLocation", model.EquipLocation)
                    .AddIfNotEmptyOrDbNull("@ServiceOrderNbr", model.ServiceOrderNbr)
                    .AddIfNotEmptyOrDbNull("@CompletionDate", model.CompletionDate)
                    .AddIfNotEmptyOrDbNull("@CompletionTime", model.CompletionTime)
                    .AddIfNotEmptyOrDbNull("@ReportRemarks", model.ReportRemarks)
                    .AddIfNotEmptyOrDbNull("@Directions", model.Directions)
                    .AddIfNotEmptyOrDbNull("@MeterNbr", model.MeterNbr)
                    .AddIfNotEmptyOrDbNull("@MeterReadDate", model.MeterReadDate)
                    .AddIfNotEmptyOrDbNull("@MeterTestDate", model.MeterTestDate)
                    .AddIfNotEmptyOrDbNull("@MeterTestResults", model.MeterTestResults)
                    .AddIfNotEmptyOrDbNull("@IncidentCode", model.IncidentCode)
                    .AddIfNotEmptyOrDbNull("@EstRestoreDate", model.EstRestoreDate)
                    .AddIfNotEmptyOrDbNull("@EstRestoreTime", model.EstRestoreTime)
                    .AddIfNotEmptyOrDbNull("@IntStartDate", model.IntStartDate)
                    .AddIfNotEmptyOrDbNull("@IntStartTime", model.IntStartTime)
                    .AddIfNotEmptyOrDbNull("@RepairRecommended", model.RepairRecommended)
                    .AddIfNotEmptyOrDbNull("@Rescheduled", model.Rescheduled)
                    .AddIfNotEmptyOrDbNull("@InterDurationPeriod", model.InterDurationPeriod)
                    .AddIfNotEmptyOrDbNull("@AreaOutage", model.AreaOutage)
                    .AddIfNotEmptyOrDbNull("@CustRepairRemarks", model.CustRepairRemarks)
                    .AddIfNotEmptyOrDbNull("@MeterReadUOM", model.MeterReadUom)
                    .AddIfNotEmptyOrDbNull("@MeterRead", model.MeterRead)
                    .AddIfNotEmptyOrDbNull("@MeterReadCode", model.MeterReadCode)
                    .AddWithValue("@RemarksPermanentSuspend", model.RemarksPermanentSuspend)
                    .AddWithValue("@Membership", model.Membership)
                    .AddWithValue("@DisconnectAuthorization", model.DisconnectAuthorization)
                    .AddOutParameter("@Service_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var serviceKey = (int)keyParameter.Value;
                model.ServiceKey = serviceKey;

                return serviceKey;
            }
        }

        public int InsertServiceChange(Type650ServiceChange model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp650ServiceChangeInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@Service_Key", model.ServiceKey)
                    .AddIfNotEmptyOrDbNull("@ChangeReason", model.ChangeReason)
                    .AddOutParameter("@ServiceChange_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var changeKey = (int)keyParameter.Value;
                model.ServiceChangeKey = changeKey;

                return changeKey;
            }
        }

        public int InsertServiceMeter(Type650ServiceMeter model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp650ServiceMeterInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@Service_Key", model.ServiceKey)
                    .AddIfNotEmptyOrDbNull("@MeterNumber", model.MeterNumber)
                    .AddOutParameter("@ServiceMeter_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var meterKey = (int)keyParameter.Value;
                model.ServiceMeterKey = meterKey;

                return meterKey;
            }
        }

        public int InsertServicePole(Type650ServicePole model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp650ServicePoleInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@Service_Key", model.ServiceKey)
                    .AddIfNotEmptyOrDbNull("@PoleNbr", model.PoleNbr)
                    .AddOutParameter("@Pole_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var poleKey = (int)keyParameter.Value;
                model.ServicePoleKey = poleKey;

                return poleKey;
            }
        }

        public int InsertServiceReject(Type650ServiceReject model)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand("csp650ServiceRejectInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@Service_Key", model.ServiceKey)
                    .AddIfNotEmptyOrDbNull("@RejectCode", model.RejectCode)
                    .AddIfNotEmptyOrDbNull("@RejectReason", model.RejectReason)
                    .AddIfNotEmptyOrDbNull("@UnexCode", model.UnexCode)
                    .AddIfNotEmptyOrDbNull("@UnexReason", model.UnexReason)
                    .AddOutParameter("@ServiceReject_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var rejectKey = (int)keyParameter.Value;
                model.ServiceRejectKey = rejectKey;

                return rejectKey;
            }
        }
    }
}
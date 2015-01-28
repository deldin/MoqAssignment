using System;
using System.Data;
using System.Data.SqlClient;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;

namespace Ista.FileServices.MarketFiles.DataAccess
{
    public class Import867XmlDataAccess : IMarket867Import
    {
        private readonly string _connectionString;

        public Import867XmlDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        public int InsertHeader(Type867Header model)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand("csp867HeaderInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@MarketFileId", model.MarketFileId)
                    .AddWithValue("@TransactionSetId", model.TransactionSetId)
                    .AddWithValue("@TransactionSetControlNbr", model.TransactionSetControlNbr)
                    .AddIfNotEmptyOrDbNull("@TransactionSetPurposeCode", model.TransactionSetPurposeCode)
                    .AddIfNotEmptyOrDbNull("@TransactionNbr", model.TransactionNbr)
                    .AddIfNotEmptyOrDbNull("@TransactionDate", model.TransactionDate)
                    .AddIfNotEmptyOrDbNull("@ReportTypeCode", model.ReportTypeCode)
                    .AddIfNotEmptyOrDbNull("@ActionCode", model.ActionCode)
                    .AddIfNotEmptyOrDbNull("@ReferenceNbr", model.ReferenceNbr)
                    .AddIfNotEmptyOrDbNull("@DocumentDueDate", model.DocumentDueDate)
                    .AddIfNotEmptyOrDbNull("@EsiId", model.EsiId)
                    .AddIfNotEmptyOrDbNull("@PowerRegion", model.PowerRegion)
                    .AddIfNotEmptyOrDbNull("@OriginalTransactionNbr", model.OriginalTransactionNbr)
                    .AddIfNotEmptyOrDbNull("@TdspDuns", model.TdspDuns)
                    .AddIfNotEmptyOrDbNull("@TdspName", model.TdspName)
                    .AddWithValue("@CrDuns", model.CrDuns)
                    .AddWithValue("@CrName", model.CrName)
                    .AddWithValue("@Direction", model.DirectionFlag)
                    .AddWithValue("@UtilityAccountNumber", model.UtilityAccountNumber)
                    .AddWithValue("@PreviousUtilityAccountNumber", model.PreviousUtilityAccountNumber)
                    .AddWithValue("@TransactionTypeID", model.TransactionTypeID)
                    .AddWithValue("@MarketID", model.MarketID)
                    .AddWithValue("@ProviderID", model.ProviderID)
                    .AddWithValue("@EstimationReason", model.EstimationReason)
                    .AddWithValue("@EstimationDescription", model.EstimationDescription)
                    .AddWithValue("@DoorHangerFlag", model.DoorHangerFlag)
                    .AddWithValue("@ESNCount", model.EsnCount)
                    .AddWithValue("@QOCount", model.QoCount)
                    .AddWithValue("@NextMeterReadDate", model.NextMeterReadDate)
                    .AddWithValue("@InvoiceNbr", model.InvoiceNbr)
                    .AddWithValue("@UtilityContractID", model.UtilityContractID)
                    .AddWithValue("@ContainsConsumption",1)//CISRFC-783                     
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

        public int InsertAccountBillQuanity(Type867AccountBillQty model)
        {
            return -1;
        }

        public int InsertNonIntervalSummary(Type867NonIntervalSummary model)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand("csp867NonIntervalSummaryInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@867_Key", model.HeaderKey)
                    .AddIfNotEmptyOrDbNull("@TypeCode", model.TypeCode)
                    .AddIfNotEmptyOrDbNull("@MeterUOM", model.MeterUOM)
                    .AddIfNotEmptyOrDbNull("@MeterInterval", model.MeterInterval)
                    .AddWithValue("@CommodityCode", model.CommodityCode)
                    .AddWithValue("@NumberOfDials", model.NumberOfDials)
                    .AddWithValue("@ServicePointId", model.ServicePointId)
                    .AddWithValue("@UtilityRateServiceClass", model.UtilityRateServiceClass)
                    .AddWithValue("@RateSubClass", model.RateSubClass)
                    .AddOutParameter("@NonIntervalSummary_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var key = (int)keyParameter.Value;
                model.NonIntervalSummaryKey = key;

                return key;
            }
        }

        public int InsertNetIntervalSummary(Type867NetIntervalSummary model)
        {
            return -1;
        }

        public int InsertIntervalSummary(Type867IntervalSummary model)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand("csp867IntervalSummaryInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@867_Key", model.HeaderKey)
                    .AddIfNotEmptyOrDbNull("@TypeCode", model.TypeCode)
                    .AddIfNotEmptyOrDbNull("@MeterNumber", model.MeterNumber)
                    .AddIfNotEmptyOrDbNull("@MovementTypeCode", model.MovementTypeCode)
                    .AddIfNotEmptyOrDbNull("@ServicePeriodStart", model.ServicePeriodStart)
                    .AddIfNotEmptyOrDbNull("@ServicePeriodEnd", model.ServicePeriodEnd)
                    .AddIfNotEmptyOrDbNull("@ExchangeDate", model.ExchangeDate)
                    .AddIfNotEmptyOrDbNull("@ChannelNumber", model.ChannelNumber)
                    .AddIfNotEmptyOrDbNull("@MeterRole", model.MeterRole)
                    .AddIfNotEmptyOrDbNull("@MeterUOM", model.MeterUOM)
                    .AddIfNotEmptyOrDbNull("@MeterInterval", model.MeterInterval)
                    .AddWithValue("@CommodityCode", model.CommodityCode)
                    .AddWithValue("@NumberOfDials", model.NumberOfDials)
                    .AddWithValue("@ServicePointId", model.ServicePointId)
                    .AddWithValue("@UtilityRateServiceClass", model.UtilityRateServiceClass)
                    .AddWithValue("@RateSubClass", model.RateSubClass)
                    .AddOutParameter("@IntervalSummary_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var key = (int)keyParameter.Value;
                model.IntervalSummaryKey = key;

                return key;
            }
        }

        public int InsertUnMeterDetail(Type867UnMeterDetail model)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand("csp867UnmeterDetailInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@867_Key", model.HeaderKey)
                    .AddIfNotEmptyOrDbNull("@TypeCode", model.TypeCode)
                    .AddIfNotEmptyOrDbNull("@ServicePeriodStart", model.ServicePeriodStart)
                    .AddIfNotEmptyOrDbNull("@ServicePeriodEnd", model.ServicePeriodEnd)
                    .AddIfNotEmptyOrDbNull("@ServiceType", model.ServiceType)
                    .AddIfNotEmptyOrDbNull("@Description", model.Description)
                    .AddWithValue("@CommodityCode", model.CommodityCode)
                    .AddWithValue("@UtilityRateServiceClass", model.UtilityRateServiceClass)
                    .AddWithValue("@RateSubClass", model.RateSubClass)
                    .AddOutParameter("@UnmeterDetail_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var key = (int)keyParameter.Value;
                model.UnMeterDetailKey = key;

                return key;
            }
        }

        public void InsertNonIntervalSummaryQty(Type867NonIntervalSummaryQty model)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand("csp867NonIntervalSummaryQtyInsert"))
            {
                command.AddWithValue("@NonIntervalSummary_Key", model.NonIntervalSummaryKey)
                    .AddIfNotEmptyOrDbNull("@Qualifier", model.Qualifier)
                    .AddIfNotEmptyOrDbNull("@Quantity", model.Quantity)
                    .AddIfNotEmptyOrDbNull("@MeasurementSignificanceCode", model.MeasurementSignificanceCode)
                    .AddIfNotEmptyOrDbNull("@ServicePeriodStart", model.ServicePeriodStart)
                    .AddIfNotEmptyOrDbNull("@ServicePeriodEnd", model.ServicePeriodEnd)
                    .AddWithValue("@RangeMin", model.RangeMin)
                    .AddWithValue("@RangeMax", model.RangeMax)
                    .AddWithValue("@ThermFactor", model.ThermFactor)
                    .AddWithValue("@DegreeDayFactor", model.DegreeDayFactor)
                    .AddWithValue("@CompositeUOM", model.CompositeUom);
                
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void InsertNetIntervalSummaryQty(Type867NetIntervalSummaryQty model)
        {
        }

        public void InsertIntervalSummaryQty(Type867IntervalSummaryQty model)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand("csp867IntervalSummaryQtyInsert"))
            {
                command.AddWithValue("@IntervalSummary_Key", model.IntervalSummaryKey)
                    .AddIfNotEmptyOrDbNull("@Qualifier", model.Qualifier)
                    .AddIfNotEmptyOrDbNull("@Quantity", model.Quantity)
                    .AddIfNotEmptyOrDbNull("@MeasurementCode", model.MeasurementCode)
                    .AddIfNotEmptyOrDbNull("@CompositeUOM", model.CompositeUom)
                    .AddIfNotEmptyOrDbNull("@UOM", model.Uom)
                    .AddIfNotEmptyOrDbNull("@BeginRead", model.BeginRead)
                    .AddIfNotEmptyOrDbNull("@EndRead", model.EndRead)
                    .AddIfNotEmptyOrDbNull("@MeasurementSignificanceCode", model.MeasurementSignificanceCode)
                    .AddIfNotEmptyOrDbNull("@TransformerLossFactor", model.TransformerLossFactor)
                    .AddIfNotEmptyOrDbNull("@MeterMultiplier", model.MeterMultiplier)
                    .AddIfNotEmptyOrDbNull("@PowerFactor", model.PowerFactor)
                    .AddWithValue("@RangeMin", model.RangeMin)
                    .AddWithValue("@RangeMax", model.RangeMax)
                    .AddWithValue("@ThermFactor", model.ThermFactor)
                    .AddWithValue("@DegreeDayFactor", model.DegreeDayFactor);
                
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void InsertUnMeterDetailQty(Type867UnMeterDetailQty model)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand("csp867UnmeterDetailQtyInsert"))
            {
                command.AddWithValue("@UnmeterDetail_Key", model.UnMeterDetailKey)
                    .AddIfNotEmptyOrDbNull("@Qualifier", model.Qualifier)
                    .AddIfNotEmptyOrDbNull("@Quantity", model.Quantity)
                    .AddIfNotEmptyOrDbNull("@CompositeUOM", model.CompositeUom)
                    .AddIfNotEmptyOrDbNull("@UOM", model.Uom)
                    .AddIfNotEmptyOrDbNull("@NumberOfDevices", model.NumberOfDevices)
                    .AddIfNotEmptyOrDbNull("@ConsumptionPerDevice", model.ConsumptionPerDevice);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public int InsertIntervalSummaryAcrossMeters(Type867IntervalSummaryAcrossMeters model)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand("csp867IntervalSummaryAcrossMetersInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@867_Key", model.HeaderKey)
                    .AddIfNotEmptyOrDbNull("@TypeCode", model.TypeCode)
                    .AddIfNotEmptyOrDbNull("@ServicePeriodStart", model.ServicePeriodStart)
                    .AddIfNotEmptyOrDbNull("@ServicePeriodStartTime", model.ServicePeriodStartTime)
                    .AddIfNotEmptyOrDbNull("@ServicePeriodEnd", model.ServicePeriodEnd)
                    .AddIfNotEmptyOrDbNull("@ServicePeriodEndTime", model.ServicePeriodEndTime)
                    .AddIfNotEmptyOrDbNull("@MeterRole", model.MeterRole)
                    .AddIfNotEmptyOrDbNull("@MeterUOM", model.MeterUOM)
                    .AddIfNotEmptyOrDbNull("@MeterInterval", model.MeterInterval)

                    .AddOutParameter("@IntervalSummaryAcrossMeters_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var key = (int)keyParameter.Value;
                model.IntervalSummaryAcrossMetersKey = key;

                return key;
            }
        }

        public void InsertIntervalSummaryAcrossMetersQty(Type867IntervalSummaryAcrossMetersQty model)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand("csp867IntervalSummaryAcrossMetersQtyInsert"))
            {
                command.AddWithValue("@IntervalSummaryAcrossMeters_Key", model.IntervalSummaryAcrossMetersKey)
                    .AddIfNotEmptyOrDbNull("@Qualifier", model.Qualifier)
                    .AddIfNotEmptyOrDbNull("@Quantity", model.Quantity)
                    .AddIfNotEmptyOrDbNull("@IntervalEndDate", model.IntervalEndDate)
                    .AddIfNotEmptyOrDbNull("@IntervalEndTime", model.IntervalEndTime);
                
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }
        
        public int InsertNonIntervalDetail(Type867NonIntervalDetail model)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand("csp867NonIntervalDetailInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@867_Key", model.HeaderKey)
                    .AddIfNotEmptyOrDbNull("@TypeCode", model.TypeCode)
                    .AddIfNotEmptyOrDbNull("@MeterNumber", model.MeterNumber)
                    .AddIfNotEmptyOrDbNull("@MovementTypeCode", model.MovementTypeCode)
                    .AddIfNotEmptyOrDbNull("@ServicePeriodStart", model.ServicePeriodStart)
                    .AddIfNotEmptyOrDbNull("@ServicePeriodEnd", model.ServicePeriodEnd)
                    .AddIfNotEmptyOrDbNull("@ExchangeDate", model.ExchangeDate)
                    .AddIfNotEmptyOrDbNull("@MeterRole", model.MeterRole)
                    .AddIfNotEmptyOrDbNull("@MeterUOM", model.MeterUom)
                    .AddIfNotEmptyOrDbNull("@MeterInterval", model.MeterInterval)
                    .AddWithValue("@CommodityCode", model.CommodityCode)
                    .AddWithValue("@NumberOfDials", model.NumberOfDials)
                    .AddWithValue("@ServicePointId", model.ServicePointId)
                    .AddWithValue("@UtilityRateServiceClass", model.UtilityRateServiceClass)
                    .AddWithValue("@RateSubClass", model.RateSubClass)
                    .AddWithValue("@Ratchet_DateTime", model.RatchetDateTime)
                    .AddOutParameter("@NonIntervalDetail_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var key = (int)keyParameter.Value;
                model.NonIntervalDetailKey = key;

                return key;
            }
        }

        public void InsertNonIntervalDetailQty(Type867NonIntervalDetailQty model)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand("csp867NonIntervalDetailQtyInsert"))
            {
                command.AddWithValue("@NonIntervalDetail_Key", model.NonIntervalDetailKey)
                    .AddIfNotEmptyOrDbNull("@Qualifier", model.Qualifier)
                    .AddIfNotEmptyOrDbNull("@Quantity", model.Quantity)
                    .AddIfNotEmptyOrDbNull("@MeasurementCode", model.MeasurementCode)
                    .AddIfNotEmptyOrDbNull("@CompositeUOM", model.CompositeUom)
                    .AddIfNotEmptyOrDbNull("@UOM", model.Uom)
                    .AddIfNotEmptyOrDbNull("@BeginRead", model.BeginRead)
                    .AddIfNotEmptyOrDbNull("@EndRead", model.EndRead)
                    .AddIfNotEmptyOrDbNull("@MeasurementSignificanceCode", model.MeasurementSignificanceCode)
                    .AddIfNotEmptyOrDbNull("@TransformerLossFactor", model.TransformerLossFactor)
                    .AddIfNotEmptyOrDbNull("@MeterMultiplier", model.MeterMultiplier)
                    .AddIfNotEmptyOrDbNull("@PowerFactor", model.PowerFactor)
                    .AddWithValue("@RangeMin", model.RangeMin)
                    .AddWithValue("@RangeMax", model.RangeMax)
                    .AddWithValue("@ThermFactor", model.ThermFactor)
                    .AddWithValue("@DegreeDayFactor", model.DegreeDayFactor);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public int InsertIntervalDetail(Type867IntervalDetail model)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand("csp867IntervalDetailInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@867_Key", model.HeaderKey)
                    .AddWithValue("@IntervalSummary_Key", model.IntervalSummaryKey)
                    .AddIfNotEmptyOrDbNull("@TypeCode", model.TypeCode)
                    .AddIfNotEmptyOrDbNull("@MeterNumber", model.MeterNumber)
                    .AddIfNotEmptyOrDbNull("@ServicePeriodStart", model.ServicePeriodStart)
                    .AddIfNotEmptyOrDbNull("@ServicePeriodEnd", model.ServicePeriodEnd)
                    .AddIfNotEmptyOrDbNull("@ExchangeDate", model.ExchangeDate)
                    .AddIfNotEmptyOrDbNull("@ChannelNumber", model.ChannelNumber)
                    .AddIfNotEmptyOrDbNull("@MeterUOM", model.MeterUOM)
                    .AddIfNotEmptyOrDbNull("@MeterInterval", model.MeterInterval)
                    .AddIfNotEmptyOrDbNull("@MeterRole", model.MeterRole)
                    .AddWithValue("@CommodityCode", model.CommodityCode)
                    .AddWithValue("@NumberOfDials", model.NumberOfDials)
                    .AddWithValue("@ServicePointId", model.ServicePointId)
                    .AddWithValue("@UtilityRateServiceClass", model.UtilityRateServiceClass)
                    .AddWithValue("@RateSubClass", model.RateSubClass)
                    .AddOutParameter("@IntervalDetail_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var key = (int)keyParameter.Value;
                model.IntervalDetailKey = key;

                return key;
            }
        }

        public void InsertIntervalDetailQty(Type867IntervalDetailQty model)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand("csp867IntervalDetailQtyInsert"))
            {
                command.AddWithValue("@IntervalDetail_Key", model.IntervalDetailKey)
                    .AddIfNotEmptyOrDbNull("@Qualifier", model.Qualifier)
                    .AddIfNotEmptyOrDbNull("@Quantity", model.Quantity)
                    .AddIfNotEmptyOrDbNull("@IntervalEndDate", model.IntervalEndDate)
                    .AddIfNotEmptyOrDbNull("@IntervalEndTime", model.IntervalEndTime)
                    .AddWithValue("@RangeMin", model.RangeMin)
                    .AddWithValue("@RangeMax", model.RangeMax)
                    .AddWithValue("@ThermFactor", model.ThermFactor)
                    .AddWithValue("@DegreeDayFactor", model.DegreeDayFactor);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public int InsertScheduleDeterminants(Type867ScheduleDeterminants model)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand("csp867ScheduleDeterminantsInsert"))
            {
                SqlParameter keyParameter;
                
                command.AddWithValue("@867_Key", model.HeaderKey)
                    .AddWithValue("@Capacity_Obligation", model.CapacityObligation)
                    .AddWithValue("@Transmission_Obligation", model.TransmissionObligation)
                    .AddWithValue("@Load_Profile", model.LoadProfile)
                    .AddWithValue("@LDC_Rate_Class", model.LDCRateClass)
                    .AddWithValue("@Zone", model.Zone)
                    .AddWithValue("@BillCycle", model.BillCycle)
                    .AddWithValue("@MeterNumber", model.MeterNumber)
                    .AddWithValue("@EffectiveDate", model.EffectiveDate)
                    .AddWithValue("@LossFactor", model.LossFactor)
                    .AddWithValue("@ServiceVoltage", model.ServiceVoltage)
                    .AddWithValue("@SpecialMeterConfig", model.SpecialMeterConfig)
                    .AddWithValue("@MaximumGeneration", model.MaximumGeneration)
                    .AddWithValue("@LDC_Rate_Sub_Class", model.LDCRateSubClass)
                    .AddOutParameter("@ScheduleDeterminants_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var key = (int)keyParameter.Value;
                model.ScheduleDeterminantsKey = key;

                return key;
            }
        }

        public int InsertSwitch(Type867Switch model)
        {
            return -1;
        }

        public void InsertSwitchQty(Type867SwitchQty model)
        {
        }
        
        public void InsertName(Type867Name model)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand("csp867NameInsert"))
            {
                command.AddWithValue("@Header_Key", model.HeaderKey)
                    .AddWithValue("@867_Key", model.HeaderKey)
                    .AddWithValue("@EntityIdType",model.EntityIdType)
                    .AddWithValue("@EntityName",model.EntityName)
                    .AddWithValue("@EntityDuns",model.EntityDuns)
                    .AddWithValue("@EntityIdCode",model.EntityIdCode)
                    .AddWithValue("@ServiceAddress1",model.ServiceAddress1)
                    .AddWithValue("@ServiceAddress2",model.ServiceAddress2)
                    .AddWithValue("@ServiceCity",model.ServiceCity)
                    .AddWithValue("@ServiceState",model.ServiceState)
                    .AddWithValue("@ServiceZipCode",model.ServiceZipCode);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public int InsertUnMeterSummary(Type867UnMeterSummary model)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand("csp867UnmeterSummaryInsert"))
            {
                SqlParameter keyParameter;

                command.AddWithValue("@867_Key", model.HeaderKey)
                    .AddIfNotEmptyOrDbNull("@TypeCode", model.TypeCode)
                    .AddIfNotEmptyOrDbNull("@MeterUOM", model.MeterUom)
                    .AddIfNotEmptyOrDbNull("@MeterInterval", model.MeterInterval)
                    .AddWithValue("@CommodityCode", model.CommodityCode)
                    .AddWithValue("@UtilityRateServiceClass", model.UtilityRateServiceClass)
                    .AddWithValue("@RateSubClass", model.RateSubClass)
                    .AddOutParameter("@UnmeterSummary_Key", SqlDbType.Int, out keyParameter);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();

                if (keyParameter.Value == null)
                    throw new Exception();

                var key = (int)keyParameter.Value;
                model.UnMeterSummaryKey = key;

                return key;
            }
        }

        public void InsertUnMeterSummaryQty(Type867UnMeterSummaryQty model)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand("csp867UnmeterSummaryQtyInsert"))
            {
                command.AddWithValue("@UnmeterSummary_Key", model.UnMeterSummaryKey)
                    .AddIfNotEmptyOrDbNull("@Qualifier", model.Qualifier)
                    .AddIfNotEmptyOrDbNull("@Quantity", model.Quantity)
                    .AddIfNotEmptyOrDbNull("@MeasurementSignificanceCode", model.MeasurementSignificanceCode)
                    .AddIfNotEmptyOrDbNull("@ServicePeriodStart", model.ServicePeriodStart)
                    .AddIfNotEmptyOrDbNull("@ServicePeriodEnd", model.ServicePeriodEnd);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void InsertGasProfileFactorEvaluation(Type867GasProfileFactorEvaluation model)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand("esp_867GasProfileFactorEvaluationInsert"))
            {
                command.AddWithValue("@867_Key", model.HeaderKey)
                    .AddIfNotEmptyOrDbNull("@ProfilePeriodStartDate", model.ProfilePeriodStartDate)
                    .AddIfNotEmptyOrDbNull("@CustomerServiceInitdate", model.CustomerServiceInitDate)
                    .AddIfNotEmptyOrDbNull("@UtilityRateServiceClass", model.UtilityRateServiceClass)
                    .AddIfNotEmptyOrDbNull("@RateSubClass", model.RateSubClass)
                    .AddIfNotEmptyOrDbNull("@NonHeatLoadFactorQty", model.NonHeatLoadFactorQty)
                    .AddIfNotEmptyOrDbNull("@WeatherNormLoadFactorQty", model.WeatherNormLoadFactorQty)
                    .AddIfNotEmptyOrDbNull("@LoadFactorRatio", model.LoadFactorRatio)
                    .AddIfNotEmptyOrDbNull("@UFGRatePct", model.UFGRatePct)
                    .AddIfNotEmptyOrDbNull("@MaximumDeliveryQty", model.MaximumDeliveryQty);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void InsertGasProfileFactorSample(Type867GasProfileFactorSample model)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand("esp_867GasProfileFactorSampleInsert"))
            {
                command.AddWithValue("@867_Key", model.HeaderKey)
                    .AddIfNotEmptyOrDbNull("@ReportMonth", model.ReportMonth)
                    .AddIfNotEmptyOrDbNull("@AnnualPeriod", model.AnnualPeriod)
                    .AddIfNotEmptyOrDbNull("@NormProjectedUsageQty", model.NormProjectedUsageQty)
                    .AddIfNotEmptyOrDbNull("@WeatherNormUsageProjectedQty", model.WeatherNormUsageProjectedQty)
                    .AddIfNotEmptyOrDbNull("@NormProjectedDeliveryQty", model.NormProjectedDeliveryQty)
                    .AddIfNotEmptyOrDbNull("@WeatherNormProjectedDeliveryQty", model.WeatherNormProjectedDeliveryQty)
                    .AddIfNotEmptyOrDbNull("@ProjectedDailyDeliveryQty", model.ProjectedDailyDeliveryQty)
                    .AddIfNotEmptyOrDbNull("@DesignProjectedUsageQty", model.DesignProjectedUsageQty)
                    .AddIfNotEmptyOrDbNull("@DesignProjectedDeliveryQty", model.DesignProjectedDeliveryQty)
                    .AddIfNotEmptyOrDbNull("@ProjectedBalancingUseQty", model.ProjectedBalancingUseQty)
                    .AddIfNotEmptyOrDbNull("@ProjectedSwingChargeAmt", model.ProjectedSwingChargeAmt);
                
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type867Header : IType867Model, IMarketHeaderModel
    {
        private readonly List<Type867Name> names;
        private readonly List<Type867NonIntervalDetail> nonIntervalDetails;
        private readonly List<Type867NonIntervalSummary> nonIntervalSummaries;
        private readonly List<Type867UnMeterDetail> unMeterDetails;
        private readonly List<Type867UnMeterSummary> unMeterSummaries;
        private readonly List<Type867IntervalDetail> intervalDetails;
        private readonly List<Type867IntervalSummary> intervalSummaries;
        private readonly List<Type867IntervalSummaryAcrossMeters> intervalSummaryAcrossMeters;
        private readonly List<Type867ScheduleDeterminants> scheduleDeterminants;
        private readonly List<Type867GasProfileFactorEvaluation> gasProfileFactorEvaluations;
        private readonly List<Type867GasProfileFactorSample> gasProfileFactorSamples;
        private readonly List<Type867AccountBillQty> accountBillQtys;
        private readonly List<Type867NetIntervalSummary> netIntervalSummaries;
        private readonly List<Type867Switch> switches;

        public Type867Types ModelType
        {
            get { return Type867Types.Header; }
        }

        public int MarketFileId { get; set; }
        public int HeaderKey { get; set; }
        public string TransactionSetId { get; set; }
        public string TransactionSetControlNbr { get; set; }
        public string TransactionSetPurposeCode { get; set; }
        public string TransactionNbr { get; set; }
        public string TransactionDate { get; set; }
        public string ReportTypeCode { get; set; }
        public string ActionCode { get; set; }
        public string ReferenceNbr { get; set; }
        public string DocumentDueDate { get; set; }
        public string EsiId { get; set; }
        public string PowerRegion { get; set; }
        public string OriginalTransactionNbr { get; set; }
        public string TdspDuns { get; set; }
        public string TdspName { get; set; }
        public string CrDuns { get; set; }
        public string CrName { get; set; }
        public int CTRProcessFlag { get; set; }
        public bool ProcessFlag { get; set; }
        public DateTime ProcessDate  { get; set; }
        public bool DirectionFlag { get; set; }
        public string UtilityAccountNumber { get; set; }
        public int TransactionTypeID { get; set; }
        public int MarketID { get; set; }
        public int ProviderID { get; set; }
        public string PreviousUtilityAccountNumber { get; set; }
        public string EstimationReason { get; set; }
        public string EstimationDescription { get; set; }
        public string DoorHangerFlag { get; set; }
        public string EsnCount { get; set; }
        public string QoCount { get; set; }
        public string NextMeterReadDate { get; set; }
        public string InvoiceNbr { get; set; }
        public string EspAccountNumber { get; set; }
        public string UtilityContractID { get; set; }
        
        public Type867Name[] Names
        {
            get { return names.ToArray(); }   

        }
        public Type867NonIntervalDetail[] NonIntervalDetails
        {
            get { return nonIntervalDetails.ToArray(); }

        }
        public Type867NonIntervalSummary[] NonIntervalSummaries
        {
            get { return nonIntervalSummaries.ToArray(); }

        }
        public Type867UnMeterDetail[] UnMeterDetails
        {
            get { return unMeterDetails.ToArray(); }   

        }
        public Type867UnMeterSummary[] UnMeterSummaries
        {
            get { return unMeterSummaries.ToArray(); }

        }
        public Type867IntervalDetail[] IntervalDetails
        {
            get { return intervalDetails.ToArray(); }

        }
        public Type867IntervalSummary[] IntervalSummaries
        {
            get { return intervalSummaries.ToArray(); }

        }
        public Type867IntervalSummaryAcrossMeters[] IntervalSummaryAcrossMeters
        {
            get { return intervalSummaryAcrossMeters.ToArray(); }

        }
        public Type867ScheduleDeterminants[] ScheduleDeterminants
        {
            get { return scheduleDeterminants.ToArray(); }

        }
        public Type867GasProfileFactorEvaluation[] GasProfileFactorEvaluations
        {
            get { return gasProfileFactorEvaluations.ToArray(); }

        }
        public Type867GasProfileFactorSample[] GasProfileFactorSamples
        {
            get { return gasProfileFactorSamples.ToArray(); }

        }
        public Type867AccountBillQty[] AccountBillQtys
        {
            get { return accountBillQtys.ToArray(); }
        }
        public Type867NetIntervalSummary[] NetIntervalSummaries
        {
            get { return netIntervalSummaries.ToArray(); }
        }
        public Type867Switch[] Switches
        {
            get { return switches.ToArray(); }
        }

        public Type867Header()
        {
            names = new List<Type867Name>();
            nonIntervalDetails = new List<Type867NonIntervalDetail>();
            nonIntervalSummaries = new List<Type867NonIntervalSummary>();
            unMeterDetails = new List<Type867UnMeterDetail>();
            unMeterSummaries = new List<Type867UnMeterSummary>();
            intervalDetails = new List<Type867IntervalDetail>();
            intervalSummaries = new List<Type867IntervalSummary>();
            intervalSummaryAcrossMeters = new List<Type867IntervalSummaryAcrossMeters>();
            scheduleDeterminants = new List<Type867ScheduleDeterminants>();
            gasProfileFactorEvaluations = new List<Type867GasProfileFactorEvaluation>();
            gasProfileFactorSamples = new List<Type867GasProfileFactorSample>();
            accountBillQtys = new List<Type867AccountBillQty>();
            netIntervalSummaries = new List<Type867NetIntervalSummary>();
            switches = new List<Type867Switch>();
        }
        
        public void AddName(Type867Name item)
        {
            names.Add(item);
        }
        
        public void AddNonIntervalDetail (Type867NonIntervalDetail item)
        {
            nonIntervalDetails.Add(item);
        }

        public void AddNonIntervalSummary (Type867NonIntervalSummary item)
        {
            nonIntervalSummaries.Add(item);
        }

        public void AddUnMeterDetail (Type867UnMeterDetail item)
        {
            unMeterDetails.Add(item);
        }

        public void AddUnMeterSummary(Type867UnMeterSummary item)
        {
            unMeterSummaries.Add(item);
        }

        public void AddIntervalDetail (Type867IntervalDetail item)
        {
            intervalDetails.Add(item);
        }

        public void AddIntervalSummary(Type867IntervalSummary item)
        {
            intervalSummaries.Add(item);
        }
        
        public void AddIntervalSummaryAcrossMeters(Type867IntervalSummaryAcrossMeters item)
        {
            intervalSummaryAcrossMeters.Add(item);
        }

        public void AddScheduledDeterminant(Type867ScheduleDeterminants item)
        {
            scheduleDeterminants.Add(item);
        }

        public void AddGasProfileFactorEvaluation(Type867GasProfileFactorEvaluation item)
        {
            gasProfileFactorEvaluations.Add(item);
        }
        
        public void AddGasProfileFactorSample(Type867GasProfileFactorSample item)
        {
            gasProfileFactorSamples.Add(item);
        }

        public void AddAccountBillQuantity(Type867AccountBillQty item)
        {
            accountBillQtys.Add(item);
        }

        public void AddNetInervalSummary(Type867NetIntervalSummary item)
        {
            netIntervalSummaries.Add(item);
        }

        public void AddSwitch(Type867Switch item)
        {
            switches.Add(item);
        }
    }
}

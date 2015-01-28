using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type650Service : IType650Model
    {
        private readonly List<Type650ServicePole> poles;
        private readonly List<Type650ServiceChange> changes;
        private readonly List<Type650ServiceReject> rejects;
        private readonly List<Type650ServiceMeter> meters;

        public Type650Types ModelType
        {
            get { return Type650Types.Service; }
        }

        public int HeaderKey { get; set; }
        public int? ServiceKey { get; set; }
        public string PurposeCode { get; set; }
        public string PriorityCode { get; set; }
        public string EsiId { get; set; }
        public string SpecialProcessCode { get; set; }
        public string ServiceReqDate { get; set; }
        public string NotBeforeDate { get; set; }
        public string CallAhead { get; set; }
        public string PremLocation { get; set; }
        public string AccStatusCode { get; set; }
        public string AccStatusDesc { get; set; }
        public string EquipLocation { get; set; }
        public string ServiceOrderNbr { get; set; }
        public string CompletionDate { get; set; }
        public string CompletionTime { get; set; }
        public string ReportRemarks { get; set; }
        public string Directions { get; set; }
        public string MeterNbr { get; set; }
        public string MeterReadDate { get; set; }
        public string MeterTestDate { get; set; }
        public string MeterTestResults { get; set; }
        public string IncidentCode { get; set; }
        public string EstRestoreDate { get; set; }
        public string EstRestoreTime { get; set; }
        public string IntStartDate { get; set; }
        public string IntStartTime { get; set; }
        public string RepairRecommended { get; set; }
        public string Rescheduled { get; set; }
        public string InterDurationPeriod { get; set; }
        public string AreaOutage { get; set; }
        public string CustRepairRemarks { get; set; }
        public string MeterReadUom { get; set; }
        public string MeterRead { get; set; }
        public string MeterReadCode { get; set; }
        public string Membership { get; set; }
        public string RemarksPermanentSuspend { get; set; }
        public string DisconnectAuthorization { get; set; }
        public string PremiseTypeVerification { get; set; }
        public string PremiseTypeDesc { get; set; }
        public string SwitchHoldIndicator { get; set; }
        public string SwitchHoldDesc { get; set; }

        public Type650ServicePole[] Poles
        {
            get { return poles.ToArray(); }
        }

        public Type650ServiceChange[] Changes
        {
            get { return changes.ToArray(); }
        }

        public Type650ServiceReject[] Rejects
        {
            get { return rejects.ToArray(); }
        }

        public Type650ServiceMeter[] Meters
        {
            get { return meters.ToArray(); }
        }

        public Type650Service()
        {
            poles = new List<Type650ServicePole>();
            changes = new List<Type650ServiceChange>();
            rejects = new List<Type650ServiceReject>();
            meters = new List<Type650ServiceMeter>();
        }

        public void AddPole(Type650ServicePole item)
        {
            poles.Add(item);
        }

        public void AddChange(Type650ServiceChange item)
        {
            changes.Add(item);
        }

        public void AddReject(Type650ServiceReject item)
        {
            rejects.Add(item);
        }

        public void AddMeter(Type650ServiceMeter item)
        {
            meters.Add(item);
        }
    }
}
using System;

namespace Ista.FileServices.Service.Models
{
    public class TaskScheduleContinuousModel : TaskScheduleModel
    {
        public int IdlePeriod { get; set; }
        public string IdlePeriodType { get; set; }

        public override bool IsContinuous
        {
            get { return true; }
        }

        public override DateTime IdentifyNextEntry(DateTime date)
        {
            if (string.IsNullOrWhiteSpace(IdlePeriodType))
                return date.AddMinutes(15);

            switch (IdlePeriodType.Trim().ToUpper())
            {
                case "SECONDS":
                    return date.AddSeconds(IdlePeriod);
                case "MINUTES":
                    return date.AddMinutes(IdlePeriod);
                case "HOURS":
                    return date.AddHours(IdlePeriod);
                case "DAYS":
                    return date.AddDays(IdlePeriod);
            }

            return date.AddMinutes(15);
        }
    }
}

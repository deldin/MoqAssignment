using System.Collections.Generic;

namespace Ista.FileServices.Service.Models
{
    public class ExecutionRequestScheduleModel
    {
        public string Type { get; set; }
        public string PeriodType { get; set; }
        public List<int> Periods { get; set; }
    }
}
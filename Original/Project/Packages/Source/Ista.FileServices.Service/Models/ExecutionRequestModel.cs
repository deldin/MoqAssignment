using System.Collections.Generic;

namespace Ista.FileServices.Service.Models
{
    public class ExecutionRequestModel
    {
        public string Type { get; set; }
        public string TaskId { get; set; }
        public List<ExecutionRequestItemModel> Items { get; set; }
        public List<ExecutionRequestScheduleModel> Schedules { get; set; }
    }
}
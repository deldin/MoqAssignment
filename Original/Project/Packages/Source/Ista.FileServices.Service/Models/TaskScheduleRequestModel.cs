using System;
using System.Collections.Generic;

namespace Ista.FileServices.Service.Models
{
    public class TaskScheduleRequestModel
    {
        public int RequestId { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedOn { get; set; }

        public string TaskId { get; set; }
        public string Type { get; set; }
        public List<TaskScheduleItemRequestModel> Items { get; set; }
        public List<TaskScheduleTypeRequestModel> Schedules { get; set; }
    }

    public class TaskScheduleItemRequestModel
    {
        public string TaskId { get; set; }
        public int Order { get; set; }
    }

    public class TaskScheduleTypeRequestModel
    {
        public string Type { get; set; }
        public string PeriodType { get; set; }
        public List<int> Periods { get; set; }
    }
}

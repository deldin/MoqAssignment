using System;
using System.Collections.Generic;
using System.Linq;

namespace Ista.FileServices.Service.Models
{
    public abstract class TaskScheduleModel
    {
        private readonly List<TaskScheduleItemModel> collection;

        public string TaskId { get; set; }

        public bool IsContainer
        {
            get { return collection.Any(); }
        }

        public virtual bool IsContinuous
        {
            get { return false; }
        }

        public TaskScheduleItemModel[] ScheduleItems
        {
            get { return collection.ToArray(); }
        }

        protected TaskScheduleModel()
        {
            collection = new List<TaskScheduleItemModel>();
        }

        public void AddScheduleItem(TaskScheduleItemModel item)
        {
            collection.Add(item);
        }

        public abstract DateTime IdentifyNextEntry(DateTime date);
    }
}

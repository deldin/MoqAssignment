using System;
using Ista.FileServices.Service.Models;

namespace Ista.FileServices.Service.Interfaces
{
    public interface IMiramarScheduleProvider
    {
        string[] IdentifyTasksToExecute(DateTime date);
        string[] IdentifyExecutionPlan(string taskId);
        bool IsScheduled(string taskId);
        void PopTaskFromConsideration(string taskId);
        void PublishSchedules();
        void PushTaskForConsideration(string taskId);
        void AddOrUpdateTaskSchedule(TaskScheduleModel model);
    }
}

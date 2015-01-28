using System;
using System.Collections.Generic;
using Ista.FileServices.Service.Models;

namespace Ista.FileServices.Service.Interfaces
{
    public interface IMiramarPublisher : IDisposable
    {
        void Publish<T>(string messageType, string messageAction, T message);
        void PublishError(int clientId, string taskId, DateTime date, string message);
        void PublishException(int clientId, string taskId, DateTime date, Exception exception);
        void PublishHeartbeat(DateTime date);
        void PublishMessage(int clientId, string taskId, string type, DateTime date, string message);
        void PublishSchedules(TaskScheduleModel[] tasks, Dictionary<string, DateTime> schedules);
        void PublishScheduleAdded(TaskScheduleModel task, DateTime date);
        void PublishScheduleRemoved(string taskId);
        void PublishStatusActive(int clientId, string taskId, DateTime date);
        void PublishStatusActive(int clientId, string taskId, DateTime date, string message);
        void PublishStatusCanceled(int clientId, string taskId, DateTime date);
        void PublishStatusCanceled(int clientId, string taskId, DateTime date, string message);
        void PublishStatusComplete(int clientId, string taskId, DateTime date);
        void PublishStatusComplete(int clientId, string taskId, DateTime date, string message);
        void PublishStatusFailed(int clientId, string taskId, DateTime date);
        void PublishStatusFailed(int clientId, string taskId, DateTime date, string message);
        void PublishTaskConfiguration(TaskConfigurationModel[] tasks);
    }
}

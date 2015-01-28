using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.Infrastructure.Queuing;
using Ista.FileServices.Service.Interfaces;
using Ista.FileServices.Service.Models;

namespace Ista.FileServices.Service
{
    public class MiramarPublisher : IMiramarPublisher
    {
        private readonly IMessageQueueFactory messageFactory;
        private readonly string miramarIdentifier;
        private readonly string monitorExchange;

        private bool messagePublisherActive;
        private bool messagePublisherInitialized;
        private IMessageQueuePublisher messagePublisher;
        
        public MiramarPublisher()
            : this(MessageQueueFactory.CreateInactiveFactory())
        {
        }

        public MiramarPublisher(IMessageQueueFactory messageFactory)
        {
            this.messageFactory = messageFactory;

            miramarIdentifier = ConfigurationManager.AppSettings["MiramarIdentifier"];
            monitorExchange = ConfigurationManager.AppSettings["RabbitMqMonitorExchange"];
        }

        public void Dispose()
        {
            if (messagePublisher != null)
                messagePublisher.Dispose();
        }

        public void Publish<T>(string messageType, string messageAction, T message)
        {
            InitializePublisher();

            if (!messagePublisherActive)
                return;

            var properties = IstaMessageProperties.CreatePersistent(messageType, messageAction);
            properties.AppId = miramarIdentifier ?? "unknown";

            messagePublisher.Publish(monitorExchange, message, properties);
        }

        public void PublishError(int clientId, string taskId, DateTime date, string message)
        {
            Publish("message", "error", new
            {
                ClientId = clientId,
                TaskId = taskId,
                Type = "error",
                Date = date,
                Message = message,
            });
        }

        public void PublishException(int clientId, string taskId, DateTime date, Exception exception)
        {
            if (exception == null)
            {
                PublishError(clientId, taskId, date,
                    string.Format("An unknown error occured while executing task \"{0}\" for Client {1}.", taskId,
                        clientId));

                return;
            }

            if (exception is AggregateException)
                exception = exception.InnerException;

            var exceptionType = exception.GetType();
            var message = string.Format("{0}: {1}\r\n{2}", exceptionType.FullName, exception.Message, exception.StackTrace);

            PublishError(clientId, taskId, date, message);
        }

        public void PublishHeartbeat(DateTime date)
        {
            Publish("controller", "heartbeat", new
            {
                MiramarIdentifier = miramarIdentifier,
                Date = date,
            });
        }

        public void PublishMessage(int clientId, string taskId, string type, DateTime date, string message)
        {
            Publish("message", type, new
            {
                ClientId = clientId,
                TaskId = taskId,
                Type = type,
                Date = date,
                Message = message
            });
        }

        public void PublishScheduleAdded(TaskScheduleModel task, DateTime date)
        {
            var payload = new
            {
                task.TaskId,
                Date = date,
                Execution = task.ScheduleItems
                    .Select(y => new {taskId = y.TaskId, order = y.Order,})
                    .ToArray(),
            };

            Publish("schedule", "add", payload);
        }

        public void PublishScheduleRemoved(string taskId)
        {
            var payload = new
            {
                TaskId = taskId
            };

            Publish("schedule", "remove", payload);
        }

        public void PublishSchedules(TaskScheduleModel[] tasks, Dictionary<string, DateTime> schedules)
        {
            var payload = tasks
                .Join(schedules, o => o.TaskId, i => i.Key, (o, i) => new { Schedule = o, Date = i.Value })
                .Select(x => new
                {
                    x.Schedule.TaskId,
                    x.Date,
                    Execution = x.Schedule.ScheduleItems
                        .Select(y => new { taskId = y.TaskId, order = y.Order, })
                        .ToArray(),
                }).ToArray();

            Publish("schedule", "init", payload);
        }

        public void PublishStatusActive(int clientId, string taskId, DateTime date)
        {
            PublishStatusActive(clientId, taskId, date, string.Empty);
        }

        public void PublishStatusActive(int clientId, string taskId, DateTime date, string message)
        {
            Publish("status", "active", new
            {
                ClientId = clientId,
                TaskId = taskId,
                Date = date,
                Message = message
            });
        }

        public void PublishStatusCanceled(int clientId, string taskId, DateTime date)
        {
            PublishStatusCanceled(clientId, taskId, date, string.Empty);
        }

        public void PublishStatusCanceled(int clientId, string taskId, DateTime date, string message)
        {
            Publish("status", "canceled", new
            {
                ClientId = clientId,
                TaskId = taskId,
                Date = date,
                Message = message,
            });
        }

        public void PublishStatusComplete(int clientId, string taskId, DateTime date)
        {
            PublishStatusComplete(clientId, taskId, date, string.Empty);
        }

        public void PublishStatusComplete(int clientId, string taskId, DateTime date, string message)
        {
            Publish("status", "complete", new
            {
                ClientId = clientId,
                TaskId = taskId,
                Date = date,
                Message = message,
            });
        }

        public void PublishStatusFailed(int clientId, string taskId, DateTime date)
        {
            PublishStatusFailed(clientId, taskId, date, string.Empty);
        }

        public void PublishStatusFailed(int clientId, string taskId, DateTime date, string message)
        {
            Publish("status", "failed", new
            {
                ClientId = clientId,
                TaskId = taskId,
                Date = date,
                Message = message,
            });
        }

        public void PublishTaskConfiguration(TaskConfigurationModel[] tasks)
        {
            var configuration = tasks
                .Select(x => new
                {
                    x.ClientId,
                    x.TaskId,
                    x.DisplayName,
                    Subtasks = x.SubTasks
                        .Select((item, index) => new
                        {
                            item.TaskId,
                            item.DisplayName,
                            Order = (index + 1)
                        }).ToArray()
                }).ToArray();

            Publish("configuration", "init", new
            {
                Date = DateTime.Now,
                Configuration = configuration,
            });
        }

        private void InitializePublisher()
        {
            if (messagePublisherInitialized)
                return;

            messagePublisherInitialized = true;

            if (!messageFactory.IsActive || !messageFactory.IsOpen)
                return;

            if (string.IsNullOrWhiteSpace(monitorExchange))
                return;

            messagePublisher = messageFactory.CreatePublisher();
            messagePublisherActive = true;
        }
    }
}

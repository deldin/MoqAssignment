using System;
using System.Configuration;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.Infrastructure.Queuing;
using NLog;
using NLog.Targets;

namespace Ista.FileServices.Infrastructure.Logging
{
    [Target("QueueAdapter")]
    public class QueueAdapter : TargetWithLayout
    {
        private readonly IMessageQueuePublisher messagePublisher;
        private readonly bool messagePublisherIsActive;
        
        public QueueAdapter(IMessageQueueFactory messageFactory)
        {
            messagePublisherIsActive = false;

            if (!messageFactory.IsActive || !messageFactory.IsOpen) 
                return;

            messagePublisherIsActive = true;
            messagePublisher = messageFactory.CreatePublisher();
        }

        protected override void Dispose(bool disposing)
        {
            if (messagePublisher != null)
                messagePublisher.Dispose();
        }

        protected override void Write(LogEventInfo logEvent)
        {
            if (!messagePublisherIsActive)
                return;

            var miramarIdentifier = ConfigurationManager.AppSettings["MiramarIdentifier"];
            var monitorExchange = ConfigurationManager.AppSettings["RabbitMqMonitorExchange"];
            if (string.IsNullOrWhiteSpace(monitorExchange))
                return;

            var properties = logEvent.Properties;
            var messageType = "info";
            if (properties.ContainsKey("MessageType"))
                messageType = properties["MessageType"].ToString();

            var message = Layout.Render(logEvent);
            var messagePayload = new
            {
                clientId = properties["ClientId"],
                taskId = properties["TaskId"],
                type = messageType,
                date = DateTime.Now,
                message,
            };

            var messageProperties = IstaMessageProperties.CreatePersistent("message", "message");
            messageProperties.AppId = miramarIdentifier ?? "unknown";

            messagePublisher.Publish(monitorExchange, messagePayload, messageProperties);
        }
    }
}

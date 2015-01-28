using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using Ista.FileServices.Infrastructure;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.Service.Interfaces;
using StructureMap;

namespace Ista.FileServices.Service
{
    public class ScheduleService
    {
        private readonly IMessageQueueFactory queueFactory;
        private readonly IMiramarContextProvider contextProvider;
        private readonly IMiramarTaskProvider taskProvider;
        private readonly IMiramarScheduleProvider scheduleProvider;
        private readonly ILogger logger;
        
        public ScheduleService(IMessageQueueFactory queueFactory, IMiramarTaskProvider taskProvider, IMiramarScheduleProvider scheduleProvider, IMiramarContextProvider contextProvider)
        {
            this.contextProvider = contextProvider;
            this.queueFactory = queueFactory;
            this.taskProvider = taskProvider;
            this.scheduleProvider = scheduleProvider;

            logger = InfrastructureFactory.CreateLogger("ScheduleQueue");
        }

        public void ThreadRun(CancellationToken token)
        {
            if (!queueFactory.IsActive)
            {
                logger.Warn("Message queue is not active or has been disabled. Messages will not be consumed.");
                return;
            }

            var identifier = ConfigurationManager.AppSettings["MiramarIdentifier"];
            var queue = ConfigurationManager.AppSettings["RabbitMqMessageQueue"];
            if (string.IsNullOrWhiteSpace(queue))
            {
                logger.Warn("RabbitMQ Message Queue has not been configured or is empty. Messages will not be consumed.");
                return;
            }

            var handlers = ObjectFactory.GetAllInstances<IScheduleMessageHandler>()
                .ToArray();

            if (handlers.Length == 0)
            {
                logger.Warn("No message handlers have been registered. Messages will not be consumed.");
                return;
            }

            using (var service = queueFactory.CreateService())
            {
                service.Consume(queue, token,
                    message =>
                    {
                        var properties = message.Properties;
                        var miramarIdentifier = string.Empty;
                        if (properties.IsAppIdSet)
                            miramarIdentifier = properties.AppId;

                        if (!miramarIdentifier.Equals(identifier, StringComparison.OrdinalIgnoreCase))
                        {
                            logger.WarnFormat(
                                "Message of type \"{0}\" with action \"{1}\" contains an invalid identifier. The message \"AppId\" must equals \"{2}\".",
                                message.Type, message.Action, identifier);

                            return false;
                        }

                        var handler = handlers
                            .FirstOrDefault(x => x.IsSatisfiedBy(message));

                        if (handler == null)
                        {
                            logger.WarnFormat(
                                "No handler found for messge of type \"{0}\" with action \"{1}\". Message will be rejected.",
                                message.Type, message.Action);

                            return false;
                        }

                        return handler.Handle(taskProvider, scheduleProvider, contextProvider, message);
                    });

                logger.Warn("Message queue service has stopped consuming messages.");
            }
        }
    }
}
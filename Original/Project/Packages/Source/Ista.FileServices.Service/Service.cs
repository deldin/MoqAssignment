using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Ista.FileServices.Infrastructure;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.Infrastructure.Logging;
using Ista.FileServices.Infrastructure.Queuing;
using Ista.FileServices.Service.Handlers;
using Ista.FileServices.Service.Interfaces;
using Ista.FileServices.Service.Parsers;
using Ista.Miramar.Interfaces;
using NLog;
using NLog.Config;
using StructureMap;

namespace Ista.FileServices.Service
{
    partial class Service : ServiceBase
    {
        private readonly ILogger logger;
        private readonly CancellationTokenSource serviceSource;

        private IMessageQueueFactory queueFactory;
        private IMiramarPublisher queuePublisher;
        private Task serviceTask;
        private Task messageTask;
        
        static void Main()
        {
            var services = new ServiceBase[] {new Service()};
            Run(services);
        }

        public Service()
        {
            InitializeComponent();

            ObjectFactory.Initialize(x =>
            {
                x.For<IScheduleMessageHandler>()
                    .AddInstances(i =>
                    {
                        i.Type<HandlerAddSchedule>();
                        i.Type<HandlerChangeLogging>();
                        i.Type<HandlerChangeSchedule>();
                        i.Type<HandlerPublishConfiguration>();
                        i.Type<HandlerPublishSchedule>();
                        i.Type<HandlerRemoveSchedule>();
                        i.Type<HandlerResumeSchedule>();
                        i.Type<HandlerStopTask>();
                    });

                x.Scan(s =>
                    {
                        s.AssembliesFromApplicationBaseDirectory();
                        s.AddAllTypesOf<IMiramarTaskFactory>();
                    });
            });

            logger = InfrastructureFactory.CreateLogger("Service");
            serviceSource = new CancellationTokenSource();
        }

        protected override void OnStart(string[] args)
        {
            logger.Debug("Starting up service.");

            queueFactory = CreateMessageFactory(logger);
            if (!queueFactory.IsActive)
                logger.InfoFormat("RabbitMQ is not active. Messaging will not be available.");

            queuePublisher = new MiramarPublisher(queueFactory);

            var directory = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(directory, "miramar.config");
            
            var contextProvider = new MiramarContextProvider();
            var taskProvider = MiramarConfigurationParser.ParseConfiguration(queuePublisher, path);
            var scheduleProvider = MiramarSchedulingParser.ParseSchedule(queuePublisher, path);

            serviceTask = new Task(() =>
            {
                logger.Info("Starting Miramar Controller.");
                
                var controller = new MiramarController(queuePublisher, taskProvider, scheduleProvider, contextProvider);
                controller.ThreadRun(serviceSource.Token);
            });
            serviceTask.ContinueWith(x => logger.Info("Controller is shutting down."));

            messageTask = new Task(() =>
            {
                logger.Info("Starting Miramar Scheduler.");

                var scheduler = new ScheduleService(queueFactory, taskProvider, scheduleProvider, contextProvider);
                scheduler.ThreadRun(serviceSource.Token);
            });
            messageTask.ContinueWith(x => logger.Info("Scheduler is shutting down."));

            AddMiramarAdapter(queueFactory);

            serviceTask.Start();
            messageTask.Start();

            logger.Info("Miramar Controller is now running.");
        }

        protected override void OnStop()
        {
            logger.Info("Miramar Controller is stopping.");

            serviceSource.Cancel();
            Task.WaitAll(serviceTask, messageTask);
            InfrastructureFactory.ForceLogFlush();

            if (queuePublisher != null)
                queuePublisher.Dispose();

            if (queueFactory != null && queueFactory.IsActive)
                if (queueFactory.IsOpen)
                    queueFactory.Dispose();
        }

        static void AddMiramarAdapter(IMessageQueueFactory queueFactory)
        {
            var queueAdapter = new QueueAdapter(queueFactory)
            {
                Name = "miramar",
                Layout = "${message} ${onexception:${newline}${exception:format=message,method,tostring}}"
            };

            var logConfiguration = LogManager.Configuration;
            logConfiguration.AddTarget("Queue", queueAdapter);
            logConfiguration.LoggingRules.Add(new LoggingRule("Client-*", LogLevel.Info, queueAdapter));

            LogManager.Configuration = logConfiguration;
        }

        static IMessageQueueFactory CreateMessageFactory(ILogger logger)
        {
            var rabbitMqHost = ConfigurationManager.AppSettings["RabbitMqHost"];
            if (string.IsNullOrWhiteSpace(rabbitMqHost))
            {
                logger.Warn("RabbitMQ Host has not been configured. All queue based operations will not be performed.");
                return MessageQueueFactory.CreateInactiveFactory();
            }

            var messageFactory = new MessageQueueFactory(rabbitMqHost);

            try
            {
                logger.TraceFormat("Attempt to open connection to RabbitMQ on host \"{0}\".", rabbitMqHost);
                messageFactory.OpenConnection();
            }
            catch (IOException ex)
            {
                logger.ErrorFormat(ex,
                    "An error occurred while opening a connection to RabbitMQ on host \"{0}\".", rabbitMqHost);
            }

            if (messageFactory.IsOpen)
                return messageFactory;

            logger.WarnFormat(
                "Unable to open a connection to RabbitMQ Host \"{0}\". All queue based operations will not be performed.",
                rabbitMqHost);

            return MessageQueueFactory.CreateInactiveFactory();
        }
    }
}

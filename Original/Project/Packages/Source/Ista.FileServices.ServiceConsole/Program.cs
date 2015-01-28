using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ista.FileServices.Infrastructure;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.Infrastructure.Logging;
using Ista.FileServices.Infrastructure.Queuing;
using Ista.FileServices.Service;
using Ista.FileServices.Service.Handlers;
using Ista.FileServices.Service.Interfaces;
using Ista.FileServices.Service.Parsers;
using Ista.Miramar.Interfaces;
using NLog;
using NLog.Config;
using StructureMap;

namespace Ista.FileServices.ServiceConsole
{
    class Program
    {
        static void Main(string[] args)
        {
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

            var logger = InfrastructureFactory.CreateLogger("Console");
            logger.Debug("Starting up console application.");

            if (args.Length != 0)
            {
                logger.Info("Waiting on debugger attach. Press any key to continue.");
                Console.ReadKey();
            }

            var messageFactory = CreateMessageFactory(logger);
            if (!messageFactory.IsActive)
                logger.InfoFormat("RabbitMQ is not active. Messaging will not be available.");

            var source = new CancellationTokenSource();
            var publisher = new MiramarPublisher(messageFactory);
            var contextProvider = new MiramarContextProvider();
            var taskProvider = MiramarConfigurationParser.ParseConfiguration(publisher, "miramar.config");
            var scheduleProvider = MiramarSchedulingParser.ParseSchedule(publisher, "miramar.config");
            
            var controllerTask = new Task(() =>
            {
                logger.Info("Starting Miramar Controller.");
                
                var controller = new MiramarController(publisher, taskProvider, scheduleProvider, contextProvider, true);
                controller.ThreadRun(source.Token);
            });
            controllerTask.ContinueWith(x => logger.Info("Console application is shutting down."));

            var schedulerTask = new Task(() =>
            {
                logger.Info("Starting Miramar Scheduler.");

                var scheduler = new ScheduleService(messageFactory, taskProvider, scheduleProvider, contextProvider);
                scheduler.ThreadRun(source.Token);
            });
            schedulerTask.ContinueWith(x => logger.Info("Scheduler application is shutting down."));

            AddMiramarAdapter(messageFactory);

            controllerTask.Start();
            schedulerTask.Start();
            
            logger.Info("Miramar Controller is now running.");
            logger.Info("Miramar Scheduler is now running");
            logger.Info("Press CTRL+C or 'Q' to stop.");

            Console.CancelKeyPress += (s, e) =>
            {
                logger.Warn("CTRL+C detected. Attempting to quit.");
                CancelAndWait(source, messageFactory, 
                    controllerTask, schedulerTask);
            };

            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key != ConsoleKey.Q)
                    continue;

                logger.Warn("'Q' detected. Attempting to stop.");
                CancelAndWait(source, messageFactory, 
                    controllerTask, schedulerTask);

                break;
            }
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

        static void CancelAndWait(CancellationTokenSource source, IMessageQueueFactory messageFactory, params Task[] tasks)
        {
            source.Cancel();
            Task.WaitAll(tasks);
            
            InfrastructureFactory.ForceLogFlush();
            InfrastructureFactory.CloseLogTargets();

            if (messageFactory != null)
                if (messageFactory.IsOpen)
                    messageFactory.Dispose();
        }
    }
}

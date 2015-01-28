using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ista.FileServices.Infrastructure;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.Service.Contexts;
using Ista.FileServices.Service.DataAccess;
using Ista.FileServices.Service.Interfaces;
using Ista.FileServices.Service.Models;
using Ista.FileServices.Service.Parsers;
using Ista.Miramar.Interfaces;
using StructureMap;

namespace Ista.FileServices.Service
{
    public class MiramarController
    {
        private readonly ILogger logger;
        private readonly IMiramarPublisher publisher;
        private readonly IAdminDataAccess adminDataAccess;
        private readonly bool allowMetaData;

        private IMiramarContextProvider contextProvider;
        private IMiramarTaskProvider taskProvider;
        private IMiramarScheduleProvider scheduleProvider;
        
        public MiramarController()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["BillingAdmin"].ConnectionString;
            adminDataAccess = new AdminDataAccess(connectionString);

            logger = InfrastructureFactory.CreateLogger("Controller");

            publisher = new MiramarPublisher();
            allowMetaData = false;
        }

        public MiramarController(IMiramarTaskProvider taskProvider, IMiramarScheduleProvider scheduleProvider, IMiramarContextProvider contextProvider)
            : this()
        {
            this.contextProvider = contextProvider;
            this.taskProvider = taskProvider;
            this.scheduleProvider = scheduleProvider;
        }

        public MiramarController(IMiramarPublisher publisher, IMiramarTaskProvider taskProvider, IMiramarScheduleProvider scheduleProvider, IMiramarContextProvider contextProvider)
            : this(taskProvider, scheduleProvider, contextProvider)
        {
            this.publisher = publisher;
        }

        public MiramarController(IMiramarPublisher publisher, IMiramarTaskProvider taskProvider, IMiramarScheduleProvider scheduleProvider, IMiramarContextProvider contextProvider, bool allowMetaData)
            : this(publisher, taskProvider, scheduleProvider, contextProvider)
        {
            this.allowMetaData = allowMetaData;
        }

        public void ThreadRun(CancellationToken token)
        {
            if (taskProvider == null)
                taskProvider = MiramarConfigurationParser.ParseConfiguration("miramar.config");

            if (scheduleProvider == null)
                scheduleProvider = MiramarSchedulingParser.ParseSchedule("miramar.config");

            if (contextProvider == null)
                contextProvider = new MiramarContextProvider();

            taskProvider.PublishConfiguration();
            scheduleProvider.PublishSchedules();

            using (var manual = new ManualResetEventSlim())
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                        break;

                    publisher.PublishHeartbeat(DateTime.Now);

                    var identifiers = scheduleProvider.IdentifyTasksToExecute(DateTime.Now);
                    logger.TraceFormat("Identified {0} tasks that should be ran.", identifiers.Length);

                    foreach (var identifier in identifiers)
                    {
                        logger.TraceFormat("Identified \"{0}\" should be ran.", identifier);

                        ActiveTaskContext context;
                        if (contextProvider.TryGetValue(identifier, out context) && context.IsRunning)
                        {
                            logger.WarnFormat("Task \"{0}\" has been scheduled to run but is currently active.",
                                identifier);

                            logger.InfoFormat("Task \"{0}\" has been active since {1:yyyy-MM-dd hh:mm:ss}.",
                                context.TaskName, context.StartDate);

                            continue;
                        }

                        ExecuteTask(identifier, token);
                    }

                    try
                    {
                        manual.Wait(TimeSpan.FromSeconds(15), token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }

            var runningTasks = contextProvider
                .GetRunningContexts()
                .Select(x => x.RunningTask)
                .ToArray();

            if (contextProvider.Any())
                WaitOnTasks(runningTasks);
        }

        public void ExecuteTask(string taskId, CancellationToken token)
        {
            scheduleProvider.PopTaskFromConsideration(taskId);

            var model = taskProvider.LoadTaskConfiguration(taskId);
            if (model == null)
            {
                logger.WarnFormat("Unable to identify configuration for task \"{0}\".", taskId);
                return;
            }

            var context = ActiveTaskContext.Create(model.ClientId, model.TaskId, model.TaskName);
            if (!model.IsContainer)
            {
                ExecuteTask(context, model, token);
                return;
            }

            var collection = model.SubTasks;
            var executionPlan = scheduleProvider.IdentifyExecutionPlan(model.TaskId);
            if (executionPlan.Any())
            {
                collection = collection
                    .Join(executionPlan, o => o.TaskId, i => i, (o, i) => o)
                    .ToArray();

                var invalidEntries = executionPlan
                    .Except(model.SubTasks.Select(x => x.TaskId))
                    .ToArray();

                if (invalidEntries.Any())
                {
                    logger.WarnFormat(
                        "Invalid execution plan found for task \"{0}\". The following tasks are invalid for this configuration: \"{1}\".",
                        model.TaskId, string.Join(", ", invalidEntries));
                }
            }

            ExecuteTaskContainer(context, collection, token);
        }

        public void ExecuteTaskContainer(ActiveTaskContext context, TaskConfigurationModel[] models, CancellationToken token)
        {
            var task = new Task(() =>
            {
                var info = adminDataAccess.LoadClientInfo(context.ClientId);
                if (info == null)
                {
                    var message =
                        string.Format("Unable to load configuration for Client \"{0}\". Tasks will not execute.",
                            context.ClientId);

                    throw new InvalidOperationException(message);
                }

                try
                {
                    publisher.PublishStatusActive(context.ClientId, context.TaskId, DateTime.Now);
                    
                    foreach (var model in models)
                    {
                        if (context.ServiceToken.IsCancellationRequested)
                            context.ServiceToken.ThrowIfCancellationRequested();

                        if (context.Token.IsCancellationRequested)
                            break;

                        var instance = LoadTaskInstance(info, model);
                        logger.InfoFormat("Executing task \"{0}\" for Client \"{1}\".", model.TaskName, model.ClientId);

                        publisher.PublishStatusActive(context.ClientId, model.TaskId, DateTime.Now);
                        instance.Execute(context.Token);
                        publisher.PublishStatusComplete(context.ClientId, model.TaskId, DateTime.Now);
                    }
                }
                catch (OperationCanceledException)
                {
                    if (context.ServiceToken.IsCancellationRequested)
                        context.ServiceToken.ThrowIfCancellationRequested();
                }
            }, token, TaskCreationOptions.LongRunning);

            task.ContinueWith(x => ContinueOnComplete(context), TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith(x => ContinueOnCancellation(context), TaskContinuationOptions.OnlyOnCanceled);
            task.ContinueWith(x => ContinueOnFault(x, context), TaskContinuationOptions.OnlyOnFaulted);

            contextProvider.TryAdd(context.TaskId, context);
            context.SetActiveTask(task, token);

            task.Start();
        }

        public void ExecuteTask(ActiveTaskContext context, TaskConfigurationModel model, CancellationToken token)
        {
            var task = new Task(() =>
            {
                var info = adminDataAccess.LoadClientInfo(context.ClientId);
                if (info == null)
                {
                    var message =
                        string.Format("Unable to load configuration for Client \"{0}\". Tasks will not be executed.",
                            context.ClientId);

                    throw new InvalidOperationException(message);
                }

                try
                {
                    var instance = LoadTaskInstance(info, model);
                    logger.TraceFormat("Executing task \"{0}\" for Client \"{1}\".", model.TaskName, model.ClientId);

                    publisher.PublishStatusActive(context.ClientId, context.TaskId, DateTime.Now);
                    instance.Execute(context.Token);
                }
                catch (OperationCanceledException)
                {
                    if (context.ServiceToken.IsCancellationRequested)
                        context.ServiceToken.ThrowIfCancellationRequested();
                }
            }, token, TaskCreationOptions.LongRunning);

            task.ContinueWith(x => ContinueOnComplete(context), TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith(x => ContinueOnCancellation(context), TaskContinuationOptions.OnlyOnCanceled);
            task.ContinueWith(x => ContinueOnFault(x, context), TaskContinuationOptions.OnlyOnFaulted);

            contextProvider.TryAdd(context.TaskId, context);
            context.SetActiveTask(task, token);

            task.Start();
        }

        private void ContinueOnComplete(ActiveTaskContext context)
        {
            var messageFormat = (!context.WasTaskCancelled)
                                    ? "Task \"{0}\" for Client {1} completed."
                                    : "Task \"{0}\" for Client {1} was cancelled. This task may not have completed.";

            logger.InfoFormat(messageFormat, context.TaskName, context.ClientId);
            if (context.WasTaskCancelled)
                publisher.PublishStatusCanceled(context.ClientId, context.TaskId, DateTime.Now);
            else
                publisher.PublishStatusComplete(context.ClientId, context.TaskId, DateTime.Now);
            
            ActiveTaskContext existingContext;
            if (!contextProvider.TryRemove(context.TaskId, out existingContext))
            {
                logger.WarnFormat(
                    "Unable to remove task \"{0}\" for Client {1}. This may occur if the task was already removed or evicted from the dictionary.",
                    context.TaskName, context.ClientId);

                return;
            }

            if (!context.WasTaskCancelled)
                scheduleProvider.PushTaskForConsideration(context.TaskId);

            context.Dispose();
        }

        private void ContinueOnCancellation(ActiveTaskContext context)
        {
            const string messageFormat = "Task \"{0}\" for Client {1} was asked to stop. This task shut down gracefully.";

            logger.InfoFormat(messageFormat, context.TaskName, context.ClientId);
            publisher.PublishStatusCanceled(context.ClientId, context.TaskId, DateTime.Now);
            
            ActiveTaskContext existingContext;
            if (!contextProvider.TryRemove(context.TaskId, out existingContext))
            {
                logger.WarnFormat(
                    "Unable to remove task \"{0}\" for Client {1}. This may occur if the task was already removed or evicted from the dictionary.",
                    context.TaskName, context.ClientId);

                return;
            }

            logger.WarnFormat("Removed task \"{0}\" attached to ClientId {1} from the dictionary.",
                context.TaskName, context.ClientId);

            context.Dispose();
        }

        private void ContinueOnFault(Task task, ActiveTaskContext context)
        {
            const string messageFormat = "Fatal error occured with task \"{0}\" for Client {1}.";

            logger.ErrorFormat(task.Exception, messageFormat, context.TaskName, context.ClientId);
            publisher.PublishException(context.ClientId, context.TaskId, DateTime.Now, task.Exception);
            publisher.PublishStatusFailed(context.ClientId, context.TaskId, DateTime.Now);

            var taskShouldBeConsidered = FaultIsSqlTimeout(task.Exception);

            ActiveTaskContext existing;
            if (!contextProvider.TryRemove(context.TaskId, out existing))
            {
                logger.WarnFormat(
                    "Unable to remove task \"{0}\" for Client {1}. This may occur if the task was already removed or evicted from the dictionary.",
                    context.TaskName, context.ClientId);

                return;
            }

            if (taskShouldBeConsidered)
                scheduleProvider.PushTaskForConsideration(context.TaskId);
            else
                logger.WarnFormat("Removed task \"{0}\" attached to ClientId {1} from the dictionary.",
                    context.TaskName, context.ClientId);

            context.Dispose();
        }

        private bool FaultIsSqlTimeout(AggregateException exception)
        {
            if (exception == null)
                return false;

            var exceptions = exception.InnerExceptions;
            var sqlExceptions = exceptions
                .Where(x => x is SqlException)
                .Cast<SqlException>()
                .ToArray();

            if (!sqlExceptions.Any())
                return false;

            for (var index = 0; index < sqlExceptions.Length; index++)
            {
                var sqlException = sqlExceptions[index];
                for (var errorIndex = 0; errorIndex < sqlException.Errors.Count; errorIndex++)
                {
                    var sqlError = sqlException.Errors[errorIndex];
                    if (sqlError.Number == -2)
                        return true;
                }
            }

            return false;
        }

        private IMiramarTask LoadTaskInstance(IMiramarClientInfo info, TaskConfigurationModel model)
        {
            var factory = ObjectFactory.GetAllInstances<IMiramarTaskFactory>()
                .FirstOrDefault(x => x.IsSatisfiedBy(model.TaskName));

            if (factory == null)
            {
                var message =
                string.Format("No corresponding Task Factory could be found for task \"{0}\". Task will not be executed.",
                    model.TaskName);

                throw new InvalidOperationException(message);
            }

            IMiramarTask instance;
            if (allowMetaData && model.HasMetaData)
            {
                instance = LoadTaskInstanceWithMetaData(factory, info, model);
                if (instance != null)
                    return instance;
            }

            instance = factory.GetTask(info, model.TaskName);
            if (instance == null)
            {
                var message =
                string.Format("Unable to create instance for task \"{0}\" for Client \"{1}\". Task will not be executed.",
                    model.TaskName, info.ClientId);

                throw new InvalidOperationException(message);
            }

            return instance;
        }

        private IMiramarTask LoadTaskInstanceWithMetaData(IMiramarTaskFactory factory, IMiramarClientInfo info, TaskConfigurationModel model)
        {
            var metaDataFactory = factory as IMiramarMetaDataTaskFactory;
            if (metaDataFactory == null)
            {
                logger.WarnFormat(
                    "The corresponding Task Factory for task \"{0}\" does not support instantiation with meta data. Task will be created without meta data.",
                    model.TaskName);

                return null;
            }

            if (!metaDataFactory.AllowsMetaDataConfiguration(model.TaskName))
            {
                logger.WarnFormat(
                    "Task \"{0}\" has been configured with meta data but is not supported. Task will be created without meta data.",
                    model.TaskName);

                return null;
            }

            var metaDataInstance = metaDataFactory.GetTask(info, model.TaskName, model.MetaData);
            if (metaDataInstance != null)
                return metaDataInstance;

            var message =
                string.Format(
                    "Unable to create instance for task \"{0}\" for Client \"{1}\" with meta data. Task will not be executed.",
                    model.TaskName, info.ClientId);

            throw new InvalidOperationException(message);
        }

        private void WaitOnTasks(Task[] tasks)
        {
            if (!tasks.Any())
                return;

            try
            {
                Task.WaitAll(tasks);
            }
            catch (AggregateException ex)
            {
                logger.WarnFormat(ex, "Expected error occurred while waiting for \"{0}\" task(s) to complete.",
                    tasks.Length);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(ex, "Unknown error occurred while waiting for \"{0}\" task(s) to complete.",
                    tasks.Length);
            }
        }
    }
}

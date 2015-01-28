using System;
using System.Collections.Generic;
using System.Linq;
using Ista.FileServices.Infrastructure;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.Service.Interfaces;
using Ista.FileServices.Service.Models;

namespace Ista.FileServices.Service
{
    public class MiramarScheduleProvider : IMiramarScheduleProvider
    {
        private readonly ILogger logger;
        private readonly IMiramarPublisher publisher;
        private readonly List<TaskScheduleModel> collection;
        private readonly Dictionary<string, DateTime> schedules;
        private readonly Dictionary<string, List<string>> executionPlans;

        public MiramarScheduleProvider(TaskScheduleModel[] collection)
        {
            this.collection = new List<TaskScheduleModel>(collection);
            
            logger = InfrastructureFactory.CreateLogger("Scheduler");
            publisher = new MiramarPublisher();

            schedules = new Dictionary<string, DateTime>();
            executionPlans = new Dictionary<string, List<string>>();

            PushTasksForInitialConsideration(collection);
        }

        public MiramarScheduleProvider(IMiramarPublisher publisher, TaskScheduleModel[] collection)
            : this(collection)
        {
            this.publisher = publisher;
        }

        public string[] IdentifyTasksToExecute(DateTime date)
        {
            lock (schedules)
            {
                return schedules
                    .Where(x => x.Value <= date)
                    .Select(x => x.Key)
                    .ToArray();
            }
        }

        public string[] IdentifyExecutionPlan(string taskId)
        {
            lock (executionPlans)
            {
                List<string> plans;
                if (executionPlans.TryGetValue(taskId, out plans) && plans.Count != 0)
                    return plans.ToArray();

                return new string[0];
            }
        }

        public bool IsScheduled(string taskId)
        {
            lock (schedules)
            {
                return schedules.Keys
                    .Any(x => x.Equals(taskId, StringComparison.OrdinalIgnoreCase));
            }
        }

        public void PopTaskFromConsideration(string taskId)
        {
            if (!schedules.ContainsKey(taskId))
                return;

            logger.DebugFormat("Removing task \"{0}\" from consideration.", taskId);
            publisher.PublishScheduleRemoved(taskId);

            lock (schedules)
            {
                schedules.Remove(taskId);
            }
        }

        public void PublishSchedules()
        {
            publisher.PublishSchedules(collection.ToArray(), schedules);
        }

        public void PushTaskForConsideration(string taskId)
        {
            lock (schedules)
            {
                if (schedules.ContainsKey(taskId))
                {
                    logger.WarnFormat("Task \"{0}\" has already been scheduled. Tash will not be considered.", taskId);
                    return;
                }
            }

            var model = collection.FirstOrDefault(x => x.TaskId.Equals(taskId, StringComparison.Ordinal));
            if (model == null)
            {
                logger.ErrorFormat("Task \"{0}\" was not found. Task cannot be considered.", taskId);
                return;
            }

            PushTaskForConsideration(model);
        }

        public void PushTaskForConsideration(TaskScheduleModel model)
        {
            var executionDate = model.IdentifyNextEntry(DateTime.Now);
            logger.DebugFormat("Task \"{0}\" next execution is: \"{1}\".", model.TaskId, executionDate);

            publisher.PublishScheduleAdded(model, executionDate);

            lock (schedules)
            {
                schedules[model.TaskId] = executionDate;
            }

            if (!model.IsContainer)
                return;

            var ordering = model.ScheduleItems
                .OrderBy(x => x.Order)
                .Select(x => x.TaskId)
                .ToArray();

            logger.DebugFormat("Task \"{0}\" is a container task. Execution plan: \"{1}\".",
                model.TaskId, string.Join(", ", ordering));

            lock (executionPlans)
            {
                var execution = new List<string>(ordering);
                executionPlans[model.TaskId] = execution;
            }
        }

        public void PushTasksForInitialConsideration(TaskScheduleModel[] models)
        {
            foreach (var model in models)
                PushTaskForInitialConsideration(model);
        }

        public void PushTaskForInitialConsideration(TaskScheduleModel model)
        {
            var executionDate = model.IdentifyNextEntry(DateTime.Now);
            if (model.IsContinuous)
                executionDate = DateTime.Now;

            logger.DebugFormat("Task \"{0}\" next execution is: \"{1}\".", model.TaskId, executionDate);

            schedules[model.TaskId] = executionDate;

            if (!model.IsContainer)
                return;

            var ordering = model.ScheduleItems
                .OrderBy(x => x.Order)
                .Select(x => x.TaskId)
                .ToArray();

            logger.DebugFormat("Task \"{0}\" is a container task. Execution plan: \"{1}\".",
                model.TaskId, string.Join(", ", ordering));

            lock (executionPlans)
            {
                var execution = new List<string>(ordering);
                executionPlans[model.TaskId] = execution;
            }
        }

        public void AddOrUpdateTaskSchedule(TaskScheduleModel model)
        {
            var existing = collection
                .FirstOrDefault(x => x.TaskId.Equals(model.TaskId));

            if (existing != null)
                collection.Remove(existing);

            collection.Add(model);

            if (!schedules.ContainsKey(model.TaskId))
                PushTaskForConsideration(model);
        }
    }
}

using System;
using System.Linq;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.Infrastructure.Serializers;
using Ista.FileServices.Service.Interfaces;
using Ista.FileServices.Service.Models;

namespace Ista.FileServices.Service.Handlers
{
    public class HandlerChangeSchedule : IScheduleMessageHandler
    {
        public bool IsSatisfiedBy(IIstaMessage message)
        {
            if (!message.Type.Equals("schedule", StringComparison.OrdinalIgnoreCase))
                return false;

            return (message.Action.Equals("change", StringComparison.OrdinalIgnoreCase));
        }

        public bool Handle(IMiramarTaskProvider taskProvider, IMiramarScheduleProvider scheduleProvider, IMiramarContextProvider contextProvider, IIstaMessage message)
        {
            var request = JsonMessageSerializer.DeserializeType<TaskScheduleRequestModel>(message.Body);
            if (request == null)
                return false;

            var requestedTask = taskProvider.LoadTaskConfiguration(request.TaskId);
            if (requestedTask == null)
                return false;

            var schedule = request.Type.Equals("continuous", StringComparison.OrdinalIgnoreCase)
                               ? CreateContinuousSchedule(request)
                               : CreateDateTimeSchedule(request);

            if (request.Items != null && request.Items.Count != 0)
            {
                foreach (var item in request.Items)
                    schedule.AddScheduleItem(new TaskScheduleItemModel
                    {
                        Order = item.Order,
                        TaskId = item.TaskId,
                    });
            }

            scheduleProvider.PopTaskFromConsideration(request.TaskId);
            scheduleProvider.AddOrUpdateTaskSchedule(schedule);
            return true;
        }

        public TaskScheduleModel CreateContinuousSchedule(TaskScheduleRequestModel model)
        {
            var schedule = new TaskScheduleContinuousModel
            {
                TaskId = model.TaskId,
                IdlePeriod = 15,
                IdlePeriodType = "minutes",
            };

            var idleSchedule = model.Schedules
                .FirstOrDefault(x => x.Type.Equals("idle", StringComparison.OrdinalIgnoreCase));

            if (idleSchedule == null || idleSchedule.Periods == null)
                return schedule;

            var periods = idleSchedule.Periods.ToArray();
            if (!periods.Any())
                return schedule;

            var periodType = idleSchedule.PeriodType;
            if (string.IsNullOrWhiteSpace(periodType))
                return schedule;

            var periodTypes = new[] { "seconds", "minutes", "hours", "days" };
            if (!periodTypes.Contains(periodType, StringComparer.OrdinalIgnoreCase))
                return schedule;

            schedule.IdlePeriod = periods.First();
            schedule.IdlePeriodType = periodType;
            return schedule;
        }

        public TaskScheduleModel CreateDateTimeSchedule(TaskScheduleRequestModel model)
        {
            var schedule = new TaskScheduleDateTimeModel
            {
                TaskId = model.TaskId,
                DaysConsidered = Enumerable.Range(0, 7).ToArray(),
                HoursConsidered = Enumerable.Range(0, 24).ToArray(),
                MinutesConsidered = new[] { 0 },
            };

            if (model.Schedules == null || model.Schedules.Count == 0)
                return schedule;

            ParseIntervalSchedule(model, schedule);
            ParseIncludeSchedules(model, schedule);
            ParseExcludeSchedules(model, schedule);

            return schedule;
        }

        public void ParseIntervalSchedule(TaskScheduleRequestModel model, TaskScheduleDateTimeModel schedule)
        {
            var intervalSchedule = model.Schedules
                .FirstOrDefault(x => x.Type.Equals("interval", StringComparison.OrdinalIgnoreCase));

            if (intervalSchedule == null)
                return;

            var periods = intervalSchedule.Periods.ToArray();
            if (!periods.Any())
                return;

            var periodType = intervalSchedule.PeriodType;
            if (string.IsNullOrWhiteSpace(periodType))
                return;

            var period = periods.First();
            switch (periodType.Trim().ToUpper())
            {
                case "MINUTES":
                    schedule.MinutesConsidered = Enumerable.Range(0, 60)
                        .Where(x => (x % period) == 0)
                        .ToArray();
                    break;
                case "HOURS":
                    schedule.HoursConsidered = Enumerable.Range(0, 24)
                        .Where(x => (x % period) == 0)
                        .ToArray();
                    break;
            }
        }

        public void ParseIncludeSchedules(TaskScheduleRequestModel model, TaskScheduleDateTimeModel schedule)
        {
            var includeSchedules = model.Schedules
                .Where(x => x.Type.Equals("include", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (!includeSchedules.Any())
                return;

            foreach (var includeSchedule in includeSchedules)
            {
                var periodType = includeSchedule.PeriodType;
                if (string.IsNullOrWhiteSpace(periodType))
                    continue;

                var periods = includeSchedule.Periods.ToArray();
                if (!periods.Any())
                    continue;

                switch (periodType.Trim().ToUpper())
                {
                    case "HOURLY":
                        schedule.MinutesConsidered = periods;
                        break;
                    case "DAILY":
                        schedule.HoursConsidered = periods;
                        break;
                    case "WEEKLY":
                        schedule.DaysConsidered = periods;
                        break;
                }
            }
        }

        public void ParseExcludeSchedules(TaskScheduleRequestModel model, TaskScheduleDateTimeModel schedule)
        {
            var excludeSchedules = model.Schedules
                .Where(x => x.Type.Equals("exclude", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (!excludeSchedules.Any())
                return;

            foreach (var excludeSchedule in excludeSchedules)
            {
                var periodType = excludeSchedule.PeriodType;
                if (string.IsNullOrWhiteSpace(periodType))
                    continue;

                var periods = excludeSchedule.Periods.ToArray();
                if (!periods.Any())
                    continue;

                switch (periodType.Trim().ToUpper())
                {
                    case "HOURLY":
                        if (schedule.MinutesConsidered.Length == 0)
                            schedule.MinutesConsidered = Enumerable.Range(0, 60).ToArray();
                        schedule.MinutesConsidered = schedule.MinutesConsidered
                            .Except(periods)
                            .ToArray();
                        break;
                    case "DAILY":
                        if (schedule.HoursConsidered.Length == 0)
                            schedule.HoursConsidered = Enumerable.Range(0, 24).ToArray();
                        schedule.HoursConsidered = schedule.HoursConsidered
                            .Except(periods)
                            .ToArray();
                        break;
                    case "WEEKLY":
                        if (schedule.DaysConsidered.Length == 0)
                            schedule.DaysConsidered = Enumerable.Range(0, 7).ToArray();
                        schedule.DaysConsidered = schedule.DaysConsidered
                            .Except(periods)
                            .ToArray();
                        break;
                }
            }
        }
    }
}

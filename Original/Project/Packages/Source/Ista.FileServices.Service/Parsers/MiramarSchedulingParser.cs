using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Ista.FileServices.Service.Elements;
using Ista.FileServices.Service.Enumerations;
using Ista.FileServices.Service.Interfaces;
using Ista.FileServices.Service.Models;

namespace Ista.FileServices.Service.Parsers
{
    public class MiramarSchedulingParser
    {
        private readonly MiramarVisitor visitor;

        public MiramarSchedulingParser(MiramarVisitor visitor)
        {
            this.visitor = visitor;
        }

        public IMiramarScheduleProvider CreateScheduleProvider(string path)
        {
            var publisher = new MiramarPublisher();
            return CreateScheduleProvider(publisher, path);
        }

        public IMiramarScheduleProvider CreateScheduleProvider(IMiramarPublisher publisher, string path)
        {
            var fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
                return new MiramarScheduleProvider(publisher, new TaskScheduleModel[0]);

            using (var stream = fileInfo.OpenRead())
            {
                var document = XDocument.Load(stream);
                var element = visitor.VisitExecution(document);

                return CreateScheduleProvider(publisher, element);
            }
        }

        public IMiramarScheduleProvider CreateScheduleProvider(IMiramarPublisher publisher, ExecutionElement element)
        {
            if (element == null)
                return new MiramarScheduleProvider(publisher, new TaskScheduleModel[0]);

            var collection = element.ExecutionTypes
                .Select(CreateModel)
                .ToArray();

            return new MiramarScheduleProvider(publisher, collection);
        }

        public TaskScheduleModel CreateModel(ExecutionTypeElement element)
        {
            var type = element.Type;
            if (type == ExecutionTypes.Continuous)
            {
                var continuousElement = element as ContinuousElement;
                if (continuousElement == null)
                    throw new InvalidCastException();

                return CreateContinousModel(continuousElement);
            }

            if (type == ExecutionTypes.Scheduled)
            {
                var scheduledElement = element as ScheduledElement;
                if (scheduledElement == null)
                    throw new InvalidCastException();

                return CreateScheduleModel(scheduledElement);
            }

            return CreateDefaultModel(element);
        }

        public TaskScheduleModel CreateDefaultModel(ExecutionTypeElement element)
        {
            var model = new TaskScheduleContinuousModel
            {
                TaskId = element.Id,
                IdlePeriod = 15,
                IdlePeriodType = "minutes",
            };

            foreach (var subModel in element.ExecutionList)
            {
                model.AddScheduleItem(new TaskScheduleItemModel
                {
                    TaskId = subModel.Id,
                    Order = subModel.Order,
                });
            }

            return model;
        }

        public TaskScheduleModel CreateContinousModel(ContinuousElement element)
        {
            var model = new TaskScheduleContinuousModel
            {
                TaskId = element.Id,
                IdlePeriod = element.IdlePeriod,
                IdlePeriodType = element.IdlePeriodType
            };

            foreach (var subModel in element.ExecutionList)
            {
                model.AddScheduleItem(new TaskScheduleItemModel
                {
                    TaskId = subModel.Id,
                    Order = subModel.Order,

                });
            }

            return model;
        }

        public TaskScheduleModel CreateScheduleModel(ScheduledElement element)
        {
            var model = new TaskScheduleDateTimeModel
            {
                TaskId = element.Id,
                MinutesConsidered = element.MinutesConsidered,
                HoursConsidered = element.HoursConsidered,
                DaysConsidered = element.DaysConsidered,
            };

            foreach (var subModel in element.ExecutionList)
            {
                model.AddScheduleItem(new TaskScheduleItemModel
                {
                    TaskId = subModel.Id,
                    Order = subModel.Order,

                });
            }

            return model;
        }

        public static IMiramarScheduleProvider ParseSchedule(string path)
        {
            var visitor = new MiramarVisitor();
            var parser = new MiramarSchedulingParser(visitor);

            return parser.CreateScheduleProvider(path);
        }

        public static IMiramarScheduleProvider ParseSchedule(IMiramarPublisher publisher, string path)
        {
            var visitor = new MiramarVisitor();
            var parser = new MiramarSchedulingParser(visitor);

            return parser.CreateScheduleProvider(publisher, path);
        }
    }
}
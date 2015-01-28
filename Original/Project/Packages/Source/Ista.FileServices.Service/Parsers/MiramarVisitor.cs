using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Ista.FileServices.Service.Elements;
using Ista.FileServices.Service.Enumerations;

namespace Ista.FileServices.Service.Parsers
{
    public class MiramarVisitor
    {
        private MetaElement metaData;

        public ConfigurationElement VisitConfiguration(XDocument document)
        {
            var item = new ConfigurationElement();
            
            var root = document.Root;
            if (root == null || !root.HasElements)
                return item;

            var metaElement = root.Element("Meta");
            if (metaElement != null)
                VisitMeta(metaElement);

            var configurationElement = root.Element("Configuration");
            if (configurationElement != null)
                VisitConfiguration(item, configurationElement);

            return item;
        }

        public ExecutionElement VisitExecution(XDocument document)
        {
            var item = new ExecutionElement();

            var root = document.Root;
            if (root == null || !root.HasElements)
                return item;

            var metaElement = root.Element("Meta");
            if (metaElement != null)
                VisitMeta(metaElement);

            var executionElement = root.Element("Execution");
            if (executionElement != null)
                VisitExecution(item, executionElement);

            return item;
        }

        public void VisitMeta(XElement element)
        {
            metaData = new MetaElement();

            var entries = element.Elements("Entry")
                .Where(x => x.Attribute("key") != null && x.Attribute("value") != null);

            foreach (var entry in entries)
                metaData.AddEntry((string)entry.Attribute("key"), (string)entry.Attribute("value"));
        }

        public void VisitConfiguration(ConfigurationElement item, XContainer container)
        {
            foreach (var element in container.Elements())
            {
                var name = element.Name.LocalName;

                ElementTypes type;
                if (!Enum.TryParse(name, out type))
                    continue;

                switch (type)
                {
                    case ElementTypes.PerClient:
                        VisitPerClientTask(item, element);
                        break;
                    case ElementTypes.Task:
                        VisitTask(item, element);
                        break;
                }
            }
        }

        public void VisitExecution(ExecutionElement item, XContainer container)
        {
            foreach (var element in container.Elements())
            {
                var name = element.Name.LocalName;

                ElementTypes type;
                if (!Enum.TryParse(name, out type))
                    continue;

                switch (type)
                {
                    case ElementTypes.PerClient:
                        VisitPerClientSchedule(item, element);
                        break;
                    case ElementTypes.Continuous:
                        VisitContinuous(item, element);
                        break;
                    case ElementTypes.Scheduled:
                        VisitScheduled(item, element);
                        break;
                }
            }
        }

        public void VisitScheduled(ScheduledElement item, XContainer container)
        {
            foreach (var element in container.Elements())
            {
                var name = element.Name.LocalName;

                ElementTypes type;
                if (!Enum.TryParse(name, out type))
                    continue;

                switch (type)
                {
                    case ElementTypes.IntervalSchedule:
                        VisitIntervalSchedule(item, element);
                        break;
                    case ElementTypes.WeeklySchedule:
                        VisitWeeklySchedule(item, element);
                        break;
                    case ElementTypes.HourlySchedule:
                        VisitHourlySchedule(item, element);
                        break;
                    case ElementTypes.DailySchedule:
                        VisitDailySchedule(item, element);
                        break;
                }
            }
        }

        public void VisitExecutionOrder(ExecutionTypeElement item, XContainer container)
        {
            foreach (var element in container.Elements())
            {
                var name = element.Name.LocalName;

                ElementTypes type;
                if (!Enum.TryParse(name, out type))
                    continue;

                if (type != ElementTypes.Item)
                    continue;

                var order = new ExecutionTypeItemElement
                {
                    Id = (string)element.Attribute("taskId"),
                    Order = (int)element.Attribute("order"),
                };

                if (item.ClientId.HasValue)
                    order.Id = string.Format("client.{0}.{1}", item.ClientId, order.Id);

                item.AddItem(order);
            }
        }

        public void VisitPerClientTask(ConfigurationElement parent, XElement element)
        {
            if (!element.HasElements)
                return;

            if (!metaData.HasEntries)
            {
                VisitConfiguration(parent, element);
                return;
            }

            string metaClientIdentifiers;
            if (!metaData.TryGetEntry("ClientIdentifiers", out metaClientIdentifiers))
            {
                VisitConfiguration(parent, element);
                return;
            }

            var clientIdentifiers = metaClientIdentifiers.Split(',')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .ToArray();

            foreach (var clientIdentifier in clientIdentifiers)
            {
                var clientElement = new XElement(element);
                clientElement.Descendants()
                    .Where(x => x.Attribute("id") != null)
                    .ToList()
                    .ForEach(x => x.Add(new XAttribute("clientId", clientIdentifier)));

                VisitConfiguration(parent, clientElement);
            }
        }

        public void VisitTask(ConfigurationElement parent, XElement element)
        {
            var item = CreateTask(element);
            parent.AddTask(item);

            if (!element.HasElements)
                return;

            foreach (var childElement in element.Elements("Task"))
                VisitTask(item, childElement);
        }

        public void VisitTask(TaskElement parent, XElement element)
        {
            var item = CreateTask(parent, element);
            parent.AddSubTask(item);
        }

        public void VisitPerClientSchedule(ExecutionElement parent, XElement element)
        {
            if (!element.HasElements)
                return;

            if (!metaData.HasEntries)
            {
                VisitExecution(parent, element);
                return;
            }

            string metaClientIdentifiers;
            if (!metaData.TryGetEntry("ClientIdentifiers", out metaClientIdentifiers))
            {
                VisitExecution(parent, element);
                return;
            }

            var clientIdentifiers = metaClientIdentifiers.Split(',')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .ToArray();

            foreach (var clientIdentifier in clientIdentifiers)
            {
                var clientElement = new XElement(element);
                clientElement.Descendants()
                    .Where(x => x.Attribute("taskId") != null)
                    .ToList()
                    .ForEach(x => x.Add(new XAttribute("clientId", clientIdentifier)));

                VisitExecution(parent, clientElement);
            }
        }

        public void VisitContinuous(ExecutionElement parent, XElement element)
        {
            var item = new ContinuousElement
            {
                Id = (string)element.Attribute("taskId"),
                IdlePeriod = (int)element.Attribute("idlePeriod"),
                IdlePeriodType = (string)element.Attribute("idlePeriodType"),
                ClientId = (int?)element.Attribute("clientId"),
            };

            parent.AddExecutionType(item);

            if (item.ClientId.HasValue)
                item.Id = string.Format("client.{0}.{1}", item.ClientId, item.Id);
            else
            {
                if (!item.ClientId.HasValue && metaData.HasEntries)
                {
                    string metaClientId;
                    if (!metaData.TryGetEntry("ClientId", out metaClientId))
                    {
                        int clientId;
                        if (int.TryParse(metaClientId, out clientId))
                            item.ClientId = clientId;
                    }
                }
            }

            if (!element.HasElements)
                return;

            VisitExecutionOrder(item, element);
        }

        public void VisitScheduled(ExecutionElement parent, XElement element)
        {
            var item = new ScheduledElement
            {
                Id = (string)element.Attribute("taskId"),
                ClientId = (int?)element.Attribute("clientId"),
                DaysConsidered = Enumerable.Range(0, 7).ToArray(),
                HoursConsidered = Enumerable.Range(0, 24).ToArray(),
                MinutesConsidered = new[] { 0 },
            };

            parent.AddExecutionType(item);

            if (item.ClientId.HasValue)
                item.Id = string.Format("client.{0}.{1}", item.ClientId, item.Id);
            else
            {
                if (!item.ClientId.HasValue && metaData.HasEntries)
                {
                    string metaClientId;
                    if (!metaData.TryGetEntry("ClientId", out metaClientId))
                    {
                        int clientId;
                        if (int.TryParse(metaClientId, out clientId))
                            item.ClientId = clientId;
                    }
                }
            }

            if (!element.HasElements)
                return;

            VisitScheduled(item, element);
            VisitExecutionOrder(item, element);
        }

        public void VisitIntervalSchedule(ScheduledElement parent, XElement element)
        {
            var period = (int)element.Attribute("period");
            var periodType = (string)element.Attribute("periodType");

            switch (periodType)
            {
                case "seconds":
                    parent.SecondsConsidered = Enumerable.Range(0, 60)
                        .Where(x => (x % period) == 0)
                        .ToArray();
                    break;
                case "minutes":
                    parent.MinutesConsidered = Enumerable.Range(0, 60)
                        .Where(x => (x % period) == 0)
                        .ToArray();
                    break;
                case "hours":
                    parent.HoursConsidered = Enumerable.Range(0, 24)
                        .Where(x => (x % period) == 0)
                        .ToArray();
                    break;
            }
        }

        public void VisitWeeklySchedule(ScheduledElement parent, XElement element)
        {
            if (element.Attribute("includeDays") != null)
            {
                var include = new List<int>();
                var includeAttribute = (string)element.Attribute("includeDays");
                includeAttribute.Split(',')
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList()
                    .ForEach(x =>
                    {
                        int value;
                        if (int.TryParse(x, out value) && value >= 0 && value <= 6)
                            include.Add(value);
                    });

                parent.DaysConsidered = include.ToArray();
            }

            if (parent.DaysConsidered.Length == 0)
                parent.DaysConsidered = Enumerable.Range(0, 7).ToArray();

            if (element.Attribute("excludeDays") == null)
                return;

            var exclude = new List<int>();
            var excludeAttribute = (string)element.Attribute("excludeDays");
            excludeAttribute.Split(',')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList()
                .ForEach(x =>
                {
                    int value;
                    if (int.TryParse(x, out value) && value >= 0 && value <= 6)
                        exclude.Add(value);
                });

            parent.DaysConsidered = parent.DaysConsidered
                .Except(exclude)
                .ToArray();
        }

        public void VisitDailySchedule(ScheduledElement parent, XElement element)
        {
            if (element.Attribute("includeHours") != null)
            {
                var include = new List<int>();
                var includeAttribute = (string)element.Attribute("includeHours");
                includeAttribute.Split(',')
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList()
                    .ForEach(x =>
                    {
                        int value;
                        if (int.TryParse(x, out value) && value >= 0 && value <= 23)
                            include.Add(value);
                    });

                parent.HoursConsidered = include.ToArray();
            }

            if (parent.HoursConsidered.Length == 0)
                parent.HoursConsidered = Enumerable.Range(0, 24).ToArray();

            if (element.Attribute("excludeHours") == null)
                return;

            var exclude = new List<int>();
            var excludeAttribute = (string)element.Attribute("excludeHours");
            excludeAttribute.Split(',')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList()
                .ForEach(x =>
                {
                    int value;
                    if (int.TryParse(x, out value) && value >= 0 && value <= 23)
                        exclude.Add(value);
                });

            parent.HoursConsidered = parent.HoursConsidered
                .Except(exclude)
                .ToArray();
        }

        public void VisitHourlySchedule(ScheduledElement parent, XElement element)
        {
            if (element.Attribute("includeMinutes") != null)
            {
                var include = new List<int>();
                var includeAttribute = (string)element.Attribute("includeMinutes");
                includeAttribute.Split(',')
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList()
                    .ForEach(x =>
                    {
                        int value;
                        if (int.TryParse(x, out value) && value >= 0 && value <= 59)
                            include.Add(value);
                    });

                parent.MinutesConsidered = include.ToArray();
            }

            if (parent.MinutesConsidered.Length == 0)
                parent.MinutesConsidered = Enumerable.Range(0, 60).ToArray();

            if (element.Attribute("excludeMinutes") == null)
                return;

            if (parent.MinutesConsidered.Length == 1 && parent.MinutesConsidered[0] == 0)
                parent.MinutesConsidered = Enumerable.Range(0, 60).ToArray();

            var exclude = new List<int>();
            var excludeAttribute = (string)element.Attribute("excludeMinutes");
            excludeAttribute.Split(',')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList()
                .ForEach(x =>
                {
                    int value;
                    if (int.TryParse(x, out value) && value >= 0 && value <= 59)
                        exclude.Add(value);
                });

            parent.MinutesConsidered = parent.MinutesConsidered
                .Except(exclude)
                .ToArray();
        }

        public TaskElement CreateTask(XElement element)
        {
            var item = new TaskElement
            {
                Id = (string)element.Attribute("id"),
                Name = (string)element.Attribute("id"),
                DisplayName = (string)element.Attribute("displayName"),
                ClientId = (int?)element.Attribute("clientId"),
            };

            if (element.Attribute("name") != null)
                item.Name = (string)element.Attribute("name");

            if (element.Attribute("logName") != null)
                item.LogName = (string)element.Attribute("logName");

            if (element.HasElements && element.Element("Meta") != null)
                CreateTaskMetaData(item, element);

            if (item.ClientId.HasValue)
            {
                item.Id = string.Format("client.{0}.{1}", item.ClientId, item.Id);
                return item;
            }

            if (!metaData.HasEntries)
                return item;

            string metaClientId;
            if (!metaData.TryGetEntry("ClientId", out metaClientId))
                return item;

            int clientId;
            if (int.TryParse(metaClientId, out clientId))
                item.ClientId = clientId;

            return item;
        }

        public TaskElement CreateTask(TaskElement parent, XElement element)
        {
            var item = new TaskElement
            {
                Id = (string)element.Attribute("id"),
                Name = (string)element.Attribute("id"),
                DisplayName = (string)element.Attribute("displayName"),
                ClientId = parent.ClientId,
            };

            if (element.Attribute("name") != null)
                item.Name = (string)element.Attribute("name");

            if (element.Attribute("logName") != null)
                item.LogName = (string)element.Attribute("logName");
            else
                item.LogName = parent.LogName;

            if (element.HasElements && element.Element("Meta") != null)
                CreateTaskMetaData(item, element);

            if (item.ClientId.HasValue)
                item.Id = string.Format("client.{0}.{1}", item.ClientId, item.Id);

            return item;
        }

        public void CreateTaskMetaData(TaskElement item, XElement element)
        {
            var metaElement = element.Element("Meta");
            if (metaElement == null)
                return;

            var entries = metaElement.Elements("Entry")
                .Where(x => x.Attribute("key") != null && x.Attribute("value") != null);

            foreach (var entry in entries)
                item.AddMetaData((string)entry.Attribute("key"), (string)entry.Attribute("value"));
        }
    }
}

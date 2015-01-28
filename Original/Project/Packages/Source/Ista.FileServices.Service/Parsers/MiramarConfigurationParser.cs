using System.IO;
using System.Linq;
using System.Xml.Linq;
using Ista.FileServices.Service.Elements;
using Ista.FileServices.Service.Interfaces;
using Ista.FileServices.Service.Models;

namespace Ista.FileServices.Service.Parsers
{
    public class MiramarConfigurationParser
    {
        private readonly MiramarVisitor visitor;

        public MiramarConfigurationParser(MiramarVisitor visitor)
        {
            this.visitor = visitor;
        }

        public IMiramarTaskProvider CreateTaskProvider(string path)
        {
            var publisher = new MiramarPublisher();
            return CreateTaskProvider(publisher, path);
        }

        public IMiramarTaskProvider CreateTaskProvider(IMiramarPublisher publisher, string path)
        {
            var fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
                return new MiramarTaskProvider(publisher, new TaskConfigurationModel[0]);

            using (var stream = fileInfo.OpenRead())
            {
                var document = XDocument.Load(stream);
                var element = visitor.VisitConfiguration(document);

                return CreateTaskProvider(publisher, element);
            }
        }

        public IMiramarTaskProvider CreateTaskProvider(IMiramarPublisher publisher, ConfigurationElement element)
        {
            if (element == null)
                return new MiramarTaskProvider(publisher, new TaskConfigurationModel[0]);

            var collection = element.Tasks
                .Select(CreateModel)
                .ToArray();

            return new MiramarTaskProvider(publisher, collection);
        }

        public TaskConfigurationModel CreateModel(TaskElement element)
        {
            var item = new TaskConfigurationModel
            {
                TaskId = element.Id,
                TaskName = element.Name,
                DisplayName = element.DisplayName,
                ClientId = element.ClientId ?? 1,
            };

            if (!string.IsNullOrEmpty(element.LogName))
                item.LogName = element.LogName;

            if (element.HasMetaData)
                foreach(var metaItem in element.MetaData)
                    item.AddMetaData(metaItem.Key, metaItem.Value);

            if (element.IsContainer)
            {
                foreach (var taskElement in element.SubTasks)
                {
                    var child = CreateModel(taskElement);
                    item.AddSubTask(child);
                }
            }

            return item;
        }

        public static IMiramarTaskProvider ParseConfiguration(string path)
        {
            var visitor = new MiramarVisitor();
            var parser = new MiramarConfigurationParser(visitor);

            return parser.CreateTaskProvider(path);
        }

        public static IMiramarTaskProvider ParseConfiguration(IMiramarPublisher publisher, string path)
        {
            var visitor = new MiramarVisitor();
            var parser = new MiramarConfigurationParser(visitor);

            return parser.CreateTaskProvider(publisher, path);
        }
    }
}

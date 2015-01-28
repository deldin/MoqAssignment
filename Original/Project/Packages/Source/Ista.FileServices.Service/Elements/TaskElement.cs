using System.Collections.Generic;
using System.Linq;
using Ista.FileServices.Service.Enumerations;

namespace Ista.FileServices.Service.Elements
{
    public class TaskElement : AbstractElement
    {
        private readonly List<TaskElement> collection;
        private readonly Dictionary<string, string> dictionary; 
        
        public override ElementTypes ElementType
        {
            get { return ElementTypes.Task; }
        }

        public bool IsContainer
        {
            get { return collection.Any(); }
        }

        public bool HasMetaData
        {
            get { return (dictionary.Count != 0); }
        }

        public KeyValuePair<string, string>[] MetaData
        {
            get { return dictionary.ToArray(); }
        } 

        public TaskElement[] SubTasks
        {
            get { return collection.ToArray(); }
        }

        public int? ClientId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string LogName { get; set; }

        public TaskElement()
        {
            collection = new List<TaskElement>();
            dictionary = new Dictionary<string, string>();
        }

        public void AddMetaData(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            dictionary[key] = value;
        }

        public void AddSubTask(TaskElement item)
        {
            collection.Add(item);
        }
    }
}

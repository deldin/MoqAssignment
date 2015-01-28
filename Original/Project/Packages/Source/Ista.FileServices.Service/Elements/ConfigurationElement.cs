using System.Collections.Generic;
using Ista.FileServices.Service.Enumerations;

namespace Ista.FileServices.Service.Elements
{
    public class ConfigurationElement : AbstractElement
    {
        private readonly List<TaskElement> collection;
 
        public override ElementTypes ElementType
        {
            get { return ElementTypes.Configuration; }
        }

        public TaskElement[] Tasks
        {
            get { return collection.ToArray(); }
        }

        public ConfigurationElement()
        {
            collection = new List<TaskElement>();
        }

        public void AddTask(TaskElement item)
        {
            collection.Add(item);
        }
    }
}

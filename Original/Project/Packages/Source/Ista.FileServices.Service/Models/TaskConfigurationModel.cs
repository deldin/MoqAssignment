using System.Collections.Generic;
using System.Linq;

namespace Ista.FileServices.Service.Models
{
    public class TaskConfigurationModel
    {
        private readonly List<TaskConfigurationModel> collection;
        private readonly Dictionary<string, string> dictionary; 

        public int ClientId { get; set; }
        public string TaskId { get; set; }
        public string TaskName { get; set; }
        public string DisplayName { get; set; }
        public string LogName { get; set; }

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

        public TaskConfigurationModel[] SubTasks
        {
            get { return collection.ToArray(); }
        }
        
        public TaskConfigurationModel()
        {
            collection = new List<TaskConfigurationModel>();
            dictionary = new Dictionary<string, string>();
        }

        public void AddMetaData(string key, string value)
        {
            dictionary[key] = value;
        }

        public void AddSubTask(TaskConfigurationModel item)
        {
            collection.Add(item);
        }
    }
}

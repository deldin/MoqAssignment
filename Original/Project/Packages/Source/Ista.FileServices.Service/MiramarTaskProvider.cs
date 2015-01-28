using System;
using System.Linq;
using Ista.FileServices.Service.Interfaces;
using Ista.FileServices.Service.Models;

namespace Ista.FileServices.Service
{
    public class MiramarTaskProvider : IMiramarTaskProvider
    {
        private readonly IMiramarPublisher publisher;
        private readonly TaskConfigurationModel[] collection;

        public MiramarTaskProvider(TaskConfigurationModel[] collection)
        {
            this.collection = collection;

            publisher = new MiramarPublisher();
        }

        public MiramarTaskProvider(IMiramarPublisher publisher, TaskConfigurationModel[] collection)
            : this(collection)
        {
            this.publisher = publisher;
        }

        public TaskConfigurationModel LoadTaskConfiguration(string taskId)
        {
            return collection.FirstOrDefault(x => x.TaskId.Equals(taskId, StringComparison.OrdinalIgnoreCase));
        }

        public TaskConfigurationModel LoadSubTaskConfiguration(string taskId, string subTaskId)
        {
            return collection
                .Where(x => x.IsContainer && x.TaskId.Equals(taskId, StringComparison.OrdinalIgnoreCase))
                .SelectMany(x => x.SubTasks)
                .FirstOrDefault(x => x.TaskId.Equals(subTaskId, StringComparison.OrdinalIgnoreCase));
        }

        public void PublishConfiguration()
        {
            publisher.PublishTaskConfiguration(collection);
        }
    }
}

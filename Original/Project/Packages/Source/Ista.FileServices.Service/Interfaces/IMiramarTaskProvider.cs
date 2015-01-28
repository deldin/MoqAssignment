using Ista.FileServices.Service.Models;

namespace Ista.FileServices.Service.Interfaces
{
    public interface IMiramarTaskProvider
    {
        TaskConfigurationModel LoadTaskConfiguration(string taskId);
        TaskConfigurationModel LoadSubTaskConfiguration(string taskId, string subTaskId);
        void PublishConfiguration();
    }
}

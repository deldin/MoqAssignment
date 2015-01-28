using Ista.FileServices.Service.Contexts;

namespace Ista.FileServices.Service.Interfaces
{
    public interface IMiramarContextProvider
    {
        ActiveTaskContext[] GetRunningContexts();
        bool Any();
        bool CancelTask(string taskId);
        bool TryAdd(string taskId, ActiveTaskContext context);
        bool TryGetValue(string taskId, out ActiveTaskContext context);
        bool TryRemove(string taskId, out ActiveTaskContext context);
    }
}

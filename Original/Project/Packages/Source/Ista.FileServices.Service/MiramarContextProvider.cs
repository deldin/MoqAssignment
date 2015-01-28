using System.Collections.Concurrent;
using System.Linq;
using Ista.FileServices.Service.Contexts;
using Ista.FileServices.Service.Interfaces;

namespace Ista.FileServices.Service
{
    public class MiramarContextProvider : IMiramarContextProvider
    {
        private readonly ConcurrentDictionary<string, ActiveTaskContext> collection;
 
        public MiramarContextProvider()
        {
            collection = new ConcurrentDictionary<string, ActiveTaskContext>();
        }

        public ActiveTaskContext[] GetRunningContexts()
        {
            return collection.Values
                .Where(x => x.IsRunning)
                .ToArray();
        }

        public bool Any()
        {
            return collection.Any();
        }

        public bool CancelTask(string taskId)
        {
            ActiveTaskContext context;
            if (collection.TryGetValue(taskId, out context))
            {
                context.CancelTask();
                return true;
            }

            return false;
        }

        public bool TryAdd(string taskId, ActiveTaskContext context)
        {
            return collection.TryAdd(taskId, context);
        }

        public bool TryGetValue(string taskId, out ActiveTaskContext context)
        {
            return collection.TryGetValue(taskId, out context);
        }

        public bool TryRemove(string taskId, out ActiveTaskContext context)
        {
            return collection.TryRemove(taskId, out context);
        }
    }
}

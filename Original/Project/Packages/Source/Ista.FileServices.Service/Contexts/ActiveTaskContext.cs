using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ista.FileServices.Service.Contexts
{
    public class ActiveTaskContext
    {
        private Task activeTask;
        private CancellationTokenSource linkedSource;
        private CancellationTokenSource internalSource;

        public int ClientId { get; private set; }
        public string TaskId { get; private set; }
        public string TaskName { get; private set; }

        public DateTime StartDate { get; private set; }
        public CancellationToken ServiceToken { get; private set; }

        public bool IsRunning
        {
            get { return (activeTask != null && (activeTask.Status == TaskStatus.Running)); }
        }

        public Task RunningTask
        {
            get { return activeTask; }
        }

        public CancellationToken Token
        {
            get { return linkedSource.Token; }
        }

        public bool WasTaskCancelled
        {
            get { return (linkedSource.IsCancellationRequested); }
        }

        public bool WasServiceCancelled
        {
            get { return (ServiceToken.IsCancellationRequested); }
        }

        public void Dispose()
        {
            if (activeTask != null)
            {
                activeTask.Dispose();
                activeTask = null;
            }

            if (linkedSource != null)
            {
                linkedSource.Dispose();
                linkedSource = null;
            }

            if (internalSource != null)
            {
                internalSource.Dispose();
                internalSource = null;
            }
        }

        public void CancelTask()
        {
            internalSource.Cancel();
        }

        public void SetActiveTask(Task task, CancellationToken token)
        {
            activeTask = task;

            internalSource = new CancellationTokenSource();
            linkedSource = CancellationTokenSource.CreateLinkedTokenSource(token, internalSource.Token);

            StartDate = DateTime.Now;
            ServiceToken = token;
        }

        public static ActiveTaskContext Create(int clientId, string taskId, string taskName)
        {
            return new ActiveTaskContext
            {
                ClientId = clientId,
                TaskId = taskId,
                TaskName = taskName,
            };
        }
    }
}

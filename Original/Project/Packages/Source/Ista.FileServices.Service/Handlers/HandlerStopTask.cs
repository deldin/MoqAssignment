using System;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.Infrastructure.Serializers;
using Ista.FileServices.Service.Interfaces;

namespace Ista.FileServices.Service.Handlers
{
    public class HandlerStopTask : IScheduleMessageHandler
    {
        public bool IsSatisfiedBy(IIstaMessage message)
        {
            if (!message.Type.Equals("task", StringComparison.OrdinalIgnoreCase))
                return false;

            return (message.Action.Equals("stop", StringComparison.OrdinalIgnoreCase));
        }

        public bool Handle(IMiramarTaskProvider taskProvider, IMiramarScheduleProvider scheduleProvider, IMiramarContextProvider contextProvider, IIstaMessage message)
        {
            var request = JsonMessageSerializer.DeserializeType(message.Body, new
            {
                requestId = 0,
                requestedBy = string.Empty,
                requestedOn = DateTime.Now,
                taskId = string.Empty,
            });

            if (request == null)
                return false;

            contextProvider.CancelTask(request.taskId);
            return true;
        }
    }
}

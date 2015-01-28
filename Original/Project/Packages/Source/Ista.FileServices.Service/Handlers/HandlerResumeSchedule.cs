using System;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.Infrastructure.Serializers;
using Ista.FileServices.Service.Interfaces;

namespace Ista.FileServices.Service.Handlers
{
    public class HandlerResumeSchedule : IScheduleMessageHandler
    {
        public bool IsSatisfiedBy(IIstaMessage message)
        {
            if (!message.Type.Equals("schedule", StringComparison.OrdinalIgnoreCase))
                return false;

            return (message.Action.Equals("resume", StringComparison.OrdinalIgnoreCase));
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

            scheduleProvider.PushTaskForConsideration(request.taskId);
            return true;
        }
    }
}
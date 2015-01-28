using System;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.Infrastructure.Serializers;
using Ista.FileServices.Service.Interfaces;

namespace Ista.FileServices.Service.Handlers
{
    public class HandlerRemoveSchedule : IScheduleMessageHandler
    {
        public bool IsSatisfiedBy(IIstaMessage message)
        {
            if (!message.Type.Equals("schedule", StringComparison.OrdinalIgnoreCase))
                return false;

            return (message.Action.Equals("remove", StringComparison.OrdinalIgnoreCase));
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

            scheduleProvider.PopTaskFromConsideration(request.taskId);
            return true;
        }
    }
}

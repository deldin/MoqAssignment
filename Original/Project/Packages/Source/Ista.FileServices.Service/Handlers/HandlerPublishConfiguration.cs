using System;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.Infrastructure.Serializers;
using Ista.FileServices.Service.Interfaces;

namespace Ista.FileServices.Service.Handlers
{
    public class HandlerPublishConfiguration : IScheduleMessageHandler
    {
        public bool IsSatisfiedBy(IIstaMessage message)
        {
            if (!message.Type.Equals("configuration", StringComparison.OrdinalIgnoreCase))
                return false;

            return (message.Action.Equals("publish", StringComparison.OrdinalIgnoreCase));
        }

        public bool Handle(IMiramarTaskProvider taskProvider, IMiramarScheduleProvider scheduleProvider, IMiramarContextProvider contextProvider, IIstaMessage message)
        {
            var request = JsonMessageSerializer.DeserializeType(message.Body, new
            {
                requestId = 0,
                requestedBy = string.Empty,
                requestedOn = DateTime.Now,
            });

            if (request == null)
                return false;

            taskProvider.PublishConfiguration();
            return true;
        }
    }
}

using Ista.FileServices.Infrastructure.Interfaces;

namespace Ista.FileServices.Service.Interfaces
{
    public interface IScheduleMessageHandler
    {
        bool IsSatisfiedBy(IIstaMessage message);
        bool Handle(IMiramarTaskProvider taskProvider, IMiramarScheduleProvider scheduleProvider, IMiramarContextProvider contextProvider, IIstaMessage message);
    }
}
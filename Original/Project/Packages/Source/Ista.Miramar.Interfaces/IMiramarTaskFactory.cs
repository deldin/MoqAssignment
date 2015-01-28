
namespace Ista.Miramar.Interfaces
{
    public interface IMiramarTaskFactory
    {
        bool IsSatisfiedBy(string taskId);
        IMiramarTask GetTask(IMiramarClientInfo clientInfo, string taskId);
    }
}

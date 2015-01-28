
namespace Ista.Miramar.Interfaces
{
    public interface IMiramarTaskStrategy : IMiramarTask
    {
        bool IsSatisfiedBy(string taskId);
    }
}

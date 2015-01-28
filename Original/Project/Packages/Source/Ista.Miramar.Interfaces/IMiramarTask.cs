using System.Threading;

namespace Ista.Miramar.Interfaces
{
    public interface IMiramarTask
    {
        void Execute(CancellationToken token);
    }
}

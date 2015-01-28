using System;
using System.Threading;

namespace Ista.FileServices.Infrastructure.Interfaces
{
    public interface IMessageQueueService : IDisposable
    {
        void Consume(string queue, CancellationToken token, Func<IIstaMessage, bool> command);
        void Consume(string queue, ushort prefetch, CancellationToken token, Func<IIstaMessage, bool> command);
    }
}
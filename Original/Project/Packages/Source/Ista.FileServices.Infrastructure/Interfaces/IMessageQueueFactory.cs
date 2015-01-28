using System;

namespace Ista.FileServices.Infrastructure.Interfaces
{
    public interface IMessageQueueFactory : IDisposable
    {
        bool IsActive { get; }
        bool IsOpen { get; }

        IMessageQueueService CreateService();
        IMessageQueuePublisher CreatePublisher();
        void OpenConnection();
    }
}
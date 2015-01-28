using System;

namespace Ista.FileServices.Infrastructure.Interfaces
{
    public interface IMessageQueuePublisher : IDisposable
    {
        void Publish(string exchangeName, string message, IIstaMessageProperties properties);
        void Publish(string exchangeName, string exchangeRoute, string message, IIstaMessageProperties properties);

        void Publish<T>(string exchangeName, T message, IIstaMessageProperties properties);
        void Publish<T>(string exchangeName, string exchangeRoute, T message, IIstaMessageProperties properties);
    }
}

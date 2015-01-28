using System.Text;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.Infrastructure.Serializers;
using RabbitMQ.Client;

namespace Ista.FileServices.Infrastructure.Queuing
{
    public class MessageQueuePublisher : IMessageQueuePublisher
    {
        private readonly IModel channel;

        public MessageQueuePublisher(IModel channel)
        {
            this.channel = channel;
        }

        public void Dispose()
        {
            if (channel.IsOpen)
                channel.Dispose();
        }

        public void Publish(string exchangeName, string message, IIstaMessageProperties properties)
        {
            Publish(exchangeName, string.Empty, message, properties);
        }

        public void Publish(string exchangeName, string exchangeRoute, string message, IIstaMessageProperties properties)
        {
            var messageProperties = MessageQueueFactory.ConvertProperties(channel, properties);
            var messagePayload = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchangeName, exchangeRoute, false, false, messageProperties, messagePayload);
        }

        public void Publish<T>(string exchangeName, T message, IIstaMessageProperties properties)
        {
            Publish(exchangeName, string.Empty, message, properties);
        }

        public void Publish<T>(string exchangeName, string exchangeRoute, T message, IIstaMessageProperties properties)
        {
            var messageProperties = MessageQueueFactory.ConvertProperties(channel, properties);
            var messagePayload = JsonMessageSerializer.SerializeToBytes(message);

            channel.BasicPublish(exchangeName, exchangeRoute, false, false, messageProperties, messagePayload);
        }
    }
}

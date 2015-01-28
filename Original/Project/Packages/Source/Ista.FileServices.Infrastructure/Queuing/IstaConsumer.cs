using Ista.FileServices.Infrastructure.Interfaces;
using RabbitMQ.Client;

namespace Ista.FileServices.Infrastructure.Queuing
{
    public class IstaConsumer : IBasicConsumer
    {
        private readonly IModel channel;
        private readonly IstaQueue<IIstaMessage> queue;

        public IModel Model
        {
            get { return channel; }
        }

        public IstaQueue<IIstaMessage> Queue
        {
            get { return queue; }
        } 

        public ShutdownEventArgs ShutdownReason { get; private set; }
        public string ConsumerTag { get; private set; }
        public bool IsRunning { get; private set; }

        public IstaConsumer(IModel channel)
            : this(channel, new IstaQueue<IIstaMessage>())
        {
        }

        public IstaConsumer(IModel channel, IstaQueue<IIstaMessage> queue)
        {
            this.channel = channel;
            this.queue = queue;

            ShutdownReason = null;
            ConsumerTag = null;
            IsRunning = false;
        }

        public void HandleBasicCancel(string consumerTag)
        {
            OnCancel();
        }

        public void HandleBasicCancelOk(string consumerTag)
        {
            OnCancel();
        }

        public void HandleBasicConsumeOk(string consumerTag)
        {
            ConsumerTag = consumerTag;
            IsRunning = true;
        }

        public void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            var messageProperties = MessageQueueFactory.ConvertProperties(properties);

            var correlationId = "unknown";
            var action = "unknown";
            var type = "unknown";

            if (messageProperties.IsCorrelationIdSet)
                correlationId = messageProperties.CorrelationId;

            if (messageProperties.IsMessageActionSet)
                action = messageProperties.MessageAction;

            if (messageProperties.IsMessageTypeSet)
                type = messageProperties.MessageType;

            var item = new IstaMessage
            {
                MessageId = deliveryTag,
                Action = action,
                Type = type,
                CorrelationId = correlationId,
                ConsumerId = consumerTag,
                Exchange = exchange,
                ExchangeRoute = routingKey,
                Redelivered = redelivered,
                Body = body,
                Properties = messageProperties,
            };

            queue.Enqueue(item);
        }

        public void HandleModelShutdown(IModel model, ShutdownEventArgs reason)
        {
            ShutdownReason = reason;
            OnCancel();
        }

        public void OnCancel()
        {
            queue.Close();
            IsRunning = false;
        }
    }
}
using System;
using System.IO;
using System.Threading;
using Ista.FileServices.Infrastructure.Interfaces;
using RabbitMQ.Client;

namespace Ista.FileServices.Infrastructure.Queuing
{
    public class MessageQueueService : IMessageQueueService
    {
        private readonly IModel channel;

        public MessageQueueService(IModel channel)
        {
            this.channel = channel;
        }

        public void Dispose()
        {
            if (channel.IsOpen)
                channel.Dispose();
        }

        public void Consume(string queue, CancellationToken token, Func<IIstaMessage, bool> command)
        {
            Consume(queue, 2, token, command);
        }

        public void Consume(string queue, ushort prefetch, CancellationToken token, Func<IIstaMessage, bool> command)
        {
            var consumer = new IstaConsumer(channel);
            
            channel.BasicQos(0, prefetch, false);
            channel.BasicConsume(queue, false, consumer);
            
            while (consumer.IsRunning)
            {
                if (token.IsCancellationRequested)
                    break;

                try
                {
                    var item = consumer.Queue.Dequeue(token);
                    if (item == null)
                        continue;

                    var handled = command(item);
                    if (handled)
                        channel.BasicAck(item.MessageId, false);
                    else
                        channel.BasicReject(item.MessageId, false);
                }
                catch (EndOfStreamException)
                {
                    break;
                }
            }
        }
    }
}
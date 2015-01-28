using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ista.FileServices.Infrastructure.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Ista.FileServices.Infrastructure.Queuing
{
    public class MessageQueueFactory : IMessageQueueFactory
    {
        private readonly ConnectionFactory factory;
        private IConnection connection;

        public bool IsActive { get; private set; }
        public bool IsOpen { get; private set; }

        public MessageQueueFactory(string host)
            : this(true)
        {
            factory = new ConnectionFactory
            {
                HostName = host,
                UserName = ConnectionFactory.DefaultUser,
                Password = ConnectionFactory.DefaultPass,
                VirtualHost = ConnectionFactory.DefaultVHost,
                Protocol = Protocols.DefaultProtocol,
                Port = AmqpTcpEndpoint.UseDefaultPort,
            };
        }

        private MessageQueueFactory(bool active)
        {
            IsActive = active;
        }

        public void Dispose()
        {
            if (connection != null && connection.IsOpen)
                connection.Close();
        }

        public void OpenConnection()
        {
            try
            {
                connection = factory.CreateConnection();
                IsOpen = true;
            }
            catch (BrokerUnreachableException ex)
            {
                IsOpen = false;
                var message = string.Format("Unable to open connection to RabbitMQ Host \"{0}\".", factory.HostName);
                throw new IOException(message, ex);
            }
        }

        public IMessageQueueService CreateService()
        {
            if (connection == null || !connection.IsOpen)
                throw new IOException("Connection is not open.");

            var channel = connection.CreateModel();
            return new MessageQueueService(channel);
        }

        public IMessageQueuePublisher CreatePublisher()
        {
            if (connection == null || !connection.IsOpen)
                throw new IOException("Connection is not open.");

            var channel = connection.CreateModel();
            return new MessageQueuePublisher(channel);
        }

        public static IMessageQueueFactory CreateInactiveFactory()
        {
            return new MessageQueueFactory(false);
        }

        public static IBasicProperties ConvertProperties(IModel channel, IIstaMessageProperties properties)
        {
            var item = channel.CreateBasicProperties();
            if (properties == null)
                return item;

            if (properties.IsAppIdSet)
                item.AppId = properties.AppId;
            if (properties.IsContentEncodingSet)
                item.ContentEncoding = properties.ContentEncoding;
            if (properties.IsContentTypeSet)
                item.ContentType = properties.ContentType;
            if (properties.IsCorrelationIdSet)
                item.CorrelationId = properties.CorrelationId;
            if (properties.IsExpirationSet)
                item.Expiration = properties.Expiration;
            if (properties.IsExternalMessageSet)
                item.MessageId = properties.ExternalMessageId;
            if (properties.IsReplyToSet)
                item.ReplyTo = properties.ReplyTo;
            if (properties.IsTypeSet)
                item.Type = properties.Type;
            if (properties.IsUserIdSet)
                item.UserId = properties.UserId;
            if (properties.IsPrioritySet)
                item.Priority = (byte)properties.Priority;
            if (properties.IsTimestampSet)
                item.Timestamp = new AmqpTimestamp(properties.Timestamp);

            item.Headers = new Dictionary<string, object>();
            item.Headers["x-action"] = properties.MessageAction;
            item.Headers["x-type"] = properties.MessageType;

            if (properties.HasHeaders)
                foreach (var header in properties.Headers)
                    item.Headers[header.Key] = header.Value;

            item.SetPersistent(properties.Persistent);
            return item;
        }

        public static IIstaMessageProperties ConvertProperties(IBasicProperties properties)
        {
            var item = new IstaMessageProperties
            {
                AppId = properties.AppId,
                ContentEncoding = properties.ContentEncoding,
                ContentType = properties.ContentType,
                CorrelationId = properties.CorrelationId,
                Expiration = properties.Expiration,
                ExternalMessageId = properties.MessageId,
                ReplyTo = properties.ReplyTo,
                Type = properties.Type,
                UserId = properties.UserId,
            };

            if (properties.IsDeliveryModePresent())
                item.Persistent = (properties.DeliveryMode.Equals(2));

            if (properties.IsPriorityPresent())
                item.Priority = properties.Priority;

            if (properties.IsTimestampPresent())
                item.Timestamp = properties.Timestamp.UnixTime;

            if (!properties.IsHeadersPresent())
                return item;

            var collection = properties.Headers as Hashtable;
            if (collection == null)
                return item;

            if (collection["x-action"] != null)
            {
                var bytes = collection["x-action"] as byte[];
                if (bytes != null && bytes.Length != 0)
                    item.MessageAction = Encoding.UTF8.GetString(bytes);
            }

            if (collection["x-type"] != null)
            {
                var bytes = collection["x-type"] as byte[];
                if (bytes != null && bytes.Length != 0)
                    item.MessageType = Encoding.UTF8.GetString(bytes);
            }

            foreach (string key in collection.Keys)
            {
                if (key.Equals("x-action", StringComparison.Ordinal))
                    continue;

                if (key.Equals("x-type", StringComparison.Ordinal))
                    continue;

                item.AddHeader(key, collection[key]);
            }

            return item;
        }
    }
}

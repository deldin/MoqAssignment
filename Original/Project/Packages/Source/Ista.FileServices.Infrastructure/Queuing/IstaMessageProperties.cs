using System;
using System.Collections.Generic;
using Ista.FileServices.Infrastructure.Interfaces;

namespace Ista.FileServices.Infrastructure.Queuing
{
    public class IstaMessageProperties : IIstaMessageProperties
    {
        private readonly Dictionary<string, object> headers;

        private string appId;
        private string contentEncoding;
        private string contentType;
        private string correlationId;
        private string expiration;
        private string externalMessageId;
        private string messageAction;
        private string messageType;
        private string replyTo;
        private string type;
        private string userId;
        private long? timestamp;
        private int? priority;

        public bool HasHeaders
        {
            get { return (headers.Count != 0); }
        }

        public IEnumerable<KeyValuePair<string, object>> Headers
        {
            get { return headers; }
        }

        public bool IsAppIdSet { get; private set; }
        public bool IsContentEncodingSet { get; private set; }
        public bool IsContentTypeSet { get; private set; }
        public bool IsCorrelationIdSet { get; private set; }
        public bool IsExpirationSet { get; private set; }
        public bool IsExternalMessageSet { get; private set; }
        public bool IsMessageActionSet { get; private set; }
        public bool IsMessageTypeSet { get; private set; }
        public bool IsReplyToSet { get; private set; }
        public bool IsTypeSet { get; private set; }
        public bool IsUserIdSet { get; private set; }
        public bool Persistent { get; set; }

        public bool IsPrioritySet
        {
            get { return (priority.HasValue); }
        }

        public bool IsTimestampSet
        {
            get { return (timestamp.HasValue); }
        }

        public string AppId
        {
            get { return appId; }
            set
            {
                appId = value;
                IsAppIdSet = !(string.IsNullOrEmpty(value));
            }
        }

        public string ContentEncoding
        {
            get { return contentEncoding; }
            set
            {
                contentEncoding = value;
                IsContentEncodingSet = !(string.IsNullOrEmpty(value));
            }
        }

        public string ContentType
        {
            get { return contentType; }
            set
            {
                contentType = value;
                IsContentTypeSet = !(string.IsNullOrEmpty(value));
            }
        }

        public string CorrelationId
        {
            get { return correlationId; }
            set
            {
                correlationId = value;
                IsCorrelationIdSet = !(string.IsNullOrEmpty(value));
            }
        }

        public string Expiration
        {
            get { return expiration; }
            set
            {
                expiration = value;
                IsExpirationSet = !(string.IsNullOrEmpty(value));
            }
        }

        public string ExternalMessageId
        {
            get { return externalMessageId; }
            set
            {
                externalMessageId = value;
                IsExternalMessageSet = !(string.IsNullOrEmpty(value));
            }
        }

        public string MessageAction
        {
            get { return messageAction; }
            set
            {
                messageAction = value;
                IsMessageActionSet = !(string.IsNullOrEmpty(value));
            }
        }

        public string MessageType
        {
            get { return messageType; }
            set
            {
                messageType = value;
                IsMessageTypeSet = !(string.IsNullOrEmpty(value));
            }
        }

        public string ReplyTo
        {
            get { return replyTo; }
            set
            {
                replyTo = value;
                IsReplyToSet = !(string.IsNullOrEmpty(value));
            }
        }

        public string Type
        {
            get { return type; }
            set
            {
                type = value;
                IsTypeSet = !(string.IsNullOrEmpty(value));
            }
        }

        public string UserId
        {
            get { return userId; }
            set
            {
                userId = value;
                IsUserIdSet = !(string.IsNullOrEmpty(value));
            }
        }

        public int Priority
        {
            get { return priority ?? 0; }
            set { priority = value; }
        }

        public long Timestamp
        {
            get { return timestamp ?? 0; }
            set { timestamp = value; }
        }

        public IstaMessageProperties()
        {
            headers = new Dictionary<string, object>();
        }

        public void AddHeader(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (value == null)
                return;

            headers[key] = value;
        }

        public static IstaMessageProperties CreatePersistent(string type, string action)
        {
            var correlationId = Guid.NewGuid().ToString("N");
            return CreatePersistent(correlationId, type, action);
        }

        public static IstaMessageProperties CreatePersistent(string correlationId, string type, string action)
        {
            return new IstaMessageProperties
            {
                CorrelationId = correlationId,
                MessageType = type,
                MessageAction = action,
                Persistent = true,
            };
        }

        public static IstaMessageProperties CreateTransient(string type, string action)
        {
            var correlationId = Guid.NewGuid().ToString("N");
            return CreateTransient(correlationId, type, action);
        }

        public static IstaMessageProperties CreateTransient(string correlationId, string type, string action)
        {
            return new IstaMessageProperties
            {
                CorrelationId = correlationId,
                MessageType = type,
                MessageAction = action,
                Persistent = true,
            };
        }
    }
}
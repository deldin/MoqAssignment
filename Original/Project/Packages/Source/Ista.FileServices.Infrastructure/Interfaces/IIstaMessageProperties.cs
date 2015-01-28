using System.Collections.Generic;

namespace Ista.FileServices.Infrastructure.Interfaces
{
    public interface IIstaMessageProperties
    {
        bool IsAppIdSet { get; }
        bool IsContentEncodingSet { get; }
        bool IsContentTypeSet { get; }
        bool IsCorrelationIdSet { get; }
        bool IsExpirationSet { get; }
        bool IsExternalMessageSet { get; }
        bool IsMessageActionSet { get; }
        bool IsMessageTypeSet { get; }
        bool IsPrioritySet { get; }
        bool IsReplyToSet { get; }
        bool IsTimestampSet { get; }
        bool IsTypeSet { get; }
        bool IsUserIdSet { get; }
        bool Persistent { get; }
        bool HasHeaders { get; }

        string AppId { get; }
        string ContentEncoding { get; }
        string ContentType { get; }
        string CorrelationId { get; }
        string Expiration { get; }
        string ExternalMessageId { get; }
        string MessageAction { get; }
        string MessageType { get; }
        string ReplyTo { get; }
        string Type { get; }
        string UserId { get; }
        long Timestamp { get; }
        int Priority { get; }

        IEnumerable<KeyValuePair<string, object>> Headers { get; }

        void AddHeader(string key, object value);
    }
}
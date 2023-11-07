using DidacticPotato.Types;

namespace DidacticPotato.MessageBrokers.Outbox.Models;

public class OutboxMessage : IEntity<string>
{
    public string Id { get; set; }
    public string OriginatedId { get; set; }
    public string CorrelationId { get; set; }
    public object Message { get; set; }
    public string SerializedMessage { get; set; }
    public string MessageType { get; set; }
    public string SpanContext { get; set; }
    public object MessageContext { get; set; }
    public string SerializedMessageContext { get; set; }
    public string MessageContextType { get; set; }
    public IDictionary<string, object> Headers { get; set; }
    public DateTimeOffset SentAt { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
}
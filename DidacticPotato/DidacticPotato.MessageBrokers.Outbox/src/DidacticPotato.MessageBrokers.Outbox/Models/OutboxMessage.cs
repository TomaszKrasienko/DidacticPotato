namespace DidacticPotato.MessageBrokers.Outbox.Models;

public record OutboxMessage(string Id, string CorrelationId, object Message, DateTimeOffset SentAt);
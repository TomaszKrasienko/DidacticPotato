using DidacticPotato.MessageBrokers.Outbox.Models;

namespace DidacticPotato.MessageBrokers.Outbox.Abstractions;

public interface IMessageOutboxAccessor
{
    Task<IReadOnlyList<OutboxMessage>> GetUnsentAsync();
    Task ProcessAsync(OutboxMessage message);
}
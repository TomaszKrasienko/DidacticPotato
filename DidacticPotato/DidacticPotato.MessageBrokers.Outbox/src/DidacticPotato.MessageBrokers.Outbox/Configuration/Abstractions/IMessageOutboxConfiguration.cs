using DidacticPotato.MessageBrokers.Outbox.Configuration.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DidacticPotato.MessageBrokers.Outbox.Configuration;

public interface IMessageOutboxConfiguration
{
    public IServiceCollection Services { get; }
    public OutboxOptions Options { get; }

}
using DidacticPotato.MessageBrokers.Outbox.Configuration.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DidacticPotato.MessageBrokers.Outbox.Configuration;

public class MessageOutboxConfiguration : IMessageOutboxConfiguration
{
    public IServiceCollection Services { get; }
    public OutboxOptions Options { get; }

    public MessageOutboxConfiguration(IServiceCollection services, OutboxOptions options)
    {
        Services = services;
        Options = options;
    }
}
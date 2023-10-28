using DidacticPotato.MessageBrokers.RabbitMQ.Conventions.Abstractions;

namespace DidacticPotato.MessageBrokers.RabbitMQ.Conventions;

internal sealed class RabbitMqConventionsProvider : IConventionsProvider
{
    private readonly IConventionsRegistry _conventionsRegistry;

    public RabbitMqConventionsProvider(IConventionsRegistry conventionsRegistry)
        => _conventionsRegistry = conventionsRegistry;

    public IConvention Get<T>()
        => _conventionsRegistry.Get(typeof(T));

    public IConvention Get(Type type)
        => _conventionsRegistry.Get(type);
}
using System.Collections.Immutable;

namespace DidacticPotato.MessageBrokers.RabbitMQ.Conventions.Abstractions;

public interface IConventionsRegistry
{
    void Add<T>(IConvention convention);
    void Add(Type type, IConvention convention);
    IConvention Get(Type type);
    ImmutableDictionary<Type, IConvention> GetAll();
}
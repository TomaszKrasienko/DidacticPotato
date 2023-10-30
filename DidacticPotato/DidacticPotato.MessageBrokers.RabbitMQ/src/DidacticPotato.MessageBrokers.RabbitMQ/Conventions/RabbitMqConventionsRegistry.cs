using System.Collections.Immutable;
using DidacticPotato.MessageBrokers.RabbitMQ.Conventions.Abstractions;
using Microsoft.AspNetCore.Http;

namespace DidacticPotato.MessageBrokers.RabbitMQ.Conventions;

internal sealed class RabbitMqConventionsRegistry : IConventionsRegistry
{
    private readonly Dictionary<Type, IConvention> _conventions = new Dictionary<Type, IConvention>();

    public void Add<T>(IConvention convention)
        => Add(typeof(T), convention);
    
    public void Add(Type type, IConvention convention)
        => _conventions.TryAdd(type, convention);
    
    public IConvention Get(Type type)
        => _conventions.TryGetValue(type, out var convention) ? convention : null;
    

    public ImmutableDictionary<Type, IConvention> GetAll()
        => _conventions.ToImmutableDictionary();
}
namespace DidacticPotato.MessageBrokers.RabbitMQ.Conventions.Abstractions;

public interface IConventionsProvider
{
    IConvention Get<T>();
    IConvention Get(Type type);
}
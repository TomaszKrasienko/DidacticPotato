namespace DidacticPotato.MessageBrokers.RabbitMQ.Conventions.Abstractions;

public interface IConvention
{
    string ExchangeName { get; }
    string ExchangeType { get; }
    bool ExchangeDurable { get; }
    bool ExchangeAutoDelete { get; }
    Dictionary<string, object> ExchangeArguments {get;}
    string RoutingKey { get; }
    string QueueName { get; }
    bool QueueDurable { get; }
    bool QueueExclusive { get; }
    bool QueueAutoDelete { get; }
    Dictionary<string, object> QueueArguments {get;}
}
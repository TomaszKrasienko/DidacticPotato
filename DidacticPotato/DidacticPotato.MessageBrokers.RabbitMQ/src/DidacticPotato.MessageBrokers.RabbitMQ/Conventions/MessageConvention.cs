using DidacticPotato.MessageBrokers.RabbitMQ.Conventions.Abstractions;

namespace DidacticPotato.MessageBrokers.RabbitMQ.Conventions;

public class MessageConvention : IConvention
{
    public string ExchangeName { get; }
    public string ExchangeType { get; }
    public bool ExchangeDurable { get; }
    public bool ExchangeAutoDelete { get; }
    public Dictionary<string, object> ExchangeArguments { get; }
    public string RoutingKey { get; }
    public string QueueName { get; }
    public bool QueueDurable { get; }
    public bool QueueExclusive { get; }
    public bool QueueAutoDelete { get; }
    public Dictionary<string, object> QueueArguments { get; }

    public MessageConvention(string exchangeName, string exchangeType, bool exchangeDurable, bool exchangeAutoDelete, 
        Dictionary<string, object> exchangeArguments, string routingKey, string queueName, bool queueDurable, 
        bool queueExclusive, bool queueAutoDelete, Dictionary<string, object> queueArguments)
    {
        ExchangeName = exchangeName;
        ExchangeType = exchangeType;
        ExchangeDurable = exchangeDurable;
        ExchangeAutoDelete = exchangeAutoDelete;
        ExchangeArguments = exchangeArguments;
        RoutingKey = routingKey;
        QueueName = queueName;
        QueueDurable = queueDurable;
        QueueExclusive = queueExclusive;
        QueueAutoDelete = queueAutoDelete;
        QueueArguments = queueArguments;
    }
}
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
    public string Queue { get; }

    public MessageConvention(string routingKey, string queue, string exchangeName, string exchangeType, bool exchangeDurable, 
        bool exchangeAutoDelete, Dictionary<string, object> exchangeArguments)
    {
        ExchangeName = exchangeName;
        RoutingKey = routingKey;
        Queue = queue;
        ExchangeType = exchangeType;
        ExchangeDurable = exchangeDurable;
        ExchangeAutoDelete = exchangeAutoDelete;
        ExchangeArguments = exchangeArguments;
    }
}
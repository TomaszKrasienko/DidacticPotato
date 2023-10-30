using DidacticPotato.MessageBrokers.Publishers;
using DidacticPotato.MessageBrokers.RabbitMQ.Clients.Abstractions;
using DidacticPotato.MessageBrokers.RabbitMQ.Conventions.Abstractions;

namespace DidacticPotato.MessageBrokers.RabbitMQ;

public class RabbitMqBusPublisher : IBusPublisher
{
    private readonly IRabbitMqClient _rabbitMqClient;
    private readonly IConventionsProvider _conventionsProvider;

    public RabbitMqBusPublisher(IRabbitMqClient rabbitMqClient, IConventionsProvider conventionsProvider)
    {
        _rabbitMqClient = rabbitMqClient;
        _conventionsProvider = conventionsProvider;
    }
    
    public Task PublishAsync<T>(T message, string messageId = null, string correlationId = null) where T : class
    {
        IConvention convention = _conventionsProvider.Get<T>();
        _rabbitMqClient.Send(message, convention, messageId, correlationId);
        return Task.CompletedTask;
    }
}
using DidacticPotato.MessageBrokers.Publishers;

namespace DidacticPotato.MessageBrokers.RabbitMQ;

public class RabbitMqBusPublisher : IBusPublisher
{
    public Task PublishAsync<T>(T message, string messageId = null, string correlationId = null)
    {
        throw new NotImplementedException();
    }
}
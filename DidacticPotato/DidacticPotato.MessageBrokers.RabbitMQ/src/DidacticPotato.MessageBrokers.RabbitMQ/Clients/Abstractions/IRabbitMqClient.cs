using DidacticPotato.MessageBrokers.RabbitMQ.Conventions.Abstractions;

namespace DidacticPotato.MessageBrokers.RabbitMQ.Clients.Abstractions;

public interface IRabbitMqClient
{
    public void Send(object message, IConvention convention, string messageId = null, string correlationId = null);
}
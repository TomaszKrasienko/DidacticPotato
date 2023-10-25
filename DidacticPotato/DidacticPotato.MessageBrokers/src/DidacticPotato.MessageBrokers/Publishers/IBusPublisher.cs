namespace DidacticPotato.MessageBrokers.Publishers;

public interface IBusPublisher
{
    Task PublishAsync<T>(T message, string messageId = null, string correlationId = null);
}
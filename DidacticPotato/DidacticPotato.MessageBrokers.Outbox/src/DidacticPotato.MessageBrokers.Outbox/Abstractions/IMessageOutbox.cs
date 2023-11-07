namespace DidacticPotato.MessageBrokers.Outbox.Abstractions;

public interface IMessageOutbox
{
    Task HandleAsync(string messageId, Func<Task> handler);

    Task SendAsync<T>(T message, string originatedMessageId = null, string messageId = null,
        string correlationId = null, string spanContext = null, object messageContext = null,
        IDictionary<string, object> headers = null) where T : class;
}
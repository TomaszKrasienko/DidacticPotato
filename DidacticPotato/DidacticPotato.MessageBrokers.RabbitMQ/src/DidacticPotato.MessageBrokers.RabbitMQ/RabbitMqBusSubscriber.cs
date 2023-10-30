using DidacticPotato.MessageBrokers.RabbitMQ.Subscribers;
using DidacticPotato.MessageBrokers.Subscribers;

namespace DidacticPotato.MessageBrokers.RabbitMQ;

internal sealed class RabbitMqBusSubscriber : IBusSubscriber
{
    private readonly MessageSubscriberChannel _messageSubscriberChannel;
    public RabbitMqBusSubscriber(MessageSubscriberChannel messageSubscriberChannel)
    {
        _messageSubscriberChannel = messageSubscriberChannel;
    }

    public IBusSubscriber Subscribe<T>(Func<IServiceProvider, T, object, Task> handle) where T : class
    {
        var type = typeof(T);
        _messageSubscriberChannel.Writer.TryWrite(MessageSubscriber.Subscribe(type,
            ((serviceProvider, message, context) => handle(serviceProvider, (T) message, context))));
        return this;
    }
    
    public void Dispose()
    {
    }
}
namespace DidacticPotato.MessageBrokers.RabbitMQ.Subscribers.Abstractions;

internal interface IMessageSubscriber
{
    MessageSubscriberAction Action { get; }
    Type Type { get; }
    Func<IServiceProvider, object, object, Task> Handle { get; }
}
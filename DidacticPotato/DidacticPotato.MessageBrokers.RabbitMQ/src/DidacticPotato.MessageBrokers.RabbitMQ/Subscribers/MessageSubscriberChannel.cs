using System.Threading.Channels;
using DidacticPotato.MessageBrokers.RabbitMQ.Subscribers.Abstractions;

namespace DidacticPotato.MessageBrokers.RabbitMQ.Subscribers;

internal class MessageSubscriberChannel
{
    private readonly Channel<IMessageSubscriber> _channel = Channel.CreateUnbounded<IMessageSubscriber>();
    
    public ChannelReader<IMessageSubscriber> Reader => _channel.Reader;
    public ChannelWriter<IMessageSubscriber> Writer => _channel.Writer;
}
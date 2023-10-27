using System.Collections.Concurrent;
using DidacticPotato.MessageBrokers.RabbitMQ.Clients.Abstractions;
using DidacticPotato.MessageBrokers.RabbitMQ.Configuration;
using RabbitMQ.Client;

namespace DidacticPotato.MessageBrokers.RabbitMQ.Clients;

public class RabbitMqClient : IRabbitMqClient
{
    private readonly object _lockObject = new();
    private readonly IConnection _producerConnection;
    private readonly int _maxChannels;
    private readonly ConcurrentDictionary<int, IModel> _channels;
    private int _channelsCount = 0;

    public RabbitMqClient(ProducerConnection producerConnection)
    {
        _producerConnection = producerConnection.Connection;
    }
    
    public void Send(object message)
    {
        var threadId = Thread.CurrentThread.ManagedThreadId;
        if (!_channels.TryGetValue(threadId, out var channel))
        {
            lock (_lockObject)
            {
                if (_channelsCount >= _maxChannels)
                {
                    throw new InvalidOperationException($"Cannot create RabbitMQ producer channel for thread." +
                                                        $"(Reached the limit of {_maxChannels} channels. " +
                                                        $"Try modify `MaxProducerChannel` setting");
                }
            }

            channel = _producerConnection.CreateModel();
            _channels.TryAdd(threadId, channel);
            _channelsCount++;
        }
        
    }
}
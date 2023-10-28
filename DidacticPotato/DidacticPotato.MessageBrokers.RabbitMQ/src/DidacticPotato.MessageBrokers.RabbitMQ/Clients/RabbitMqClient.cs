using System.Collections.Concurrent;
using System.Text;
using DidacticPotato.MessageBrokers.RabbitMQ.Clients.Abstractions;
using DidacticPotato.MessageBrokers.RabbitMQ.Configuration;
using DidacticPotato.MessageBrokers.RabbitMQ.Configuration.Models;
using DidacticPotato.MessageBrokers.RabbitMQ.Conventions.Abstractions;
using DidacticPotato.Serializer;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace DidacticPotato.MessageBrokers.RabbitMQ.Clients;

public class RabbitMqClient : IRabbitMqClient
{
    private readonly ISerializer _serializer;
    private readonly object _lockObject = new();
    private readonly IConnection _producerConnection;
    private readonly int _maxChannels;
    private readonly ConcurrentDictionary<int, IModel> _channels;
    private int _channelsCount = 0;

    public RabbitMqClient(ProducerConnection producerConnection, ISerializer serializer, IOptions<RabbitMqOptions> options)
    {
        _serializer = serializer;
        _producerConnection = producerConnection.Connection;
        _channels = new ConcurrentDictionary<int, IModel>();
        _maxChannels = options.Value.RequestedChannelMax;
    }
    
    public void Send(object message, IConvention convention, string messageId = null, string correlationId = null)
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

        var body = Encoding.UTF8.GetBytes(_serializer.ToJson(message));
        var properties = channel.CreateBasicProperties();
        properties.Persistent = false;
        properties.MessageId = string.IsNullOrWhiteSpace(messageId) ? Guid.NewGuid().ToString() : messageId;
        properties.CorrelationId = string.IsNullOrWhiteSpace(correlationId) ? Guid.NewGuid().ToString() : correlationId;
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        properties.Headers = new Dictionary<string, object>();
        channel.ExchangeDeclare(convention.ExchangeName, convention.ExchangeType, convention.ExchangeDurable,
            convention.ExchangeAutoDelete, convention.ExchangeArguments);
        channel.BasicPublish(convention.ExchangeName, convention.RoutingKey, properties, body.ToArray() );
    }
}
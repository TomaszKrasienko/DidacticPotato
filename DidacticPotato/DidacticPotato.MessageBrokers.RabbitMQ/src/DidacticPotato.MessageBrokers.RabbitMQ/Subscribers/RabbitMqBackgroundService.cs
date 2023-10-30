using System.Collections.Concurrent;
using System.Text;
using DidacticPotato.MessageBrokers.RabbitMQ.Configuration;
using DidacticPotato.MessageBrokers.RabbitMQ.Configuration.Models;
using DidacticPotato.MessageBrokers.RabbitMQ.Conventions;
using DidacticPotato.MessageBrokers.RabbitMQ.Conventions.Abstractions;
using DidacticPotato.MessageBrokers.RabbitMQ.Properties;
using DidacticPotato.MessageBrokers.RabbitMQ.Subscribers.Abstractions;
using DidacticPotato.Serializer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DidacticPotato.MessageBrokers.RabbitMQ.Subscribers;

public class RabbitMqBackgroundService : BackgroundService
{
    private readonly ILogger<RabbitMqBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _consumerConnection;
    private readonly MessageSubscriberChannel _messageSubscriberChannel;
    private readonly ConcurrentDictionary<string, IModel> _channels = new();
    private readonly IConventionsProvider _conventionsProvider;
    private readonly RabbitMqOptions _options;
    private readonly ISerializer _serializer;

    public RabbitMqBackgroundService(IServiceProvider serviceProvider, IServiceScopeFactory serviceScope, ISerializer serializer, ILogger<RabbitMqBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _serializer = serializer;
        _logger = logger;
        _consumerConnection = serviceProvider.GetRequiredService<ConsumerConnection>().Connection;
        _messageSubscriberChannel = serviceProvider.GetRequiredService<MessageSubscriberChannel>();
        _conventionsProvider = serviceProvider.GetRequiredService<IConventionsProvider>();
        _options = serviceProvider.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var messageSubscriber in _messageSubscriberChannel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                switch (messageSubscriber.Action)
                {
                    case MessageSubscriberAction.Subscribe:
                        Subscribe(messageSubscriber);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    private void Subscribe(IMessageSubscriber messageSubscriber)
    {
        var convention = _conventionsProvider.Get(messageSubscriber.Type);
        var channelKey = GetChannelKey(convention);
        if (_channels.ContainsKey(channelKey))
        {
            return;
        }
        
        var channel = _consumerConnection.CreateModel();
        if (!_channels.TryAdd(channelKey, channel))
        {
            channel.Dispose();
            return;
        }
        
        channel.QueueDeclare(convention.QueueName, convention.QueueDurable, convention.QueueExclusive,
            convention.QueueAutoDelete, convention.QueueArguments);
        channel.BasicQos(_options.Qos.PreferSize, _options.Qos.PreferCount, _options.Qos.Global);
        
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (_, args) =>await Test(_, args, messageSubscriber, channel);
        channel.BasicConsume(convention.QueueName, false, consumer);
    }

    private async Task Test(object _, BasicDeliverEventArgs args, IMessageSubscriber messageSubscriber, IModel channel)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var messageId = args.BasicProperties.MessageId;
            var correlationId = args.BasicProperties.CorrelationId;
            var timeStamp = args.BasicProperties.Timestamp;
            var stringMessage = Encoding.UTF8.GetString(args.Body.Span);
            var message = _serializer.ToObject(Encoding.UTF8.GetString(args.Body.Span), messageSubscriber.Type);
        
            Task Next(object m, object ctx, BasicDeliverEventArgs a)
                => TryHandleAsync(channel, m, messageId, correlationId, ctx, a,
                    messageSubscriber.Handle);

            await Next(message, MessageProperties.Create(args), args);
        }
        catch (Exception ex)
        {
            channel.BasicNack(args.DeliveryTag, false, false);
            Task.Yield();
        }
    }
    
    private async Task TryHandleAsync(IModel channel, object message, string messageId, string correlationId,
        object messageContext, BasicDeliverEventArgs args, Func<IServiceProvider, object, object, Task> handle)
    {
        var currentRetry = 0;
        var retries = _options.RetryPolicy.Count;
        var interval = _options.RetryPolicy.Interval;
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(retries, _ => interval);

        await retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                await handle(_serviceProvider, message, messageContext);
                channel.BasicAck(args.DeliveryTag, false);
                Task.Yield();
            }
            catch (Exception ex)
            {
                var hasNextRetry = retries < currentRetry++;
                if (hasNextRetry)
                {
                    throw;
                }

                channel.BasicNack(args.DeliveryTag, false, false);
            }
        });
    }
    

    private string GetChannelKey(IConvention convention)
        => $"{convention.ExchangeName}:{convention.QueueName}";
}
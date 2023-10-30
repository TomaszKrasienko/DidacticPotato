

using DidacticPotato.MessageBrokers.Outbox.Abstractions;
using DidacticPotato.MessageBrokers.Outbox.Configuration.Models;
using DidacticPotato.MessageBrokers.Publishers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DidacticPotato.MessageBrokers.Outbox.Processors;

internal sealed class OutboxProcessor : IHostedService
{
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly OutboxOptions _options;
    private readonly IBusPublisher _publisher;
    private Timer _timer;
    public OutboxProcessor(ILogger<OutboxProcessor> logger, IServiceProvider serviceProvider, 
        IOptions<OutboxOptions> options, IBusPublisher publisher)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _publisher = publisher;
        if (_options.Interval <= TimeSpan.Zero)
        {
            throw new Exception($"Invalid outbox interval: {_options.Interval}");
        }
        if (_options.IsEnabled)
        {
            logger.LogInformation($"Outbox is enabled. Message processing every {_options.Interval}");
        }
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_options.IsEnabled)
        {
            _timer = new Timer(SendOutboxMessages, null, TimeSpan.Zero, _options.Interval);
        }

        return Task.CompletedTask;
    }

    private void SendOutboxMessages(object state)
    {
        _ = SendOutboxMessagesAsync();
    }

    private async Task SendOutboxMessagesAsync()
    {
        _logger.LogInformation($"Starting processing outbox messages...");
        using var scope = _serviceProvider.CreateScope();
        var outboxAccessor = _serviceProvider.GetRequiredService<IMessageOutboxAccessor>();
        var messages = await outboxAccessor.GetUnsentAsync();
        _logger.LogTrace($"Found {messages.Count} messages to process");
        if (messages.Any())
        {
            _logger.LogTrace($"No message to be processed in outbox");
        }
        foreach (var message in messages.OrderBy(x => x.SentAt))
        {
            await _publisher.PublishAsync(message.Message, message.Id, message.CorrelationId);
            await outboxAccessor.ProcessAsync(message);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Finished processing messages from outbox");
        return Task.CompletedTask;
    }
}
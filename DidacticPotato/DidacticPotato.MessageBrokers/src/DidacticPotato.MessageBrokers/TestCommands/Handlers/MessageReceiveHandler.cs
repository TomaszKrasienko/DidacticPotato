using Microsoft.Extensions.Logging;

namespace DidacticPotato.MessageBrokers.TestCommands.Handlers;

public class MessageReceiveHandler : IMessageReceiveHandler
{
    private readonly ILogger<MessageProcessingHandler> _logger;
    
    public MessageReceiveHandler(ILogger<MessageProcessingHandler> logger)
    {
        _logger = logger;
    }
    public Task Handle(MessageReceived messageReceived)
    {
        _logger.LogInformation(messageReceived.ToString());
        return Task.CompletedTask;
    }
}
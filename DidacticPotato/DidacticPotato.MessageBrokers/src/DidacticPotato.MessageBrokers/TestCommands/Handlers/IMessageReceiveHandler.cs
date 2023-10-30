namespace DidacticPotato.MessageBrokers.TestCommands.Handlers;

public interface IMessageReceiveHandler
{
    Task Handle(MessageReceived messageReceived);
}
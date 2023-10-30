namespace DidacticPotato.MessageBrokers.TestEvents;

public record MessageSent(string Author, string Content, DateTime CreationDate);
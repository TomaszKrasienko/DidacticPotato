namespace DidacticPotato.Api.Events;

public record MessageSent(string Author, string Content, DateTime CreationDate);
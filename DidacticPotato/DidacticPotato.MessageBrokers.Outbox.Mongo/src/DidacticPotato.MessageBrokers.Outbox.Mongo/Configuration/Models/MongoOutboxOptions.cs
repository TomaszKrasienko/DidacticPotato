namespace DidacticPotato.MessageBrokers.Outbox.Mongo.Configuration.Models;

public record MongoOutboxOptions
{
    public string InboxCollection { get; set; } = "inbox";
    public string OutboxCollection { get; set; } = "outbox";
}
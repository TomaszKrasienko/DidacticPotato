namespace DidacticPotato.MessageBrokers.Outbox.Configuration.Models;

public sealed record OutboxOptions
{
    public bool IsEnabled { get; init; }
    public TimeSpan Interval { get; init; }
}
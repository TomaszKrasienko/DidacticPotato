using System.Security.AccessControl;
using DidacticPotato.Types;

namespace DidacticPotato.MessageBrokers.Outbox.Models;

public class InboxMessage : IEntity<string>
{
    public string Id { get; set; }
    public DateTime ProcessedAt { get; set; }
}
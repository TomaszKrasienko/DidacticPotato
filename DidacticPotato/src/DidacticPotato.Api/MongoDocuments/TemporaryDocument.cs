using System;
using DidacticPotato.Types;

namespace DidacticPotato.Api.MongoDocuments;

record TemporaryDocument : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public DateTime CreationDate { get; set; }
}
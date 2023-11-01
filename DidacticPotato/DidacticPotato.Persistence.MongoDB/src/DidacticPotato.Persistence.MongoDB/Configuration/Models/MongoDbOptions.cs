namespace DidacticPotato.Persistence.MongoDB.Models;

public record MongoDbOptions
{
    public string ConnectionString { get; init; }
    public string Database { get; init; }
}
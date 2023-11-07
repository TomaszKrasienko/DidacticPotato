namespace DidacticPotato.Api.Configuration.Models;

public record ServicesAvailabilityOptions
{
    public AvailabilityOptions MongoDb { get; init; }
    public AvailabilityOptions RabbitMq { get; init; }
}

public record AvailabilityOptions
{
    public bool Enabled { get; init; }
}
namespace DidacticPotato.MessageBrokers.RabbitMQ.Configuration.Models;

public record RabbitMqOptions
{
    public string ConnectionName { get; init; }
    public string HostName { get; init; }
    public int Port { get; init; }
    public string UserName { get; init; }
    public string Password { get; init; }
    public TimeSpan RequestedHeartbeat { get; init; } = TimeSpan.FromSeconds(60);
    public TimeSpan RequestedConnectionTimeout { get; init; } = TimeSpan.FromSeconds(30);
    public ushort RequestedChannelMax { get; init; } = 1000;
    public RabbitMqSslOptions Ssl { get; init; }
    public List<RoutingOptions> Routing { get; init; }
}

public record RabbitMqSslOptions
{
    public string ServerName { get; init; }
    public string CertificatePath { get; init; }
    public bool Enabled { get; init; }
}

public record RoutingOptions
{
    public string Type { get; init; }
    public ExchangeOptions Exchange { get; init; }
    public string QueueName { get; init; }
    public string RoutingKey { get; init; }
}

public record ExchangeOptions
{
    public string Name { get; init; }
    public string Type { get; init; } = "direction";
    public bool Durable { get; init; } = false;
    public bool AutoDelete { get; init; } = false;
    public Dictionary<string, object> Arguments { get; init; }
}
using RabbitMQ.Client;

namespace DidacticPotato.MessageBrokers.RabbitMQ.Configuration;

public class ConsumerConnection
{
    public IConnection Connection { get; }

    public ConsumerConnection(IConnection connection)
    {
        Connection = connection;
    }
}
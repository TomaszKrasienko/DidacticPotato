using RabbitMQ.Client;

namespace DidacticPotato.MessageBrokers.RabbitMQ.Configuration;

public class ProducerConnection
{
    public IConnection Connection { get; }

    public ProducerConnection(IConnection connection)
    {
        Connection = connection;
    }
}
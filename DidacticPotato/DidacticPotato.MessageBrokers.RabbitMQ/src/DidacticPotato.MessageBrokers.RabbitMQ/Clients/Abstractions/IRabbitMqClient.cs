namespace DidacticPotato.MessageBrokers.RabbitMQ.Clients.Abstractions;

internal interface IRabbitMqClient
{
    public void Send(object message);
}
using Microsoft.AspNetCore.Http.HttpResults;
using RabbitMQ.Client.Events;

namespace DidacticPotato.MessageBrokers.RabbitMQ.Properties;

public class MessageProperties
{
    public string MessageId { get; }
    public string CorrelationId { get; }
    public long Timestamp { get; }
    public IDictionary<string, object> Headers { get; }

    private MessageProperties(string messageId, string correlationId, long timestamp, IDictionary<string, object> headers)
    {
        MessageId = messageId;
        CorrelationId = correlationId;
        Timestamp = timestamp;
        Headers = headers;
    }

    public static MessageProperties Create(BasicDeliverEventArgs args)
    {
        var messageId = args.BasicProperties.MessageId;
        var correlationId = args.BasicProperties.CorrelationId;
        var timeStamp = args.BasicProperties.Timestamp;
        var headers = args.BasicProperties.Headers;
        return new MessageProperties(messageId, correlationId, timeStamp.UnixTime, headers);
    }
    
}
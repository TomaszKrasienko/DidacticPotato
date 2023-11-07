using DidacticPotato.MessageBrokers.Outbox.Abstractions;
using DidacticPotato.MessageBrokers.Outbox.Configuration.Models;
using DidacticPotato.MessageBrokers.Outbox.Models;
using DidacticPotato.Persistence.MongoDB.Factories.Abstractions;
using DidacticPotato.Persistence.MongoDB.Repositories;
using DidacticPotato.Serializer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DidacticPotato.MessageBrokers.Outbox.Mongo.Internals;

internal sealed class MongoMessageOutbox : IMessageOutbox, IMessageOutboxAccessor
{
    private readonly IMongoSessionFactory _sessionFactory;
    private readonly IMongoRepository<InboxMessage, string> _inboxRepository;
    private readonly IMongoRepository<OutboxMessage, string> _outboxRepository;
    private readonly ISerializer _serializer;
    private readonly bool _transactionEnabled;
    private readonly bool _enabled;
    private readonly string _emptyJson = "{}";

    public MongoMessageOutbox(IMongoSessionFactory sessionFactory, 
        IMongoRepository<InboxMessage, string> inboxRepository, 
        IMongoRepository<OutboxMessage, string> outboxRepository, 
        bool transactionEnabled,
        IOptions<OutboxOptions> options,
        ISerializer serializer)
    {
        _sessionFactory = sessionFactory;
        _inboxRepository = inboxRepository;
        _outboxRepository = outboxRepository;
        _transactionEnabled = transactionEnabled;
        _serializer = serializer;
        _enabled = options.Value.IsEnabled;
    }
    
    public async Task HandleAsync(string messageId, Func<Task> handler)
    {
        if (!_enabled)
        {
            //Todo: Logger
            return;
        }

        if (string.IsNullOrWhiteSpace(messageId))
        {
            throw new ArgumentException("Message id to be processed can not be empty");
        }

        if (await _inboxRepository.ExistsAsync(x => x.Id.ToString() == messageId))
        {
            //Todo: Logger
            return;
        }

        IClientSessionHandle session = null;
        if (_transactionEnabled)
        {
            session = await _sessionFactory.CreateAsync();
            session.StartTransaction();
        }

        try
        {
            //Todo: logger;
            await handler();
            await _inboxRepository.AddAsync(new InboxMessage()
            {
                Id = messageId,
                ProcessedAt = DateTime.Now
            });
            if (session is not null)
            {
                await session.CommitTransactionAsync();
            }
            //Todo: logger
        }
        catch (Exception ex)
        {
            //Todo:Logger
            if (session is not null)
            {
                await session.AbortTransactionAsync();
            }

            throw;
        }
        finally
        {
            session?.Dispose();
        }
    }

    public async Task SendAsync<T>(T message, string? originatedMessageId = null, string? messageId = null,
        string? correlationId = null, string? spanContext = null, object? messageContext = null, 
        IDictionary<string, object>? headers = null)
        where T : class
    {
        if (_enabled)
        {
            //Todo logger
            return;
        }

        var outboxMessage = new OutboxMessage()
        {
            Id = messageId = messageId ?? Guid.NewGuid().ToString(),
            OriginatedId = originatedMessageId,
            CorrelationId = correlationId,
            Message = message,
            SerializedMessage = _serializer.ToJson(message),
            MessageType = message.GetType().AssemblyQualifiedName,
            SpanContext = spanContext,
            MessageContext = messageContext,
            SerializedMessageContext = messageContext is null 
                ? _emptyJson 
                : _serializer.ToJson(messageContext),
            MessageContextType = message.GetType().AssemblyQualifiedName,
            Headers = headers,
            SentAt = DateTimeOffset.Now
        };

        await _outboxRepository.AddAsync(outboxMessage);
    }

    public async Task<IReadOnlyList<OutboxMessage>> GetUnsentAsync()
    {
        var outboxMessages = await _outboxRepository.FindAsync(x => x.ProcessedAt == null);
        return outboxMessages.Select(x =>
        {
            if (x.MessageContextType is not null)
            {
                var messageType = Type.GetType(x.MessageContextType);
                x.Message = _serializer.ToObject(x.SerializedMessageContext, messageType);
            }
            
            if (x.MessageType is not null)
            {
                var messageType = Type.GetType(x.MessageType);
                x.Message = _serializer.ToObject(x.SerializedMessage, messageType);
            }

            return x;
        }).ToList();
    }

    public async Task ProcessAsync(OutboxMessage message)
    {
        var item = await _outboxRepository.GetByIdAsync(message.Id);
        item.ProcessedAt = DateTimeOffset.Now;
        await _outboxRepository.AddAsync(item);
    }
}
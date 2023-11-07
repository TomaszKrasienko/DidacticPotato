using DidacticPotato.MessageBrokers.Outbox.Abstractions;
using DidacticPotato.MessageBrokers.Outbox.Models;
using DidacticPotato.MessageBrokers.Outbox.Mongo.Configuration.Models;
using DidacticPotato.MessageBrokers.Outbox.Mongo.Internals;
using DidacticPotato.Persistence.MongoDB.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace DidacticPotato.MessageBrokers.Outbox.Mongo.Configuration;

public static class Extensions
{
    public static IServiceCollection SetMongoOutbox(this IServiceCollection services)
    {
        MongoOutboxOptions options = new();
        services.SetMongoRepository<InboxMessage, string>(options.InboxCollection);
        services.SetMongoRepository<OutboxMessage, string>(options.OutboxCollection);
        services.AddTransient<IMessageOutbox, MongoMessageOutbox>();
        services.AddTransient<IMessageOutboxAccessor, MongoMessageOutbox>();
        
        BsonClassMap.RegisterClassMap<OutboxMessage>(m =>
        {
            m.AutoMap();
            m.UnmapMember(p => p.Message);
            m.UnmapMember(p => p.MessageContext);
        });

        return services;
    }
}
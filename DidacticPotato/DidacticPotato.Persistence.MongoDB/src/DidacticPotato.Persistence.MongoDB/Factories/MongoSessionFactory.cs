using DidacticPotato.Persistence.MongoDB.Factories.Abstractions;
using MongoDB.Driver;

namespace DidacticPotato.Persistence.MongoDB.Factories;

public class MongoSessionFactory : IMongoSessionFactory
{
    private readonly IMongoClient _mongoClient;

    public MongoSessionFactory(IMongoClient mongoClient)
        => _mongoClient = mongoClient;

    public Task<IClientSessionHandle> CreateAsync()
        => _mongoClient.StartSessionAsync();
}
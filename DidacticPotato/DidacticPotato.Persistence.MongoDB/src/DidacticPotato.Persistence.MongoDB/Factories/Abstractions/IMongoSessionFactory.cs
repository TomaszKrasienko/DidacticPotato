using MongoDB.Driver;

namespace DidacticPotato.Persistence.MongoDB.Factories.Abstractions;

public interface IMongoSessionFactory
{
    public Task<IClientSessionHandle> CreateAsync();
}
using System.Linq.Expressions;
using DidacticPotato.Types;
using MongoDB.Driver;

namespace DidacticPotato.Persistence.MongoDB.Repositories;

public class MongoRepository<TEntity, TIdentifier> : IMongoRepository<TEntity, TIdentifier> where TEntity : IEntity<TIdentifier>
{
    private readonly IMongoCollection<TEntity> _collection;

    public MongoRepository(IMongoDatabase mongoDatabase, string collectionName)
        => _collection = mongoDatabase.GetCollection<TEntity>(collectionName);

    public Task<TEntity> GetByIdAsync(TIdentifier id)
        => GetAsync(x => x.Id.Equals(id));

    public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        => _collection.Find(predicate).SingleOrDefaultAsync();

    public Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        => _collection.Find(predicate).ToListAsync();

    public Task AddAsync(TEntity entity)
        => _collection.InsertOneAsync(entity);

    public Task UpdateAsync(TEntity entity)
        => UpdateAsync(entity, x => x.Id.Equals(entity.Id));

    public Task UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> predicate)
        => _collection.FindOneAndReplaceAsync(predicate, entity);

    public Task DeleteAsync(TIdentifier id)
        => DeleteAsync(x => x.Id.Equals(id));

    public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        => _collection.DeleteOneAsync(predicate);

    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        => _collection.Find(predicate).AnyAsync();
}
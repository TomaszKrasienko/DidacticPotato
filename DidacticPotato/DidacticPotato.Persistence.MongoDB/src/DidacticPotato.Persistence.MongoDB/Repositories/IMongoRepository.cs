using System.Linq.Expressions;
using DidacticPotato.Persistence.MongoDB.Models;

namespace DidacticPotato.Persistence.MongoDB.Repositories;

public interface IMongoRepository<TEntity, TIdentifier> where TEntity : IEntity<TIdentifier>
{
    Task<TEntity> GetByIdAsync(TIdentifier id);
    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);
    Task AddAsync(TEntity entity);
}
using System.Linq.Expressions;
using DidacticPotato.Types;

namespace DidacticPotato.Persistence.MongoDB.Repositories;

public interface IMongoRepository<TEntity, in TIdentifier> where TEntity : IEntity<TIdentifier>
{
    Task<TEntity> GetByIdAsync(TIdentifier id);
    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);
    Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> predicate);
    Task DeleteAsync(TIdentifier id);
    Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
}
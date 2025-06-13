using System.Linq.Expressions;
using SharedKernel;

namespace Domain.Repositories;
public interface IRepository<TEntity> where TEntity : Entity
{
    public IUnitOfWork UnitOfWork { get; }

    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> query, CancellationToken cancellationToken = default);

    Task RemoveAsync(Expression<Func<TEntity, bool>> query, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> FilterAsync(Expression<Func<TEntity, bool>> query, CancellationToken cancellationToken = default);

    Task<IEnumerable<TProjection>> FilterAsync<TProjection>(Expression<Func<TEntity, bool>> query,
        Expression<Func<TEntity, TProjection>> projection,
        CancellationToken cancellationToken = default);
    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> query, CancellationToken cancellationToken = default);

    Task<TProjection> FirstOrDefaultAsync<TProjection>(Expression<Func<TEntity, bool>> query,
        Expression<Func<TEntity, TProjection>> projection,
        CancellationToken cancellationToken = default);
    IAsyncEnumerable<TEntity> ChunckAsync(Expression<Func<TEntity, bool>> query, CancellationToken cancellationToken = default);

    IAsyncEnumerable<TProjection> ChunckAsync<TProjection>(Expression<Func<TEntity, bool>> query,
        Expression<Func<TEntity, TProjection>> projection, CancellationToken cancellationToken = default);
}

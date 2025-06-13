using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Application.Abstractions.Data;
using Domain.Notifications;
using Domain.Repositories;
using MongoDB.Driver;
using SharedKernel;

namespace Infrastructure.Database.Repositories;

public abstract class MongoRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    private readonly IDatabaseContext _context;
    protected IMongoCollection<TEntity> Collection;

    public IUnitOfWork UnitOfWork { get; }
    protected MongoRepository(IDatabaseContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        UnitOfWork = unitOfWork;
    }

    #region Public Method's

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Collection ??= _context.GetCollection<TEntity>(GetCollectionName(), GetCollectionPartition());
        FilterDefinitionBuilder<TEntity> builderFilter = Builders<TEntity>.Filter;
        FilterDefinition<TEntity> filter = builderFilter.And(builderFilter.Where(x => x.Id == entity.Id));
        var options = new ReplaceOptions { IsUpsert = true };

        entity.UpdateLastModifiedDate();

        await Collection.ReplaceOneAsync(UnitOfWork.Session as IClientSessionHandle,
            filter,
            entity,
            options,
            cancellationToken);
    }

    public Task RemoveAsync(Expression<Func<TEntity, bool>> query, CancellationToken cancellationToken = default)
    {
        Collection ??= GetCollection();
        return Collection.DeleteOneAsync(query, cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> query, CancellationToken cancellationToken = default)
    {
        Collection ??= GetCollection();
        return await Collection.CountDocumentsAsync(query, cancellationToken: cancellationToken) > 0;
    }

    public async Task<TProjection> FirstOrDefaultAsync<TProjection>(Expression<Func<TEntity, bool>> query,
        Expression<Func<TEntity, TProjection>> projection,
        CancellationToken cancellationToken = default)
    {
        Collection ??= GetCollection();
        FilterDefinition<TEntity> filterDefinition = CreateFilterFromQuery(query);

        return await Collection.Find(filterDefinition).Project(projection).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> query, CancellationToken cancellationToken = default)
    {
        Collection ??= GetCollection();
        FilterDefinition<TEntity> filterDefinition = CreateFilterFromQuery(query);

        return await Collection.Find(filterDefinition).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TProjection>> FilterAsync<TProjection>(Expression<Func<TEntity, bool>> query,
        Expression<Func<TEntity, TProjection>> projection,
        CancellationToken cancellationToken = default)
    {
        Collection ??= GetCollection();
        FilterDefinition<TEntity> filterDefinition = CreateFilterFromQuery(query);

        return await Collection.Find(filterDefinition).Project(projection).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> FilterAsync(Expression<Func<TEntity, bool>> query, CancellationToken cancellationToken = default)
    {
        Collection ??= GetCollection();
        FilterDefinition<TEntity> filterDefinition = CreateFilterFromQuery(query);

        return await Collection.Find(filterDefinition).ToListAsync(cancellationToken);
    }

    public async IAsyncEnumerable<TProjection> ChunckAsync<TProjection>(Expression<Func<TEntity, bool>> query,
        Expression<Func<TEntity, TProjection>> projection,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Collection ??= GetCollection();
        FilterDefinition<TEntity> filterDefinition = CreateFilterFromQuery(query);
        IAsyncCursor<TProjection> cursor = await Collection.Find(filterDefinition).Project(projection).ToCursorAsync(cancellationToken);

        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (TProjection? item in cursor.Current)
            {
                yield return item;
            }
        }
    }

    public async IAsyncEnumerable<TEntity> ChunckAsync(Expression<Func<TEntity, bool>> query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Collection ??= GetCollection();
        FilterDefinition<TEntity> filterDefinition = CreateFilterFromQuery(query);
        IAsyncCursor<TEntity> cursor = await Collection.Find(filterDefinition).ToCursorAsync(cancellationToken);

        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (TEntity item in cursor.Current)
            {
                yield return item;
            }
        }
    }

    #endregion

    #region Protected Method's

    protected virtual string GetCollectionName() => typeof(TEntity).Name;

    protected virtual string GetCollectionPartition() => string.Empty;

    protected IMongoCollection<TEntity> GetCollection() => _context.GetCollection<TEntity>(GetCollectionName(), GetCollectionPartition());

    protected FilterDefinition<TEntity> CreateFilterFromQuery(Expression<Func<TEntity, bool>> query)
    {
        FilterDefinitionBuilder<TEntity> builderFilter = Builders<TEntity>.Filter;
        return builderFilter.And(builderFilter.Where(query));
    }

    #endregion
}

public sealed class NotificationRepository : MongoRepository<Notification>, INotificationRespository
{
    public const string CollectionName = "Notifications";

    public NotificationRepository(IDatabaseContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    protected override string GetCollectionName() => CollectionName;
}

public sealed class NotificationScheduleRepository : MongoRepository<NotificationSchedule>, INotificationScheduleRespository
{
    public const string CollectionName = "NotificationSchedules";

    public NotificationScheduleRepository(IDatabaseContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    protected override string GetCollectionName() => CollectionName;
}

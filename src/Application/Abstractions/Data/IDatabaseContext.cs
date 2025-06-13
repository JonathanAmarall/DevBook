using MongoDB.Driver;

namespace Application.Abstractions.Data;
public interface IDatabaseContext
{
    IMongoClient Client { get; }
    IMongoDatabase Database { get; }
    IMongoCollection<TEntity> GetCollection<TEntity>(string collectionName, string? partitionKey = null);
}

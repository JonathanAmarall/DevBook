using Application.Abstractions.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Database.Context;

public class MongoDbContext : IDatabaseContext, IDisposable
{

    public IMongoClient Client { get; }
    public IMongoDatabase Database { get; }

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        Client = new MongoClient(settings.Value.ConnectionString);
        Database = Client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<TEntity> GetCollection<TEntity>(string collectionName, string? partitionKey = null)
        => string.IsNullOrEmpty(partitionKey)
            ? Database.GetCollection<TEntity>(collectionName)
            : Database.GetCollection<TEntity>($"{partitionKey}_{collectionName}");

    public void Dispose()
    {
        Client.Dispose();
    }
}

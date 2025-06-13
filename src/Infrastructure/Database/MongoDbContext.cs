using Application.Abstractions.Data;
using Domain.Entries;
using Domain.Notifications;
using Domain.Users;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Database;

public class MongoDbContext : IDatabaseContext, IDisposable
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;
    public MongoDbContext(IOptions<MongoDbSettings> options)
    {
        _client = new MongoClient(options.Value.ConnectionString);
        _database = _client.GetDatabase(options.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    public IMongoCollection<Entry> LogEntries => _database.GetCollection<Entry>("Entries");
    public IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>("Notifications");
    public IMongoCollection<NotificationSchedule> NotificationSchedules => _database.GetCollection<NotificationSchedule>("NotificationSchedules");

    public void Dispose()
    {
        _client.Dispose();
    }
}

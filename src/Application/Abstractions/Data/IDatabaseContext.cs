using Domain.LogEntry;
using Domain.Project;
using Domain.Users;
using MongoDB.Driver;

namespace Application.Abstractions.Data;
public interface IDatabaseContext
{
    IMongoCollection<LogEntry> LogEntries { get; }
    IMongoCollection<Project> Projects { get; }
    IMongoCollection<User> Users { get; }
}

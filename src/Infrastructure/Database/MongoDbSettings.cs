namespace Infrastructure.Database;

public sealed record MongoDbSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "LogBookDb";
}

namespace Infrastructure.Database;

public sealed record MongoDbSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}

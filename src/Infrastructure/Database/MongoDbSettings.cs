﻿namespace Infrastructure.Database;

public sealed record MongoDbSettings
{
    public string ConnectionString { get; init; } = "mongodb://localhost:27017";
    public string DatabaseName { get; init; } = "LogBookDb";
}

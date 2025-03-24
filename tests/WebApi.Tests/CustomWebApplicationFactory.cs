using Application.Abstractions.Authentication;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Testcontainers.MongoDb;
using Web.Api;
using WireMock.Server;

namespace WebApi.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public WireMockServer WireMockServer { get; private set; }

    private readonly MongoDbContainer MongoContainer = new MongoDbBuilder()
            .WithUsername("")
            .WithPassword("")
            .WithImage("mongo:latest")
            .WithExtraHost("host.docker.internal", "host-gateway")
            .WithCommand("--replSet", "rs0")
            .WithWaitStrategy(Wait.ForUnixContainer())
            .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Add Using settings here
        builder.UseSetting("MongoDbSettings:ConnectionString", MongoContainer.GetConnectionString());

        builder.ConfigureServices(services => services.AddScoped<IUserContext, UserContextTest>());
    }

    public async Task InitializeAsync()
    {
        WireMockServer = WireMockServer.Start();
        await MongoContainer.StartAsync();
        await InitializeReplicaSetAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        WireMockServer.Dispose();
        await MongoContainer.DisposeAsync();
        await base.DisposeAsync();
    }

    private async Task InitializeReplicaSetAsync()
    {
        using var client = new MongoClient(MongoContainer.GetConnectionString());
        IMongoDatabase adminDatabase = client.GetDatabase("admin");

        var command = new BsonDocument("replSetInitiate", new BsonDocument());
        await adminDatabase.RunCommandAsync<BsonDocument>(command);

        // Wait for the replica set to be fully initialized
        bool isReplicaSetInitialized = false;
        while (!isReplicaSetInitialized)
        {
            BsonDocument replSetStatus = await adminDatabase.RunCommandAsync<BsonDocument>(new BsonDocument("replSetGetStatus", 1));
            isReplicaSetInitialized = replSetStatus["ok"] == 1;
            if (!isReplicaSetInitialized)
            {
                await Task.Delay(1000);
            }
        }
    }
}

public class UserContextTest : IUserContext
{
    public string UserId => "xpto";
}

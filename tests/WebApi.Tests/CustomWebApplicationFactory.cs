using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using DotNet.Testcontainers.Builders;
using Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Testcontainers.MongoDb;
using Web.Api;
using WireMock.Server;

namespace WebApi.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public WireMockServer WireMockServer { get; private set; }
    public string DatabaseName = "DevLog-IntegrationTest";
    private const string BaseAddress = "http://localhost:5009";

    private readonly MongoDbContainer MongoContainer = new MongoDbBuilder()
            .WithUsername("")
            .WithPassword("")
            .WithImage("mongo:latest")
            .WithExtraHost("host.docker.internal", "host-gateway")
            .WithCommand("--replSet", "rs0")
            .WithWaitStrategy(Wait.ForUnixContainer())
            .Build();
    public string ConnectionString => MongoContainer.GetConnectionString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseUrls(BaseAddress);
        builder.UseEnvironment("IntegrationTest");
        builder.UseSetting("MongoDbSettings:ConnectionString", MongoContainer.GetConnectionString());
        builder.ConfigureServices(services =>
        {
            ServiceDescriptor? settingsDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IConfigureOptions<MongoDbSettings>));
            if (settingsDescriptor is not null)
            {
                services.Remove(settingsDescriptor);
            }

            services.Configure<MongoDbSettings>(opts =>
            {
                opts.ConnectionString = ConnectionString;
                opts.DatabaseName = DatabaseName;
            });

            ServiceDescriptor? contextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IDatabaseContext));

            if (contextDescriptor is not null)
            {
                services.Remove(contextDescriptor);
            }

            services.AddScoped<IDatabaseContext, MongoDbContext>();
        });
        builder.ConfigureServices(services => services.AddScoped<IUserContext, UserContextTest>());
        builder.UseTestServer(options => options.BaseAddress = new Uri(BaseAddress));
    }

    public async Task InitializeAsync()
    {
        WireMockServer = WireMockServer.Start(port: 9877);

        await MongoContainer.StartAsync();
        await InitializeReplicaSetAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        WireMockServer.Stop();
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

        bool isReplicaSetInitialized = false;
        while (!isReplicaSetInitialized)
        {
            BsonDocument replSetStatus = await adminDatabase
                .RunCommandAsync<BsonDocument>(new BsonDocument("replSetGetStatus", 1));

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
    public string UserId => Guid.NewGuid().ToString();
}

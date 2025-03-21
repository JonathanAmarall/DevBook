using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.MongoDb;
using Web.Api;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

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
    }

    public async Task InitializeAsync()
    {
        WireMockServer = WireMockServer.Start();
        await MongoContainer.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        WireMockServer.Dispose();
        await MongoContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}

public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    protected readonly CustomWebApplicationFactory _factory;
    protected readonly HttpClient _httpClient;

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    protected void SetupGetApiMock(string path, object response, int statusCode = 200)
    {
        _factory.WireMockServer
            .Given(Request.Create()
                .WithPath(path)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(statusCode)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(response));
    }
}

using System.Net;
using Domain.Users;
using MongoDB.Driver;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WebApi.Tests;

public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    protected HttpClient HttpClient;
    protected MongoClient MongoClient;
    protected IMongoDatabase Database;

    protected readonly AutoFixture.Fixture Fixture = new();

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.InitializeAsync();

        HttpClient = _factory.CreateClient();
        MongoClient = new MongoClient(_factory.ConnectionString);
        Database = MongoClient.GetDatabase(_factory.DatabaseName);

        IMongoCollection<User> users = Database.GetCollection<User>("Users");

        await users.ReplaceOneAsync(
            Builders<User>.Filter.Eq(u => u.Id, GetUser().Id),
            GetUser(),
            new ReplaceOptions { IsUpsert = true });
    }

    public async Task DisposeAsync()
    {
        HttpClient.Dispose();
        MongoClient = null!;
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        MongoClient?.Dispose();
        HttpClient?.Dispose();
    }

    protected User GetUser() => new()
    {
        Id = "xpto",
        Email = "john.doe@mail.com",
        FullName = "John Doe",
        Username = "john.doe"
    };

    protected void SetupGetApiMock(string path, object response, int statusCode = 200)
    {
        _factory.WireMockServer
            .Given(Request.Create().WithPath(path).UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(statusCode)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(response));
    }

    protected static void CreateRequestWithJsonResponse(WireMockServer server, string path, string result, Dictionary<string, string> headers, HttpStatusCode status = HttpStatusCode.OK)
    {
        if (headers.Count == 0)
        {
            headers.TryAdd("Content-Type", "application/json");
        }

        IResponseBuilder response = Response
            .Create()
            .WithHeaders(headers)
            .WithStatusCode(status.GetHashCode())
            .WithBody(result);

        CreateServerResponse(server, response, path);
    }

    protected static void CreateServerResponse(WireMockServer server, IResponseBuilder response, string path, string? sagaKey = default)
    {
        IRequestBuilder request = Request.Create().WithUrl(path).UsingAnyMethod();
        if (!string.IsNullOrEmpty(sagaKey))
        {
            request.WithHeader("saga-key", sagaKey);
        }

        server
           .Given(request)
           .RespondWith(response);
    }
}

[CollectionDefinition("IntegrationTest", DisableParallelization = true)]
public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
}

using System.Net;
using Domain.Users;
using MongoDB.Driver;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WebApi.Tests;

public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    protected readonly CustomWebApplicationFactory _factory;
    protected readonly HttpClient _httpClient;
    protected readonly MongoClient _mongoClient;

    protected readonly AutoFixture.Fixture _fixture = new();

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
        _mongoClient = new MongoClient(factory.ConnectionString);

        IMongoDatabase db = _mongoClient.GetDatabase(_factory.DatabaseName);
        User user = GetUser();
        IMongoCollection<User> collection = db.GetCollection<User>("Users");

        collection.ReplaceOne(
            Builders<User>.Filter.Eq(u => u.Id, user.Id),
            user,
            new ReplaceOptions { IsUpsert = true });
    }

    protected User GetUser()
    {
        return new User { Id = "xpto", Email = "john.doe@mail.com", FullName = "John Doe", Username = "john.doe" };
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

    public void Dispose()
    {
        _mongoClient.Dispose();
        _httpClient.Dispose();
        _factory.Dispose();
    }
}

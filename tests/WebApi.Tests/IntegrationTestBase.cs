using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;

namespace WebApi.Tests;

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

namespace WebApi.Tests.Scenarios.Endpoints.LogEntry;
public class SearchLogEntryTests : IntegrationTestBase
{
    private const string CreateLogEntryPath = "/api/v1/log-entry";

    public SearchLogEntryTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GivenSearchLogEntryIsCalling_WhenCommandIsValid_ThenReturnSuccessful()
    {
        // Arrange
        // Act
        HttpResponseMessage? response = await _httpClient.GetAsync(CreateLogEntryPath);

        // Assert
        response?.EnsureSuccessStatusCode(); // Status Code 200-299

        Assert.Equal("application/json; charset=utf-8",
            response?.Content?.Headers?.ContentType?.ToString());
    }
}

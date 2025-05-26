using Application.LogBook.Search;
using FluentAssertions;
using SharedKernel;

namespace WebApi.Tests.Scenarios.Endpoints.LogEntry;
public class SearchTests : IntegrationTestBase
{
    private const string CreateLogEntryPath = "/api/v1/log-entry";

    public SearchTests(CustomWebApplicationFactory factory) : base(factory)
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
        response?.Content?.Headers?.ContentType?.ToString()
            .Should()
            .Be("application/json; charset=utf-8");

        string responseBody = await response!.Content.ReadAsStringAsync();

        PagedList<SearchLogEntryQueryResponse>? logEntryResponse = Newtonsoft.Json.JsonConvert
            .DeserializeObject<PagedList<SearchLogEntryQueryResponse>>(responseBody);

        logEntryResponse.Should().NotBeNull();
    }
}

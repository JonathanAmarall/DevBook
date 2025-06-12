using Application.Entries.Search;
using FluentAssertions;
using SharedKernel;

namespace WebApi.Tests.Scenarios.Endpoints.LogEntry;

[Collection("IntegrationTest collection")]
public class SearchTests : IntegrationTestBase
{
    private const string SearchLogEntryPath = "/api/v1/log-entry";

    public SearchTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GivenSearchLogEntryIsCalling_WhenCommandIsValid_ThenReturnSuccessful()
    {
        // Arrange
        // Act
        HttpResponseMessage? response = await HttpClient.GetAsync(SearchLogEntryPath);

        // Assert
        response?.EnsureSuccessStatusCode();
        response?.Content?.Headers?.ContentType?.ToString()
            .Should()
            .Be("application/json; charset=utf-8");

        string responseBody = await response!.Content.ReadAsStringAsync();

        PagedList<SearchEntryQueryResponse>? logEntryResponse = Newtonsoft.Json.JsonConvert
            .DeserializeObject<PagedList<SearchEntryQueryResponse>>(responseBody);

        logEntryResponse.Should().NotBeNull();
    }
}

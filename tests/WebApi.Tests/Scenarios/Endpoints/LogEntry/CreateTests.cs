using Application.LogEntry.GetById;
using Domain.LogEntry;
using FluentAssertions;

namespace WebApi.Tests.Scenarios.Endpoints.LogEntry;
public class CreateTests : IntegrationTestBase
{
    private const string CreateLogEntryPath = "/api/v1/log-entry";

    public CreateTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GivenCreateIsCalling_WhenCommandIsValid_ThenReturnSuccessful()
    {
        // Arrange
        var body = new
        {
            Title = "Test Log Entry",
            Description = "Test Description",
            Tags = new[] { "tag1", "tag2" },
            ProjectName = "Test Project",
            Category = LogCategory.Feature
        };

        using var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body), System.Text.Encoding.UTF8, "application/json");
        // Act
        HttpResponseMessage? response = await HttpClient.PostAsync(CreateLogEntryPath, content);

        // Assert
        response?.EnsureSuccessStatusCode();
        response?.Content?.Headers?.ContentType?.ToString()
            .Should()
            .Be("application/json; charset=utf-8");

        string responseBody = await response!.Content.ReadAsStringAsync();

        LogEntryResponse logEntryResponse = Newtonsoft.Json.JsonConvert
            .DeserializeObject<LogEntryResponse>(responseBody);

        logEntryResponse.Should().NotBeNull();
        logEntryResponse!.Title.Should().Be(body.Title);
        logEntryResponse.Description.Should().Be(body.Description);
        logEntryResponse.Tags.Should().BeEquivalentTo(body.Tags);
        logEntryResponse.Category.Should().Be(body.Category.ToString());
        logEntryResponse.Status.Should().Be(LogStatus.Resolved.ToString());
    }

    [Fact]
    public async Task GivenCreateIsCalling_WhenCommandInvalid_ThenReturnFailure()
    {
        // Arrange
        var body = new
        {
            Title = "Test Log Entry",
            Tags = new[] { "tag1", "tag2" },
            ProjectName = "Test Project",
            Category = LogCategory.Feature
        };

        using var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body), System.Text.Encoding.UTF8, "application/json");
        // Act
        HttpResponseMessage? response = await HttpClient.PostAsync(CreateLogEntryPath, content);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response?.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        string responseBody = await response!.Content.ReadAsStringAsync();
        responseBody.Should().NotBeEmpty();
    }
}

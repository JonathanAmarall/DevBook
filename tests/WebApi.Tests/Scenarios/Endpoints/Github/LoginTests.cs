using System.Net;
using Application.Users.Common;
using AutoFixture;
using FluentAssertions;
using Infrastructure.ExternalServices.Github;
using Newtonsoft.Json;

namespace WebApi.Tests.Scenarios.Endpoints.Github;
public class LoginTests : IntegrationTestBase
{
    private const string LoginPath = "/auth/github/login";
    private const string OAuthAccessTokenEndpointPath = "*/login/oauth/access_token*";
    private const string GetUserEndpointPath = "*/user*";

    public LoginTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GivenLoginIsCalling_WhenCommandIsValidAndAlreadyExistUserRegistered_ThenUpdateUserAndReturnSuccessful()
    {
        // Arrange
        string oAuthTokernResponse = "access_token=abc123&scope=repo&token_type=bearer\r\n";
        CreateRequestWithJsonResponse(
            _factory.WireMockServer,
            OAuthAccessTokenEndpointPath, oAuthTokernResponse,
            new() { { "Content-Type", "application/x-www-form-urlencoded" } });

        GithubUserResponse githubUserResponse = _fixture.Build<GithubUserResponse>().With(x => x.Email, GetUser().Email).Create();
        string githubUserResponseJson = JsonConvert.SerializeObject(githubUserResponse);
        CreateRequestWithJsonResponse(_factory.WireMockServer, GetUserEndpointPath, githubUserResponseJson, []);

        using var content = new StringContent(
            JsonConvert.SerializeObject(new { Code = "test_code" }),
            System.Text.Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage? response = await _httpClient.PostAsync(LoginPath, content);

        // Assert
        response?.IsSuccessStatusCode.Should().BeTrue();
        string responseBody = await response!.Content.ReadAsStringAsync();
        UserResponse userResponse = JsonConvert.DeserializeObject<UserResponse>(responseBody);
        userResponse!.Id.Should().Be(GetUser().Id);
        userResponse!.Email.Should().Be(githubUserResponse.Email);
    }

    [Fact]
    public async Task GivenLoginIsCalling_WhenCommandIsValidAndGithubAccessTokenReturnFailure_ThenReturnBadRequest()
    {
        // Arrange
        CreateRequestWithJsonResponse(
            _factory.WireMockServer,
            OAuthAccessTokenEndpointPath,
            string.Empty,
            new() { { "Content-Type", "application/x-www-form-urlencoded" } },
            HttpStatusCode.InternalServerError);

        using var content = new StringContent(
            JsonConvert.SerializeObject(new { Code = "test_code" }),
            System.Text.Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage? response = await _httpClient.PostAsync(LoginPath, content);

        // Assert
        response?.IsSuccessStatusCode.Should().BeFalse();
        response?.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GivenLoginIsCalling_WhenCommandIsValidAndGithubGetUserReturnFailure_ThenReturnBadRequest()
    {
        // Arrange
        string oAuthTokernResponse = "access_token=abc123&scope=repo&token_type=bearer\r\n";
        CreateRequestWithJsonResponse(
            _factory.WireMockServer,
            OAuthAccessTokenEndpointPath, oAuthTokernResponse,
            new() { { "Content-Type", "application/x-www-form-urlencoded" } });

        CreateRequestWithJsonResponse(_factory.WireMockServer, GetUserEndpointPath, string.Empty, [], HttpStatusCode.InternalServerError);

        using var content = new StringContent(
            JsonConvert.SerializeObject(new { Code = "test_code" }),
            System.Text.Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage? response = await _httpClient.PostAsync(LoginPath, content);

        // Assert
        response?.IsSuccessStatusCode.Should().BeFalse();
        response?.StatusCode.Should().Be(HttpStatusCode.InternalServerError);





    }
}

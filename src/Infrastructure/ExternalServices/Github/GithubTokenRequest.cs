using System.Text.Json.Serialization;
using Refit;

namespace Infrastructure.ExternalServices.Github;

public record GithubTokenRequest
{
    [AliasAs("client_id")]
    public string ClientId { get; init; }

    [AliasAs("client_secret")]
    public string ClientSecret { get; init; }

    [AliasAs("code")]
    public string Code { get; init; }
}

public record GitHubTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; }

    [JsonPropertyName("scope")]
    public string Scope { get; init; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; init; }
}

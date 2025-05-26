using Application.Abstractions.Authentication;
using Infrastructure.ExternalServices.Github;
using Microsoft.Extensions.Configuration;
using Refit;
using SharedKernel;

namespace Infrastructure.Authentication;
internal sealed class GithubOAuthProvider : IOAuthProvider
{
    private readonly IGithubApi _githubApi;
    private readonly IGithubAuthApi _githubAuthApi;
    private readonly IConfiguration _configuration;
    public GithubOAuthProvider(IGithubApi githubApi, IGithubAuthApi githubAuthApi, IConfiguration configuration)
    {
        _githubApi = githubApi;
        _githubAuthApi = githubAuthApi;
        _configuration = configuration;
    }
    public async Task<Result<OAuthUserResponse>> GetUserAsync(string code, CancellationToken cancellationToken)
    {
        GithubTokenRequest request = new()
        {
            ClientId = _configuration["GitHubAuth:ClientId"]!,
            ClientSecret = _configuration["GitHubAuth:ClientSecret"]!,
            Code = code
        };

        ApiResponse<GitHubTokenResponse> githubTokenResponse = await _githubAuthApi.LoginAccessTokenAsync(request, cancellationToken);

        if (!githubTokenResponse.IsSuccessful)
        {
            return Result.Failure<OAuthUserResponse>(
                new Error(githubTokenResponse.StatusCode.ToString(),
                githubTokenResponse.Error.Message,
                ErrorType.Validation));
        }

        string authHeader = GenerateBearerToken(githubTokenResponse);

        ApiResponse<GithubUserResponse> githubUserResponse = await _githubApi.GetUserAsync(authHeader, cancellationToken);

        if (!githubUserResponse.IsSuccessful)
        {
            return Result.Failure<OAuthUserResponse>(
                new Error(githubUserResponse.StatusCode.ToString(),
                githubUserResponse.Error.Message,
                ErrorType.Problem));
        }

        return new OAuthUserResponse
        {
            Email = githubUserResponse.Content.Email,
            AvatarUrl = githubUserResponse.Content.AvatarUrl,
            Bio = githubUserResponse.Content.Bio,
            Name = githubUserResponse.Content.Name,
            Username = githubUserResponse.Content.Login
        };
    }

    private static string GenerateBearerToken(ApiResponse<GitHubTokenResponse> githubTokenResponse)
    {
        return $"Bearer {githubTokenResponse.Content!.AccessToken}";
    }
}

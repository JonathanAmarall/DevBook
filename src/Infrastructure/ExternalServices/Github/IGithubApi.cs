using Refit;

namespace Infrastructure.ExternalServices.Github;
public interface IGithubApi
{
    [Get("/user")]
    Task<ApiResponse<GithubUserResponse>> GetUserAsync([Header("Authorization")] string authorization, CancellationToken cancellationToken);
}

public interface IGithubAuthApi
{
    [Headers("Content-Type: application/x-www-form-urlencoded")]
    [Post("/login/oauth/access_token")]
    Task<ApiResponse<GitHubTokenResponse>> LoginAccessTokenAsync([Body(BodySerializationMethod.UrlEncoded)] GithubTokenRequest request, CancellationToken cancellationToken);
}

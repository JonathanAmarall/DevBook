using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication;

namespace Web.Api.Endpoints.Github;

internal sealed class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/github/login", async (HttpContext context, IConfiguration configuration) =>
        {
            string redirectUrl = configuration["GitHubAuth:RedirectUri"];
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            await context.ChallengeAsync(GitHubAuthenticationDefaults.AuthenticationScheme, properties);
        })
        .WithTags(Tags.Github);
    }
}

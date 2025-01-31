using Microsoft.AspNetCore.Authentication;

namespace Web.Api.Endpoints.Github;

internal sealed class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/github/login", async (HttpContext httpContext) =>
        {
            // Redireciona para o GitHub para autenticação
            var authProps = new AuthenticationProperties { RedirectUri = "/api/auth/github/success" };
            await httpContext.ChallengeAsync("GitHub", authProps);
        })
        .WithTags(Tags.Github);
    }
}

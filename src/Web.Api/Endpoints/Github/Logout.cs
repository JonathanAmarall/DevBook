using Microsoft.AspNetCore.Authentication;

namespace Web.Api.Endpoints.Github;

internal sealed class Logout : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/github/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync();
            return Results.Redirect("/");
        })
        .WithTags(Tags.Github);
    }
}

namespace Web.Api.Endpoints.Github;

internal sealed class Callback : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/github/callback", () => Results.Redirect("/"))
        .WithTags(Tags.Github);
    }
}

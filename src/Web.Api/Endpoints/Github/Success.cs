namespace Web.Api.Endpoints.Github;

internal sealed class Success : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/github/success", (HttpContext httpContext) =>
        {
            // Retorna informações do usuário autenticado
            if (!httpContext.User.Identity?.IsAuthenticated ?? false)
            {
                return Results.Unauthorized();
            }

            var claims = httpContext.User.Claims
                .Select(c => new { c.Type, c.Value })
                .ToList();

            return Results.Ok(claims);
        }).WithTags(Tags.Github);
    }
}


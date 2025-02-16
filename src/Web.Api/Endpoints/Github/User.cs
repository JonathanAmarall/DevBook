namespace Web.Api.Endpoints.Github;

internal sealed class User : IEndpoint
{
#pragma warning disable S1075 // URIs should not be hardcoded
    private const string RequestUri = "https://api.github.com/user";
#pragma warning restore S1075 // URIs should not be hardcoded

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/user", async (HttpContext context) =>
        {
            string token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Results.Unauthorized();
            }

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

            string userResponse = await httpClient.GetStringAsync(RequestUri);

            return Results.Ok(userResponse);
        })
        .WithTags(Tags.Github);
    }
}

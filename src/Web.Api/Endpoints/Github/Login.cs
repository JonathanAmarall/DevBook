using Application.Users.Common;
using Application.Users.RegisterExternal;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Github;
public class TokenRequest
{
    public required string Code { get; init; }
}

internal sealed class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/github/login", async (
            [FromBody] TokenRequest request,
            HttpContext context,
            [FromServices] ISender sender,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            RegisterExternalUserCommand command = new(request.Code);
            Result<UserResponse> result = await sender.Send(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Github);
    }
}

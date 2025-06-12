using Application.Users.Common;
using Application.Users.Login;
using Application.Users.RegisterExternal;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users;

internal sealed class Login : IEndpoint
{
    public sealed record Request(string Email, string Password);
    public sealed record TokenRequest(string Code);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/users/login", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new LoginUserCommand(request.Email, request.Password);

            Result<UserResponse> result = await sender.Send(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users);

        app.MapPost("api/v1/users/login/external", async (
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
        .WithTags(Tags.Users);
    }
}

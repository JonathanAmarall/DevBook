using System.Security.Claims;
using Application.Users.Common;
using Application.Users.RegisterExternal;
using AspNet.Security.OAuth.GitHub;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using SharedKernel;

namespace Web.Api.Endpoints.Github;

internal sealed class Callback : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/github/callback", async (HttpContext context, ISender sender, IConfiguration configuration, CancellationToken cancellationToken) =>
        {
            AuthenticateResult authenticateResult = await context.AuthenticateAsync(GitHubAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded || authenticateResult.Principal is null)
            {
                return Results.Unauthorized();
            }

            RegisterExternalUserCommand command = CreateCommand(authenticateResult);

            Result<UserResponse> result = await sender.Send(command, cancellationToken);
            //string token = GenerateJwtToken(userId!, username!);

            if (result.IsFailure)
            {
                return Results.NotFound();
            }

            string frontendUrl = HandleFrontendUrl(result.Value, configuration);
            return Results.Redirect(frontendUrl);
        })
        .WithTags(Tags.Github);
    }

    private static string HandleFrontendUrl(UserResponse user, IConfiguration configuration)
    {
        string frontendUrl = configuration["GitHubAuth:FrontendRedirectUri"]!;

        return $"{frontendUrl}?" +
            $"userId={user.Id}&" +
            $"username={user.Username}&" +
            $"avatarUrl={user.AvatarUrl}&" +
            $"email={user.Email}&" +
            $"bio={user.Bio}&" +
            $"fullName={user.FullName}";
    }

    private static RegisterExternalUserCommand CreateCommand(AuthenticateResult authenticateResult)
    {
        var claims = authenticateResult.Principal!.Claims.ToList();
        string userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        string username = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        string email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        string fullName = claims.FirstOrDefault(c => c.Type == "urn:github:name")?.Value;
        string avatarUrl = claims.FirstOrDefault(c => c.Type == "urn:github:avatar")?.Value;
        string bio = claims.FirstOrDefault(c => c.Type == "urn:github:bio")?.Value;

        return new RegisterExternalUserCommand(
            email!,
            username!,
            new Uri(avatarUrl!),
            bio!,
            userId!,
            fullName!);
    }
}

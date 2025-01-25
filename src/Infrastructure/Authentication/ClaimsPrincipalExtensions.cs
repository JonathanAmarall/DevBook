using System.Security.Claims;

namespace Infrastructure.Authentication;

internal static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        return userId;
    }
}

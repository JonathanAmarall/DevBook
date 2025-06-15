namespace Application.Users.Common;

public sealed record UserResponse
{
    public string Id { get; init; }
    public string Email { get; init; }
    public string Username { get; init; }
    public string FullName { get; init; }
    public Uri? AvatarUrl { get; init; }
    public string? Bio { get; init; }
    public Token? Token { get; init; }
}

public record Token(string Value);

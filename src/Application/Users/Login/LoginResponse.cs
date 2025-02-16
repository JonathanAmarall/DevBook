using Application.Users.Common;

namespace Application.Users.Login;

public record LoginResponse
{
    public UserResponse User { get; init; }
    public string Token { get; init; }
}

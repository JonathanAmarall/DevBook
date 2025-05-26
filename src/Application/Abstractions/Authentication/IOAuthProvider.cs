using SharedKernel;

namespace Application.Abstractions.Authentication;
public interface IOAuthProvider
{
    Task<Result<OAuthUserResponse>> GetUserAsync(string code, CancellationToken cancellationToken);
}

public record OAuthUserResponse
{
    public string Email { get; set; }
    public Uri AvatarUrl { get; set; }
    public string Bio { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
}




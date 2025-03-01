using SharedKernel;

namespace Domain.Users;

public sealed class User : Entity
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string ExternalId { get; init; }
    public string Email { get; init; }
    public string Username { get; init; }
    public string FullName { get; init; }
    public string PasswordHash { get; init; }
    public Uri AvatarUrl { get; init; }
    public string Bio { get; init; }

    public bool IsExternalUser() => !string.IsNullOrWhiteSpace(ExternalId);
}

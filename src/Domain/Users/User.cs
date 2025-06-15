using SharedKernel;

namespace Domain.Users;

public sealed class User : Entity
{
    public string? ExternalId { get; init; }
    public string Email { get; init; }
    public string Username { get; init; }
    public string FullName { get; private set; }
    public string PasswordHash { get; init; }
    public Uri? AvatarUrl { get; private set; }
    public string? Bio { get; private set; }

    public User(string email, string userName, string fullName, string passwordHash, Uri? avatarUri, string? bio, string? externalId)
    {
        Email = email;
        Username = userName;
        FullName = fullName;
        PasswordHash = passwordHash;
        AvatarUrl = avatarUri;
        Bio = bio;
        ExternalId = externalId;
    }

    public bool IsExternalUser() => !string.IsNullOrWhiteSpace(ExternalId);

    public void UpdateMetadata(Uri avatarUrl, string name, string bio)
    {
        AvatarUrl = avatarUrl;
        FullName = name;
        Bio = bio;

        UpdateLastModifiedDate();
    }
}

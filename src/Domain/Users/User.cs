using SharedKernel;

namespace Domain.Users;

public sealed class User : Entity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ExternalId { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public string PasswordHash { get; set; }
    public Uri AvatarUrl { get; set; }
    public string Bio { get; set; }
}

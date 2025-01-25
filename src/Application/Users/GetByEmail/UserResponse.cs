namespace Application.Users.GetByEmail;

public sealed record UserResponse
{
    public string Id { get; init; }

    public string Email { get; init; }

    public string FirstName { get; init; }

    public string LastName { get; init; }
}

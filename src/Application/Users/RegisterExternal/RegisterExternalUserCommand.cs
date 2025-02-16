using Application.Abstractions.Messaging;
using Application.Users.Common;

namespace Application.Users.RegisterExternal;

public sealed record RegisterExternalUserCommand(
    string Email,
    string Username,
    Uri AvatarUrl,
    string Bio,
    string ExternalId,
    string FullName) : ICommand<UserResponse>;

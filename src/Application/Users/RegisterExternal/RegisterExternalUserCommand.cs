using Application.Abstractions.Messaging;
using Application.Users.Common;

namespace Application.Users.RegisterExternal;

public sealed record RegisterExternalUserCommand(
    string Code) : ICommand<UserResponse>;

using Application.Abstractions.Messaging;

namespace Application.Users.Register;

public sealed record RegisterUserCommand(string Email, string FullName, string Username, string Password)
    : ICommand<string>;

using Application.Abstractions.Messaging;

namespace Application.Users.GetById;

public sealed record GetUserByIdQuery(string UserId) : IQuery<UserResponse>;

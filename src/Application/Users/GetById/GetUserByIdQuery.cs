using Application.Abstractions.Messaging;
using Application.Users.Common;
using SharedKernel;

namespace Application.Users.GetById;

public sealed record GetUserByIdQuery(string UserId) : PagedRequest, IQuery<UserResponse>;

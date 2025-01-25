using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SharedKernel;

namespace Application.Users.GetById;

internal sealed class GetUserByIdQueryHandler(IDatabaseContext context, IUserContext userContext)
    : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        if (query.UserId != userContext.UserId)
        {
            return Result.Failure<UserResponse>(UserErrors.Unauthorized());
        }

        User? user = await context.Users.Find(x => x.Id == query.UserId)
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound(query.UserId));
        }

        return new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };
    }
}

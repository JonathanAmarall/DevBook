using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Users.Common;
using Domain.Repositories;
using Domain.Users;
using SharedKernel;

namespace Application.Users.GetByEmail;

internal sealed class GetUserByEmailQueryHandler(
    IUserRespository userRespository,
    IUserContext userContext)
    : IQueryHandler<GetUserByEmailQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
    {
        User? user = await userRespository.FirstOrDefaultAsync(
            u => u.Email == query.Email && u.Id == userContext.UserId,
            cancellationToken: cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFoundByEmail);
        }

        if (user.Id != userContext.UserId)
        {
            return Result.Failure<UserResponse>(UserErrors.Unauthorized());
        }

        return new UserResponse
        {
            Id = user.Id,
            AvatarUrl = user.AvatarUrl,
            Bio = user.Bio,
            Email = user.Email,
            Username = user.Username,
            FullName = user.FullName
        };
    }
}

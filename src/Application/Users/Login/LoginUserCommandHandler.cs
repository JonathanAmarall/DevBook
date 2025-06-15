using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Users.Common;
using Domain.Repositories;
using Domain.Users;
using SharedKernel;

namespace Application.Users.Login;

internal sealed class GithubLoginUserCommandHandler(
    IUserRespository userRespository,
    IPasswordHasher passwordHasher,
    IUserContext userContext,
    ITokenProvider tokenProvider) : ICommandHandler<LoginUserCommand, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRespository.FirstOrDefaultAsync(
             u => u.Email == command.Email && u.Id == userContext.UserId,
             cancellationToken: cancellationToken);

        if (user is null || user.IsExternalUser())
        {
            return Result.Failure<UserResponse>(UserErrors.NotFoundByEmail);
        }

        bool verified = passwordHasher.Verify(command.Password, user.PasswordHash);
        if (!verified)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFoundByEmail);
        }

        string token = tokenProvider.Create(user);

        return new UserResponse
        {
            Id = user.Id,
            AvatarUrl = user.AvatarUrl,
            Bio = user.Bio,
            Email = user.Email,
            Username = user.Username,
            FullName = user.FullName,
            Token = new Token(token)
        };
    }
}

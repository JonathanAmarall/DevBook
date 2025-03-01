using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Users.Common;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SharedKernel;

namespace Application.Users.Login;

internal sealed class GithubLoginUserCommandHandler(
    IDatabaseContext context,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider) : ICommandHandler<LoginUserCommand, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        User? user = await context.Users.Find(x => x.Email == command.Email)
            .SingleOrDefaultAsync(cancellationToken);

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

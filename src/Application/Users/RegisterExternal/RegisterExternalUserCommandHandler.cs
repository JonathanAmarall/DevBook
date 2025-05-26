using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Users.Common;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SharedKernel;
using UserResponse = Application.Users.Common.UserResponse;

namespace Application.Users.RegisterExternal;

internal sealed class RegisterExternalUserCommandHandler(
    IDatabaseContext context,
    ITokenProvider tokenProvider,
    IOAuthProvider oAuthProvider) : ICommandHandler<RegisterExternalUserCommand, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(RegisterExternalUserCommand command, CancellationToken cancellationToken)
    {
        Result<OAuthUserResponse> oAuthUserResponse = await oAuthProvider
            .GetUserAsync(command.Code, cancellationToken);

        if (oAuthUserResponse.IsFailure)
        {
            return Result.Failure<UserResponse>(oAuthUserResponse.Error);
        }

        User checkUser = await FindUserByEmailAsync(oAuthUserResponse.Value!.Email, cancellationToken);
        if (checkUser != null)
        {
            await UpdateUserAsync(oAuthUserResponse.Value!, checkUser, cancellationToken);
            return CreateSuccessResult(checkUser);
        }

        var user = new User()
        {
            Email = oAuthUserResponse.Value.Email,
            AvatarUrl = oAuthUserResponse.Value.AvatarUrl,
            Bio = oAuthUserResponse.Value.Bio,
            Username = oAuthUserResponse.Value.Name
        };

        user.Raise(new UserRegisteredDomainEvent(user.Id));

        await context.Users.InsertOneAsync(user, cancellationToken: cancellationToken);

        return CreateSuccessResult(user);
    }

    private Result<UserResponse> CreateSuccessResult(User user)
    {
        string token = tokenProvider.Create(user);

        return Result.Success(new UserResponse
        {
            Id = user.Id,
            AvatarUrl = user.AvatarUrl,
            Bio = user.Bio,
            Email = user.Email,
            Username = user.Username,
            FullName = user.FullName,
            Token = new Token(token)
        });
    }

    private async Task UpdateUserAsync(OAuthUserResponse oAuthUser, User checkUser, CancellationToken cancellationToken)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.Id, checkUser.Id);
        UpdateDefinition<User> update = Builders<User>.Update
            .Set(u => u.AvatarUrl, oAuthUser.AvatarUrl)
            .Set(u => u.FullName, oAuthUser.Name)
            .Set(u => u.Bio, oAuthUser.Bio);

        await context.Users.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }

    private async Task<User> FindUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq(x => x.Email, email);
        User? maybeUser = await context.Users.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return maybeUser;
    }
}

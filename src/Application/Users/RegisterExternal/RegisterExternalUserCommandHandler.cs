using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Users.Common;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SharedKernel;

namespace Application.Users.RegisterExternal;

internal sealed class RegisterExternalUserCommandHandler(IDatabaseContext context)
    : ICommandHandler<RegisterExternalUserCommand, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(RegisterExternalUserCommand command, CancellationToken cancellationToken)
    {
        User checkUser = await FindUserByEmailAsync(command, cancellationToken);

        if (checkUser != null)
        {
            await UpdateUserAsync(command, checkUser, cancellationToken);
            return CreateSuccessResult(checkUser);
        }

        var user = new User()
        {
            Email = command.Email,
            AvatarUrl = command.AvatarUrl,
            Bio = command.Bio,
            Username = command.Username,
            ExternalId = command.ExternalId
        };

        user.Raise(new UserRegisteredDomainEvent(user.Id));

        await context.Users.InsertOneAsync(user, cancellationToken: cancellationToken);

        return CreateSuccessResult(user);
    }

    private static Result<UserResponse> CreateSuccessResult(User user)
    {
        return Result.Success(new UserResponse
        {
            Id = user.Id,
            AvatarUrl = user.AvatarUrl,
            Bio = user.Bio,
            Email = user.Email,
            Username = user.Username,
            FullName = user.FullName
        });
    }

    private async Task UpdateUserAsync(RegisterExternalUserCommand command, User checkUser, CancellationToken cancellationToken)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.Id, checkUser.Id);
        UpdateDefinition<User> update = Builders<User>.Update
            .Set(u => u.AvatarUrl, command.AvatarUrl)
            .Set(u => u.Username, command.Username)
            .Set(u => u.Bio, command.Bio);

        await context.Users.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }

    private async Task<User> FindUserByEmailAsync(RegisterExternalUserCommand command, CancellationToken cancellationToken)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq(x => x.Email, command.Email);
        User? maybeUser = await context.Users.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return maybeUser;
    }
}

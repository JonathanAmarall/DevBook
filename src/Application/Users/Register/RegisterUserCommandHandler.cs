using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SharedKernel;

namespace Application.Users.Register;

internal sealed class RegisterUserCommandHandler(
    IDatabaseContext context,
    IPasswordHasher passwordHasher,
    IDomainEventsDispatcher eventsDispatcher)
    : ICommandHandler<RegisterUserCommand, string>
{
    public async Task<Result<string>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq(x => x.Email, command.Email);
        if (await context.GetCollection<User>("Users").Find(filter).FirstOrDefaultAsync(cancellationToken) != null)
        {
            return Result.Failure<string>(UserErrors.EmailNotUnique);
        }

        var user = new User
        {
            Email = command.Email,
            Username = command.Username,
            FullName = command.FullName,
            PasswordHash = passwordHasher.Hash(command.Password)
        };

        await context.GetCollection<User>("Users").InsertOneAsync(user, cancellationToken: cancellationToken);

        await eventsDispatcher.DispatchAsync([new UserRegisteredDomainEvent(user.Id)], cancellationToken);

        return user.Id;
    }
}

using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SharedKernel;

namespace Application.Users.Register;

internal sealed class RegisterUserCommandHandler(IDatabaseContext context, IPasswordHasher passwordHasher)
    : ICommandHandler<RegisterUserCommand, string>
{
    public async Task<Result<string>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq(x => x.Email, command.Email);
        if (await context.Users.Find(filter).FirstOrDefaultAsync(cancellationToken) != null)
        {
            return Result.Failure<string>(UserErrors.EmailNotUnique);
        }

        var user = new User
        {
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            PasswordHash = passwordHasher.Hash(command.Password)
        };

        user.Raise(new UserRegisteredDomainEvent(user.Id));

        await context.Users.InsertOneAsync(user, cancellationToken: cancellationToken);

        return user.Id;
    }
}

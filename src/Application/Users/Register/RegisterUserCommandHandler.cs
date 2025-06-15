using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Users;
using SharedKernel;

namespace Application.Users.Register;

internal sealed class RegisterUserCommandHandler(
    IUserRepository userRespository,
    IPasswordHasher passwordHasher,
    IDomainEventsDispatcher eventsDispatcher)
    : ICommandHandler<RegisterUserCommand, string>
{
    public async Task<Result<string>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        User? existingUser = await userRespository.FirstOrDefaultAsync(
            u => u.Email == command.Email || u.Username == command.Username,
            cancellationToken: cancellationToken);

        if (existingUser is not null)
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

        await userRespository.UnitOfWork.StartTransactionAsync(cancellationToken);
        await userRespository.AddAsync(user, cancellationToken: cancellationToken);
        await userRespository.UnitOfWork.CommitChangesAsync(cancellationToken);

        await eventsDispatcher.DispatchAsync([new UserRegisteredDomainEvent(user.Id)], cancellationToken);

        return user.Id;
    }
}

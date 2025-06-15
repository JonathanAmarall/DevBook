using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Users.Common;
using Domain.Repositories;
using Domain.Users;
using SharedKernel;
using UserResponse = Application.Users.Common.UserResponse;

namespace Application.Users.RegisterExternal;

internal sealed class RegisterExternalUserCommandHandler(
    IUserRespository userRespository,
    ITokenProvider tokenProvider,
    IOAuthProvider oAuthProvider,
    IDomainEventsDispatcher eventsDispatcher) : ICommandHandler<RegisterExternalUserCommand, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(RegisterExternalUserCommand command, CancellationToken cancellationToken)
    {
        Result<OAuthUserResponse> oAuthUserResponse = await oAuthProvider
            .GetUserAsync(command.Code, cancellationToken);

        if (oAuthUserResponse.IsFailure)
        {
            return Result.Failure<UserResponse>(oAuthUserResponse.Error);
        }

        User? user = await userRespository.FirstOrDefaultAsync(
            u => u.Email == oAuthUserResponse.Value!.Email && u.IsExternalUser(),
            cancellationToken: cancellationToken);

        await userRespository.UnitOfWork.StartTransactionAsync(cancellationToken);

        if (user is not null)
        {
            user.UpdateMetadata(
                avatarUrl: oAuthUserResponse.Value.AvatarUrl,
                name: oAuthUserResponse.Value.Name,
                bio: oAuthUserResponse.Value.Bio);

            await userRespository.AddAsync(user, cancellationToken: cancellationToken);
            await userRespository.UnitOfWork.CommitChangesAsync(cancellationToken);

            return CreateSuccessResult(user);
        }

        user = new User(
            oAuthUserResponse.Value.Email,
            oAuthUserResponse.Value.Name,
            oAuthUserResponse.Value.AvatarUrl,
            oAuthUserResponse.Value.Bio,
            string.Empty);

        await userRespository.AddAsync(user, cancellationToken: cancellationToken);
        await userRespository.UnitOfWork.CommitChangesAsync(cancellationToken);

        await eventsDispatcher.DispatchAsync([new UserRegisteredDomainEvent(user.Id)], cancellationToken);

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
}

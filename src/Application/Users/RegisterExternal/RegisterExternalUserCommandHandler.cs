using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.ExternalServices.Github;
using Application.Users.Common;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Refit;
using SharedKernel;
using UserResponse = Application.Users.Common.UserResponse;

namespace Application.Users.RegisterExternal;

internal sealed class RegisterExternalUserCommandHandler(
    IDatabaseContext context,
    ITokenProvider tokenProvider,
    IConfiguration configuration,
    IGithubApi githubApi,
    IGithubAuthApi githubAuthApi) : ICommandHandler<RegisterExternalUserCommand, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(RegisterExternalUserCommand command, CancellationToken cancellationToken)
    {
        GithubTokenRequest request = new()
        {
            ClientId = configuration["GitHubAuth:ClientId"]!,
            ClientSecret = configuration["GitHubAuth:ClientSecret"]!,
            Code = command.Code
        };

        ApiResponse<GitHubTokenResponse> githubTokenResponse = await githubAuthApi.LoginAccessTokenAsync(request, cancellationToken);
        if (!githubTokenResponse.IsSuccessful)
        {
            return Result.Failure<UserResponse>(new Error(githubTokenResponse.StatusCode.ToString(), githubTokenResponse.Error.Message, ErrorType.Validation));
        }

        string authHeader = $"Bearer {githubTokenResponse.Content.AccessToken}";
        ApiResponse<GithubUserResponse> githubUserResponse = await githubApi.GetUserAsync(authHeader, cancellationToken);
        if (!githubUserResponse.IsSuccessful)
        {
            return Result.Failure<UserResponse>(new Error(githubUserResponse.StatusCode.ToString(), githubUserResponse.Error.Message, ErrorType.Failure));
        }

        User checkUser = await FindUserByEmailAsync(githubUserResponse.Content.Email, cancellationToken);

        if (checkUser != null)
        {
            await UpdateUserAsync(githubUserResponse.Content, checkUser, cancellationToken);
            return CreateSuccessResult(checkUser);
        }

        var user = new User()
        {
            Email = githubUserResponse.Content.Email,
            AvatarUrl = githubUserResponse.Content.AvatarUrl,
            Bio = githubUserResponse.Content.Bio,
            Username = githubUserResponse.Content.Name
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

    private async Task UpdateUserAsync(GithubUserResponse githubUser, User checkUser, CancellationToken cancellationToken)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.Id, checkUser.Id);
        UpdateDefinition<User> update = Builders<User>.Update
            .Set(u => u.AvatarUrl, githubUser.AvatarUrl)
            .Set(u => u.FullName, githubUser.Name)
            .Set(u => u.Bio, githubUser.Bio);

        await context.Users.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }

    private async Task<User> FindUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq(x => x.Email, email);
        User? maybeUser = await context.Users.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return maybeUser;
    }
}

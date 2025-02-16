using FluentValidation;

namespace Application.Users.RegisterExternal;

internal sealed class RegisterExternalUserCommandValidator : AbstractValidator<RegisterExternalUserCommand>
{
    public RegisterExternalUserCommandValidator()
    {
        RuleFor(c => c.ExternalId).NotEmpty();
        RuleFor(c => c.Username).NotEmpty();
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
    }
}

using FluentValidation;

namespace Application.Users.Register;

internal sealed class RegisterExternalUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterExternalUserCommandValidator()
    {
        RuleFor(c => c.Username).NotEmpty();
        RuleFor(c => c.FullName).NotEmpty();
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.Password).NotEmpty().MinimumLength(8);
    }
}

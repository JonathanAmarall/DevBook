using FluentValidation;

namespace Application.Users.RegisterExternal;

internal sealed class RegisterExternalUserCommandValidator : AbstractValidator<RegisterExternalUserCommand>
{
    public RegisterExternalUserCommandValidator()
    {
        RuleFor(c => c.Code).NotEmpty();
    }
}

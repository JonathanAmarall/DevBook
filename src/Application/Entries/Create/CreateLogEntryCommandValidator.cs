using Application.Entries.Create;
using FluentValidation;

namespace Application.LogBook.Create;

internal class CreateLogEntryCommandValidator : AbstractValidator<CreateEntryCommand>
{
    public CreateLogEntryCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(3);
    }
}

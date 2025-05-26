using Application.LogEntry.Create;
using FluentValidation;

namespace Application.LogBook.Create;

internal class CreateLogEntryCommandValidator : AbstractValidator<CreateLogEntryCommand>
{
    public CreateLogEntryCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(3);
    }
}

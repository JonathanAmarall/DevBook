using FluentValidation;

namespace Application.LogBook.GetById;

internal sealed class GetLogEntryByIdQueryValidator : AbstractValidator<GetLogEntryByIdQuery>
{
    public GetLogEntryByIdQueryValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}


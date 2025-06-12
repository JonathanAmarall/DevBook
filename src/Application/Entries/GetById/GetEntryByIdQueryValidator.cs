using FluentValidation;

namespace Application.Entries.GetById;

internal sealed class GetEntryByIdQueryValidator : AbstractValidator<GetEntryByIdQuery>
{
    public GetEntryByIdQueryValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}


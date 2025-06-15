using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Entries;
using Domain.Repositories;
using SharedKernel;

namespace Application.Entries.GetById;

internal sealed class GetEntryByIdQueryHandler(
    IEntryRepository entryRepository,
    IUserContext userContext) : IQueryHandler<GetEntryByIdQuery, EntryResponse>
{
    public async Task<Result<EntryResponse>> Handle(GetEntryByIdQuery request, CancellationToken cancellationToken)
    {
        Entry? entry = await entryRepository.FirstOrDefaultAsync(
            x => x.Id == request.Id && x.UserId == userContext.UserId,
            cancellationToken: cancellationToken);

        return entry is null
            ? Result.Failure<EntryResponse>(EntryErrors.NotFound(request.Id))
            : new EntryResponse(entry);
    }
}

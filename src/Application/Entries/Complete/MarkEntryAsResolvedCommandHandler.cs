using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Entries.GetById;
using Domain.Entries;
using Domain.Repositories;
using SharedKernel;

namespace Application.Entries.Complete;

internal sealed class MarkEntryAsResolvedCommandHandler : ICommandHandler<MarkEntryAsResolvedCommand, EntryResponse>
{
    private readonly IUserContext _userContext;
    private readonly IEntryRepository _entryRespository;

    public MarkEntryAsResolvedCommandHandler(IUserContext userContext, IEntryRepository entryRespository)
    {
        _userContext = userContext;
        _entryRespository = entryRespository;
    }

    public async Task<Result<EntryResponse>> Handle(MarkEntryAsResolvedCommand request, CancellationToken cancellationToken)
    {
        Entry? entry = await _entryRespository.FirstOrDefaultAsync(x =>
            x.Id == request.LogId &&
            x.UserId == _userContext.UserId, cancellationToken);

        if (entry is null)
        {
            return Result.Failure<EntryResponse>(EntryErrors.NotFound(request.LogId));
        }

        entry.MarkAsResolved();

        await _entryRespository.UnitOfWork.StartTransactionAsync(cancellationToken);
        await _entryRespository.AddAsync(entry, cancellationToken: cancellationToken);
        await _entryRespository.UnitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success<EntryResponse>(new(entry));
    }
}

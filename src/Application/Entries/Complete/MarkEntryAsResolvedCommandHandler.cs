using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Entries.GetById;
using Domain.Entries;
using MongoDB.Driver;
using SharedKernel;

namespace Application.Entries.Complete;

internal sealed class MarkEntryAsResolvedCommandHandler : ICommandHandler<MarkEntryAsResolvedCommand, EntryResponse>
{
    private readonly IDatabaseContext _dbContext;
    private readonly IUserContext _userContext;

    public MarkEntryAsResolvedCommandHandler(IDatabaseContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<Result<EntryResponse>> Handle(MarkEntryAsResolvedCommand request, CancellationToken cancellationToken)
    {
        Entry? entry = await _dbContext.LogEntries
            .Find(x => x.Id == request.LogId && x.UserId == _userContext.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (entry is null)
        {
            return Result.Failure<EntryResponse>(EntryErrors.NotFound(request.LogId));
        }

        entry.MarkAsResolved();

        await _dbContext.LogEntries.ReplaceOneAsync(
            x => x.Id == entry.Id,
            entry,
            cancellationToken: cancellationToken
        );

        return Result.Success<EntryResponse>(new(entry));
    }
}

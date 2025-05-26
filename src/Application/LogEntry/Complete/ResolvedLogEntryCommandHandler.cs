using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.LogEntry.GetById;
using Domain.LogEntry;
using MongoDB.Driver;
using SharedKernel;

namespace Application.LogEntry.Complete;

internal sealed class ResolvedLogEntryCommandHandler : ICommandHandler<ResolvedLogEntryCommand, LogEntryResponse>
{
    private readonly IDatabaseContext _dbContext;
    private readonly IUserContext _userContext;

    public ResolvedLogEntryCommandHandler(IDatabaseContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<Result<LogEntryResponse>> Handle(ResolvedLogEntryCommand request, CancellationToken cancellationToken)
    {
        Domain.LogEntry.LogEntry? logEntry = await _dbContext.LogEntries
            .Find(x => x.Id == request.LogId && x.UserId == _userContext.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (logEntry is null)
        {
            return Result.Failure<LogEntryResponse>(LogEntryErrors.NotFound(request.LogId));
        }

        logEntry.MarkAsResolved();

        await _dbContext.LogEntries.ReplaceOneAsync(
            x => x.Id == logEntry.Id,
            logEntry,
            cancellationToken: cancellationToken
        );

        return Result.Success<LogEntryResponse>(new(logEntry));
    }
}

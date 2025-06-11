using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.LogEntry.GetById;
using Domain.Users;
using MongoDB.Driver;
using SharedKernel;

namespace Application.LogEntry.Create;

internal sealed class CreateLogEntryCommandHandler : ICommandHandler<CreateLogEntryCommand, LogEntryResponse>
{
    private readonly IDatabaseContext _context;
    private readonly IUserContext userContext;
    public CreateLogEntryCommandHandler(IDatabaseContext context, IUserContext userContext)
    {
        _context = context;
        this.userContext = userContext;
    }

    public async Task<Result<LogEntryResponse>> Handle(CreateLogEntryCommand request, CancellationToken cancellationToken)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq(x => x.Id, userContext.UserId);
        User user = await _context.Users.Find(filter).SingleOrDefaultAsync(cancellationToken);
        if (user == null)
        {
            return Result.Failure<LogEntryResponse>(UserErrors.NotFound(userContext.UserId));
        }

        Domain.LogEntry.LogEntry logEntry = new(
            request.Title,
            request.Description,
            request.Category,
            request.Tags,
            userContext.UserId);

        await _context.LogEntries.InsertOneAsync(logEntry, cancellationToken: cancellationToken);

        return Result.Success<LogEntryResponse>(new(logEntry));
    }
}

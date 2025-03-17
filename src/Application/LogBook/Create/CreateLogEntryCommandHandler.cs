using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.LogEntry;
using Domain.Users;
using MongoDB.Driver;
using SharedKernel;

namespace Application.LogBook.Create;

internal sealed class CreateLogEntryCommandHandler : ICommandHandler<CreateLogEntryCommand, CreateLogEntryCommandResponse>
{
    private readonly IDatabaseContext _context;
    private readonly IUserContext userContext;
    private readonly IDateTimeProvider dateTimeProvider;
    public CreateLogEntryCommandHandler(IDatabaseContext context, IUserContext userContext, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        this.userContext = userContext;
        this.dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<CreateLogEntryCommandResponse>> Handle(CreateLogEntryCommand request, CancellationToken cancellationToken)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq(x => x.Id, userContext.UserId);
        User user = await _context.Users.Find(filter).SingleOrDefaultAsync(cancellationToken);
        if (user == null)
        {
            return Result.Failure<CreateLogEntryCommandResponse>(UserErrors.NotFound(userContext.UserId));
        }

        LogEntry logEntry = new()
        {
            Category = request.Category,
            Description = request.Description,
            ProjectId = request.ProjectId,
            Title = request.Title,
            UserId = user.Id,
            Tags = request.Tags,
            Attachments = request.Attachments.Select(x => new Attachment
            {
                FileName = x.FileName,
                UploadedAt = dateTimeProvider.UtcNow,
                Url = x.Url
            }).ToList()
        };

        await _context.LogEntries.InsertOneAsync(logEntry, cancellationToken: cancellationToken);

        return Result.Success<CreateLogEntryCommandResponse>(new());
    }
}

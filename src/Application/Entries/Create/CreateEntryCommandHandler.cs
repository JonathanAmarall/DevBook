using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Entries.GetById;
using Domain.Entries;
using Domain.Users;
using MongoDB.Driver;
using SharedKernel;

namespace Application.Entries.Create;

internal sealed class CreateEntryCommandHandler : ICommandHandler<CreateEntryCommand, EntryResponse>
{
    private readonly IDatabaseContext _context;
    private readonly IUserContext userContext;
    public CreateEntryCommandHandler(IDatabaseContext context, IUserContext userContext)
    {
        _context = context;
        this.userContext = userContext;
    }

    public async Task<Result<EntryResponse>> Handle(CreateEntryCommand request, CancellationToken cancellationToken)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq(x => x.Id, userContext.UserId);
        User user = await _context.GetCollection<User>("Users").Find(filter).SingleOrDefaultAsync(cancellationToken);
        if (user == null)
        {
            return Result.Failure<EntryResponse>(UserErrors.NotFound(userContext.UserId));
        }

        Entry entry = new(
            request.Title,
            request.Description,
            request.Category,
            request.Tags,
            userContext.UserId);

        await _context.GetCollection<Entry>("Entries").InsertOneAsync(entry, cancellationToken: cancellationToken);

        return Result.Success<EntryResponse>(new(entry));
    }
}

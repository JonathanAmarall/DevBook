using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Entries.GetById;
using Domain.Entries;
using Domain.Repositories;
using Domain.Users;
using SharedKernel;

namespace Application.Entries.Create;

internal sealed class CreateEntryCommandHandler : ICommandHandler<CreateEntryCommand, EntryResponse>
{
    private readonly IUserContext userContext;
    private readonly IEntryRepository _entryRepository;
    private readonly IUserRespository _userRespository;

    public CreateEntryCommandHandler(IUserContext userContext, IEntryRepository entryRepository, IUserRespository userRespository)
    {
        this.userContext = userContext;
        _entryRepository = entryRepository;
        _userRespository = userRespository;
    }

    public async Task<Result<EntryResponse>> Handle(CreateEntryCommand request, CancellationToken cancellationToken)
    {

        User? user = await _userRespository.FirstOrDefaultAsync(u => u.Id == userContext.UserId, cancellationToken);
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

        await _entryRepository.UnitOfWork.StartTransactionAsync(cancellationToken);
        await _entryRepository.AddAsync(entry, cancellationToken: cancellationToken);
        await _entryRepository.UnitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success<EntryResponse>(new(entry));
    }
}

using Application.Abstractions.Data;
using Domain.Entries;
using Domain.Repositories;

namespace Infrastructure.Database.Repositories;

public sealed class EntryRepository : MongoRepository<Entry>, IEntryRepository
{
    public const string CollectionName = "Entries";

    public EntryRepository(IDatabaseContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    protected override string GetCollectionName() => CollectionName;
}

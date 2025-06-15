using Application.Abstractions.Data;
using Domain.Repositories;
using Domain.Users;

namespace Infrastructure.Database.Repositories;

public sealed class UserRepository : MongoRepository<User>, IUserRepository
{
    public const string CollectionName = "Users";

    public UserRepository(IDatabaseContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    protected override string GetCollectionName() => CollectionName;
}

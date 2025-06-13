namespace Domain.Repositories;

public interface IUnitOfWork
{
    IDisposable Session { get; }
    Task CommitChangesAsync(CancellationToken cancellationToken = default);
    Task StartTransactionAsync(CancellationToken cancellationToken = default);
}

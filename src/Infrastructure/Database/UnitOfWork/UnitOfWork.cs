using Application.Abstractions.Data;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Infrastructure.Database.UnitOfWork;
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly IDatabaseContext _databaseContext;
    private readonly ILogger<UnitOfWork> _logger;
    private IClientSessionHandle _session;

    public UnitOfWork(IDatabaseContext databaseContext, ILogger<UnitOfWork> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public IDisposable Session => _session;

    public async Task StartTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_session?.IsInTransaction is true)
        {
            return;
        }

        _session ??= await _databaseContext.Client.StartSessionAsync(cancellationToken: cancellationToken);
        _session.StartTransaction(new TransactionOptions(maxCommitTime: TimeSpan.FromMinutes(1)));
        _logger.LogInformation("Start session into StartTransactionAsync command.");
    }

    public async Task CommitChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _session!.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await _session!.AbortTransactionAsync(cancellationToken);
            _logger.LogError(ex, "An error happen on save commit database transaction");
        }
    }
}

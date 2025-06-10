namespace SharedKernel;

public abstract class Entity
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public DateTime CreatedOnUtc { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedOnUtc { get; protected set; }

    private readonly List<IDomainEvent> _domainEvents = [];
    public List<IDomainEvent> DomainEvents => [.. _domainEvents];

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}

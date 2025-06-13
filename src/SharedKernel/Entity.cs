namespace SharedKernel;

public abstract class Entity
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public DateTime CreatedOnUtc { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedOnUtc { get; protected set; }

    public void UpdateLastModifiedDate()
    {
        UpdatedOnUtc = DateTime.UtcNow;
    }
}

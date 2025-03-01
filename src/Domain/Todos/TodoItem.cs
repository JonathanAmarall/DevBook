using SharedKernel;

namespace Domain.Todos;

public sealed class TodoItem : Entity
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Description { get; init; }
    public DateTime? DueDate { get; init; }
    public List<string> Labels { get; init; } = [];
    public bool IsCompleted { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public Priority Priority { get; init; }
}

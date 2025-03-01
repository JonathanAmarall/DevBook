using SharedKernel;

namespace Domain.Project;

public class Project : Entity
{
    public string Id { get; init; } = Guid.NewGuid().ToString(); // ObjectId do MongoDB
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    // Referência ao usuário proprietário do projeto
    public string UserId { get; init; } = string.Empty;
}


using SharedKernel;

namespace Domain.Project;

public class Project : Entity
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); // ObjectId do MongoDB
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Referência ao usuário proprietário do projeto
    public string UserId { get; set; } = string.Empty;
}


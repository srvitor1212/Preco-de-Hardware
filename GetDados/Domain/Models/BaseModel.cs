namespace Domain.Models;

public abstract class BaseModel
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.Now;
    public DateTimeOffset? UpdatedAt { get; private set; }

    public void SetDataAtualizacao() => UpdatedAt = DateTimeOffset.Now;
}

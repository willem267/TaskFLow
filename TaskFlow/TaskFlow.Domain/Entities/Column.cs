namespace TaskFlow.Domain.Entities;

public class Column
{
    public Guid Id { get; set; }
    public Guid BoardId { get; set; }
    public string Name { get; set; } = string.Empty;
    public float Position { get; set; }

    public Board Board { get; set; } = null!;
    public ICollection<TaskItem> Tasks { get; set; } = [];
}
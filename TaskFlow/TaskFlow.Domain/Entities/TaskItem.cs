namespace TaskFlow.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public Guid ColumnId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public float Position { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid? AssigneeId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }

    public Column Column { get; set; } = null!;
    public User? Assignee { get; set; }
}
namespace TaskFlow.Domain.Entities;

public class ActivityLog
{
    public Guid Id { get; set; }
    public Guid BoardId { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;  
    public string EntityType { get; set; } = string.Empty; 
    public string? EntityName { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}
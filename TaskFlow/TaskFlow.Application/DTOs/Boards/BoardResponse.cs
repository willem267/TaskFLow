namespace TaskFlow.Application.DTOs.Boards;

public record BoardSummaryResponse(
    Guid Id,
    string Name,
    string OwnerName,
    long MemberCount,
    DateTime CreatedAt
    
);

public record AssigneeResponse(Guid Id, string Name, string? Avatar);

public record TaskResponse(
    Guid Id,
    Guid ColumnId,
    string Title,
    string? Description,
    float Position,
    DateTime? DueDate,
    DateTime CreatedAt,
    AssigneeResponse? Assignee
);


public class ColumnResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public float Position { get; set; }
    public List<TaskResponse> Tasks { get; set; } = [];
}

public class BoardDetailResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } 
    public List<ColumnResponse> Columns { get; set; } = [];
}
namespace TaskFlow.Application.DTOs.Tasks;

public record CreateTaskRequest(
    Guid ColumnId,
    string Title,
    string? Description,
    DateTime? DueDate,
    Guid? AssigneeId
);

public record UpdateTaskRequest(
    string Title,
    string? Description,
    DateTime? DueDate,
    Guid? AssigneeId
);

public record MoveTaskRequest(
    Guid TargetColumnId,
    Guid? PreviousTaskId,  
    Guid? NextTaskId       
);
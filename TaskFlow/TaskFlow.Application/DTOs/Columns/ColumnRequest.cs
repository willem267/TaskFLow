namespace TaskFlow.Application.DTOs.Columns;

public record CreateColumnRequest(Guid BoardId, string Name);
public record UpdateColumnRequest(string Name);
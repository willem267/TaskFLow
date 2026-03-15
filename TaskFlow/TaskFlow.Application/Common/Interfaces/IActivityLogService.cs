namespace TaskFlow.Application.Common.Interfaces;

public interface IActivityLogService
{
    Task LogAsync(Guid boardId, string action, string entityType, string? entityName = null);
}
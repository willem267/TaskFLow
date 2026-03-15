using TaskFlow.Application.DTOs.Boards;
using TaskFlow.Application.DTOs.Realtime;

namespace TaskFlow.Application.Common.Interfaces;

public interface IBoardNotifier
{
    Task NotifyTaskCreatedAsync(Guid boardId, TaskResponse task);
    Task NotifyTaskUpdatedAsync(Guid boardId, TaskResponse task);
    Task NotifyTaskDeletedAsync(Guid boardId, Guid taskId);
    Task NotifyTaskMovedAsync(Guid boardId, TaskMovedPayload payload);
    Task NotifyColumnCreatedAsync(Guid boardId, ColumnResponse column);
    Task NotifyColumnUpdatedAsync(Guid boardId, ColumnResponse column);
    Task NotifyColumnDeletedAsync(Guid boardId, Guid columnId);
}
using Microsoft.AspNetCore.SignalR;
using TaskFlow.API.Hubs;
using TaskFlow.Application.Common.Constants;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs.Boards;
using TaskFlow.Application.DTOs.Realtime;

namespace TaskFlow.Infrastructure.Realtime;

public class BoardNotifier : IBoardNotifier
{
    private readonly IHubContext<BoardHub> _hubContext;

    public BoardNotifier(IHubContext<BoardHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyTaskCreatedAsync(Guid boardId, TaskResponse task)
        => SendAsync(boardId, BoardEvents.TaskCreated, task);

    public Task NotifyTaskUpdatedAsync(Guid boardId, TaskResponse task)
        => SendAsync(boardId, BoardEvents.TaskUpdated, task);

    public Task NotifyTaskDeletedAsync(Guid boardId, Guid taskId)
        => SendAsync(boardId, BoardEvents.TaskDeleted, new { taskId });

    public Task NotifyTaskMovedAsync(Guid boardId, TaskMovedPayload payload)
        => SendAsync(boardId, BoardEvents.TaskMoved, payload);

    public Task NotifyColumnCreatedAsync(Guid boardId, ColumnResponse column)
        => SendAsync(boardId, BoardEvents.ColumnCreated, column);

    public Task NotifyColumnUpdatedAsync(Guid boardId, ColumnResponse column)
        => SendAsync(boardId, BoardEvents.ColumnUpdated, column);

    public Task NotifyColumnDeletedAsync(Guid boardId, Guid columnId)
        => SendAsync(boardId, BoardEvents.ColumnDeleted, new { columnId });

    private Task SendAsync(Guid boardId, string eventName, object payload)
        => _hubContext.Clients
            .Group($"board:{boardId}")
            .SendAsync(eventName, payload);
}
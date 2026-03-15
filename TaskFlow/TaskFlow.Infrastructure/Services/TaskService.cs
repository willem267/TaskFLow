using TaskFlow.Application.Common.Constants;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs.Boards;
using TaskFlow.Application.DTOs.Realtime;
using TaskFlow.Application.DTOs.Tasks;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence.Context;

namespace TaskFlow.Infrastructure.Services;

public class TaskService : ITaskService
{
    private readonly ApplicationDbContext _context;
    private readonly ITaskRepository _taskRepository;
    private readonly IBoardRepository _boardRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IBoardNotifier _notifier;
    private readonly IActivityLogService _activityLog;

    public TaskService(ApplicationDbContext context, ITaskRepository taskRepository, IBoardRepository boardRepository,
        ICurrentUser currentUser, IBoardNotifier notifier, IActivityLogService activityLog)
    {
        _context = context;
        _taskRepository = taskRepository;
        _boardRepository = boardRepository;
        _currentUser = currentUser;
        _notifier = notifier;
        _activityLog = activityLog;
    }
    
    public async Task<TaskResponse> CreateTaskAsync(CreateTaskRequest request)
    {
        var boardId = await _taskRepository.GetBoardIdByColumnIdAsync(request.ColumnId);
        await EnsureMemberAsync(boardId);

        var maxPosition = await _taskRepository.GetMaxPositionInColumnAsync(request.ColumnId);

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            ColumnId = request.ColumnId,
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            AssigneeId = request.AssigneeId,
            Position = maxPosition + 1000,
            CreatedBy = _currentUser.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        var response = MapToResponse(task);

        await _notifier.NotifyTaskCreatedAsync(boardId, response);
        await _activityLog.LogAsync(boardId, ActivityActions.Created, "task", task.Title);

        return response;
    }

    public async Task UpdateTaskAsync(Guid taskId, UpdateTaskRequest request)
    {
        var task = await GetTaskOrThrowAsync(taskId);
        var boardId = await _taskRepository.GetBoardIdByColumnIdAsync(task.ColumnId);
        await EnsureMemberAsync(boardId);

        task.Title = request.Title;
        task.Description = request.Description;
        task.DueDate = request.DueDate;
        task.AssigneeId = request.AssigneeId;

        await _context.SaveChangesAsync();

        await _notifier.NotifyTaskUpdatedAsync(boardId, MapToResponse(task));
        await _activityLog.LogAsync(boardId, ActivityActions.Updated, "task", task.Title);
    }

    public async Task DeleteTaskAsync(Guid taskId)
    {
        var task = await GetTaskOrThrowAsync(taskId);
        var boardId = await _taskRepository.GetBoardIdByColumnIdAsync(task.ColumnId);
        await EnsureMemberAsync(boardId);

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        await _notifier.NotifyTaskDeletedAsync(boardId, taskId);
        await _activityLog.LogAsync(boardId, ActivityActions.Deleted, "task", task.Title);
    }

    public async Task<TaskResponse> MoveTaskAsync(Guid taskId, MoveTaskRequest request)
    {
        var task = await GetTaskOrThrowAsync(taskId);
        var boardId = await _taskRepository.GetBoardIdByTaskIdAsync(taskId);
        await EnsureMemberAsync(boardId);

        var newPosition = await CalculatePositionAsync(request);

        task.ColumnId = request.TargetColumnId;
        task.Position = newPosition;

        await _context.SaveChangesAsync();

        var payload = new TaskMovedPayload(taskId, request.TargetColumnId, newPosition);
        await _notifier.NotifyTaskMovedAsync(boardId, payload);
        await _activityLog.LogAsync(boardId, ActivityActions.Moved, "task", task.Title);

        return MapToResponse(task);
    }
    
    
    // ---- Position logic ----

    private async Task<float> CalculatePositionAsync(MoveTaskRequest request)
    {
        // Kéo lên đầu column — không có task phía trên
        if (request.PreviousTaskId is null && request.NextTaskId is not null)
        {
            var nextPosition = await _taskRepository.GetPositionByIdAsync(request.NextTaskId.Value);
            return (nextPosition ?? 1000) / 2;
        }

        // Kéo xuống cuối column — không có task phía dưới
        if (request.PreviousTaskId is not null && request.NextTaskId is null)
        {
            var prevPosition = await _taskRepository.GetPositionByIdAsync(request.PreviousTaskId.Value);
            return (prevPosition ?? 0) + 1000;
        }

        // Kéo vào giữa 2 task
        if (request.PreviousTaskId is not null && request.NextTaskId is not null)
        {
            var prevPosition = await _taskRepository.GetPositionByIdAsync(request.PreviousTaskId.Value);
            var nextPosition = await _taskRepository.GetPositionByIdAsync(request.NextTaskId.Value);
            return ((prevPosition ?? 0) + (nextPosition ?? 0)) / 2;
        }

        // Column rỗng
        return 1000;
    }
    
    
    // ---- Helpers ----

    private async Task<TaskItem> GetTaskOrThrowAsync(Guid taskId)
    {
        return await _context.Tasks.FindAsync(taskId)
               ?? throw new KeyNotFoundException("Task not found.");
    }

    private async Task EnsureMemberAsync(Guid boardId)
    {
        var isMember = await _boardRepository.IsMemberAsync(boardId, _currentUser.Id);
        if (!isMember)
            throw new UnauthorizedAccessException("You are not a member of this board.");
    }

    private static TaskResponse MapToResponse(TaskItem task) => new(
        task.Id,
        task.ColumnId,
        task.Title,
        task.Description,
        task.Position,
        task.DueDate,
        task.CreatedAt,
        null // assignee load riêng nếu cần
    );
}
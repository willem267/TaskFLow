using TaskFlow.Application.DTOs.Boards;
using TaskFlow.Application.DTOs.Tasks;

namespace TaskFlow.Application.Common.Interfaces;

public interface ITaskService
{
    Task<TaskResponse> CreateTaskAsync(CreateTaskRequest request);
    Task UpdateTaskAsync(Guid taskId, UpdateTaskRequest request);
    Task DeleteTaskAsync(Guid taskId);
    Task<TaskResponse> MoveTaskAsync(Guid taskId, MoveTaskRequest request);
}
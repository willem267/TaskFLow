namespace TaskFlow.Application.Common.Interfaces;

public interface ITaskRepository
{
    Task<float> GetMaxPositionInColumnAsync(Guid columnId);
    Task<float?> GetPositionByIdAsync(Guid taskId);
    Task<Guid> GetBoardIdByColumnIdAsync(Guid columnId);
    Task<Guid> GetBoardIdByTaskIdAsync(Guid taskId);
}
namespace TaskFlow.Application.Common.Interfaces;

public interface IColumnRepository
{
    Task<float> GetMaxPositionAsync(Guid boardId);
}
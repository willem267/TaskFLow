using TaskFlow.Application.DTOs.Boards;
using TaskFlow.Application.DTOs.Columns;

namespace TaskFlow.Application.Common.Interfaces;

public interface IColumnService
{
    Task<ColumnResponse> CreateColumnAsync(CreateColumnRequest request);
    Task UpdateColumnAsync(Guid columnId, UpdateColumnRequest request);
    Task DeleteColumnAsync(Guid columnId);
}
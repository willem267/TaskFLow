using TaskFlow.Application.DTOs.ActivityLogs;
using TaskFlow.Application.DTOs.Boards;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces;

public interface IBoardRepository
{
    Task<List<BoardSummaryResponse>> GetBoardsByUserIdAsync(Guid userId);
    Task<BoardDetailResponse?> GetBoardDetailAsync(Guid boardId);
    Task<bool> IsMemberAsync(Guid boardId, Guid userId);
    Task<string?> GetMemberRoleAsync(Guid boardId, Guid userId);
    Task<List<ActivityLogResponse>> GetActivityLogsAsync(Guid boardId, int limit = 50);
    Task<List<MemberResponse>> GetMembersAsync(Guid boardId);
}
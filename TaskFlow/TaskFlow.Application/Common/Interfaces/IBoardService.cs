using TaskFlow.Application.DTOs.ActivityLogs;
using TaskFlow.Application.DTOs.Boards;

namespace TaskFlow.Application.Common.Interfaces;

public interface IBoardService
{
    Task<List<BoardSummaryResponse>> GetMyBoardsAsync();
    Task<BoardDetailResponse> GetBoardDetailAsync(Guid boardId);
    Task<BoardSummaryResponse> CreateBoardAsync(CreateBoardRequest request);
    Task UpdateBoardAsync(Guid boardId, UpdateBoardRequest request);
    Task DeleteBoardAsync(Guid boardId);
    Task<List<ActivityLogResponse>> GetActivityLogsAsync(Guid boardId, int limit);
    Task<List<MemberResponse>> GetMembersAsync(Guid boardId);
    Task InviteMemberAsync(Guid boardId, InviteMemberRequest request);
    Task RemoveMemberAsync(Guid boardId, Guid userId);
}
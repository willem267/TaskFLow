using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs.ActivityLogs;
using TaskFlow.Application.DTOs.Boards;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enum;
using TaskFlow.Infrastructure.Persistence.Context;

namespace TaskFlow.Infrastructure.Services;

public class BoardService : IBoardService
{
    private readonly IBoardRepository _boardRepository;
    private readonly ICurrentUser _currentUser;
    private readonly ApplicationDbContext _context;

    public BoardService(
        ApplicationDbContext context,
        IBoardRepository boardRepository,
        ICurrentUser currentUser)
    {
        _context = context;
        _boardRepository = boardRepository;
        _currentUser = currentUser;
    }

    public async Task<List<BoardSummaryResponse>> GetMyBoardsAsync() =>
        await _boardRepository.GetBoardsByUserIdAsync(_currentUser.Id);
   

    public async Task<BoardDetailResponse> GetBoardDetailAsync(Guid boardId)
    {
        await EnsureMemberAsync(boardId);

        var board = await _boardRepository.GetBoardDetailAsync(boardId);
        if (board is null)
            throw new KeyNotFoundException("Board not found.");

        return board;
    }

    public async Task<BoardSummaryResponse> CreateBoardAsync(CreateBoardRequest request)
    {
        var board = new Board
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            OwnerId = _currentUser.Id,
            CreatedAt = DateTime.UtcNow
        };

        var member = new BoardMember
        {
            BoardId = board.Id,
            UserId = _currentUser.Id,
            Role = BoardMemberRole.Owner
        };

        await _context.Boards.AddAsync(board);
        await _context.BoardMembers.AddAsync(member);
        await _context.SaveChangesAsync();

        var boards = await _boardRepository.GetBoardsByUserIdAsync(_currentUser.Id);
        return boards.First(b => b.Id == board.Id);
    }

    public async Task UpdateBoardAsync(Guid boardId, UpdateBoardRequest request)
    {
        await EnsureOwnerAsync(boardId);

        var board = await _context.Boards.FindAsync(boardId)
                    ?? throw new KeyNotFoundException("Board not found.");

        board.Name = request.Name;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteBoardAsync(Guid boardId)
    {
       
        await EnsureOwnerAsync(boardId);

        var board = await _context.Boards.FindAsync(boardId)
                    ?? throw new KeyNotFoundException("Board not found.");

        _context.Boards.Remove(board);
        await _context.SaveChangesAsync();
    }
    
    
    public async Task<List<ActivityLogResponse>> GetActivityLogsAsync(Guid boardId, int limit)
    {
        await EnsureMemberAsync(boardId);
        return await _boardRepository.GetActivityLogsAsync(boardId, limit);
    }
    
    
    public async Task<List<MemberResponse>> GetMembersAsync(Guid boardId)
    {
        await EnsureMemberAsync(boardId);
        return await _boardRepository.GetMembersAsync(boardId);
    }

    public async Task InviteMemberAsync(Guid boardId, InviteMemberRequest request)
    {
        await EnsureOwnerAsync(boardId);

        // Tìm user theo email
        var user = await _context.Users
                       .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower().Trim())
                   ?? throw new KeyNotFoundException("User not found.");

        // Kiểm tra đã là member chưa
        var exists = await _boardRepository.IsMemberAsync(boardId, user.Id);
        if (exists)
            throw new InvalidOperationException("User is already a member.");

        await _context.BoardMembers.AddAsync(new BoardMember
        {
            BoardId = boardId,
            UserId = user.Id,
            Role = BoardMemberRole.Member
        });

        await _context.SaveChangesAsync();
    }

    public async Task RemoveMemberAsync(Guid boardId, Guid userId)
    {
        await EnsureOwnerAsync(boardId);

        // Không cho xóa chính mình
        if (userId == _currentUser.Id)
            throw new InvalidOperationException("Cannot remove yourself from the board.");

        var member = await _context.BoardMembers
                         .FirstOrDefaultAsync(bm => bm.BoardId == boardId && bm.UserId == userId)
                     ?? throw new KeyNotFoundException("Member not found.");

        _context.BoardMembers.Remove(member);
        await _context.SaveChangesAsync();
    }
    
    //helpers
    private async Task EnsureMemberAsync(Guid boardId)
    {
        var isMember = await _boardRepository.IsMemberAsync(boardId, _currentUser.Id);
        if (!isMember)
            throw new UnauthorizedAccessException("You are not a member of this board.");
    }

    private async Task EnsureOwnerAsync(Guid boardId)
    {
        var role = await _boardRepository.GetMemberRoleAsync(boardId, _currentUser.Id);
        if (role != "Owner")
            throw new UnauthorizedAccessException("Only the board owner can perform this action.");
    }
}
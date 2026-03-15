using TaskFlow.Application.Common.Constants;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs.Boards;
using TaskFlow.Application.DTOs.Columns;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence.Context;

namespace TaskFlow.Infrastructure.Services;

public class ColumnService : IColumnService
{
    private readonly ApplicationDbContext _context;
    private readonly IBoardRepository _boardRepository;
    private readonly IColumnRepository _columnRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IBoardNotifier _notifier;
    private readonly IActivityLogService _activityLog;

    public ColumnService(
        ApplicationDbContext context,
        IBoardRepository boardRepository,
        IColumnRepository columnRepository,
        ICurrentUser currentUser,
        IBoardNotifier notifier,
        IActivityLogService activityLog)
    {
        _context = context;
        _boardRepository = boardRepository;
        _columnRepository = columnRepository;
        _currentUser = currentUser;
        _notifier = notifier;
        _activityLog = activityLog;
    }
    
    public async Task<ColumnResponse> CreateColumnAsync(CreateColumnRequest request)
    {
        
        await EnsureMemberAsync(request.BoardId);

        var maxPosition = await _columnRepository.GetMaxPositionAsync(request.BoardId);

        var column = new Column
        {
            Id = Guid.NewGuid(),
            BoardId = request.BoardId,
            Name = request.Name,
            Position = maxPosition + 1000
        };

        await _context.Columns.AddAsync(column);
        await _context.SaveChangesAsync();

        var response =  new ColumnResponse
        {
            Id = column.Id,
            Name = column.Name,
            Position = column.Position
        };
        
        await _notifier.NotifyColumnCreatedAsync(request.BoardId, response);
        await _activityLog.LogAsync(request.BoardId, ActivityActions.Created, "column", column.Name);
        return response;
    }

    public async Task UpdateColumnAsync(Guid columnId, UpdateColumnRequest request)
    {
        var column = await GetColumnOrThrowAsync(columnId);
        await EnsureMemberAsync(column.BoardId);

        column.Name = request.Name;
        await _context.SaveChangesAsync();
        var response = new ColumnResponse
        {
            Id = column.Id,
            Name = column.Name,
            Position = column.Position
        };

        await _notifier.NotifyColumnUpdatedAsync(column.BoardId, response);
        await _activityLog.LogAsync(column.BoardId, ActivityActions.Updated, "column", column.Name);
    }

    public async Task DeleteColumnAsync(Guid columnId)
    {
        var column = await GetColumnOrThrowAsync(columnId);
        await EnsureMemberAsync(column.BoardId);

        _context.Columns.Remove(column);
        await _context.SaveChangesAsync();
        
        await _notifier.NotifyColumnDeletedAsync(column.BoardId, columnId);
        await _activityLog.LogAsync(column.BoardId, ActivityActions.Deleted, "column", column.Name);
    }
    
    //helpers
    private async Task EnsureMemberAsync(Guid boardId)
    {
        var isMember = await _boardRepository.IsMemberAsync(boardId, _currentUser.Id);
        if (!isMember)
            throw new UnauthorizedAccessException("You are not a member of this board.");
    }   
    
    private async Task<Column> GetColumnOrThrowAsync(Guid columnId)
    {
        return await _context.Columns.FindAsync(columnId)
               ?? throw new KeyNotFoundException("Column not found.");
    }

}
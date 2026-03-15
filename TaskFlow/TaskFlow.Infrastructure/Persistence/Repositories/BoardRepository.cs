using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs.ActivityLogs;
using TaskFlow.Application.DTOs.Boards;
using TaskFlow.Infrastructure.Persistence.Context;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public class BoardRepository : IBoardRepository
{
    private readonly ApplicationDbContext _context;
    
    public BoardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BoardSummaryResponse>> GetBoardsByUserIdAsync(Guid userId)
    {
        var connection = _context.Database.GetDbConnection();
        
        if(connection.State !=ConnectionState.Open)
            await connection.OpenAsync();

        const string sql = """
                  SELECT 
                      b.id,
                      b.name,
                      u.name           AS "OwnerName",
                      COUNT(bm.user_id) AS "MemberCount",
                      b.created_at     AS "CreatedAt"
                  FROM boards b
                  INNER JOIN users u ON u.id = b.owner_id
                  INNER JOIN board_members bm ON bm.board_id = b.id
                  WHERE b.id IN (
                      SELECT board_id FROM board_members WHERE user_id = @UserId
                  )
                  GROUP BY b.id, u.name
                  ORDER BY b.created_at DESC
                  """;
        var result = await connection.QueryAsync<BoardSummaryResponse>(sql, new {userId});
        return result.ToList();
    }
    
   public async Task<BoardDetailResponse?> GetBoardDetailAsync(Guid boardId)
{
    var connection = _context.Database.GetDbConnection();
    
    if (connection.State != ConnectionState.Open)
        await connection.OpenAsync();
    
    const string sql = """
        SELECT
            b.id          AS "BoardId",
            b.name        AS "BoardName",
            b.created_at  AS "BoardCreatedAt",
            c.id          AS "ColumnId",
            c.name        AS "ColumnName",
            c.position    AS "ColumnPosition",
            t.id          AS "TaskId",
            t.title       AS "TaskTitle",
            t.description AS "TaskDescription",
            t.position    AS "TaskPosition",
            t.due_date    AS "TaskDueDate",
            t.created_at  AS "TaskCreatedAt",
            u.id          AS "AssigneeId",
            u.name        AS "AssigneeName",
            u.avatar      AS "AssigneeAvatar"
        FROM boards b
        LEFT JOIN columns c ON c.board_id = b.id
        LEFT JOIN tasks t ON t.column_id = c.id
        LEFT JOIN users u ON u.id = t.assignee_id
        WHERE b.id = @BoardId
        ORDER BY c.position, t.position
    """;

    BoardDetailResponse? board = null;
    var columnMap = new Dictionary<Guid, ColumnResponse>();
    var taskSet = new HashSet<Guid>();
    
    var rows = await connection.QueryAsync(sql, new { BoardId = boardId });
    
    foreach (var row in rows)
    {
        if (board is null)
        {
            board = new BoardDetailResponse
            {
                Id = row.BoardId,
                Name = row.BoardName,
                CreatedAt = row.BoardCreatedAt
            };
        }
    
        
        if (row.ColumnId != null)
        {
            Guid columnId = (Guid)row.ColumnId;
    
            if (!columnMap.ContainsKey(columnId))
            {
                var column = new ColumnResponse
                {
                    Id = columnId,
                    Name = row.ColumnName,
                    Position = (float)row.ColumnPosition
                };
                columnMap[columnId] = column;
                board.Columns.Add(column);
            }
    
            if (row.TaskId != null)
            {
                Guid taskId = (Guid)row.TaskId;
    
                if (taskSet.Add(taskId))
                {
                    AssigneeResponse? assignee = row.AssigneeId == null ? null
                        : new AssigneeResponse(
                            (Guid)row.AssigneeId,
                            (string)row.AssigneeName,
                            (string?)row.AssigneeAvatar);
    
                    var task = new TaskResponse(
                        taskId,
                        columnId, 
                        (string)row.TaskTitle,
                        (string?)row.TaskDescription,
                        (float)row.TaskPosition,
                        (DateTime?)row.TaskDueDate,
                        (DateTime)row.TaskCreatedAt,
                        assignee);
    
                    columnMap[columnId].Tasks.Add(task);
                }
            }
        }
    }

    return board;
}
    
    public async Task<List<ActivityLogResponse>> GetActivityLogsAsync(Guid boardId, int limit = 50)
    {
        var connection = _context.Database.GetDbConnection();
        
        if(connection.State !=ConnectionState.Open)
            await connection.OpenAsync();
        
        const string sql = """
                               SELECT
                                   al.id,
                                   al.action,
                                   al.entity_type,
                                   al.entity_name,
                                   u.name AS user_name,
                                   al.created_at
                               FROM activity_logs al
                               INNER JOIN users u ON u.id = al.user_id
                               WHERE al.board_id = @BoardId
                               ORDER BY al.created_at DESC
                               LIMIT @Limit
                           """;

        var result = await connection.QueryAsync<ActivityLogResponse>(sql, new { BoardId = boardId, Limit = limit });
        return result.ToList();
    }
    

    public async Task<bool> IsMemberAsync(Guid boardId, Guid userId)
    {
        var connection = _context.Database.GetDbConnection();
        
        if(connection.State !=ConnectionState.Open)
            await connection.OpenAsync();
        
        const string sql = """
                               SELECT COUNT(1) FROM board_members
                               WHERE board_id = @BoardId AND user_id = @UserId
                           """;
        var count = await connection.ExecuteScalarAsync<int>(sql, new { BoardId = boardId, UserId = userId });
        return count > 0;
        
    }

    public async Task<string?> GetMemberRoleAsync(Guid boardId, Guid userId)
    {
        var connection = _context.Database.GetDbConnection();
        
        if(connection.State !=ConnectionState.Open)
            await connection.OpenAsync();
        
        const string sql = """
                               SELECT role FROM board_members
                               WHERE board_id = @BoardId AND user_id = @UserId
                           """;

        return await connection.QueryFirstOrDefaultAsync<string>(sql, new { BoardId = boardId, UserId = userId });
    }
    
    public async Task<List<MemberResponse>> GetMembersAsync(Guid boardId)
    {
        var connection = _context.Database.GetDbConnection();
        
        if(connection.State !=ConnectionState.Open)
            await connection.OpenAsync();
        const string sql = """
                               SELECT u.id AS UserId, u.name, u.email, bm.role
                               FROM board_members bm
                               INNER JOIN users u ON u.id = bm.user_id
                               WHERE bm.board_id = @BoardId
                           """;

        var result = await connection.QueryAsync<MemberResponse>(sql, new { BoardId = boardId });
        return result.ToList();
    }
}
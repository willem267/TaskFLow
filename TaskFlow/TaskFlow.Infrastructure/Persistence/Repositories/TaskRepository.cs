using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Infrastructure.Persistence.Context;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public class TaskRepository: ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<float> GetMaxPositionInColumnAsync(Guid columnId)
    {
        var connection = _context.Database.GetDbConnection();
        
        if(connection.State != ConnectionState.Open) await connection.OpenAsync();

        const string sql = """
                           SELECT COALESCE(MAX(position), 0)
                           FROM tasks
                           WHERE column_id = @ColumnId
                           """;
        return await connection.ExecuteScalarAsync<float>(sql, new { ColumnId = columnId });
    }

    public async Task<float?> GetPositionByIdAsync(Guid taskId)
    {
        var connection = _context.Database.GetDbConnection();
        
        if(connection.State != ConnectionState.Open) await connection.OpenAsync();
        
        const string sql = """
                               SELECT position FROM tasks
                               WHERE id = @TaskId
                           """;

        return await connection.QueryFirstOrDefaultAsync<float?>(sql, new { TaskId = taskId });
        
    }

    public async Task<Guid> GetBoardIdByColumnIdAsync(Guid columnId)
    {
        var connection = _context.Database.GetDbConnection();
        
        if(connection.State != ConnectionState.Open) await connection.OpenAsync();
        
        const string sql = """
                               SELECT board_id FROM columns
                               WHERE id = @ColumnId
                           """;

        return await connection.ExecuteScalarAsync<Guid>(sql, new { ColumnId = columnId });
    }

    public async Task<Guid> GetBoardIdByTaskIdAsync(Guid taskId)
    {
        var connection = _context.Database.GetDbConnection();
        
        if(connection.State != ConnectionState.Open) await connection.OpenAsync();
        const string sql = """
                               SELECT c.board_id FROM tasks t
                               INNER JOIN columns c ON c.id = t.column_id
                               WHERE t.id = @TaskId
                           """;

        return await connection.ExecuteScalarAsync<Guid>(sql, new { TaskId = taskId });
    }
}
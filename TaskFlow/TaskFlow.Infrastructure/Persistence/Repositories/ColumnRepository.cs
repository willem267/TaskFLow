using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Infrastructure.Persistence.Context;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public class ColumnRepository : IColumnRepository
{
    private readonly ApplicationDbContext _context;

    public ColumnRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<float> GetMaxPositionAsync(Guid boardId)
    {
        var connection = _context.Database.GetDbConnection();
        
        if(connection.State!=ConnectionState.Open)
            await connection.OpenAsync();
        
        const string sql = """
                               SELECT COALESCE(MAX(position), 0)
                               FROM columns
                               WHERE board_id = @BoardId
                           """;

        return await connection.ExecuteScalarAsync<float>(sql, new { BoardId = boardId });
    }
}
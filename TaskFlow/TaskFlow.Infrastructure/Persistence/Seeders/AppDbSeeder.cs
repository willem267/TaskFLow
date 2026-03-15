using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enum;
using TaskFlow.Infrastructure.Persistence.Context;

namespace TaskFlow.Infrastructure.Persistence.Seeders;

public static class AppDbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (context.Users.Any())
        {
            return;
        }
        
        // Seed users
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var users = new List<User>
        {
            new() {
                Id = userId1,
                Email = "alice@example.com",
                Name = "Alice",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                CreatedAt = DateTime.UtcNow
            },
            new() {
                Id = userId2,
                Email = "bob@example.com",
                Name = "Bob",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Users.AddRangeAsync(users);

        // Seed board
        var boardId = Guid.NewGuid();
        var board = new Board
        {
            Id = boardId,
            Name = "My First Board",
            OwnerId = userId1,
            CreatedAt = DateTime.UtcNow
        };

        await context.Boards.AddAsync(board);

        // Seed board member (owner)
        await context.BoardMembers.AddAsync(new BoardMember
        {
            BoardId = boardId,
            UserId = userId1,
            Role = BoardMemberRole.Owner
        });

        // Seed columns
        var col1Id = Guid.NewGuid();
        var col2Id = Guid.NewGuid();
        var col3Id = Guid.NewGuid();

        var columns = new List<Column>
        {
            new() { Id = col1Id, BoardId = boardId, Name = "To Do",       Position = 1000 },
            new() { Id = col2Id, BoardId = boardId, Name = "In Progress",  Position = 2000 },
            new() { Id = col3Id, BoardId = boardId, Name = "Done",         Position = 3000 }
        };

        await context.Columns.AddRangeAsync(columns);

        // Seed tasks
        var tasks = new List<TaskItem>
        {
            new() { Id = Guid.NewGuid(), ColumnId = col1Id, Title = "Setup project",    Position = 1000, CreatedBy = userId1, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), ColumnId = col1Id, Title = "Design database",  Position = 2000, CreatedBy = userId1, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), ColumnId = col2Id, Title = "Build auth API",   Position = 1000, CreatedBy = userId1, AssigneeId = userId1, CreatedAt = DateTime.UtcNow },
        };

        await context.Tasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
    }
}
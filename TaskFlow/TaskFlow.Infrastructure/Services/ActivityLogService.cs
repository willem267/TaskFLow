using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence.Context;

namespace TaskFlow.Infrastructure.Services;

public class ActivityLogService : IActivityLogService
{
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUser _currentUser;

        public ActivityLogService(ApplicationDbContext context, ICurrentUser currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task LogAsync(Guid boardId, string action, string entityType, string? entityName = null)
        {
            var log = new ActivityLog
            {
                Id = Guid.NewGuid(),
                BoardId = boardId,
                UserId = _currentUser.Id,
                Action = action,
                EntityType = entityType,
                EntityName = entityName,
                CreatedAt = DateTime.UtcNow
            };

            await _context.ActivityLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
        
}
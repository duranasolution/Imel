using ImelAPI.Data;
using ImelAPI.Models;

namespace ImelAPI.Services
{
    public class AuditLogService
    {
        private readonly ApplicationDbContext _context;

        public AuditLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAction(string action, string userId, string performedBy, string tableName)
        {
            var auditLog = new AuditLog
            {
                Action = action,
                UserId = userId,
                PerformedBy = performedBy,
                TableName = tableName,
                TimeStamp = DateTime.UtcNow
            };
            _context.AuditLog.Add(auditLog);
            await _context.SaveChangesAsync();
        }
    }
}

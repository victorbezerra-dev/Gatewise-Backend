using GateWise.Core.Interfaces;
using GateWise.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class AccessLogRepository : IAccessLogRepository
{
    private readonly AppDbContext _context;

    public AccessLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AccessLog?> GetByCommandIdAsync(string commandId)
    {
        return await _context.AccessLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CommandId == commandId);
    }

    public async Task CreateAsync(AccessLog accessLog)
    {
        await _context.AccessLogs.AddAsync(accessLog);
    }

    public Task UpdateAsync(AccessLog accessLog)
    {
        _context.AccessLogs.Update(accessLog);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
namespace GateWise.Core.Interfaces;

public interface IAccessLogRepository
{
    Task<AccessLog?> GetByCommandIdAsync(string commandId);
    Task CreateAsync(AccessLog accessLog);
    Task UpdateAsync(AccessLog accessLog);
    Task SaveChangesAsync();
}
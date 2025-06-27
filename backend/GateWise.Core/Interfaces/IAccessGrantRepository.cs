using GateWise.Core.Entities;

namespace GateWise.Core.Interfaces;

public interface IAccessGrantRepository
{
    Task<IEnumerable<AccessGrant>> GetAllAsync(string? search = null);
    Task<AccessGrant?> GetByIdAsync(int id);
    Task<IEnumerable<AccessGrant>> GetByUserIdAsync(int userId);

    Task AddAsync(AccessGrant grant);
    Task UpdateAsync(AccessGrant grant);
    Task DeleteAsync(AccessGrant grant);
}

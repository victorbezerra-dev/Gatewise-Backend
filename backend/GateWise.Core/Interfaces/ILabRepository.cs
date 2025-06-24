using GateWise.Core.Entities;

namespace GateWise.Core.Interfaces;

public interface ILabRepository
{
    Task<IEnumerable<Lab>> GetAllAsync();
    Task<Lab?> GetByIdAsync(int id);
    Task AddAsync(Lab lab);
    Task UpdateAsync(Lab lab);
    Task DeleteAsync(Lab lab);
}

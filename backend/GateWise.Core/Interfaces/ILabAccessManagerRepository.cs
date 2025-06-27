using GateWise.Core.Entities;

namespace GateWise.Core.Interfaces;

public interface ILabAccessManagerRepository
{
    Task<IEnumerable<LabAccessManager>> GetAllAsync();
    Task<LabAccessManager?> GetByIdAsync(int id);
    Task AddAsync(LabAccessManager accessManager);
    Task DeleteAsync(LabAccessManager accessManager);
}

using GateWise.Core.Entities;
using GateWise.Core.Interfaces;
using GateWise.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GateWise.Infrastructure.Repositories;

public class LabAccessManagerRepository : ILabAccessManagerRepository
{
    private readonly AppDbContext _context;

    public LabAccessManagerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LabAccessManager>> GetAllAsync()
    {
        return await _context.LabAccessManagers
            .Include(l => l.Lab)
            .Include(u => u.User)
            .ToListAsync();
    }

    public async Task<LabAccessManager?> GetByIdAsync(int id)
    {
        return await _context.LabAccessManagers
            .Include(l => l.Lab)
            .Include(u => u.User)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(LabAccessManager accessManager)
    {
        _context.LabAccessManagers.Add(accessManager);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(LabAccessManager accessManager)
    {
        _context.LabAccessManagers.Remove(accessManager);
        await _context.SaveChangesAsync();
    }
}

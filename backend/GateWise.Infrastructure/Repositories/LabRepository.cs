using GateWise.Core.Entities;
using GateWise.Core.Interfaces;
using GateWise.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GateWise.Infrastructure.Repositories;

public class LabRepository : ILabRepository
{
    private readonly AppDbContext _context;

    public LabRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Lab>> GetAllAsync()
    {
        return await _context.Labs.ToListAsync();
    }

    public async Task<Lab?> GetByIdAsync(int id)
    {
        return await _context.Labs.FindAsync(id);
    }

    public async Task AddAsync(Lab lab)
    {
        lab.CreatedAt = DateTime.UtcNow;
        lab.UpdatedAt = DateTime.UtcNow;
        _context.Labs.Add(lab);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Lab lab)
    {
        lab.UpdatedAt = DateTime.UtcNow;
        _context.Labs.Update(lab);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Lab lab)
    {
        _context.Labs.Remove(lab);
        await _context.SaveChangesAsync();
    }
}

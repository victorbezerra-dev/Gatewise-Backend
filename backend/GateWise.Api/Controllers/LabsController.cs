using GateWise.Core.Entities;
using GateWise.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class LabsController : ControllerBase
{
    private readonly AppDbContext _context;

    public LabsController(AppDbContext context)
    {
        _context = context;
    }
    
    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _context.Labs.ToListAsync());

    [Authorize(Policy = "IsAdmin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var lab = await _context.Labs.FindAsync(id);
        return lab is null ? NotFound() : Ok(lab);
    }

    [Authorize(Policy = "IsAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Lab lab)
    {
        lab.CreatedAt = DateTime.UtcNow;
        lab.UpdatedAt = DateTime.UtcNow;
        _context.Labs.Add(lab);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = lab.Id }, lab);
    }

    [Authorize(Policy = "IsAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Lab lab)
    {
        var existing = await _context.Labs.FindAsync(id);
        if (existing is null) return NotFound();

        existing.Name = lab.Name;
        existing.Code = lab.Code;
        existing.ImagemUrl = lab.ImagemUrl;
        existing.Description = lab.Description;
        existing.Location = lab.Location;
        existing.Floor = lab.Floor;
        existing.Building = lab.Building;
        existing.Capacity = lab.Capacity;
        existing.IsActive = lab.IsActive;
        existing.OpenTime = lab.OpenTime;
        existing.CloseTime = lab.CloseTime;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    [Authorize(Policy = "IsAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var lab = await _context.Labs.FindAsync(id);
        if (lab is null) return NotFound();

        _context.Labs.Remove(lab);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

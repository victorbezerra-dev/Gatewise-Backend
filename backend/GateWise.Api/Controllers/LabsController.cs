using GateWise.Core.Dtos;
using GateWise.Core.Entities;
using GateWise.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/[controller]")]
public class LabsController : ControllerBase
{
    private readonly ILabRepository _labRepository;

    public LabsController(ILabRepository labRepository)
    {
        _labRepository = labRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var labs = await _labRepository.GetAllAsync();
        return Ok(labs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var lab = await _labRepository.GetByIdAsync(id);
        return lab is null ? NotFound() : Ok(lab);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LabUpsertDto dto)
    {
        var lab = new Lab
        {
            Name = dto.Name,
            Code = dto.Code,
            ImagemUrl = dto.ImagemUrl,
            Description = dto.Description,
            Location = dto.Location,
            Floor = dto.Floor,
            Building = dto.Building,
            Capacity = dto.Capacity,
            IsActive = dto.IsActive,
            OpenTime = dto.OpenTime,
            CloseTime = dto.CloseTime,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _labRepository.AddAsync(lab);

        return CreatedAtAction(nameof(Get), new { id = lab.Id }, lab);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] LabUpsertDto dto)
    {
        var existing = await _labRepository.GetByIdAsync(id);
        if (existing is null)
            return NotFound();

        existing.Name = dto.Name;
        existing.Code = dto.Code;
        existing.ImagemUrl = dto.ImagemUrl;
        existing.Description = dto.Description;
        existing.Location = dto.Location;
        existing.Floor = dto.Floor;
        existing.Building = dto.Building;
        existing.Capacity = dto.Capacity;
        existing.IsActive = dto.IsActive;
        existing.OpenTime = dto.OpenTime;
        existing.CloseTime = dto.CloseTime;
        existing.UpdatedAt = DateTime.UtcNow;

        await _labRepository.UpdateAsync(existing);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var lab = await _labRepository.GetByIdAsync(id);
        if (lab is null) return NotFound();

        await _labRepository.DeleteAsync(lab);
        return NoContent();
    }
}

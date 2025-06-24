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
    public async Task<IActionResult> Create([FromBody] Lab lab)
    {
        await _labRepository.AddAsync(lab);
        return CreatedAtAction(nameof(Get), new { id = lab.Id }, lab);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Lab lab)
    {
        var existing = await _labRepository.GetByIdAsync(id);
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

using System.Security.Claims;
using GateWise.Core.Dtos;
using GateWise.Core.DTOs;
using GateWise.Core.Entities;
using GateWise.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("api/[controller]")]
public class LabsController : ControllerBase
{
    private readonly ILabRepository _labRepository;
    private readonly ILabAccessService _labAccessService;



    public LabsController(ILabRepository labRepository, ILabAccessService labAccessService, IAccessLogRepository accessLogRepository)
    {
        _labRepository = labRepository;
        _labAccessService = labAccessService;
    }

    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var labs = await _labRepository.GetAllAsync();
        return Ok(labs);
    }

    [Authorize(Roles = "admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var lab = await _labRepository.GetByIdAsync(id);
        return lab is null ? NotFound() : Ok(lab);
    }
    
    [Authorize(Roles = "admin")]
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

    [HttpPost("{id}/open")]
    public async Task<IActionResult> OpenLab(int id, [FromBody] AccessLogCreateDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token.");

        var commandId = await _labAccessService.RequestLabAccessAsync(userId, id, dto);
        return Ok(new { commandId });
    }

    [HttpPost("access-confirmation")]
    public async Task<IActionResult> AccessConfirmation([FromBody] AccessLogConfirmDto dto)
    {
        var confirmed = await _labAccessService.ConfirmAccessAsync(dto);

        if (confirmed)
            return Ok();
        else
            return StatusCode(403, "Access not confirmed by app");
    }

    [Authorize(Roles = "admin")]
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

    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var lab = await _labRepository.GetByIdAsync(id);
        if (lab is null) return NotFound();

        await _labRepository.DeleteAsync(lab);
        return NoContent();
    }

}

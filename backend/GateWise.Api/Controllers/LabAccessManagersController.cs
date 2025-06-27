using GateWise.Application.DTOs;
using GateWise.Core.Entities;
using GateWise.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GateWise.Api.Controllers;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/[controller]")]
public class LabAccessManagersController : ControllerBase
{
    private readonly ILabAccessManagerRepository _accessManagerRepo;
    private readonly ILabRepository _labRepo;
    private readonly IUserRepository _userRepo;

    public LabAccessManagersController(
        ILabAccessManagerRepository accessManagerRepo,
        ILabRepository labRepo,
        IUserRepository userRepo)
    {
        _accessManagerRepo = accessManagerRepo;
        _labRepo = labRepo;
        _userRepo = userRepo;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LabAccessManager>>> GetAll()
    {
        var result = await _accessManagerRepo.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LabAccessManager>> GetById(int id)
    {
        var access = await _accessManagerRepo.GetByIdAsync(id);
        if (access is null)
            return NotFound();

        return Ok(access);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateLabAccessManagerDTO input)
    {
        var lab = await _labRepo.GetByIdAsync(input.LabId);
        var user = await _userRepo.GetByIdAsync(input.UserId);

        if (lab is null || user is null)
            return BadRequest("Invalid LabId or UserId.");

        var newAccess = new LabAccessManager
        {
            LabId = input.LabId,
            UserId = input.UserId
        };

        await _accessManagerRepo.AddAsync(newAccess);

        return CreatedAtAction(nameof(GetById), new { id = newAccess.Id }, newAccess);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var access = await _accessManagerRepo.GetByIdAsync(id);
        if (access is null)
            return NotFound();

        await _accessManagerRepo.DeleteAsync(access);
        return NoContent();
    }
}

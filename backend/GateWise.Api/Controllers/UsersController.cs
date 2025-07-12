using GateWise.Core.Entities;
using GateWise.Core.DTOs;
using GateWise.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GateWise.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repository;

    public UsersController(IUserRepository repository)
    {
        _repository = repository;
    }

    [Authorize()]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAll() =>
        Ok(await _repository.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetById(string id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [Authorize()]
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UserUpsertDto user)
    {

        await _repository.UpdateAsync(id, user);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}

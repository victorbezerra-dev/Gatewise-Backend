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

    [Authorize(Policy = "RequireClientIdGatewiseSync")]
    [HttpPost("sync")]
    public async Task<ActionResult<User>> Sync([FromBody] CreateUserDto userDto)
    {
        var existingUser = await _repository.GetByEmailOrRegistrationAsync(userDto.Email, userDto.RegistrationNumber);

        if (existingUser != null)
        {
            existingUser.Name = userDto.Name;
            existingUser.UserAvatarUrl = userDto.UserAvatarUrl;
            existingUser.UserType = userDto.UserType;
            existingUser.OperationalSystem = userDto.OperationalSystem;
            existingUser.OperationalSystemVersion = userDto.OperationalSystemVersion;
            existingUser.DeviceModel = userDto.DeviceModel;
            existingUser.DeviceManufactureName = userDto.DeviceManufactureName;

            await _repository.UpdateAsync(existingUser);
            return Ok(existingUser);
        }
        else
        {
            var newUser = new User(
                userDto.Name,
                userDto.Email,
                userDto.RegistrationNumber,
                userDto.UserAvatarUrl,
                userDto.UserType,
                userDto.OperationalSystem,
                userDto.OperationalSystemVersion,
                userDto.DeviceModel,
                userDto.DeviceManufactureName
            );

            await _repository.CreateAsync(newUser);
            return Ok(newUser);
        }
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, User user)
    {
        if (id != user.Id)
            return BadRequest();

        await _repository.UpdateAsync(user);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}

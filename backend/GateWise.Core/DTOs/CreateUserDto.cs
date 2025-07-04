namespace GateWise.Core.DTOs;

using GateWise.Core.Enums;

public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? RegistrationNumber { get; set; }
    public string UserAvatarUrl { get; set; } = string.Empty;
    public GateWise.Core.Enums.UserType UserType { get; set; }
    public string OperationalSystem { get; set; } = string.Empty;
    public string OperationalSystemVersion { get; set; } = string.Empty;
    public string DeviceModel { get; set; } = string.Empty;
    public string DeviceManufactureName { get; set; } = string.Empty;
}
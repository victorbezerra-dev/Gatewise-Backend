using GateWise.Core.Enums;

namespace GateWise.Core.Entities;

public class User
{
    public string Id { get; set; }

    public string Name { get; set; }
    public string Email { get; set; }
    public string RegistrationNumber { get; set; }

    public string UserAvatarUrl { get; set; }
    public UserType UserType { get; set; }

    public string OperationalSystem { get; set; }
    public string OperationalSystemVersion { get; set; }
    public string DeviceModel { get; set; }
    public string DeviceManufactureName { get; set; }

    public string? DevicePublicKeyPem { get; set; } 

    private User() { }

    public User(
        string name,
        string email,
        string registration,
        string userAvatarUrl,
        UserType userType,
        string operationalSystem,
        string operationalSystemVersion,
        string deviceModel,
        string deviceManufactureName,
        string? devicePublicKeyPem = null)
    {
        Name = name;
        Email = email;
        RegistrationNumber = registration;
        UserAvatarUrl = userAvatarUrl;
        UserType = userType;
        OperationalSystem = operationalSystem;
        OperationalSystemVersion = operationalSystemVersion;
        DeviceModel = deviceModel;
        DeviceManufactureName = deviceManufactureName;
        DevicePublicKeyPem = devicePublicKeyPem;
    }
}

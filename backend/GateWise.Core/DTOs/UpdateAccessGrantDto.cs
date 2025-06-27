using GateWise.Core.Enums;

namespace GateWise.Application.DTOs;

public class UpdateAccessGrantDto
{
    public AccessGrantStatus Status { get; set; }
    public string? Reason { get; set; }
    public DateTime? RevokedAt { get; set; }
}

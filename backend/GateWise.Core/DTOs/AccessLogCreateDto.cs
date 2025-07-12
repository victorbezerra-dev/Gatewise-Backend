namespace GateWise.Core.DTOs;

public class AccessLogCreateDto
{
    public long Timestamp { get; set; }
    public string Signature { get; set; } = string.Empty;
}

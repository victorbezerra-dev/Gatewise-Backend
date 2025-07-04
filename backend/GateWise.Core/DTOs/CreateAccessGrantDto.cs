namespace GateWise.Application.DTOs;

public class CreateAccessGrantDto
{
    public required string AuthorizedUserId { get; set; }
    public required string GrantedByUserId { get; set; }
    public int LabId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

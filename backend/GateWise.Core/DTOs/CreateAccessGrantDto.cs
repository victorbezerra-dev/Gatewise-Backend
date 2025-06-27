namespace GateWise.Application.DTOs;

public class CreateAccessGrantDto
{
    public int AuthorizedUserId { get; set; }
    public int GrantedByUserId { get; set; }
    public int LabId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

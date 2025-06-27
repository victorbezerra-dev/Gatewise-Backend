namespace GateWise.Core.Entities;

public class LabAccessManager
{
    public int Id { get; set; }

    public int LabId { get; set; }
    public Lab Lab { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}

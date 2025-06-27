using GateWise.Core.Enums;

namespace GateWise.Core.Entities;

public class AccessGrant
{
    public int Id { get; set; }

    public int AuthorizedUserId { get; set; }
    public User AuthorizedUser { get; set; } = null!;

    public int? GrantedByUserId { get; set; }
    public User? GrantedByUser { get; set; }

    public int LabId { get; set; }
    public Lab Lab { get; set; } = null!;

    public DateTime? GrantedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public string Reason { get; set; } = string.Empty;

    public AccessGrantStatus Status { get; set; } = AccessGrantStatus.Pending;
}

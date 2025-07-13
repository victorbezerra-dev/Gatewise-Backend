public class AccessLogConfirmDto
{
    public string CommandId { get; set; } = default!;
    public long Timestamp { get; set; }
    public string Signature { get; set; } = default!;
}

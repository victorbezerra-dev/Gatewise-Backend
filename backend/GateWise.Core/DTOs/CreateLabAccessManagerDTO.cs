namespace GateWise.Application.DTOs
{
    public class CreateLabAccessManagerDTO
    {
        public int LabId { get; set; }
        public required string UserId { get; set; }
    }
}

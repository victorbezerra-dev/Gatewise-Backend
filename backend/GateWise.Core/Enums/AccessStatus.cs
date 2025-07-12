namespace GateWise.Core.Enums;

public enum AccessStatus
{
    PENDING_CONFIRMATION = 0,
    GRANTED = 1,
    NO_CONFIRMATION = 2,
    SIGNATURE_REJECTED = 3,
    DENIED_BY_POLICY = 4,
    EXECUTION_FAILED = 5
}
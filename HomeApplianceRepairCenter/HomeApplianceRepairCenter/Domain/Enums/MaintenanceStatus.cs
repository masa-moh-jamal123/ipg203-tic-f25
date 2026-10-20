namespace HomeApplianceRepairCenter.Domain.Enums
{
    /// <summary>Represents the lifecycle of an appliance in the repair center.</summary>
    public enum MaintenanceStatus
    {
        Received = 1,
        Diagnosing = 2,
        WaitingForParts = 3,
        InRepair = 4,
        Repaired = 5,
        Delivered = 6,
        Cancelled = 7
    }
}

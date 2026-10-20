namespace HomeApplianceRepairCenter.Domain.Events
{
    /// <summary>Custom delegate used by status-related events.</summary>
    public delegate void DeviceStatusChangedHandler(object sender, DeviceStatusChangedEventArgs e);
}

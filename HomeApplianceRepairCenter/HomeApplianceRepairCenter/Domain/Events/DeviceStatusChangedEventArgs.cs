using System;
using HomeApplianceRepairCenter.Domain.Enums;

namespace HomeApplianceRepairCenter.Domain.Events
{
    /// <summary>Data passed with appliance status-change events.</summary>
    public sealed class DeviceStatusChangedEventArgs : EventArgs
    {
        public DeviceStatusChangedEventArgs(string serialNumber, MaintenanceStatus oldStatus,
            MaintenanceStatus newStatus, DateTime changedAt, string note)
        {
            SerialNumber = serialNumber;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            ChangedAt = changedAt;
            Note = note;
        }

        public string SerialNumber { get; private set; }
        public MaintenanceStatus OldStatus { get; private set; }
        public MaintenanceStatus NewStatus { get; private set; }
        public DateTime ChangedAt { get; private set; }
        public string Note { get; private set; }
    }
}

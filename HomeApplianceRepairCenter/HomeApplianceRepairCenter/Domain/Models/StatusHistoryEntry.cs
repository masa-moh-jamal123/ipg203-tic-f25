using System;
using HomeApplianceRepairCenter.Domain.Enums;

namespace HomeApplianceRepairCenter.Domain.Models
{
    /// <summary>Immutable record of one maintenance-status transition.</summary>
    public sealed class StatusHistoryEntry
    {
        public StatusHistoryEntry(MaintenanceStatus oldStatus, MaintenanceStatus newStatus,
            DateTime changedAt, string note)
        {
            OldStatus = oldStatus;
            NewStatus = newStatus;
            ChangedAt = changedAt;
            Note = note;
        }

        public MaintenanceStatus OldStatus { get; private set; }
        public MaintenanceStatus NewStatus { get; private set; }
        public DateTime ChangedAt { get; private set; }
        public string Note { get; private set; }

        public override string ToString()
        {
            return string.Format("{0:yyyy-MM-dd HH:mm} | {1} -> {2} | {3}", ChangedAt, OldStatus, NewStatus, Note);
        }
    }
}

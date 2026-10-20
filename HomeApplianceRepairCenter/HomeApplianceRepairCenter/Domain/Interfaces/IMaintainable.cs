using HomeApplianceRepairCenter.Domain.Enums;

namespace HomeApplianceRepairCenter.Domain.Interfaces
{
    /// <summary>Common maintenance operations implemented by every appliance.</summary>
    public interface IMaintainable
    {
        decimal CalculateRepairCost();
        void ChangeStatus(MaintenanceStatus newStatus, string note);
        string GetSummary();
    }
}

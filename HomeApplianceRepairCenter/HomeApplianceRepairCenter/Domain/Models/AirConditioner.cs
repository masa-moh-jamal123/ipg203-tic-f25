using System;
using HomeApplianceRepairCenter.Utilities;

namespace HomeApplianceRepairCenter.Domain.Models
{
    public sealed class AirConditioner : Appliance
    {
        private int _coolingCapacityBtu;
        private bool _requiresDeepCleaning;

        public AirConditioner(string serialNumber, string brand, string model, string customerName,
            string faultDescription, DateTime receivedDate, DateTime expectedDeliveryDate,
            decimal diagnosticFee, decimal partsCost, double laborHours,
            int coolingCapacityBtu, bool requiresDeepCleaning)
            : base(serialNumber, brand, model, customerName, faultDescription, receivedDate,
                  expectedDeliveryDate, diagnosticFee, partsCost, laborHours)
        {
            CoolingCapacityBtu = coolingCapacityBtu;
            RequiresDeepCleaning = requiresDeepCleaning;
        }

        public int CoolingCapacityBtu
        {
            get { return _coolingCapacityBtu; }
            set
            {
                if (!InputValidator.IsInRange(value, 5000, 60000)) throw new ArgumentOutOfRangeException("value", "Cooling capacity must be between 5,000 and 60,000 BTU.");
                _coolingCapacityBtu = value;
            }
        }

        public bool RequiresDeepCleaning
        {
            get { return _requiresDeepCleaning; }
            set { _requiresDeepCleaning = value; }
        }

        public override decimal CalculateRepairCost()
        {
            decimal laborCost = (decimal)LaborHours * 24m;
            decimal capacityFactor = (CoolingCapacityBtu / 1000m) * 1.5m;
            decimal deepCleaningCost = RequiresDeepCleaning ? 30m : 0m;
            return Math.Round(DiagnosticFee + PartsCost + laborCost + capacityFactor + deepCleaningCost, 2);
        }

        protected override string GetDeviceSpecificDetails()
        {
            return string.Format("Cooling Capacity: {0:N0} BTU | Deep Cleaning: {1}",
                CoolingCapacityBtu, RequiresDeepCleaning ? "Required" : "Not required");
        }
    }
}

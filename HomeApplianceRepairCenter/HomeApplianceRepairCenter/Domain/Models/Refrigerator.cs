using System;
using HomeApplianceRepairCenter.Utilities;

namespace HomeApplianceRepairCenter.Domain.Models
{
    public sealed class Refrigerator : Appliance
    {
        private int _coolingSystemComplexity;
        private bool _requiresRefrigerantRefill;

        public Refrigerator(string serialNumber, string brand, string model, string customerName,
            string faultDescription, DateTime receivedDate, DateTime expectedDeliveryDate,
            decimal diagnosticFee, decimal partsCost, double laborHours,
            int coolingSystemComplexity, bool requiresRefrigerantRefill)
            : base(serialNumber, brand, model, customerName, faultDescription, receivedDate,
                  expectedDeliveryDate, diagnosticFee, partsCost, laborHours)
        {
            CoolingSystemComplexity = coolingSystemComplexity;
            RequiresRefrigerantRefill = requiresRefrigerantRefill;
        }

        public int CoolingSystemComplexity
        {
            get { return _coolingSystemComplexity; }
            set
            {
                if (!InputValidator.IsInRange(value, 1, 3)) throw new ArgumentOutOfRangeException("value", "Cooling system complexity must be between 1 and 3.");
                _coolingSystemComplexity = value;
            }
        }

        public bool RequiresRefrigerantRefill
        {
            get { return _requiresRefrigerantRefill; }
            set { _requiresRefrigerantRefill = value; }
        }

        public override decimal CalculateRepairCost()
        {
            decimal laborCost = (decimal)LaborHours * 22m;
            decimal complexityCost = CoolingSystemComplexity * 20m;
            decimal refrigerantCost = RequiresRefrigerantRefill ? 35m : 0m;
            return Math.Round(DiagnosticFee + PartsCost + laborCost + complexityCost + refrigerantCost, 2);
        }

        protected override string GetDeviceSpecificDetails()
        {
            return string.Format("Cooling Complexity: {0}/3 | Refrigerant Refill: {1}",
                CoolingSystemComplexity, RequiresRefrigerantRefill ? "Required" : "Not required");
        }
    }
}

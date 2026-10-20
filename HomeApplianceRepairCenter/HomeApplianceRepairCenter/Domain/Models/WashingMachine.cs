using System;
using HomeApplianceRepairCenter.Utilities;

namespace HomeApplianceRepairCenter.Domain.Models
{
    public sealed class WashingMachine : Appliance
    {
        private double _loadCapacityKg;
        private bool _requiresDrumBearingService;

        public WashingMachine(string serialNumber, string brand, string model, string customerName,
            string faultDescription, DateTime receivedDate, DateTime expectedDeliveryDate,
            decimal diagnosticFee, decimal partsCost, double laborHours,
            double loadCapacityKg, bool requiresDrumBearingService)
            : base(serialNumber, brand, model, customerName, faultDescription, receivedDate,
                  expectedDeliveryDate, diagnosticFee, partsCost, laborHours)
        {
            LoadCapacityKg = loadCapacityKg;
            RequiresDrumBearingService = requiresDrumBearingService;
        }

        public double LoadCapacityKg
        {
            get { return _loadCapacityKg; }
            set
            {
                if (!InputValidator.IsPositive(value) || value > 30d) throw new ArgumentOutOfRangeException("value", "Load capacity must be greater than 0 and at most 30 kg.");
                _loadCapacityKg = value;
            }
        }

        public bool RequiresDrumBearingService
        {
            get { return _requiresDrumBearingService; }
            set { _requiresDrumBearingService = value; }
        }

        public override decimal CalculateRepairCost()
        {
            decimal laborCost = (decimal)LaborHours * 20m;
            decimal capacityCost = (decimal)LoadCapacityKg * 2m;
            decimal bearingServiceCost = RequiresDrumBearingService ? 45m : 0m;
            return Math.Round(DiagnosticFee + PartsCost + laborCost + capacityCost + bearingServiceCost, 2);
        }

        protected override string GetDeviceSpecificDetails()
        {
            return string.Format("Load Capacity: {0:N1} kg | Drum Bearing Service: {1}",
                LoadCapacityKg, RequiresDrumBearingService ? "Required" : "Not required");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using HomeApplianceRepairCenter.Domain.Enums;
using HomeApplianceRepairCenter.Domain.Models;

namespace HomeApplianceRepairCenter.Services
{
    /// <summary>Coordinates appliance registration, search, filtering and polymorphic calculations.</summary>
    public sealed class RepairCenter
    {
        private readonly List<Appliance> _devices = new List<Appliance>();

        public IReadOnlyList<Appliance> Devices { get { return _devices.AsReadOnly(); } }
        public int Count { get { return _devices.Count; } }

        public void AddDevice(Appliance device)
        {
            if (device == null) throw new ArgumentNullException("device");
            if (ContainsSerial(device.SerialNumber))
                throw new InvalidOperationException("A device with serial number '" + device.SerialNumber + "' already exists.");
            _devices.Add(device);
        }

        public bool ContainsSerial(string serialNumber)
        {
            string normalized = (serialNumber ?? string.Empty).Trim();
            return _devices.Any(device => string.Equals(device.SerialNumber, normalized, StringComparison.OrdinalIgnoreCase));
        }

        public Appliance FindBySerial(string serialNumber)
        {
            string normalized = (serialNumber ?? string.Empty).Trim();
            return _devices.FirstOrDefault(device => string.Equals(device.SerialNumber, normalized, StringComparison.OrdinalIgnoreCase));
        }

        public IReadOnlyList<Appliance> GetDevicesByStatus(MaintenanceStatus status)
        {
            return _devices.Where(device => device.Status == status).ToList().AsReadOnly();
        }

        public IReadOnlyList<Appliance> GetDevicesUnderMaintenance()
        {
            return _devices.Where(device => device.Status == MaintenanceStatus.Received
                || device.Status == MaintenanceStatus.Diagnosing
                || device.Status == MaintenanceStatus.WaitingForParts
                || device.Status == MaintenanceStatus.InRepair).ToList().AsReadOnly();
        }

        public IReadOnlyList<Appliance> GetRepairedDevices()
        {
            return _devices.Where(device => device.Status == MaintenanceStatus.Repaired
                || device.Status == MaintenanceStatus.Delivered).ToList().AsReadOnly();
        }

        public decimal CalculateTotalEstimatedRepairCost()
        {
            return _devices.Sum(device => device.CalculateRepairCost());
        }

        public IReadOnlyDictionary<MaintenanceStatus, int> GetStatusCounts()
        {
            Dictionary<MaintenanceStatus, int> result = new Dictionary<MaintenanceStatus, int>();
            Array values = Enum.GetValues(typeof(MaintenanceStatus));
            foreach (MaintenanceStatus status in values)
                result[status] = _devices.Count(device => device.Status == status);
            return result;
        }
    }
}

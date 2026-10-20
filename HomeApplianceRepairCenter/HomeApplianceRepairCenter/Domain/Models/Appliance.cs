using System;
using System.Collections.Generic;
using System.Linq;
using HomeApplianceRepairCenter.Domain.Enums;
using HomeApplianceRepairCenter.Domain.Events;
using HomeApplianceRepairCenter.Domain.Interfaces;
using HomeApplianceRepairCenter.Utilities;

namespace HomeApplianceRepairCenter.Domain.Models
{
    /// <summary>
    /// Abstract base class for all appliances. Implements the common interface and contains
    /// shared encapsulated state while derived classes override repair-cost calculation.
    /// </summary>
    public abstract class Appliance : IMaintainable
    {
        private static int _devicesUnderMaintenanceCount;
        private readonly string _serialNumber;
        private readonly DateTime _receivedDate;
        private readonly List<StatusHistoryEntry> _statusHistory;
        private string _brand = string.Empty;
        private string _model = string.Empty;
        private string _customerName = string.Empty;
        private string _faultDescription = string.Empty;
        private DateTime _expectedDeliveryDate;
        private decimal _diagnosticFee;
        private decimal _partsCost;
        private double _laborHours;
        private MaintenanceStatus _status;

        protected Appliance(string serialNumber, string brand, string model, string customerName,
            string faultDescription, DateTime receivedDate, DateTime expectedDeliveryDate,
            decimal diagnosticFee, decimal partsCost, double laborHours)
        {
            string normalizedSerial = (serialNumber ?? string.Empty).Trim().ToUpperInvariant();
            if (!InputValidator.IsValidSerialNumber(normalizedSerial))
                throw new ArgumentException("Serial number must contain 5-20 characters using only A-Z, 0-9, or hyphen.", "serialNumber");
            if (!InputValidator.IsValidReceivedDate(receivedDate))
                throw new ArgumentException("Received date cannot be in the future and must be year 2000 or later.", "receivedDate");
            if (!InputValidator.IsValidExpectedDeliveryDate(receivedDate, expectedDeliveryDate))
                throw new ArgumentException("Expected delivery date cannot be earlier than the received date.", "expectedDeliveryDate");

            _serialNumber = normalizedSerial;
            _receivedDate = receivedDate.Date;
            _statusHistory = new List<StatusHistoryEntry>();
            _status = MaintenanceStatus.Received;
            Brand = brand;
            Model = model;
            CustomerName = customerName;
            FaultDescription = faultDescription;
            ExpectedDeliveryDate = expectedDeliveryDate;
            DiagnosticFee = diagnosticFee;
            PartsCost = partsCost;
            LaborHours = laborHours;
            _devicesUnderMaintenanceCount++;
        }

        public event DeviceStatusChangedHandler StatusChanged;
        public event DeviceStatusChangedHandler RepairCompleted;

        public static int DevicesUnderMaintenanceCount { get { return _devicesUnderMaintenanceCount; } }
        public string SerialNumber { get { return _serialNumber; } }
        public DateTime ReceivedDate { get { return _receivedDate; } }

        public string Brand
        {
            get { return _brand; }
            set
            {
                if (!InputValidator.IsValidText(value, 2, 50)) throw new ArgumentException("Brand must be between 2 and 50 characters.", "value");
                _brand = value.Trim();
            }
        }

        public string Model
        {
            get { return _model; }
            set
            {
                if (!InputValidator.IsValidText(value, 1, 50)) throw new ArgumentException("Model must be between 1 and 50 characters.", "value");
                _model = value.Trim();
            }
        }

        public string CustomerName
        {
            get { return _customerName; }
            set
            {
                if (!InputValidator.IsValidText(value, 2, 80)) throw new ArgumentException("Customer name must be between 2 and 80 characters.", "value");
                _customerName = value.Trim();
            }
        }

        public string FaultDescription
        {
            get { return _faultDescription; }
            set
            {
                if (!InputValidator.IsValidText(value, 3, 250)) throw new ArgumentException("Fault description must be between 3 and 250 characters.", "value");
                _faultDescription = value.Trim();
            }
        }

        public DateTime ExpectedDeliveryDate
        {
            get { return _expectedDeliveryDate; }
            set
            {
                if (!InputValidator.IsValidExpectedDeliveryDate(ReceivedDate, value)) throw new ArgumentException("Expected delivery date cannot be earlier than the received date.", "value");
                _expectedDeliveryDate = value.Date;
            }
        }

        public decimal DiagnosticFee
        {
            get { return _diagnosticFee; }
            set
            {
                if (!InputValidator.IsNonNegative(value)) throw new ArgumentOutOfRangeException("value", "Diagnostic fee cannot be negative.");
                _diagnosticFee = value;
            }
        }

        public decimal PartsCost
        {
            get { return _partsCost; }
            set
            {
                if (!InputValidator.IsNonNegative(value)) throw new ArgumentOutOfRangeException("value", "Parts cost cannot be negative.");
                _partsCost = value;
            }
        }

        public double LaborHours
        {
            get { return _laborHours; }
            set
            {
                if (!InputValidator.IsNonNegative(value)) throw new ArgumentOutOfRangeException("value", "Labor hours cannot be negative.");
                _laborHours = value;
            }
        }

        public MaintenanceStatus Status { get { return _status; } }
        public bool IsOverdue { get { return IsUnderMaintenance(Status) && DateTime.Today > ExpectedDeliveryDate.Date; } }
        public IReadOnlyList<StatusHistoryEntry> StatusHistory { get { return _statusHistory.AsReadOnly(); } }

        public abstract decimal CalculateRepairCost();

        public virtual void ChangeStatus(MaintenanceStatus newStatus, string note)
        {
            string safeNote = string.IsNullOrWhiteSpace(note) ? "No note provided." : note.Trim();
            if (newStatus == Status) throw new InvalidOperationException("The device is already in the selected status.");
            IReadOnlyList<MaintenanceStatus> allowedStatuses = GetAllowedNextStatuses();
            if (!allowedStatuses.Contains(newStatus))
                throw new InvalidOperationException(string.Format("Transition from {0} to {1} is not allowed.", Status, newStatus));

            MaintenanceStatus oldStatus = _status;
            bool wasUnderMaintenance = IsUnderMaintenance(oldStatus);
            bool willBeUnderMaintenance = IsUnderMaintenance(newStatus);
            _status = newStatus;

            if (wasUnderMaintenance && !willBeUnderMaintenance) _devicesUnderMaintenanceCount--;
            else if (!wasUnderMaintenance && willBeUnderMaintenance) _devicesUnderMaintenanceCount++;

            DateTime changedAt = DateTime.Now;
            _statusHistory.Add(new StatusHistoryEntry(oldStatus, newStatus, changedAt, safeNote));
            DeviceStatusChangedEventArgs eventArgs = new DeviceStatusChangedEventArgs(SerialNumber, oldStatus, newStatus, changedAt, safeNote);
            OnStatusChanged(eventArgs);
            if (newStatus == MaintenanceStatus.Repaired) OnRepairCompleted(eventArgs);
        }

        public IReadOnlyList<MaintenanceStatus> GetAllowedNextStatuses()
        {
            switch (Status)
            {
                case MaintenanceStatus.Received:
                    return new MaintenanceStatus[] { MaintenanceStatus.Diagnosing, MaintenanceStatus.Cancelled };
                case MaintenanceStatus.Diagnosing:
                    return new MaintenanceStatus[] { MaintenanceStatus.WaitingForParts, MaintenanceStatus.InRepair, MaintenanceStatus.Cancelled };
                case MaintenanceStatus.WaitingForParts:
                    return new MaintenanceStatus[] { MaintenanceStatus.InRepair, MaintenanceStatus.Cancelled };
                case MaintenanceStatus.InRepair:
                    return new MaintenanceStatus[] { MaintenanceStatus.WaitingForParts, MaintenanceStatus.Repaired, MaintenanceStatus.Cancelled };
                case MaintenanceStatus.Repaired:
                    return new MaintenanceStatus[] { MaintenanceStatus.Delivered };
                default:
                    return new MaintenanceStatus[0];
            }
        }

        public virtual string GetSummary()
        {
            return string.Format("[{0}] {1} | {2} {3} | Status: {4} | Estimated cost: {5:N2}",
                GetType().Name, SerialNumber, Brand, Model, Status, CalculateRepairCost());
        }

        public string GetDetailedDescription()
        {
            return string.Join(Environment.NewLine, new string[]
            {
                "Type: " + GetType().Name,
                "Serial Number: " + SerialNumber,
                "Brand / Model: " + Brand + " / " + Model,
                "Customer: " + CustomerName,
                "Fault: " + FaultDescription,
                "Received: " + ReceivedDate.ToString("yyyy-MM-dd"),
                "Expected Delivery: " + ExpectedDeliveryDate.ToString("yyyy-MM-dd"),
                "Status: " + Status,
                "Overdue: " + (IsOverdue ? "Yes" : "No"),
                "Diagnostic Fee: " + DiagnosticFee.ToString("N2"),
                "Parts Cost: " + PartsCost.ToString("N2"),
                "Labor Hours: " + LaborHours.ToString("N2"),
                GetDeviceSpecificDetails(),
                "Estimated Repair Cost: " + CalculateRepairCost().ToString("N2")
            });
        }

        protected abstract string GetDeviceSpecificDetails();

        protected virtual void OnStatusChanged(DeviceStatusChangedEventArgs e)
        {
            DeviceStatusChangedHandler handler = StatusChanged;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnRepairCompleted(DeviceStatusChangedEventArgs e)
        {
            DeviceStatusChangedHandler handler = RepairCompleted;
            if (handler != null) handler(this, e);
        }

        private static bool IsUnderMaintenance(MaintenanceStatus status)
        {
            return status == MaintenanceStatus.Received || status == MaintenanceStatus.Diagnosing
                || status == MaintenanceStatus.WaitingForParts || status == MaintenanceStatus.InRepair;
        }
    }
}

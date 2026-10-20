using System;
using System.Collections.Generic;
using System.Text;
using HomeApplianceRepairCenter.Domain.Enums;
using HomeApplianceRepairCenter.Domain.Events;
using HomeApplianceRepairCenter.Domain.Models;
using HomeApplianceRepairCenter.Services;
using HomeApplianceRepairCenter.Utilities;

namespace HomeApplianceRepairCenter
{
    internal static class Program
    {
        private static readonly RepairCenter Center = new RepairCenter();

        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Home Appliance Repair Center";
            bool running = true;

            while (running)
            {
                try
                {
                    PrintHeader();
                    PrintMenu();
                    int choice = ConsoleInput.ReadInt("Select an option: ", 0, 9);
                    Console.WriteLine();

                    switch (choice)
                    {
                        case 1: RegisterDevice(); break;
                        case 2: DisplayAllDevices(); break;
                        case 3: FindAndDisplayDevice(); break;
                        case 4: UpdateDeviceStatus(); break;
                        case 5: UpdateRepairData(); break;
                        case 6: DisplayRepairedDevices(); break;
                        case 7: DisplayDevicesUnderMaintenance(); break;
                        case 8: DisplayStatistics(); break;
                        case 9: DisplayStatusHistory(); break;
                        case 0: running = false; break;
                    }
                }
                catch (ArgumentException ex) { PrintError(ex.Message); }
                catch (InvalidOperationException ex) { PrintError(ex.Message); }

                if (running) Pause();
            }

            Console.WriteLine("Thank you for using the Home Appliance Repair Center system.");
        }

        private static void PrintHeader()
        {
            Console.Clear();
            Console.WriteLine(new string('=', 72));
            Console.WriteLine("              HOME APPLIANCE REPAIR CENTER MANAGEMENT");
            Console.WriteLine(new string('=', 72));
            Console.WriteLine("Registered devices: {0} | Under maintenance (static): {1}", Center.Count, Appliance.DevicesUnderMaintenanceCount);
            Console.WriteLine(new string('-', 72));
        }

        private static void PrintMenu()
        {
            Console.WriteLine("1. Register a new appliance");
            Console.WriteLine("2. List all appliances");
            Console.WriteLine("3. Find appliance by serial number");
            Console.WriteLine("4. Update maintenance status");
            Console.WriteLine("5. Update repair data (fault / costs / labor)");
            Console.WriteLine("6. Show repaired or delivered appliances");
            Console.WriteLine("7. Show appliances still under maintenance");
            Console.WriteLine("8. Show center statistics and total estimated cost");
            Console.WriteLine("9. Show status history for an appliance");
            Console.WriteLine("0. Exit");
            Console.WriteLine(new string('-', 72));
        }

        private static void RegisterDevice()
        {
            Console.WriteLine("REGISTER NEW APPLIANCE");
            Console.WriteLine("1. Refrigerator");
            Console.WriteLine("2. Washing Machine");
            Console.WriteLine("3. Air Conditioner");

            int applianceType = ConsoleInput.ReadInt("Appliance type: ", 1, 3);
            string serialNumber = ReadUniqueSerialNumber();
            string brand = ConsoleInput.ReadRequiredText("Brand: ", 2, 50);
            string model = ConsoleInput.ReadRequiredText("Model: ", 1, 50);
            string customerName = ConsoleInput.ReadRequiredText("Customer name: ", 2, 80);
            string faultDescription = ConsoleInput.ReadRequiredText("Fault description: ", 3, 250);
            DateTime receivedDate = ConsoleInput.ReadDate("Received date (yyyy-MM-dd): ", InputValidator.IsValidReceivedDate,
                "Invalid date. Use yyyy-MM-dd; the date cannot be in the future and must be year 2000 or later.");
            DateTime expectedDeliveryDate = ConsoleInput.ReadDate("Expected delivery date (yyyy-MM-dd): ",
                delegate(DateTime date) { return InputValidator.IsValidExpectedDeliveryDate(receivedDate, date); },
                "Invalid date. Expected delivery cannot be earlier than the received date.");
            decimal diagnosticFee = ConsoleInput.ReadDecimal("Diagnostic fee: ", 0m, 1000000m);
            decimal partsCost = ConsoleInput.ReadDecimal("Initial parts cost: ", 0m, 10000000m);
            double laborHours = ConsoleInput.ReadDouble("Estimated labor hours: ", 0d, 1000d);

            Appliance appliance;
            switch (applianceType)
            {
                case 1:
                    appliance = CreateRefrigerator(serialNumber, brand, model, customerName, faultDescription,
                        receivedDate, expectedDeliveryDate, diagnosticFee, partsCost, laborHours);
                    break;
                case 2:
                    appliance = CreateWashingMachine(serialNumber, brand, model, customerName, faultDescription,
                        receivedDate, expectedDeliveryDate, diagnosticFee, partsCost, laborHours);
                    break;
                case 3:
                    appliance = CreateAirConditioner(serialNumber, brand, model, customerName, faultDescription,
                        receivedDate, expectedDeliveryDate, diagnosticFee, partsCost, laborHours);
                    break;
                default:
                    throw new InvalidOperationException("Unknown appliance type.");
            }

            SubscribeToDeviceEvents(appliance);
            Center.AddDevice(appliance);
            PrintSuccess("Device " + appliance.SerialNumber + " registered successfully.");
            Console.WriteLine("Initial estimated repair cost: {0:N2}", appliance.CalculateRepairCost());
        }

        private static Refrigerator CreateRefrigerator(string serialNumber, string brand, string model,
            string customerName, string faultDescription, DateTime receivedDate, DateTime expectedDeliveryDate,
            decimal diagnosticFee, decimal partsCost, double laborHours)
        {
            int complexity = ConsoleInput.ReadInt("Cooling system complexity (1-3): ", 1, 3);
            bool requiresRefill = ConsoleInput.ReadYesNo("Requires refrigerant refill?");
            return new Refrigerator(serialNumber, brand, model, customerName, faultDescription, receivedDate,
                expectedDeliveryDate, diagnosticFee, partsCost, laborHours, complexity, requiresRefill);
        }

        private static WashingMachine CreateWashingMachine(string serialNumber, string brand, string model,
            string customerName, string faultDescription, DateTime receivedDate, DateTime expectedDeliveryDate,
            decimal diagnosticFee, decimal partsCost, double laborHours)
        {
            double capacity = ConsoleInput.ReadDouble("Load capacity in kg: ", 1d, 30d);
            bool requiresBearingService = ConsoleInput.ReadYesNo("Requires drum bearing service?");
            return new WashingMachine(serialNumber, brand, model, customerName, faultDescription, receivedDate,
                expectedDeliveryDate, diagnosticFee, partsCost, laborHours, capacity, requiresBearingService);
        }

        private static AirConditioner CreateAirConditioner(string serialNumber, string brand, string model,
            string customerName, string faultDescription, DateTime receivedDate, DateTime expectedDeliveryDate,
            decimal diagnosticFee, decimal partsCost, double laborHours)
        {
            int btu = ConsoleInput.ReadInt("Cooling capacity in BTU (5000-60000): ", 5000, 60000);
            bool deepCleaning = ConsoleInput.ReadYesNo("Requires deep cleaning?");
            return new AirConditioner(serialNumber, brand, model, customerName, faultDescription, receivedDate,
                expectedDeliveryDate, diagnosticFee, partsCost, laborHours, btu, deepCleaning);
        }

        private static string ReadUniqueSerialNumber()
        {
            while (true)
            {
                string serialNumber = ConsoleInput.ReadSerialNumber("Serial number: ");
                if (!Center.ContainsSerial(serialNumber)) return serialNumber;
                PrintError("This serial number already exists. Enter a unique serial number.");
            }
        }

        private static void DisplayAllDevices() { Console.WriteLine("ALL APPLIANCES"); DisplayDeviceList(Center.Devices); }

        private static void FindAndDisplayDevice()
        {
            Appliance device = ReadExistingDevice();
            if (device == null) return;
            Console.WriteLine(device.GetDetailedDescription());
        }

        private static void UpdateDeviceStatus()
        {
            Appliance device = ReadExistingDevice();
            if (device == null) return;
            IReadOnlyList<MaintenanceStatus> allowedStatuses = device.GetAllowedNextStatuses();
            if (allowedStatuses.Count == 0)
            {
                Console.WriteLine("No further status transitions are available from '{0}'.", device.Status);
                return;
            }
            Console.WriteLine("Current status: {0}", device.Status);
            Console.WriteLine("Allowed next statuses:");
            for (int i = 0; i < allowedStatuses.Count; i++) Console.WriteLine("{0}. {1}", i + 1, allowedStatuses[i]);
            int choice = ConsoleInput.ReadInt("Select next status: ", 1, allowedStatuses.Count);
            string note = ConsoleInput.ReadRequiredText("Status note: ", 3, 150);
            device.ChangeStatus(allowedStatuses[choice - 1], note);
            PrintSuccess("Status updated to " + device.Status + ".");
        }

        private static void UpdateRepairData()
        {
            Appliance device = ReadExistingDevice();
            if (device == null) return;
            string fault = ConsoleInput.ReadRequiredText("Updated fault description: ", 3, 250);
            decimal partsCost = ConsoleInput.ReadDecimal("Updated parts cost: ", 0m, 10000000m);
            double laborHours = ConsoleInput.ReadDouble("Updated labor hours: ", 0d, 1000d);
            device.FaultDescription = fault;
            device.PartsCost = partsCost;
            device.LaborHours = laborHours;
            PrintSuccess("Repair data updated.");
            Console.WriteLine("New estimated repair cost: {0:N2}", device.CalculateRepairCost());
        }

        private static void DisplayRepairedDevices() { Console.WriteLine("REPAIRED / DELIVERED APPLIANCES"); DisplayDeviceList(Center.GetRepairedDevices()); }

        private static void DisplayDevicesUnderMaintenance()
        {
            Console.WriteLine("APPLIANCES UNDER MAINTENANCE");
            DisplayDeviceList(Center.GetDevicesUnderMaintenance());
            Console.WriteLine();
            Console.WriteLine("Static Appliance.DevicesUnderMaintenanceCount = {0}", Appliance.DevicesUnderMaintenanceCount);
        }

        private static void DisplayStatistics()
        {
            Console.WriteLine("CENTER STATISTICS");
            Console.WriteLine(new string('-', 72));
            Console.WriteLine("Total registered devices: {0}", Center.Count);
            Console.WriteLine("Devices under maintenance (static property): {0}", Appliance.DevicesUnderMaintenanceCount);
            Console.WriteLine("Total estimated repair cost (polymorphic): {0:N2}", Center.CalculateTotalEstimatedRepairCost());
            Console.WriteLine();
            foreach (KeyValuePair<MaintenanceStatus, int> item in Center.GetStatusCounts())
                Console.WriteLine("{0,-18}: {1}", item.Key, item.Value);
        }

        private static void DisplayStatusHistory()
        {
            Appliance device = ReadExistingDevice();
            if (device == null) return;
            Console.WriteLine("STATUS HISTORY - " + device.SerialNumber);
            Console.WriteLine(new string('-', 72));
            if (device.StatusHistory.Count == 0) { Console.WriteLine("No status changes have been recorded yet."); return; }
            foreach (StatusHistoryEntry entry in device.StatusHistory) Console.WriteLine(entry);
        }

        private static Appliance ReadExistingDevice()
        {
            if (Center.Count == 0) { Console.WriteLine("No devices have been registered yet."); return null; }
            string serialNumber = ConsoleInput.ReadSerialNumber("Serial number: ");
            Appliance device = Center.FindBySerial(serialNumber);
            if (device == null) PrintError("Device not found.");
            return device;
        }

        private static void DisplayDeviceList(IReadOnlyList<Appliance> devices)
        {
            if (devices.Count == 0) { Console.WriteLine("No matching devices found."); return; }
            for (int i = 0; i < devices.Count; i++) Console.WriteLine("{0,2}. {1}", i + 1, devices[i].GetSummary());
        }

        private static void SubscribeToDeviceEvents(Appliance device)
        {
            device.StatusChanged += OnDeviceStatusChanged;
            device.RepairCompleted += OnRepairCompleted;
        }

        private static void OnDeviceStatusChanged(object sender, DeviceStatusChangedEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine("[EVENT] Device {0}: {1} -> {2} at {3:yyyy-MM-dd HH:mm}.", e.SerialNumber, e.OldStatus, e.NewStatus, e.ChangedAt);
        }

        private static void OnRepairCompleted(object sender, DeviceStatusChangedEventArgs e)
        {
            Console.WriteLine("[REPAIR COMPLETED EVENT] Device {0} is repaired and ready for delivery.", e.SerialNumber);
        }

        private static void PrintSuccess(string message) { Console.WriteLine("SUCCESS: " + message); }
        private static void PrintError(string message) { Console.WriteLine("ERROR: " + message); }

        private static void Pause()
        {
            Console.WriteLine();
            Console.Write("Press Enter to return to the main menu...");
            Console.ReadLine();
        }
    }
}

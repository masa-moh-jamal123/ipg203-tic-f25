using System;
using System.Text.RegularExpressions;

namespace HomeApplianceRepairCenter.Utilities
{
    /// <summary>Static validation helpers used throughout the application.</summary>
    public static class InputValidator
    {
        private static readonly Regex SerialNumberPattern =
            new Regex("^[A-Z0-9-]{5,20}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static bool IsValidSerialNumber(string serialNumber)
        {
            return !string.IsNullOrWhiteSpace(serialNumber)
                   && SerialNumberPattern.IsMatch(serialNumber.Trim().ToUpperInvariant());
        }

        public static bool IsValidReceivedDate(DateTime receivedDate)
        {
            DateTime date = receivedDate.Date;
            return date <= DateTime.Today && date.Year >= 2000;
        }

        public static bool IsValidExpectedDeliveryDate(DateTime receivedDate, DateTime expectedDeliveryDate)
        {
            return expectedDeliveryDate.Date >= receivedDate.Date;
        }

        public static bool IsValidText(string value, int minLength = 2, int maxLength = 100)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            int length = value.Trim().Length;
            return length >= minLength && length <= maxLength;
        }

        public static bool IsNonNegative(decimal value) { return value >= 0m; }
        public static bool IsNonNegative(double value) { return value >= 0d; }
        public static bool IsInRange(int value, int min, int max) { return value >= min && value <= max; }
        public static bool IsPositive(double value) { return value > 0d; }
        public static bool IsPositive(int value) { return value > 0; }
    }
}

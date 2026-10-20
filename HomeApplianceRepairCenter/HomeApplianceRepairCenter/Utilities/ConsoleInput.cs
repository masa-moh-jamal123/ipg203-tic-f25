using System;
using System.Globalization;

namespace HomeApplianceRepairCenter.Utilities
{
    /// <summary>Reusable console-reading helpers.</summary>
    public static class ConsoleInput
    {
        public static string ReadRequiredText(string prompt, int minLength = 2, int maxLength = 100)
        {
            while (true)
            {
                Console.Write(prompt);
                string value = (Console.ReadLine() ?? string.Empty).Trim();
                if (InputValidator.IsValidText(value, minLength, maxLength)) return value;
                Console.WriteLine("Invalid input. Enter between {0} and {1} characters.", minLength, maxLength);
            }
        }

        public static string ReadSerialNumber(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string value = (Console.ReadLine() ?? string.Empty).Trim().ToUpperInvariant();
                if (InputValidator.IsValidSerialNumber(value)) return value;
                Console.WriteLine("Invalid serial. Use 5-20 characters containing only A-Z, 0-9, or hyphen.");
            }
        }

        public static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string value = Console.ReadLine() ?? string.Empty;
                int number;
                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out number)
                    && InputValidator.IsInRange(number, min, max)) return number;
                Console.WriteLine("Invalid number. Enter an integer from {0} to {1}.", min, max);
            }
        }

        public static double ReadDouble(string prompt, double min, double max)
        {
            while (true)
            {
                Console.Write(prompt);
                string value = Console.ReadLine() ?? string.Empty;
                double number;
                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out number)
                    && number >= min && number <= max) return number;
                Console.WriteLine("Invalid number. Enter a value from {0} to {1} (use '.' for decimals).", min, max);
            }
        }

        public static decimal ReadDecimal(string prompt, decimal min, decimal max)
        {
            while (true)
            {
                Console.Write(prompt);
                string value = Console.ReadLine() ?? string.Empty;
                decimal number;
                if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out number)
                    && number >= min && number <= max) return number;
                Console.WriteLine("Invalid amount. Enter a value from {0} to {1} (use '.' for decimals).", min, max);
            }
        }

        public static DateTime ReadDate(string prompt, Func<DateTime, bool> validator, string validationMessage)
        {
            while (true)
            {
                Console.Write(prompt);
                string value = Console.ReadLine() ?? string.Empty;
                DateTime date;
                if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out date) && validator(date)) return date.Date;
                Console.WriteLine(validationMessage);
            }
        }

        public static bool ReadYesNo(string prompt)
        {
            while (true)
            {
                Console.Write(prompt + " (Y/N): ");
                string value = (Console.ReadLine() ?? string.Empty).Trim();
                if (value.Equals("Y", StringComparison.OrdinalIgnoreCase)
                    || value.Equals("YES", StringComparison.OrdinalIgnoreCase)) return true;
                if (value.Equals("N", StringComparison.OrdinalIgnoreCase)
                    || value.Equals("NO", StringComparison.OrdinalIgnoreCase)) return false;
                Console.WriteLine("Please enter Y or N.");
            }
        }
    }
}

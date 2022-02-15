using System;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects.Common
{
    public static class StringExtensions
    {
        private static readonly string[] Numbers =
        {
            "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten",
        };

        public static bool ContainsIgnoreCase(this string input, char value)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            return input.Contains(value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoreCase(this string input, string value)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            return input.Contains(value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EqualsIgnoreCase(this string input, string toCompare) =>
            string.Equals(input, toCompare, StringComparison.OrdinalIgnoreCase);

        public static string ToEnglish(this int number)
        {
            if (number < 1 || number > Numbers.Length)
            {
                throw new ArgumentException(
                    $"This method is valid only for numbers from 1 to {Numbers.Length}");
            }

            return Numbers[number - 1];
        }

        public static string FormatForComparison(this string value) =>
            new(value.Where(c => !char.IsWhiteSpace(c)).ToArray());

        public static bool EqualsIgnoreWhiteSpace(this string a, string b) =>
            a.FormatForComparison().EqualsIgnoreCase(b.FormatForComparison());
    }
}

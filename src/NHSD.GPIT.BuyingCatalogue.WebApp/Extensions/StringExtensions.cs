using System;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Extensions
{
    public static class StringExtensions
    {
        public static int? AsNullableInt(this string input) => int.TryParse(input, out var output) ? output : null;

        public static decimal? AsNullableDecimal(this string input) =>
            decimal.TryParse(input, out var output) ? output : null;

        public static string CapitaliseFirstLetter(this string input) => input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => input[0].ToString().ToUpper() + input[1..],
        };
    }
}

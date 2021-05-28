using System;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class StringExtensions
    {
        public static string ControllerName(this Type type) => type.Name.Replace("Controller", null);

        public static bool EqualsIgnoreCase(this string input, string toCompare) =>
            input.Equals(toCompare, StringComparison.InvariantCultureIgnoreCase);
    }
}

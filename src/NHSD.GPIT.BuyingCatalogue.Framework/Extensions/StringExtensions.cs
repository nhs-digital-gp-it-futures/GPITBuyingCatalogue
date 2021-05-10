using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string input, string toCompare) =>
            input.Equals(toCompare, StringComparison.InvariantCultureIgnoreCase);
    }
}
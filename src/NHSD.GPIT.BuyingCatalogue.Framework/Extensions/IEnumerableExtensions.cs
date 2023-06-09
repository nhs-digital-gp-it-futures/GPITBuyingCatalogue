using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class IEnumerableExtensions
    {
        public static string ToFilterString(this IEnumerable<string> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            return string.Join(FilterConstants.Delimiter, values);
        }

        public static string ToFilterString(this IEnumerable<int> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            return string.Join(FilterConstants.Delimiter, values);
        }
    }
}

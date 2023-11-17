using System;
using System.Collections.Generic;
using System.Text;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class IEnumerableExtensions
    {
        public static string ToRecipientsString(this IEnumerable<string> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            return string.Join(RecipientsConstants.Delimiter, values);
        }

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

        public static string ToFilterString(this IDictionary<int, string[]> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            var builder = new StringBuilder();
            foreach (var kvp in values)
            {
                builder.Append(kvp.Key);
                var epics = string.Join(FilterConstants.Delimiter, kvp.Value ?? Array.Empty<string>());
                if (!string.IsNullOrEmpty(epics))
                {
                    builder
                        .Append(FilterConstants.Delimiter)
                        .Append(epics);
                }

                builder.Append(FilterConstants.GroupDelimiter);
            }

            return builder.ToString();
        }
    }
}

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

        public static string ToFilterString<T>(this IEnumerable<T> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            return string.Join(FilterConstants.Delimiter, values);
        }

        public static string ToFilterString<TKey, TValue>(this IDictionary<TKey, TValue[]> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            var builder = new StringBuilder();
            foreach (var kvp in values)
            {
                builder.Append(kvp.Key);
                var nestedValues = string.Join(FilterConstants.Delimiter, kvp.Value ?? Array.Empty<TValue>());
                if (!string.IsNullOrEmpty(nestedValues))
                {
                    builder
                        .Append(FilterConstants.Delimiter)
                        .Append(nestedValues);
                }

                builder.Append(FilterConstants.GroupDelimiter);
            }

            return builder.ToString();
        }
    }
}

using System;
using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(collection);

            if (items is null)
                return;

            foreach (var item in items)
                collection.Add(item);
        }

        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(collection);

            if (items is null)
                return;

            foreach (T item in items)
                collection.Remove(item);
        }
    }
}

using System;
using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            if (items is null)
                return;

            foreach (var item in items)
                collection.Add(item);
        }
    }
}

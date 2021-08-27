using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class PagedList<T>
    {
        [ExcludeFromCodeCoverage]
        public PagedList(IList<T> items, PageOptions options)
        {
            Items = items;
            Options = options;
        }

        public IList<T> Items { get; set; }

        public PageOptions Options { get; set; }
    }
}

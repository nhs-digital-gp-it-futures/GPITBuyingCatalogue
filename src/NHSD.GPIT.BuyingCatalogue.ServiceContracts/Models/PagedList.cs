using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class PagedList<T>
    {
        public PagedList(IList<T> items, PageOptions options)
        {
            Items = items;
            Options = options;
        }

        public IList<T> Items { get; set; }

        public PageOptions Options { get; set; }
    }
}

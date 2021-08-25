using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class PageOptions
    {
        public enum SortOptions
        {
            None,
            Alphabetical,
            LastUpdated,
        }

        public int PageSize { get; set; } = 20;

        public int PageNumber { get; set; }

        public int TotalNumberOfItems { get; set; }

        public SortOptions Sort { get; set; }

        public int NumberOfPages
        {
            get
            {
                if (PageSize < 0 || TotalNumberOfItems < 0)
                    return 0;

                return (int)Math.Ceiling((double)TotalNumberOfItems / PageSize);
            }
        }
    }
}

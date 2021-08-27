using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class PageOptions
    {
        public PageOptions(string page, string sortBy)
        {
            if (!string.IsNullOrWhiteSpace(page) && int.TryParse(page, out int pageOut))
                PageNumber = pageOut;

            if (!string.IsNullOrWhiteSpace(sortBy) && Enum.TryParse<SortOptions>(sortBy, true, out var sortOption))
                Sort = sortOption;
        }

        public PageOptions()
        {
        }

        public enum SortOptions
        {
            None,
            Alphabetical,
            LastUpdated,
        }

        public int PageSize { get; } = 20;

        public int PageNumber { get; init; } = 1;

        public int TotalNumberOfItems { get; set; }

        public SortOptions Sort { get; init; }

        public int NumberOfPages
        {
            get
            {
                if (TotalNumberOfItems < 0)
                    return 0;

                return (int)Math.Ceiling((double)TotalNumberOfItems / PageSize);
            }
        }
    }
}

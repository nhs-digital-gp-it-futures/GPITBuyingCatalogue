using System;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class PageOptions
    {
        public PageOptions()
        {
        }

        public PageOptions(string page)
        {
            if (!string.IsNullOrWhiteSpace(page) && int.TryParse(page, out int pageOut))
                PageNumber = pageOut;
        }

        public PageOptions(string page, string sortBy)
            : this(page)
        {
            if (!string.IsNullOrWhiteSpace(sortBy) && Enum.TryParse<SortOptions>(sortBy, true, out var sortOption))
                Sort = sortOption;
        }

        public PageOptions(string page, int pageSize)
            : this(page)
        {
            PageSize = pageSize;
        }

        public enum SortOptions
        {
            [Display(Name = "None")]
            None,

            [Display(Name = "A to Z")]
            AtoZ,

            [Display(Name = "Z to A")]
            ZToA,

            [Display(Name = "Last published")]
            LastPublished,
        }

        public int PageSize { get; init; } = 30;

        public int PageNumber { get; init; } = 1;

        public int TotalNumberOfItems { get; set; }

        public SortOptions Sort { get; init; } = SortOptions.AtoZ;

        public int NumberOfPages
        {
            get
            {
                if (TotalNumberOfItems < 0)
                    return 0;

                return (int)Math.Ceiling((double)TotalNumberOfItems / PageSize);
            }
        }

        public int NumberToSkip => PageNumber > 0
            ? PageSize * (PageNumber - 1)
            : 0;
    }
}

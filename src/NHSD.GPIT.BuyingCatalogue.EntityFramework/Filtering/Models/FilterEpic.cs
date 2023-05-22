using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models
{
    public sealed class FilterEpic : IAudited
    {
        public int FilterId { get; set; }

        public string EpicId { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public Epic Epic { get; set; }

        public Filter Filter { get; set; }
    }
}

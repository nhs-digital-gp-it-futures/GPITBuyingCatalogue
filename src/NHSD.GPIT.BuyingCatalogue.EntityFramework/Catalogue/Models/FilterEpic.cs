using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed class FilterEpic : IAudited
    {
        public FilterEpic()
        {
        }

        public FilterEpic(
            string filterId,
            string epicId)
        {
            FilterId = filterId;
            EpicId = epicId;
        }

        public string FilterId { get; set; }

        public string EpicId { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public Epic Epic { get; set; }

        public Filter Filter { get; set; }
    }
}

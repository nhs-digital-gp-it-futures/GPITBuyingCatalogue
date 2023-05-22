using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models
{
    public sealed class FilterHostingType : IAudited
    {
        public int FilterId { get; set; }

        public int Id { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public HostingType HostingType { get; set; }

        public Filter Filter { get; set; }
    }
}

using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed class FilterHostingType : IAudited
    {
        public FilterHostingType()
        {
        }

        public FilterHostingType(
            string filterId,
            int hostingTypeId)
        {
            FilterId = filterId;
            HostingTypeId = hostingTypeId;
        }

        public string FilterId { get; set; }

        public int HostingTypeId { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public HostingType HostingType { get; set; }

        public Filter Filter { get; set; }
    }
}

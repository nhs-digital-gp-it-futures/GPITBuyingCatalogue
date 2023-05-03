using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed class FilterClientApplicationType : IAudited
    {
        public FilterClientApplicationType()
        {
        }

        public FilterClientApplicationType(
            string filterId,
            int clientApplicationTypeId)
        {
            FilterId = filterId;
            ClientApplicationTypeId = clientApplicationTypeId;
        }

        public string FilterId { get; set; }

        public int ClientApplicationTypeId { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public ClientApplicationType ClientApplicationType { get; set; }

        public Filter Filter { get; set; }
    }
}

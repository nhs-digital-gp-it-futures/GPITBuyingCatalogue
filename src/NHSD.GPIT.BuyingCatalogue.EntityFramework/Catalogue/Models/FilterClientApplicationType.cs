using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed class FilterClientApplicationType : IAudited
    {
        public int FilterId { get; set; }

        public int FilterClientApplicationTypeId { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public ClientApplicationType ClientApplicationType { get; set; }

        public Filter Filter { get; set; }
    }
}

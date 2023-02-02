using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed class PricingUnit : IAudited
    {
        public short Id { get; set; }

        public string Name { get; set; }

        public string RangeDescription { get; set; }

        public string Description { get; set; }

        public string Definition { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }
    }
}

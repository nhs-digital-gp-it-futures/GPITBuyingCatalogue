using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class CataloguePriceTier : IAudited
    {
        public int Id { get; set; }

        public int CataloguePriceId { get; set; }

        public int BandStart { get; set; }

        public int? BandEnd { get; set; }

        public decimal Price { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }
    }
}

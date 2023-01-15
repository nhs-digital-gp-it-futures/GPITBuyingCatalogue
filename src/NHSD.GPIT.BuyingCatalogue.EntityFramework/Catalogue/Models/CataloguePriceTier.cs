using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed partial class CataloguePriceTier : IAudited, IPriceTier
    {
        public int Id { get; set; }

        public int CataloguePriceId { get; set; }

        public CataloguePrice CataloguePrice { get; set; }

        public int LowerRange { get; set; }

        public int? UpperRange { get; set; }

        public decimal Price { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }
    }
}

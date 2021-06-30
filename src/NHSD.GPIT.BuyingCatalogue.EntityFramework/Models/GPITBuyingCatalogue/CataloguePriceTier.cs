﻿namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public sealed class CataloguePriceTier
    {
        public int CataloguePriceTierId { get; set; }

        public int CataloguePriceId { get; set; }

        public int BandStart { get; set; }

        public int? BandEnd { get; set; }

        public decimal Price { get; set; }
    }
}

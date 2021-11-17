using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration.LegacyModels
{
    public partial class OrderItem
    {
        public int OrderId { get; set; }
        public string CatalogueItemId { get; set; }
        public int ProvisioningTypeId { get; set; }
        public int CataloguePriceTypeId { get; set; }
        public string PricingUnitName { get; set; }
        public int? TimeUnitId { get; set; }
        public int? EstimationPeriodId { get; set; }
        public int? PriceId { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? Price { get; set; }
        public DateTime? DefaultDeliveryDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}

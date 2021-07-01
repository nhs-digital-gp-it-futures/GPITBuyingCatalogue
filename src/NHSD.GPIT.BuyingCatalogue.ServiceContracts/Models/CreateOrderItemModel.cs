using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    [ExcludeFromCodeCoverage]
    public sealed class CreateOrderItemModel
    {
        public DateTime? CommencementDate { get; set; }

        public string SupplierId { get; set; }

        public DateTime? PlannedDeliveryDate { get; set; }

        public int? Quantity { get; set; }

        public string CatalogueItemName { get; set; }

        public CatalogueItemType CatalogueItemType { get; set; }

        public CatalogueItemId? CatalogueSolutionId { get; set; }

        public CatalogueItemId? CatalogueItemId { get; set; }

        public TimeUnit? EstimationPeriod { get; set; }

        public ItemUnitModel ItemUnit { get; set; }

        public int? PriceId { get; set; }

        public decimal? Price { get; set; }

        public ProvisioningType ProvisioningType { get; set; }

        public List<OrderItemRecipientModel> ServiceRecipients { get; set; }

        public TimeUnit? TimeUnit { get; set; }

        public CataloguePriceType Type { get; set; }

        public bool IsNewOrder { get; set; }

        public IEnumerable<CatalogueItemId> SolutionIds { get; set; }
    }
}

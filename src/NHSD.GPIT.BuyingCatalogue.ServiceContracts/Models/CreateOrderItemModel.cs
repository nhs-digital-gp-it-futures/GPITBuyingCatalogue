using System;
using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class CreateOrderItemModel
    {
        public DateTime? CommencementDate { get; set; }

        public string SupplierId { get; set; }

        public DateTime? PlannedDeliveryDate { get; set; }

        public int? Quantity { get; set; }

        public string CatalogueItemName { get; set; }

        public string CatalogueItemType { get; set; }

        public string CatalogueSolutionId { get; set; }

        public string CurrencyCode { get; init; }

        public string EstimationPeriod { get; set; }

        public ItemUnitModel ItemUnit { get; init; }

        public int? PriceId { get; init; }

        public decimal? Price { get; init; }

        public string ProvisioningType { get; set; }

        public List<OrderItemRecipientModel> ServiceRecipients { get; set; }

        public TimeUnitModel TimeUnit { get; set; }

        public string Type { get; init; }
    }
}

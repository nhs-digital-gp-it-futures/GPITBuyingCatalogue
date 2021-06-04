using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class CreateOrderItemModel
    {
        public string CatalogueItemName { get; init; }

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

using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities
{
    public class SelectOrderItemQuantityModel : NavBaseModel
    {
        public const string AdviceText = "Enter the total amount you want to order";
        public const string AdviceTextAmendment = "Enter the amount you want to order as part of this amendment.";
        public const string TitleText = "Quantity of {0}";

        public SelectOrderItemQuantityModel()
        {
        }

        public SelectOrderItemQuantityModel(
            CatalogueItem catalogueItem,
            IPrice price,
            int? quantity)
        {
            ItemName = catalogueItem.Name;
            ItemType = catalogueItem.CatalogueItemType.Name();
            Quantity = quantity.HasValue ? $"{quantity.Value}" : string.Empty;
            QuantityDescription = price.RangeDescription;
            ProvisioningType = price.ProvisioningType;
            BillingPeriod = price.BillingPeriod;
        }

        public override string Title => string.Format(TitleText, ItemType);

        public override string Caption => ItemName;

        public override string Advice => IsAmendment
            ? AdviceTextAmendment
            : (string.IsNullOrWhiteSpace(TimeUnit) ? $"{AdviceText}." : $"{AdviceText} {TimeUnit}.");

        public bool IsAmendment { get; set; }

        public string TimeUnit => BillingPeriod?.Description() ?? string.Empty;

        public string ItemName { get; set; }

        public string ItemType { get; set; }

        public string Quantity { get; set; }

        public string QuantityDescription { get; set; }

        public ProvisioningType ProvisioningType { get; set; }

        public TimeUnit? BillingPeriod { get; set; }

        public RoutingSource? Source { get; set; }
    }
}

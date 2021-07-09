using System;
using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions
{
    public static class CatalogueItemTypeExtensions
    {
        public static string DisplayName(this CatalogueItemType itemType) => itemType.AsString(EnumFormat.DisplayName);

        public static TimeUnit? InferEstimationPeriod(this CatalogueItemType itemType, ProvisioningType provisioningType, TimeUnit? estimationPeriod)
        {
            return itemType switch
            {
                CatalogueItemType.AssociatedService => provisioningType == ProvisioningType.OnDemand
                    ? provisioningType.InferEstimationPeriod(estimationPeriod)
                    : null,
                CatalogueItemType.AdditionalService or CatalogueItemType.Solution =>
                    provisioningType.InferEstimationPeriod(estimationPeriod),
                _ => throw new ArgumentOutOfRangeException(nameof(itemType)),
            };
        }

        public static void MarkOrderSectionAsViewed(this CatalogueItemType itemType, Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            switch (itemType)
            {
                case CatalogueItemType.Solution:
                    order.Progress.CatalogueSolutionsViewed = true;
                    break;

                case CatalogueItemType.AdditionalService:
                    order.Progress.AdditionalServicesViewed = true;
                    break;

                case CatalogueItemType.AssociatedService:
                    order.Progress.AssociatedServicesViewed = true;
                    break;

                default:
                    throw new ArgumentException($"{nameof(itemType)} must be a valid {nameof(CatalogueItemType)}", nameof(itemType));
            }
        }
    }
}

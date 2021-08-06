using System;
using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

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
    }
}

using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public class CatalogueItemType
        : EnumerationBase
    {
        public static readonly CatalogueItemType Solution = new(1, "Catalogue Solution");
        public static readonly CatalogueItemType AdditionalService = new(2, "Additional Service");
        public static readonly CatalogueItemType AssociatedService = new(3, "Associated Service");

        public CatalogueItemType(int id, string name)
                : base(id, name)
        {
        }

        public TimeUnit InferEstimationPeriod(ProvisioningType provisioningType, TimeUnit estimationPeriod)
        {
            return this == AssociatedService
                ? provisioningType == ProvisioningType.OnDemand
                    ? provisioningType.InferEstimationPeriod(estimationPeriod)
                    : null
                : provisioningType.InferEstimationPeriod(estimationPeriod);
        }

        public string DisplayName() => Name;

        public void MarkOrderSectionAsViewed(Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            if (this == Solution)
                order.OrderProgress.CatalogueSolutionsViewed = true;
            else if (this == AdditionalService)
                order.OrderProgress.AdditionalServicesViewed = true;
            else if (this == AssociatedService)
                order.OrderProgress.AssociatedServicesViewed = true;
            else
                throw new InvalidOperationException();
        }
    }
}

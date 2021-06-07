using System;
using System.Collections.Generic;

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

        // MJRTODO - not sure this is required. Take is out and check create order item still works
        public ICollection<CatalogueItem> CatalogueItems { get; set; }

        public static CatalogueItemType Parse(string name)
        {
            if (name.Equals(nameof(Solution), System.StringComparison.InvariantCultureIgnoreCase))
                return Solution;
            else if (name.Equals(nameof(AdditionalService), System.StringComparison.InvariantCultureIgnoreCase))
                return AdditionalService;
            else if (name.Equals(nameof(AssociatedService), System.StringComparison.InvariantCultureIgnoreCase))
                return AssociatedService;

            throw new ArgumentException("Invalid CatalogueItemType", nameof(name));
        }

        public TimeUnit InferEstimationPeriod(ProvisioningType provisioningType, TimeUnit estimationPeriod)
        {
            return Id == AssociatedService.Id
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

            if (Id == Solution.Id)
                order.OrderProgress.CatalogueSolutionsViewed = true;
            else if (Id == AdditionalService.Id)
                order.OrderProgress.AdditionalServicesViewed = true;
            else if (Id == AssociatedService.Id)
                order.OrderProgress.AssociatedServicesViewed = true;
            else
                throw new InvalidOperationException();
        }
    }
}
